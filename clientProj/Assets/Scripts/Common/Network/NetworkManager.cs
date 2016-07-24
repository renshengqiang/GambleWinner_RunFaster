using System;
using System.ComponentModel;
using System.Collections;

namespace Common.Network
{
    /// <summary>
    /// network state enum
    /// </summary>
    public enum NetWorkState
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
        public event Action<NetWorkState>   NetWorkStateChangedEvent;

        private EventManager                eventMgr;
        private Connection                  netConn;
        private Coder                       coder;

        public override void Init()
        {
            eventMgr = new EventManager();
            coder = new Coder();
            netConn = new Connection(this, coder);
        }

        public override void Release()
        {
            eventMgr.Dispose();
            netConn.Dispose();
        }

        public void Connect(string url, int port)
        {
            netConn.Connect(url, port);
            OnStateChange(NetWorkState.CONNECTING);
        }
        
        public void AddMsgCallback(uint msgId, Action<object> callback)
        {
            eventMgr.AddCallback(msgId, callback);
        }

        public void Send<T>(uint msgId, T msg)
        {
            netConn.Send<T>(msgId, msg);
        }

        public void OnConnected()
        {
            OnStateChange(NetWorkState.CONNECTED);
        }

        public void OnClose()
        {
            OnStateChange(NetWorkState.CLOSED);
        }

        public void OnMessage(Message msg)
        {
            if(msg != null)
            {
                ProcessMessage(msg.id, msg.msg);
            }
        }

        public void OnTimeOut()
        {
            OnStateChange(NetWorkState.TIMEOUT);
        }

        public void OnStateChange(NetWorkState newState)
        {
            if (NetWorkStateChangedEvent != null)
            {
                NetWorkStateChangedEvent(newState);
            }
        }

        public void OnError(string error)
        {
            OnStateChange(NetWorkState.ERROR);
        }

        private void ProcessMessage(uint id, object msg)
        {
            eventMgr.InvokeCallback(id, msg);
        }
    }
}