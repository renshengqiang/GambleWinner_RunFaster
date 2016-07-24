using System;
using System.Net.Sockets;
using System.Threading;

namespace Common.Network
{
    public interface IConnectionListener
    {
        void OnConnected();
        void OnClose();
        void OnMessage(Message msg);
        void OnError(string error);
        void OnTimeOut();
        void OnStateChange(NetWorkState state);
    }

    public enum ConnReadState
    {
        READ_HEAD = 1,
        READ_BODY
    }

    public class Connection : IDisposable
    {
        private Socket              socket;         // the underlying socket
        private Coder               coder;          // coder to decode buffer
        private IConnectionListener listener;       // listener
        private Buffer              buffer;         // the underlying buffer to receive network message
        private string              url;            // remote url
        private int                 port;           // remote port
        private Buffer              tempBuffer;     // temp buffer to receive network message
        private ConnReadState       readState;      // read state(reading head or body)
        private ManualResetEvent    timeoutEvent;   // check timeout use
        private int                 timeoutMSec;    // connect timeout count in millisecond
        private NetWorkState        networkState;   // current network state



        public Connection(IConnectionListener listener, Coder coder)
        {
            this.listener = listener;
            this.coder = coder;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.LingerState = new LingerOption(false, 0);
            buffer = new Buffer(4096);
            tempBuffer = new Buffer(4096);
            readState = ConnReadState.READ_HEAD;
            timeoutEvent = new ManualResetEvent(true);
            timeoutMSec = 8000;
            networkState = NetWorkState.CLOSED;
        }

        public void Connect(string url, int port)
        {
            socket.BeginConnect(url, port, Connected, this);
            this.url = url;
            this.port = port;
            readState = ConnReadState.READ_HEAD;
            NetworkStateChanged(NetWorkState.CONNECTING);
            if (timeoutEvent.WaitOne(timeoutMSec, false))
            {
                if (networkState != NetWorkState.CONNECTED && networkState != NetWorkState.ERROR)
                {
                    NetworkStateChanged(NetWorkState.TIMEOUT);
                    Dispose();
                }
            }
        }

        public void Close(bool serverClose = false)
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    NetworkStateChanged(NetWorkState.CLOSED);                    
                }
            }
            catch (Exception e)
            {
                listener.OnError(e.StackTrace + e.Message);
            }
        }

        public IAsyncResult Send<T>(uint msgId, T msg)
        {
            byte[] msgBuf = coder.Encode<T>(msgId, msg);
            try
            {
                IAsyncResult asyncSend = socket.BeginSend(msgBuf, 0, msgBuf.Length, SocketFlags.None, Sended, msgBuf);
                return asyncSend;
            }
            catch (Exception e)
            {
                NetworkStateChanged(NetWorkState.ERROR);
                listener.OnError(e.StackTrace + e.Message);
                return null;
            }
        }

        public void Dispose()
        {
            Close();
        }

        private void Connected(IAsyncResult asyncConnect)
        {
            if (socket.Connected)
            {
                socket.EndConnect(asyncConnect);
                if (listener != null)
                {
                    listener.OnConnected();
                }
                NetworkStateChanged(NetWorkState.CONNECTED);
                Thread thread = new Thread(new ThreadStart(ReceiveData));
                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                NetworkStateChanged(NetWorkState.DISCONNECTED);
            }
        }

        private void ReceiveData()
        {
            while (socket.Connected)
            {
                if (socket.Poll(-1, SelectMode.SelectRead))
                {
                    tempBuffer.Clear();

                    try
                    {
                        int len = socket.Receive(tempBuffer.GetRaw());
                        if (len > 0)
                        {
                            buffer.WriteBytes(tempBuffer);

                            if (readState == ConnReadState.READ_HEAD &&
                                buffer.ReadableBytes() >= coder.HeadLength())
                            {
                                readState = ConnReadState.READ_BODY;
                            }

                            if (readState == ConnReadState.READ_BODY &&
                                buffer.ReadableBytes() >= coder.HeadLength() + coder.GetBodyLengh(buffer, buffer.ReaderIndex()))
                            {
                                Message msg = coder.Decode(buffer);
                                listener.OnMessage(msg);
                                readState = ConnReadState.READ_HEAD;
                            }
                        }
                        else
                        {
                            Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error(string.Format("Receive message from socket error:{0}", e.ToString()));
                        listener.OnError(e.StackTrace + e.Message);
                        NetworkStateChanged(NetWorkState.ERROR);
                    }

                    listener.OnClose();
                }
            }
        }

        private void Sended(IAsyncResult ar)
        {
            if(ar.IsCompleted)
            {
                byte[] bb = (byte[])ar.AsyncState;
                int sentLen = socket.EndSend(ar);
                if(sentLen != bb.Length)
                {
                    Logger.Error(string.Format("Send data error, sent {0}/required: {1}", sentLen, bb.Length));
                }
            }
            else
            {
                Logger.Error("send data error");
            }
        }

        private void NetworkStateChanged(NetWorkState newState)
        {
            if(newState != networkState)
            {
                networkState = newState;
                listener.OnStateChange(newState);
            }
        }
    }
}