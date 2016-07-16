﻿using System;
using UnityEngine;

namespace Common
{
    [RequireComponent(typeof(GameRoot))]
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;

        public static T GetInstance()
        {
			if (_instance == null) 
			{
				_instance = GameRoot.AddSingleton<T>();
			}
            return _instance;
        }

        public void SetInstance(T t)
        {
            if (_instance == null)
            {
                _instance = t;
            }
        }

        public virtual void Init()
        {
            return;
        }

        public virtual void Release()
        {
            return;
        }
    }
}

