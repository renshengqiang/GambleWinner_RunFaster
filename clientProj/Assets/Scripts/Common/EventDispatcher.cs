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
    public delegate void EventHandler(System.Object sender, int iEventType, params System.Object[] kParam);
    public class EventArgs
    {
        public System.Object sender;
        public int eventHashCode;
        public System.Object[] param;
    }

    public class EventDispatcher : Singleton<EventDispatcher>
    {
        Dictionary<int, List<EventHandler>>    eventTable;
        Queue<EventArgs>                       receiveEventQueue;

        public static int GetEventId(string eventName)
        {
            return eventName.GetHashCode(); // C# 保证用一个进程的地址空间内不同字符串的hashcode不同
        }

        /// <summary>
        /// 进行单例的初始化
        /// </summary>
        public override void Init()
        {
            eventTable = new Dictionary<int, List<EventHandler>>();
            receiveEventQueue = new Queue<EventArgs>();

            StartCoroutine(BeginHandleReceiveMessageQueue());
        }

        /// <summary>
        /// 注册一个事件的监听者
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="handler">回调函数</param>
        public void RegisterEventHandler(string eventName, EventHandler handler)
        {
            int hashCode = GetEventId(eventName);
            RegisterEventHandler(hashCode, handler);
        }

        private void RegisterEventHandler(int eventHashCode, EventHandler handler)
        {
            if (!eventTable.ContainsKey(eventHashCode))
            {
                eventTable.Add(eventHashCode, new List<EventHandler>());
            }
            List<EventHandler> kHandlerList = eventTable[eventHashCode];
            if (!kHandlerList.Contains(handler))
            {
                kHandlerList.Add(handler);
            }
        }

        /// <summary>
        /// 接注册一个事件回调
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handler"></param>
        public void UnRegisterEventHandler(string eventName, EventHandler handler)
        {
            int hashCode = GetEventId(eventName);
            UnRegisterEventHandler(hashCode, handler);
        }
        
        private void UnRegisterEventHandler(int eventHashCode, EventHandler kHandler)
        {
            if (eventTable.ContainsKey(eventHashCode))
            {
                List<EventHandler> kHandlerList = eventTable[eventHashCode];
                kHandlerList.Remove(kHandler);
            }
        }

        /// <summary>
        /// 向系统分发消息
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="param">事件参数</param>
        public void  DispatchMessage(System.Object sender, string eventName, params System.Object[] param)
        {
            int hashCode = GetEventId(eventName);
            DispatchMessage(sender, hashCode, param);
        }

        private void DispatchMessage(System.Object sender, int eventHashCode, params System.Object[] param)
        {
            if (eventTable.ContainsKey(eventHashCode))
            {
                List<EventHandler> kHandlerList = eventTable[eventHashCode];
                for (int i = 0; i < kHandlerList.Count; i++)
                {
                    ((EventHandler)kHandlerList[i])(sender, eventHashCode, param);
                }
            }
        }

        /// <summary>
        /// 异步分发消息，会在下一帧执行
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventName">event name</param>
        /// <param name="param">parameters</param>
        public void DispatchMessageAsync(System.Object sender, string eventName, params System.Object[] param)
        {
            int hashCode = GetEventId(eventName);
            DispatchMessageAsync(sender, hashCode, param);
        }

        private void DispatchMessageAsync(System.Object sender, int eventHashCode, params System.Object[] param)
        {
            lock (receiveEventQueue)
            {
                EventArgs args = new EventArgs()
                {
                    sender = sender,
                    eventHashCode = eventHashCode,
                    param = param,
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
                        DispatchMessage(args.sender, args.eventHashCode, args.param);
                    }
                }
            }
        }
    }
}