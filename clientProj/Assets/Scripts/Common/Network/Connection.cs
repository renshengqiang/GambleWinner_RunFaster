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
        private int                 timeoutMSec = 8000; // connect timeout count in millisecond


        public Connection(IConnectionListener listener, Coder coder)
        {
            this.listener = listener;
            this.coder = coder;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.LingerState = new LingerOption(false, 0);
            buffer = new Buffer(4096);
            tempBuffer = new Buffer(4096);
        }

        public void Connect(string url, int port)
        {
            socket.BeginConnect(url, port, Connected, this);
            this.url = url;
            this.port = port;
            readState = ConnReadState.READ_HEAD;
            //if (timeoutEvent.WaitOne(timeoutMSec, false))
            //{
            //    if (netWorkState != NetWorkState.CONNECTED && netWorkState != NetWorkState.ERROR)
            //    {
            //        NetWorkChanged(NetWorkState.TIMEOUT);
            //        Dispose();
            //    }
            //}
        }

        public void Close(bool serverClose = false)
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
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

                Thread thread = new Thread(new ThreadStart(ReceiveData));
                thread.IsBackground = true;
                thread.Start();
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
                    }

                    listener.OnClose();
                }
            }
        }

        private void Sended(IAsyncResult ar)
        {
            if(ar.IsCompleted)
            {
                //Buffer bb = (Buffer)ar.AsyncState;
                //bb.ReaderIndex(bb.WriterIndex());
                socket.EndSend(ar);

            }
            else
            {
                Logger.Error("send data error");
            }
        }
    }
}