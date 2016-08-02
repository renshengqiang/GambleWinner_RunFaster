using System;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;

namespace Common.Network
{
    /// <summary>
    /// connection state enum
    /// </summary>
    public enum ConnectionState
    {
        [Description("initial state")]
        CLOSED,

        [Description("connecting server")]
        CONNECTING,

        [Description("server connected")]
        CONNECTED,

        [Description("disconnected with server")]
        DISCONNECTED,

        [Description("connect timeout")]
        TIMEOUT,

        [Description("network error")]
        ERROR
    }
    public enum ConnReadState
    {
        READ_HEAD = 1,
        READ_BODY
    }

    public interface IConnectionListener
    {
        void OnConnected();
        void OnClose();
        void OnMessage();
        void OnError(string error);
        void OnTimeOut();
        void OnStateChange(ConnectionState state);
    }

    
    public class Connection : IDisposable
    {
        private Socket              socket;         // the underlying socket
        private IConnectionListener listener;       // listener
        private Buffer              buffer;         // the underlying buffer to receive network message
        private string              url;            // remote url
        private int                 port;           // remote port
        private Buffer              tempBuffer;     // temp buffer to receive network message
        private ConnReadState       readState;      // read state(reading head or body)
        private ManualResetEvent    timeoutEvent;   // check timeout use
        private int                 timeoutMSec;    // connect timeout count in millisecond
        private ConnectionState     networkState;   // current network state
        private Buffer              sendBuffer;     // send buffer

        public Connection(IConnectionListener listener)
        {
            this.listener = listener;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.LingerState = new LingerOption(false, 0);
            buffer = new Buffer(4096);
            tempBuffer = new Buffer(4096);
            readState = ConnReadState.READ_HEAD;
            timeoutEvent = new ManualResetEvent(true);
            timeoutMSec = 8000;
            networkState = ConnectionState.CLOSED;
            sendBuffer = new Buffer(4096);
        }

        public void Connect(string url, int port)
        {
            socket.BeginConnect(url, port, Connected, this);
            this.url = url;
            this.port = port;
            readState = ConnReadState.READ_HEAD;
            NetworkStateChanged(ConnectionState.CONNECTING);
            if (timeoutEvent.WaitOne(timeoutMSec, false))
            {
                if (networkState != ConnectionState.CONNECTED && networkState != ConnectionState.ERROR)
                {
                    NetworkStateChanged(ConnectionState.TIMEOUT);
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
                    NetworkStateChanged(ConnectionState.CLOSED);                    
                }
            }
            catch (Exception e)
            {
                listener.OnError(e.StackTrace + e.Message);
            }
        }

        public bool ReadData(out byte[] outBuffer)
        {
            if(readState == ConnReadState.READ_HEAD &&
               this.buffer.ReadableBytes() >= 4 + buffer.GetInt(buffer.ReaderIndex()))
            {
                int length = buffer.ReadInt();
                outBuffer = buffer.ReadByteArray(length);
                return true;
            }
            else
            {
                outBuffer = null;
                return false;
            }
        }

        public IAsyncResult Send(byte[] data, int offset = 0, int length = -1)
        {
            if(length == -1)
            {
                length = data.Length;
            }

            sendBuffer.Clear();
            sendBuffer.WriteInt(length);
            sendBuffer.WriteBytes(data, offset, length);
            try
            {
                IAsyncResult asyncSend = socket.BeginSend(sendBuffer.GetRaw(), 0, sendBuffer.ReadableBytes(), SocketFlags.None, Sended, sendBuffer);
                return asyncSend;
            }
            catch (Exception e)
            {
                NetworkStateChanged(ConnectionState.ERROR);
                listener.OnError(e.StackTrace + e.Message);
                return null;
            }
        }

        public string Url
        {
            get { return url; }
        }

        public int Port
        {
            get { return port; }
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
                NetworkStateChanged(ConnectionState.CONNECTED);
                Thread thread = new Thread(new ThreadStart(ReceiveData));
                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                NetworkStateChanged(ConnectionState.DISCONNECTED);
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
                                buffer.ReadableBytes() >= sizeof(int))
                            {
                                readState = ConnReadState.READ_BODY;
                            }

                            if (readState == ConnReadState.READ_BODY &&
                                buffer.ReadableBytes() >= sizeof(int) + buffer.GetInt(buffer.ReaderIndex()))
                            {
                                listener.OnMessage();
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
                        NetworkStateChanged(ConnectionState.ERROR);
                    }

                    listener.OnClose();
                }
            }
        }

        private void Sended(IAsyncResult ar)
        {
            if(ar.IsCompleted)
            {
                Buffer bb = (Buffer)ar.AsyncState;
                int sentLen = socket.EndSend(ar);
                if(sentLen != bb.ReadableBytes())
                {
                    Logger.Error(string.Format("Send data error, sent {0}/required: {1}", sentLen, bb.ReadableBytes()));
                }
            }
            else
            {
                Logger.Error("send data error");
            }
        }

        private void NetworkStateChanged(ConnectionState newState)
        {
            if(newState != networkState)
            {
                networkState = newState;
                listener.OnStateChange(newState);
            }
        }
    }
}