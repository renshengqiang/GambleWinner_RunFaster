using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MoreFun
{
    public class GlobalObjectComponent : MonoBehaviour
    {
        void Awake()
        {
            //this.MustLog();
        }

        void Start()
        {
            //this.MustLog();
        }

        void OnEnable()
        {
            //this.MustLog();
        }

        void OnDisable()
        {
            //this.MustLog();
        }

        void Update()
        {
            GlobalObject.Instance._DispatchUpdate();
        }

        public void FixedUpdate()
        {
            GlobalObject.Instance._DispathFixedUpdate();
        }
        void LateUpdate()
        {
            GlobalObject.Instance._DispatchLateUpdate();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            //this.MustLog(pauseStatus);
            GlobalObject.Instance._DispatchApplicationPause(pauseStatus);
        }

        void OnLevelWasLoaded(int level)
        {
            //this.MustLog(level);
            GlobalObject.Instance._DispatchLevelWasLoaded(level);
        }
        void OnDestroy()
        {
            //this.MustLog();
        }
        void OnApplicationQuit()
        {
            //this.MustLog();
            GlobalObject.Instance._DispatchApplicationQuit();
        }
    }
}
