using UnityEngine;
using System.Collections.Generic;

namespace RunFaster
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField]
        private PokeCardsnZoneView cardsZoneView;
        [SerializeField]
        private PokeCardsnZoneView outcardsZoneView;

        /// <summary>
        /// 过牌
        /// </summary>
        public void OnBtnPass()
        {

        }
        
        /// <summary>
        /// 出牌
        /// </summary>
        public void OnBtnPlay()
        {
            List<Poke> lstPokes = cardsZoneView.GetSelectedPokes();
            outcardsZoneView.SetData(lstPokes, true, false);
        }
    }
}

