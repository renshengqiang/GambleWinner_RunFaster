using UnityEngine;

namespace Common
{
    public class GameObjectUtil
    {
        public static void AddChildAsLastSibling(GameObject parent, GameObject child)
        {
            if (parent != null && child != null)
            {
                Transform parentTrans = parent.transform;
                Transform childTrans = child.transform;
                TransformUtil.AddChildAsLastSibling(parentTrans, childTrans);
            }
            else
            {
                Logger.Error(string.Format("AddChildAsLastSibling error: parent:{0}, child:{1}", parent, child));
            }
        }
    }
}