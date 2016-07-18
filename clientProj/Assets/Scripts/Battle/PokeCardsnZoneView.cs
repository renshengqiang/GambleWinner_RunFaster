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

        public void SetData(List<Poke> data, bool showFront = true)
        {   
            for(int i=0; i<lstPokeGoes.Count; ++i)
            {
                GameObjectPool.GetInstance().Despawn(lstPokeGoes[i]);
            }

            for(int i=0; i<data.Count; ++i)
            {
                GameObject go = GameObjectPool.GetInstance().Spawn("Card");
                if(go != null)
                {
                    lstPokeGoes.Add(go);
                    PokeItemView itemView = go.GetComponent<PokeItemView>();
                    if(itemView != null)
                    {
                        // todo: 刚开始发牌的时候应该是不能点击的，等发完牌之后才能进行点击
                        // 为了展示动画发牌的效果，这里应该用协程来一个个的进行添加
                        itemView.SetData(i, data[i], showFront);
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
    }
}

