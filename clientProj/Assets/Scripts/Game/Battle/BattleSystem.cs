using UnityEngine;
using System.Collections;
using Common.Network;
using System;

namespace RunFaster
{
    
    public class BattleStartData
    {

    }

    public class BattleEndData
    {

    }

    public delegate void BattleStartHandler(BattleStartData startData);
    public delegate void BattleEndHandler(BattleEndData endData);

    public class BattleSystem : IDisposable
    {
        private event BattleStartHandler    startEvent;
        private event BattleEndHandler      endEvent;
        private NetworkManager              networkMgr;

        public void Init()
        {
            networkMgr = NetworkManager.GetInstance();
            RegCMD();
        }

        public void Dispose()
        {

        }

        public event BattleStartHandler StartEvent
        {
            add
            {
                startEvent -= value;
                startEvent += value;
            }
            remove
            {
                startEvent -= value;
            }
        }

        public event BattleEndHandler EndEvent
        {
            add
            {
                endEvent -= value;
                endEvent += value;
            }
            remove
            {
                endEvent -= value;
            }
        }


        private void RegCMD()
        {

        }

        /// <summary>
        /// 自己出牌
        /// </summary>
        public void PlayCard(PokeCardsType pokes)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void Abondon()
        {

        }

        /// <summary>
        /// 开始
        /// </summary>
        private void OnRecvBattleStart()
        {

        }

        /// <summary>
        /// 结束
        /// </summary>
        private void OnRecvBattleEnd()
        {

        }

        private void OnMyTurnMsg(object msg)
        {

        }

        /// <summary>
        /// 别人出牌
        /// </summary>
        private void OnRecvBattlePlayMsg(object msg)
        {

        }

        /// <summary>
        /// 掉线，等待此人
        /// </summary>
        private void OnRecvBattleWaiting(object msg)
        {

        }
    }
}

