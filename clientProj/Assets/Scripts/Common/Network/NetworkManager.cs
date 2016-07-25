using System;

namespace Common.Network
{
    /// <summary>
    /// network message
    /// </summary>
    public class Message
    {
        public uint     id;
        public object   msg;
    }

    public class NetworkManager : Singleton<NetworkManager>, IConnectionListener
    {
        public event Action<ConnectionState>   NetWorkStateChangedEvent;

        private EventManager                eventMgr;
        private Connection                  netConn;
        private Coder                       coder;

        public override void Init()
        {
            eventMgr = new EventManager();
            coder = new Coder();
            netConn = new Connection(this);
        }

        public override void Release()
        {
            eventMgr.Dispose();
            netConn.Dispose();
        }

        public void Connect(string url, int port)
        {
            netConn.Connect(url, port);
            OnStateChange(ConnectionState.CONNECTING);
        }
        
        public void AddMsgCallback(uint msgId, Action<object> callback)
        {
            eventMgr.AddCallback(msgId, callback);
        }

        public void Send<T>(uint msgId, T msg)
        {
            byte[] arr = coder.Encode<T>(msgId, msg);
            netConn.Send(arr);
        }

        public void OnConnected()
        {
            OnStateChange(ConnectionState.CONNECTED);
        }

        public void OnClose()
        {
            OnStateChange(ConnectionState.CLOSED);
        }

        public void OnMessage()
        {
            byte[] data = null;
            if(netConn.ReadData(out data))
            {
                Message msg = coder.Decode(data);
                if (msg != null)
                {
                    ProcessMessage(msg.id, msg.msg);
                }
            }
        }

        public void OnTimeOut()
        {
            OnStateChange(ConnectionState.TIMEOUT);
        }

        public void OnStateChange(ConnectionState newState)
        {
            if (NetWorkStateChangedEvent != null)
            {
                NetWorkStateChangedEvent(newState);
            }
        }

        public void OnError(string error)
        {
            OnStateChange(ConnectionState.ERROR);
        }

        private void ProcessMessage(uint id, object msg)
        {
            eventMgr.InvokeCallback(id, msg);
        }
    }
}