using System;
using System.Collections;
using UnityEngine;

namespace MoreFun
{
    /// <summary>
    /// <para>使用GlobalObject供任意静态逻辑类使用协程、Update()功能.</para>
    /// <para>比如：GlobalObject.Instance.StartCoroutine(你的协程函数);</para>
    /// <para>比如：GlobalObject.AddUpdate(你的Update函数);记得调用GlobalObject.RemoveUpdate()移除.</para>
    /// </summary>
    public class GlobalObject
    {
        public static readonly GlobalObject Instance = new GlobalObject();

        public delegate void GlobalVoidCallBack();
        public delegate void GlobalBoolCallBack(bool value);
        public delegate void GlobalIntCallBack(int value);

        private GameObject ms_globalObject;
        private GlobalObjectComponent ms_globalComponent;

        private event GlobalVoidCallBack OnGlobalUpdateCallBack;
        private event GlobalVoidCallBack OnGlobalFixedUpdateCallback;
        private event GlobalVoidCallBack OnGlobalLateUpdateCallBack;
        private event GlobalBoolCallBack OnGlobalApplicationPauseCallBack;
        private event GlobalVoidCallBack OnGlobalApplicationQuitCallBack;
        private event GlobalIntCallBack m_onGlobalLevelWasLoadedCallBack;

        public GlobalObject()
        {
            ms_globalObject = new GameObject("GlobalObject");
            GameObject.DontDestroyOnLoad(ms_globalObject);
            ms_globalComponent = ms_globalObject.AddComponent<GlobalObjectComponent>();
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return ms_globalComponent.StartCoroutine(routine);
        }

        public void StopCoroutine(IEnumerator routine)
        {
            ms_globalComponent.StopCoroutine(routine);
        }

        public void AddUpdate(GlobalVoidCallBack func)
        {
            OnGlobalUpdateCallBack -= func;
            OnGlobalUpdateCallBack += func;
        }

        public void RemoveUpdate(GlobalVoidCallBack func)
        {
            OnGlobalUpdateCallBack -= func;
        }

        public void AddFixedUpdate(GlobalVoidCallBack func)
        {
            OnGlobalFixedUpdateCallback -= func;
            OnGlobalFixedUpdateCallback += func;
        }

        public void RemoveFixedUpdate(GlobalVoidCallBack func)
        {
            OnGlobalFixedUpdateCallback -= func;
        }

        public void AddLateUpdate(GlobalVoidCallBack func)
        {
            OnGlobalLateUpdateCallBack -= func;
            OnGlobalLateUpdateCallBack += func;
        }

        public void RemoveLateUpdate(GlobalVoidCallBack func)
        {
            OnGlobalLateUpdateCallBack -= func;
        }

        /// <summary>
        /// bool pauseStatus
        /// </summary>
        /// <param name="func"></param>
        public void AddApplicationPause(GlobalBoolCallBack func)
        {
            OnGlobalApplicationPauseCallBack -= func;
            OnGlobalApplicationPauseCallBack += func;
        }

        public void RemoveApplicationPause(GlobalBoolCallBack func)
        {
            OnGlobalApplicationPauseCallBack -= func;
        }

        public void AddApplicationQuit(GlobalVoidCallBack func)
        {
            OnGlobalApplicationQuitCallBack -= func;
            OnGlobalApplicationQuitCallBack += func;
        }

        public void RemoveApplicationQuit(GlobalVoidCallBack func)
        {
            OnGlobalApplicationQuitCallBack -= func;
        }

        public event GlobalIntCallBack OnLevelWasLoaded
        {
            add
            {
                m_onGlobalLevelWasLoadedCallBack -= value;
                m_onGlobalLevelWasLoadedCallBack += value;
            }
            remove
            {
                m_onGlobalLevelWasLoadedCallBack -= value;
            }
        }

        internal void _DispatchUpdate()
        {
#if ENABLE_PROFILER
            Profiler.BeginSample("GlobalObject._DispatchUpdate");
#endif
            if(null != OnGlobalUpdateCallBack)
            {
                OnGlobalUpdateCallBack();
            }
            #if ENABLE_PROFILER
            Profiler.EndSample();
#endif
        }

        internal void _DispathFixedUpdate()
        {
            #if ENABLE_PROFILER
            Profiler.BeginSample("GlobalObject._DispathFixedUpdate");
            #endif
            if (null != OnGlobalFixedUpdateCallback)
            {
                OnGlobalFixedUpdateCallback();
            }
            #if ENABLE_PROFILER
            Profiler.EndSample();
            #endif
        }
        internal void _DispatchLateUpdate()
        {
            #if ENABLE_PROFILER
            Profiler.BeginSample("GlobalObject._DispatchLateUpdate");
            #endif
            if (null != OnGlobalLateUpdateCallBack)
            {
                OnGlobalLateUpdateCallBack();
            }
            #if ENABLE_PROFILER
            Profiler.EndSample();
            #endif
        }


        internal void _DispatchApplicationPause(bool pauseStatus)
        {
            #if ENABLE_PROFILER
            Profiler.BeginSample("GlobalObject._DispatchApplicationPause");
            #endif
            if (null != OnGlobalApplicationPauseCallBack)
            {
                OnGlobalApplicationPauseCallBack(pauseStatus);
            }
            #if ENABLE_PROFILER
            Profiler.EndSample();
            #endif
        }

        internal void _DispatchApplicationQuit()
        {
            if (null != OnGlobalApplicationQuitCallBack)
            {
                OnGlobalApplicationQuitCallBack();
            }
        }

        internal void _DispatchLevelWasLoaded(int level)
        {
            #if ENABLE_PROFILER
            Profiler.BeginSample("GlobalObject._DispatchLevelWasLoaded");
            #endif
            if (null != m_onGlobalLevelWasLoadedCallBack)
            {
                m_onGlobalLevelWasLoadedCallBack(level);
            }
            #if ENABLE_PROFILER
            Profiler.EndSample();
            #endif
        }
    }
}
