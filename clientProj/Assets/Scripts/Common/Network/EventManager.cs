using System;
using System.Collections.Generic;

namespace Common.Network
{
    public class EventManager : IDisposable
    {
        private Dictionary<uint, List<Action<object>>> dicCallback;

        public EventManager()
        {
            dicCallback = new Dictionary<uint, List<Action<object>>>();
        }

        /// <summary>
        /// Add callback to dicCallback by id
        /// </summary>
        /// <param name="id">event id</param>
        /// <param name="callback">callback function</param>
        public void AddCallback(uint id, Action<object> callback)
        {
            if(id > 0 && callback != null)
            {
                List<Action<object>> list = null;

                if (dicCallback.TryGetValue(id, out list))
                {
                    list.Add(callback);
                }
                else
                {
                    list = new List<Action<object>>();
                    list.Add(callback);
                    dicCallback[id] = list;
                }
            }
        }

        /// <summary>
        /// Invoke the callback when the server returns data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public void InvokeCallback(uint id, object data)
        {
            List<Action<object>> list;
            if(dicCallback.TryGetValue(id, out list))
            {
                for(int i=0; i<list.Count; ++i)
                {
                    list[i].Invoke(data);
                }
            }
        }

        public void Dispose()
        {
            dicCallback.Clear();
        }
    }
}