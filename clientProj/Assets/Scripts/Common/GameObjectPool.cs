using UnityEngine;
using System.Collections.Generic;

namespace Common
{
    public class GameObjectPool : Singleton<GameObjectPool>
    {
        private Dictionary<string, List<GameObject>> dicPool;

        public override void Init()
        {
            dicPool = new Dictionary<string, List<GameObject>>();
        }

        public override void Release()
        {
            dicPool.Clear();
        }

        public GameObject Spawn(string path)
        {
            GameObject ret = null;
            List<GameObject> lstGameObjects = null;
            dicPool.TryGetValue(path, out lstGameObjects);
            if(null == lstGameObjects)
            {
                dicPool[path] = lstGameObjects;
            }

            if(lstGameObjects.Count > 0)
            {
                ret = lstGameObjects[lstGameObjects.Count - 1];
                ret.SetActive(true);
            }
            else
            {
                ret = ResourceManager.GetInstance().GetResouce(path);
                if(ret != null)
                {
                    // todo: modify it
                    PoolItemCom poolItem = ret.AddComponent<PoolItemCom>();
                    if(poolItem != null)
                    {
                        poolItem.Path = path;
                    }
                }
            }
            return ret;
        }

        public void Despawn(GameObject go)
        {
            if(null == go)
            {
                Logger.Error("You are despawn a null gameobject!!!");
                return;
            }

            PoolItemCom poolItem = go.GetComponent<PoolItemCom>();
            if(poolItem != null)
            {
                string path  = poolItem.Path;
                List<GameObject> lstGameObjects = null;
                dicPool.TryGetValue(path, out lstGameObjects);
                if(null == lstGameObjects)
                {
                    dicPool[path] = lstGameObjects;
                }
                lstGameObjects.Add(go);
                go.transform.parent = null;
            }
            else
            {
                Logger.Error("Despawn go error: has no PoolItemCom attached！");
            }
        }
    }
}

