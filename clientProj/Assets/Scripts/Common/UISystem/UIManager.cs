using UnityEngine;
using System.Collections;

namespace Common
{
    public class UIManager : Singleton<UIManager>
    {
        public override void Init()
        {
            //todo: 用反射来获取所有的WndContext的Name
            base.Init();
        }

        public override void Release()
        {
            base.Release();
        }

        public void Open(string wndName)
        {

        }

        public void CloseTop()
        {

        }
    }
}

