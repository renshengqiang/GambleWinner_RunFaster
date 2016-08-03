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
            EventDispatcher.GetInstance().RegisterEventHandler("GameRoot.Initialized", OnGameModuleInitialized);
        }

        public override void Release()
        {
            EventDispatcher.GetInstance().UnRegisterEventHandler("GameRoot.Initialized", OnGameModuleInitialized);
        }

        private void OnGameModuleInitialized(System.Object sender, int eventType, params System.Object[] param)
        {
            Application.LoadLevel(TEST_SCENE);
        }
    }
}

