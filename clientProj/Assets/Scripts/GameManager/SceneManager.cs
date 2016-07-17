using UnityEngine;
using System.Collections;
using Common;
using RunFaster;

namespace GameManager
{
    public class SceneManager : Singleton<SceneManager>
    {
        public const string INIT_SCENE = "Init";
        public const string TEST_SCENE = "test";
        public const string BATTLE_SCENE = "Battle";

        public override void Init()
        {
            EventDispatcher.GetInstance().RegisterEventHandler((uint)GameEventType.MODULE_INITLIZED, OnGameModuleInitialized);
        }

        public override void Release()
        {
            EventDispatcher.GetInstance().UnRegisterEventHandler((uint)GameEventType.MODULE_INITLIZED, OnGameModuleInitialized);
        }

        private void OnGameModuleInitialized(uint iEventType, object kParam)
        {
            Application.LoadLevel(BATTLE_SCENE);
        }
    }
}

