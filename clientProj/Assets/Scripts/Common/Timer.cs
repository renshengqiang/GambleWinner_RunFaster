using UnityEngine;
using System.Collections.Generic;

namespace Common
{
    public class Timer : Singleton<Timer>
    {
        private int id;
        private Dictionary<int, TimerObject> dicTimerObjs;
        private Dictionary<int, TimerObject> dicTimerObjsToAdd;
        private List<int> lstTimerToRemove;

        public override void Init()
        {
            id = 0;
            dicTimerObjs = new Dictionary<int, TimerObject>();
            dicTimerObjsToAdd = new Dictionary<int, TimerObject>();
            lstTimerToRemove = new List<int>();
        }

        public override void Release()
        {
            id = 0;
            dicTimerObjs.Clear();
            dicTimerObjsToAdd.Clear();
            lstTimerToRemove.Clear();
        }

        public int AddTimer(float interval, ParamsHandler handler, params System.Object[] param)
        {
            int timerId = id++;
            TimerObject timerObj = new TimerObject(timerId, interval, 0, handler, param);
            dicTimerObjsToAdd.Add(timerId, timerObj);
            return timerId;
        }

        public int AddTimer(float interval, ParamsHandler handler, float startTime, params System.Object[] param)
        {
            int timerId = id++;
            TimerObject timerObj = new TimerObject(timerId, interval, startTime, handler, param);
            dicTimerObjsToAdd.Add(timerId, timerObj);
            return timerId;
        }

        public int AddTimer(float interval, int repeatTimes,
                            ParamsHandler handler, params System.Object[] param)
        {
            int timerId = id++;
            TimerObject timerObj = new TimerObject(timerId, interval, repeatTimes, 0, handler, param);
            dicTimerObjsToAdd.Add(timerId, timerObj);
            return timerId;
        }

        public int AddTimer(float interval, int repeatTimes, float startTime,
                            ParamsHandler handler, params System.Object[] param)
        {
            int timerId = id++;
            TimerObject timerObj = new TimerObject(timerId, interval, repeatTimes, startTime, handler, param);
            dicTimerObjsToAdd.Add(timerId, timerObj);
            return timerId;
        }

        public int AddTimer(float interval, float duration,
                            ParamsHandler handler, params System.Object[] param)
        {
            int timerId = id++;
            TimerObject timerObj = new TimerObject(timerId, interval, duration, 0, handler, param);
            dicTimerObjsToAdd.Add(timerId, timerObj);
            return timerId;
        }

        public int AddTimer(float interval, float duration, float startTime,
                            ParamsHandler handler, params System.Object[] param)
        {
            int timerId = id++;
            TimerObject timerObj = new TimerObject(timerId, interval, duration, startTime, handler, param);
            dicTimerObjsToAdd.Add(timerId, timerObj);
            return timerId;
        }

        public void RemoveTimer(int id)
        {
            lstTimerToRemove.Add(id);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            foreach(TimerObject obj in dicTimerObjsToAdd.Values)
            {
                dicTimerObjs[obj.Id] = obj;
            }
            dicTimerObjsToAdd.Clear();

            for(int i=0; i<lstTimerToRemove.Count; ++i)
            {
                if(dicTimerObjs.ContainsKey(lstTimerToRemove[i]))
                {
                    dicTimerObjs.Remove(lstTimerToRemove[i]);
                }
            }
            lstTimerToRemove.Clear();

            foreach(TimerObject obj in dicTimerObjs.Values)
            {
                obj.AddTime(deltaTime);
                if(obj.CanExecute())
                {
                    obj.Execute();
                }
                if(obj.CanDestroy())
                {
                    lstTimerToRemove.Remove(obj.Id);
                }
            }
        }
    }

    public class TimerObject
    {
        private int              id;
        private float            startTime;
        private float            interval;
        private int              repeatTimes;
        private System.Object[]  param;
        private ParamsHandler    handler;

        // internal use
        private float            excuteTimeSinceLastCbk;

        public TimerObject(int id, float interval, float startTime,
                         ParamsHandler handler, params System.Object[] param)
        {
            this.id = id;
            this.interval = interval;
            this.startTime = startTime;
            this.handler = handler;
            this.param = param;
            this.excuteTimeSinceLastCbk = 0;
        }

        public TimerObject(int id, float interval, int repeatTimes, float startTime,
                         ParamsHandler handler, params System.Object[] param)
        {
            this.id = id;
            this.interval = interval;
            this.repeatTimes = repeatTimes;
            this.startTime = startTime;
            this.handler = handler;
            this.param = param;
            this.excuteTimeSinceLastCbk = 0;
        }

        public TimerObject(int id, float interval, float duration, float startTime,
                         ParamsHandler handler, params System.Object[] param)
        {
            this.id = id;
            this.interval = interval;
            this.startTime = startTime;
            this.handler = handler;
            this.param = param;
            this.excuteTimeSinceLastCbk = 0;
            this.repeatTimes = (int)(duration / interval);
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        public void AddTime(float time)
        {
            if(Mathf.Abs(startTime - time) > Mathf.Epsilon)
            {
                startTime -= time;
            }
            else
            {
                excuteTimeSinceLastCbk += time;
            }
        }

        public bool CanExecute()
        {
            return excuteTimeSinceLastCbk > interval;
        }

        public bool CanDestroy()
        {
            return repeatTimes < 1;
        }

        public void Execute()
        {
            if(CanExecute())
            {
                if(handler != null)
                {
                    handler(param);
                }

                repeatTimes--;
                excuteTimeSinceLastCbk -= interval;
            }
        }
    }
}

