using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common;
namespace RunFaster
{
    /// <summary>
    /// PokeItemView主要负责:
    /// 1. Sprite 的显示；
    /// 2. 扑克牌按钮事件的处理；
    /// </summary>
    public class PokeItemView : MonoBehaviour
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private Button btn;

        private bool clickedUp = false;
        private LayoutElement layoutElement;
        private PokeCardsnZoneView zoneView;
        private int index = 0;

        void Awake()
        {
            layoutElement = GetComponent<LayoutElement>();
            zoneView = GetComponentInParent<PokeCardsnZoneView>();
        }

        /// <summary>
        /// 设置扑克牌的数据
        /// </summary>
        /// <param name="index">poke index, 用于和上层的 view 进行交互</param>
        /// <param name="poke">具体显示哪张扑克牌</param>
        /// <param name="showFront">是否显示正面</param>
        public void SetData(int index, Poke poke, bool showFront = true)
        {
            Sprite sprite = null;

            if(showFront)
            {
                sprite = SpriteManager.GetInstance().GetSprite(poke);
                btn.interactable = true;
            }
            else
            {
                sprite = SpriteManager.GetInstance().GetPokeBackSprite();
                btn.interactable = false;
            }

            image.sprite = sprite;

            this.index = index;
        }

        public void OnClick()
        {
            if(clickedUp)
            {
                layoutElement.preferredHeight -= 20;
                zoneView.OnItemSelected(index);
            }
            else
            {
                layoutElement.preferredHeight += 20;
                zoneView.OnItemDeselected(index);
            }
            clickedUp = !clickedUp;
        }

        public void Reset()
        {
            if(clickedUp)
            {
                layoutElement.preferredHeight -= 20;
            }
            clickedUp = false;
        }
    }
}