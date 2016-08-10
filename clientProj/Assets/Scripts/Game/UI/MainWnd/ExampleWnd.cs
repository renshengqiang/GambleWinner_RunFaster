using UnityEngine;
using System.Collections;
using Common;

namespace UI
{
    public class ExampleWnd : WndContext
    {

        public override WndType Type()
        {
            return WndType.NORMAL;
        }
        public override string Name()
        {
            return "ExampleWndName";
        }

        public override string PrefabPath()
        {
            return "ExampleWndPath";
        }
    }
}