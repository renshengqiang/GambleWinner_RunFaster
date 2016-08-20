using UnityEngine;
using System.Collections;

namespace Common
{
    public class TransformUtil
    {
        public static void AddChildAsLastSibling(Transform parent, Transform child)
        {
            if (parent != null && child != null)
            {
                child.SetParent(parent);
                child.SetAsLastSibling();
                child.localScale = Vector3.one;
            }
            else
            {
                Logger.Error(string.Format("AddChildAsLastSibling error: parent:{0}, child:{1}", parent, child));
            }
        }

        public static void SetRectTransformStretch(RectTransform trans)
        {
            trans.sizeDelta = Vector2.zero;
            trans.anchoredPosition = Vector2.zero;
        }
    }
}

