using System;
using System.Collections.Generic;

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
        public event Action<ConnectionState>    NetWorkStateChangedEvent;

        private EventManager                    eventMgr;
        private Connection                      netConn;
        private Coder                           coder;
        private List<Message>                   lstMessage;
        private List<ConnectionState>           lstChangedState;

        public override void Init()
        {
            eventMgr = new EventManager();
            coder = new Coder();
            netConn = new Connection(this);
            lstMessage = new List<Message>();
            lstChangedState = new List<ConnectionState>();
        }

        public override void Release()
        {
            eventMgr.Dispose();
            coder.Dispose();
            netConn.Dispose();
            lstMessage.Clear();
            lstChangedState.Clear();
        }

        public void Connect(string url, int port)
        {
            netConn.Connect(url, port);
            OnStateChange(ConnectionState.CONNECTING);
        }
        
        public void AddMsgCallback(uint msgId, Type type, Action<object> callback)
        {
            eventMgr.AddCallback(msgId, callback);
            coder.RegisterMsgId(msgId, type);
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
                lstMessage.Add(msg);
            }
        }

        public void OnTimeOut()
        {
            OnStateChange(ConnectionState.TIMEOUT);
        }

        public void OnStateChange(ConnectionState newState)
        {
            lstChangedState.Add(newState);
        }

        public void OnError(string error)
        {
            OnStateChange(ConnectionState.ERROR);
        }

        /// <summary>
        /// use update to invoke callback
        /// prevent network thread to invoke callback directly
        /// </summary>
        public void Update()
        {
            if(NetWorkStateChangedEvent != null)
            {
                for (int i = 0; i < lstChangedState.Count; ++i)
                {
                    NetWorkStateChangedEvent(lstChangedState[i]);
                }
            }
            lstChangedState.Clear();

            for(int i=0; i<lstMessage.Count; ++i)
            {
                eventMgr.InvokeCallback(lstMessage[i].id, lstMessage[i].msg);
            }
            lstMessage.Clear();
        }
    }
}