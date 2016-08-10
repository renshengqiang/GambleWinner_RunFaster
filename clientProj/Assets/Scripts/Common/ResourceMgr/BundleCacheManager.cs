using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MoreFun
{
    public enum LOADSTATUS
    {
        LOAD_ERROR = -1,
        LOAD_SUCCESS = 0,
    }

    /// <summary>
    /// ObjectCacheInfo用于对加载的Object进行管理
    /// </summary>
    public class ObjectCacheInfo
    {
        public delegate void LoadCallBack(string url, Object obj, LOADSTATUS result);
        public delegate void PackLoadCallBack(string url, Object[] objs, LOADSTATUS result);
        public delegate void LoadProcess(float percentage, LOADSTATUS status = LOADSTATUS.LOAD_SUCCESS);
        public delegate void LoadedCallback();

        public const int LD_TYPE_NONE = 0;
        public const int LD_TYPE_LOADING = 1;
        public const int LD_TYPE_SUCC = 2;
        public const int LD_TYPE_UNLOAD_ASSET = 3;
        public const int LD_TYPE_ERROR = -1;

        /// <summary>
        /// ASSETBUNDLE的名称
        /// </summary>
        public string bdName;
        public string url = "";
        /// <summary>
        /// 缓存的对象.
        /// </summary>
        public Object obj;
        /// <summary>
        /// 内容
        /// </summary>
        public AssetBundle content;
        /// <summary>
        /// 引用数量.
        /// </summary>
        public int refcount;
        /// <summary>
        /// 加载过程中 2 加载成功
        /// </summary>
        public int loadType = LD_TYPE_NONE;
        /// <summary>
        /// 内容加载器
        /// </summary>
        public object loader = null;
        /// <summary>
        /// 记录所有回调
        /// </summary>
        public List<LoadCallBack> callbacks = new List<LoadCallBack>();

        public ObjectCacheInfo()
        {
            url = "";
            obj = null;
            content = null;
            refcount = 0;
            loadType = LD_TYPE_NONE;
            loader = null;
        }

        public void setURL(string url)
        {
            if (!this.url.Equals(url))
            {
                this.url = url;
            }
        }

        /// <summary>
        /// 释放不用资源
        /// </summary>
        public void unloadResource()
        {
            if (content != null)
            {
                ///////////////Debug.LogError("release " + url + " " + bdName);
                content.Unload(false);
                content = null;
            }
        }

        /// <summary>
        /// 释放所有的资源
        /// </summary>
        public void dispose()
        {
            setURL("");
            callbacks.Clear();
            loader = null;
            obj = null;

            if (content != null)
            {
                content.Unload(true);
                content = null;
            }
        }
    }

    public class BundleCacheManager
    {
        public static ObjectCacheInfo borrowCacheInfo(string bdname)
        {
            ObjectCacheInfo cache = null;
            _dicPreloadObjCache.TryGetValue(bdname, out cache);

            if (cache == null)
            {
                cache = new ObjectCacheInfo();
                cache.bdName = bdname;
                _dicPreloadObjCache.Add(bdname, cache);
            }

            cache.refcount += 1;

            return cache;
        }

        public static ObjectCacheInfo borrowCacheInfo(ObjectCacheInfo cache)
        {
            if (cache != null)
            {
                cache.refcount += 1;
                ///////////////Debug.LogError("cache2: " + cache.url + " " + cache.refcount);
            }
            return cache;
        }

        public static ObjectCacheInfo GetCacheInfo(string assetpath)
        {
            ObjectCacheInfo cache = null;
            string bdname = BundleManager.parseAssetName2BundleName(assetpath);
            _dicPreloadObjCache.TryGetValue(bdname, out cache);
            return cache;
        }

        /// <summary>
        /// return the ref of Object.
        /// </summary>
        /// <param name="url">URL.</param>
        public static void returnCacheInfo(string bdname)
        {
            ObjectCacheInfo cache = null;
            _dicPreloadObjCache.TryGetValue(bdname, out cache);
            if (cache != null)
            {
                cache.refcount -= 1;

                //Debug.LogError(bdname + ": " + cache.refcount);
                if (cache.refcount <= 0)
                {
                    ///如果不在加载过程中 则立即删除
                    if (cache.loadType != ObjectCacheInfo.LD_TYPE_LOADING)
                    {
                        // 先清除依赖资源
                        BundleInfo info = BundleManager.getBundleInfo(bdname);
                        if (info != null)
                        {
                            int count = info.dep_bundle_list.Count;
                            for (int i = 0; i < count; i++)
                            {
                                BundleInfo depinfo = BundleManager.getBundleInfo((int)info.dep_bundle_list[i]);
                                BundleCacheManager.returnCacheInfo(depinfo.bundle_name);
                            }
                        }
                        //再清除自身
                        _dicPreloadObjCache.Remove(bdname);
                        cache.dispose();
                    }
                    else
                    {
#if LOG_DETAIL
                        Debug.LogWarning("[BundleCacheManager] delete asset=" + bdname + " is loading!!");
#endif
                    }
                }
            }
            else
            {
#if LOG_DETAIL
                Debug.LogWarning("引用已经清0了，怎么还要归还？");
#endif
            }
        }

        /// <summary>
        /// 释放无用资源
        /// </summary>
        public static void ReleaseAssets()
        {
            foreach (ObjectCacheInfo cacheInfo in _dicPreloadObjCache.Values)
            {
                cacheInfo.unloadResource();
            }
        }

        /// <summary>
        /// 释放单个资源
        /// </summary>
        /// <param name="bdname"></param>
        public static void UnloadResource(string bdname)
        {
            ObjectCacheInfo cache = null;
            _dicPreloadObjCache.TryGetValue(bdname, out cache);

            if (cache != null)
            {
                cache.unloadResource();
                cache.loadType = ObjectCacheInfo.LD_TYPE_UNLOAD_ASSET;
                BundleInfo info = BundleManager.getBundleInfo(bdname);
                if (info != null)
                {
                    int count = info.dep_bundle_list.Count;
                    for (int i = 0; i < count; i++)
                    {
                        BundleInfo depinfo = BundleManager.getBundleInfo((int)info.dep_bundle_list[i]);
                        ObjectCacheInfo child = null;
                        _dicPreloadObjCache.TryGetValue(depinfo.bundle_name, out child);
                        BundleCacheManager.UnloadResource(depinfo.bundle_name);
                    }
                }
            }
        }

        /// <summary>
        /// 清空Cache信息
        /// </summary>
        public static void ClearCacheInfo()
        {
            foreach (ObjectCacheInfo cacheInfo in _dicPreloadObjCache.Values)
            {
                cacheInfo.dispose();
            }
            _dicPreloadObjCache.Clear();
        }

        /// <summary>
        /// Gets the cached object.
        /// </summary>
        /// <returns>The cached object.</returns>
        /// <param name="url">URL.</param>
        public static ObjectCacheInfo getCacheInfo(string url)
        {
            ObjectCacheInfo info = null;
            _dicPreloadObjCache.TryGetValue(url, out info);
            return info;
        }

        public static List<string> getCachedUnloadRes()
        {
            List<string> list = new List<string>();

            foreach (KeyValuePair<string, ObjectCacheInfo> pair in _dicPreloadObjCache)
            {
                list.Add(pair.Key);
            }

            return list;
        }

        /// <summary>
        /// 用来存储Cache的AssetBundle中的资源，Key为加载的资源名，是bundle_name,不是asset_name
        /// </summary>
        static private Dictionary<string, ObjectCacheInfo> _dicPreloadObjCache = new Dictionary<string, ObjectCacheInfo>();
        static private Dictionary<string, ObjectCacheInfo> _dicInsceneObjCache = new Dictionary<string, ObjectCacheInfo>();
    }
}
