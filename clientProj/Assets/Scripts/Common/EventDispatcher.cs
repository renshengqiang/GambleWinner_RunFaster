using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Common
{
    /// <summary>
    /// 所有消息回调函数必须遵守的委托定义
    /// </summary>
    /// <param name="iMessageType"></param>
    /// <param name="kParam"></param>
    public delegate void EventHandler(uint iEventType, object kParam);
    public class EventArgs
    {
        public uint iEventType;
        public object kParam;
    }

    public class EventDispatcher : Singleton<EventDispatcher>
    {
        public const uint                       START_EVENT = 1;
        Dictionary<uint, List<EventHandler>>    eventTable;
        Queue<EventArgs>                        receiveEventQueue;

        /// <summary>
        /// 进行单例的初始化
        /// </summary>
        public override void Init()
        {
            eventTable = new Dictionary<uint, List<EventHandler>>();
            receiveEventQueue = new Queue<EventArgs>();

            StartCoroutine(BeginHandleReceiveMessageQueue());
        }

        /// <summary>
        /// 对一个消息注册一个新的回调函数，如果这个消息
        /// 已经有该回调函数，则不会注册第二次
        /// </summary>
        /// <param name="iMessageType"></param>
        /// <param name="kHandler"></param>
        public void RegisterEventHandler(uint iMessageType, EventHandler kHandler)
        {
            if (!eventTable.ContainsKey(iMessageType))
            {
                eventTable.Add(iMessageType, new List<EventHandler>());
            }
            List<EventHandler> kHandlerList = eventTable[iMessageType];
            if (!kHandlerList.Contains(kHandler))
            {
                kHandlerList.Add(kHandler);
            }
        }

        /// <summary>
        /// 对一个消息取消注册一个回调函数
        /// </summary>
        /// <param name="iMessageType"></param>
        /// <param name="kHandler"></param>
        public void UnRegisterEventHandler(uint iMessageType, EventHandler kHandler)
        {
            if (eventTable.ContainsKey(iMessageType))
            {
                List<EventHandler> kHandlerList = eventTable[iMessageType];
                kHandlerList.Remove(kHandler);
            }
        }

        /// <summary>
        /// 分发消息，同步
        /// </summary>
        /// <param name="iMessageType">消息类型</param>
        /// <param name="kParam">附加参数</param>
        public void DispatchMessage(uint iMessageType, object kParam = null)
        {
            if (eventTable.ContainsKey(iMessageType))
            {
                List<EventHandler> kHandlerList = eventTable[iMessageType];
                for (int i = 0; i < kHandlerList.Count; i++)
                {
                    ((EventHandler)kHandlerList[i])(iMessageType, kParam);
                }
            }
        }

        /// <summary>
        /// 分发消息，异步，会在协程BeginHandleReceiveMessageQueue的下一次检查中进行真正的消息分发
        /// </summary>
        /// <param name="iMessageType">消息类型</param>
        /// <param name="kParam">附加参数</param>
        public void DispatchMessageAsync(uint iMessageType, object kParam = null)
        {
            lock (receiveEventQueue)
            {
                EventArgs args = new EventArgs()
                {
                    iEventType = iMessageType,
                    kParam = kParam,
                };

                receiveEventQueue.Enqueue(args);
            }
        }

        private IEnumerator BeginHandleReceiveMessageQueue()
        {
            while (true)
            {
                yield return 0;
                lock (receiveEventQueue)
                {
                    while (receiveEventQueue.Count != 0)
                    {
                        EventArgs args = receiveEventQueue.Dequeue();
                        DispatchMessage(args.iEventType, args.kParam);
                    }
                }
            }
        }
    }
}