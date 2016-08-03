using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common;

namespace RunFaster
{
    public class PokeCardsnZoneView : MonoBehaviour
    {
        private List<Poke> lstPokes;
        private List<GameObject> lstPokeGoes;
        private List<int> lstSelected;

        void Awake()
        {
            lstPokes = new List<Poke>();
            lstPokeGoes = new List<GameObject>();
            lstSelected = new List<int>();
        }

        /// <summary>s
        /// 设置需要显示的牌
        /// </summary>
        /// <param name="data">牌</param>
        /// <param name="showFront">是否显示正面</param>
        /// <param name="interactive">是否可以点击</param>
        public void SetData(List<Poke> data, bool showFront = true, bool interactive = true)
        {   
            for(int i=0; i<lstPokeGoes.Count; ++i)
            {
                GameObjectPool.GetInstance().Despawn(lstPokeGoes[i]);
            }

            data.Sort(Poke.CompareForDisplay);
            lstPokes = data;
            lstPokeGoes.Clear();

            for(int i=0; i<data.Count; ++i)
            {
                GameObject go = GameObjectPool.GetInstance().Spawn("Card");
                if(go != null)
                {
                    lstPokeGoes.Add(go);
                    GameObjectUtil.AddChildAsLastSibling(gameObject, go);

                    PokeItemView itemView = go.GetComponent<PokeItemView>();
                    if(itemView != null)
                    {
                        // todo: 刚开始发牌的时候应该是不能点击的，等发完牌之后才能进行点击
                        // 为了展示动画发牌的效果，这里应该用协程来一个个的进行添加
                        itemView.SetData(i, data[i], showFront, interactive);
                    }
                    else
                    {
                        Logger.Error("item view is null");
                    }
                }
                else
                {
                    Logger.Error("spawn an empty item view");
                }
            }
        }

        /// <summary>
        /// 清空牌
        /// </summary>
        public void Clear()
        {
            SetData(new List<Poke>());
        }

        public void OnItemSelected(int index)
        {
            if(!lstSelected.Contains(index))
            {
                lstSelected.Add(index);
            }
        }

        public void OnItemDeselected(int index)
        {
            if (lstSelected.Contains(index))
            {
                lstSelected.Remove(index);
            }
        }

        /// <summary>
        /// 获取当前用户选择的牌
        /// </summary>
        /// <returns></returns>
        public List<Poke> GetSelectedPokes()
        {
            List<Poke> ret = new List<Poke>();
            for(int i=0; lstSelected != null && i<lstSelected.Count; ++i)
            {
                if (lstPokes.Count > lstSelected[i])
                {
                    ret.Add(lstPokes[lstSelected[i]]);
                }
            }
            return ret;
        }
    }
}

