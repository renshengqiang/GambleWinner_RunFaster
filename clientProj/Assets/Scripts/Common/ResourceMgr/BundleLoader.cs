using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MoreFun
{
    /// <summary>
    /// BundleLoader用于进行资源的加载
    /// </summary>
    public class BundleLoader
    {
        public static string PERSISTANT_BASE_PATH = Application.persistentDataPath + "/assetbundle/";
        public static  string STREAMING_BASE_PATH = Application.streamingAssetsPath + "/AssetBundle/";
        public const string ASSET_PATH_ROOT = "Assets/Resources/";
        private static string loadedBundle;
        private static string logFilename = "log.txt";
        private static int size = 0;
        private static System.DateTime elapsedtime;
        private static Dictionary<string, string> dicPlaceHolder = new Dictionary<string, string>();

        // 设置Log文件
        public static void SetLogFileName(string file)
        {
            logFilename = file;
            elapsedtime = System.DateTime.Now;
            loadedBundle = "";
            size = 0;
        }

        // 写加载的到log文件中
        public static void WriteLog()
        {
            if (Application.isEditor)
            {
                FileUtils.CreateAndWriteToFile(Application.persistentDataPath + "/" + logFilename, loadedBundle + "\r\n" + size);
            }
        }

        /// <summary>
        /// 尝试获得一个url对应的资源，如果该资源之前已经申请好则可以直接拿回使用
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        static public bool tryGetAsset(string url, ObjectCacheInfo.LoadCallBack callback)
        {
            ObjectCacheInfo cacheInfo = BundleCacheManager.GetCacheInfo(ASSET_PATH_ROOT + url);

            if (cacheInfo != null)
            {
                BundleCacheManager.borrowCacheInfo(cacheInfo);
                if (cacheInfo.loadType == ObjectCacheInfo.LD_TYPE_UNLOAD_ASSET)
                {
                    callback(url, cacheInfo.obj, LOADSTATUS.LOAD_SUCCESS);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 加载打包的资源，不用考虑依赖关系，直接进行读取即可
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        static public void loadAsset(string url, ObjectCacheInfo.PackLoadCallBack callback)
        {
            BundleInfo info = BundleManager.getBundleInfoByAsset(ASSET_PATH_ROOT + url);
            if (info != null)
            {
                ObjectCacheInfo cacheInfo = BundleCacheManager.borrowCacheInfo(info.bundle_name);
                BundleLoader loader = new BundleLoader(info, cacheInfo, url, callback);
                loader.loadPack();
            }
        }

        /// <summary>
        /// 从已经加载的 ab 中去加载一个资源
        /// </summary>
        /// <param name="PackedUrl"></param>
        /// <param name="resourcePathName"></param>
        /// <param name="callback"></param>
        public static void GetPackedAsset(string PackedUrl, string resourcePathName, ObjectCacheInfo.LoadCallBack callback)
        {
            ObjectCacheInfo cacheInfo = BundleCacheManager.GetCacheInfo(ASSET_PATH_ROOT + PackedUrl);
            if (cacheInfo != null)
            {
                Object obj = cacheInfo.content.Load(resourcePathName);
                if (null == obj)
                {
#if LOG_DETAIL
                    Debug.LogError("BundleLoader::GetAsset: get resource " + resourcePathName + " in " + PackedUrl + " error");
#endif
                    callback(resourcePathName, obj, LOADSTATUS.LOAD_ERROR);
                }
                else
                {
                    callback(resourcePathName, obj, LOADSTATUS.LOAD_SUCCESS);
                }
            }
            else
            {
#if LOG_DETAIL
                Debug.LogError("BundleLoader::GetAsset: 加载 " + PackedUrl + " 资源包的时候出错");
#endif
                callback(resourcePathName, null, LOADSTATUS.LOAD_ERROR);
            }
        }

        /// <summary>
        /// 加载资源，这里启动协程处理.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="callback">加载成功的回调函数.</param>
        static public void  loadAsset(string url, ObjectCacheInfo.LoadCallBack callback)
        {
            ObjectCacheInfo cacheInfo = BundleCacheManager.GetCacheInfo(ASSET_PATH_ROOT + url);

            if (cacheInfo != null)
            {
                ///存在映射信息
                BundleCacheManager.borrowCacheInfo(cacheInfo);
                
                ///侦听结果
                if (cacheInfo.loadType == ObjectCacheInfo.LD_TYPE_LOADING)
                {
                    cacheInfo.callbacks.Add(callback);
                }
                else
                {
                    callback(url, cacheInfo.obj, cacheInfo.loadType == ObjectCacheInfo.LD_TYPE_SUCC ?
                        LOADSTATUS.LOAD_SUCCESS : LOADSTATUS.LOAD_ERROR);
                }
            }
            else
            {
                BundleInfo info = BundleManager.getBundleInfoByAsset(ASSET_PATH_ROOT + url);

                if (info != null)
                {
                    // 先加载完所有资源
                    loadAsset(info, callback, false, url);
                }
                else
                {
                    string placeHolder = ResourceManager.GetPlaceHolder(url);

                    if (placeHolder != null)
                    {
                        loadAsset(placeHolder, callback);
#if LOG_DETAIL
                        Debug.LogWarning(url + " 对应的资源不存在");
#endif
                    }
                    else
                    {
#if LOG_DETAIL
                        Debug.LogWarning("Load " + url + "error: CANNOT get BundleInfo");
#endif
                        callback(url, null, LOADSTATUS.LOAD_ERROR);
                    }
                }
            }
        }

        static private void loadAsset(BundleInfo info, ObjectCacheInfo.LoadCallBack callback, bool dependByOthers, string url = "")
        {
            if (info != null)
            {
                ObjectCacheInfo cacheInfo = BundleCacheManager.borrowCacheInfo(info.bundle_name);
                
                ///设置URL
                if (!url.Equals(""))
                {
                    cacheInfo.setURL(url);
                }

                if (cacheInfo.loadType == ObjectCacheInfo.LD_TYPE_LOADING)
                {
                    cacheInfo.callbacks.Add(callback);
                }
                else if (cacheInfo.loadType == ObjectCacheInfo.LD_TYPE_ERROR)
                {
                    callback(url, cacheInfo.obj, LOADSTATUS.LOAD_ERROR);
                }
                else if (cacheInfo.loadType == ObjectCacheInfo.LD_TYPE_SUCC)
                {
                    callback(url, cacheInfo.obj, LOADSTATUS.LOAD_SUCCESS);
                }
                else
                {
                    cacheInfo.callbacks.Add(callback);
                    ///加载assetbundle
                    (new BundleLoader(info, cacheInfo, dependByOthers, url)).load();
                }
            }
            else
            {
                callback(url, null, LOADSTATUS.LOAD_ERROR);
            }
        }

        /// <summary>
        /// 卸载Asset对应的资源文件
        /// </summary>
        /// <param name="url"></param>
        static public void unloadAssetResource(string url)
        {
            ObjectCacheInfo cacheInfo = BundleCacheManager.GetCacheInfo(ASSET_PATH_ROOT + url);
            if (cacheInfo != null)
            {
                BundleCacheManager.UnloadResource(cacheInfo.bdName);
            }
        }

        /// <summary>
        /// 卸载资源， 这里要把对应Cache的引用计数减去1.
        /// </summary>
        /// <param name="url">URL.</param>
        static public void unloadAsset(string url)
        {
            ObjectCacheInfo cacheInfo = BundleCacheManager.GetCacheInfo(ASSET_PATH_ROOT + url);
#if LOG_DETAIL
            Debug.Log("卸载资源:" + url);
#endif
            if (cacheInfo != null)
            {
                //Debug.LogError("before: " + cacheInfo.url + " " + cacheInfo.bdName + " " + cacheInfo.refcount);
                BundleCacheManager.returnCacheInfo(cacheInfo.bdName);
                //Debug.LogError("after: " + cacheInfo.url + " " + cacheInfo.bdName + " " + cacheInfo.refcount);
            }
        }

        static public Object getLoadedObj(string url)
        {
            ObjectCacheInfo cacheInfo = BundleCacheManager.GetCacheInfo(ASSET_PATH_ROOT + url);

            if (cacheInfo != null)
            {
                return cacheInfo.obj;
            }

            //Debug.LogError(url + " return null");
            return null;
        }

        /// <summary>
        /// 单个ASSETBUNDLE的加载
        /// </summary>
        private BundleInfo _info;
        private ObjectCacheInfo _cache;
        private List<BundleInfo> _depBundles;
        private string _url;
        private bool _denpendByOthers;
        private ObjectCacheInfo.PackLoadCallBack _callback;

        public BundleLoader(BundleInfo info, ObjectCacheInfo cache, bool dependByOthers, string url = "")
        {
            _info = info;
            _cache = cache;
            _cache.loader = this;
            _url = url;
            _denpendByOthers = dependByOthers;
        }

        public BundleLoader(BundleInfo info, ObjectCacheInfo cache, string url, ObjectCacheInfo.PackLoadCallBack callback)
        {
            _info = info;
            _url = url;
            _callback = callback;
            _cache = cache;
        }

        private IEnumerator _loadSelfBundle()
        {
            bool loadFromPersistent = false;
            WWW www = null;
            string path;

            System.DateTime start_time = System.DateTime.Now;

            path = FileUtils.PERSISTENT_FILE_PROTOCAL_ + PERSISTANT_BASE_PATH + _info.bundle_name + "_" + _info.bundle_md5 + BundleConfig.BUNDLESUFFIX;
            www = new WWW(path);
            yield return www;
            loadFromPersistent = true;

            _cache.loadType = ObjectCacheInfo.LD_TYPE_SUCC;
            Object obj = null;
            AssetBundle ab = null;

            ///如果还在ref中
            if (_cache.refcount > 0)
            {
                try
                {
                    ab = www.assetBundle;

                    if (Application.isEditor)
                    {
                        if (loadFromPersistent)
                        {
#if LOG_DETAIL
                            Debug.Log(_info.bundle_name + BundleConfig.BUNDLESUFFIX);
#endif
                            string time = (System.DateTime.Now - start_time).ToString();
                            string allTime = (System.DateTime.Now - elapsedtime).ToString();
                            loadedBundle += _info.bundle_name + BundleConfig.BUNDLESUFFIX + " " + time + " " + www.size + " " + allTime + "\r\n";
                            size += www.size;
                        }
                        else
                        {
#if LOG_DETAIL
                            Debug.Log(_info.bundle_name + BundleConfig.BUNDLESUFFIX);
#endif
                            string time = (System.DateTime.Now - start_time).ToString();
                            string allTime = (System.DateTime.Now - elapsedtime).ToString();
                            loadedBundle += _info.bundle_name + BundleConfig.BUNDLESUFFIX + " " + time + " " + www.size + " " + allTime + "\r\n";
                            size += www.size;
                        }
                    }

                    _cache.content = ab;

                    if (ab.mainAsset.name != "")
                    {
                        obj = ab.Load(ab.mainAsset.name, typeof(Sprite));
                    }
                    else
                    {
                        Object[] objs = ab.LoadAll();           // 加载所有的GameObject
                    }

                    if (null == obj)
                    {
                        obj = ab.mainAsset;
                    }
                    
                    _cache.obj = obj;
                }
                catch
                {
#if LOG_DETAIL
                    Debug.LogWarning("***********-" + PERSISTANT_BASE_PATH + _info.bundle_name + BundleConfig.BUNDLESUFFIX);
#endif
                }
            }

            www.Dispose();
            www = null;

            if (_cache.refcount > 0)
            {
                ///回调与清空
                List<ObjectCacheInfo.LoadCallBack> funcs = _cache.callbacks;
                int count = funcs.Count;
                for (int i = 0; i < count; i++)
                {
                    ObjectCacheInfo.LoadCallBack func = funcs[i];

                    if (func != null)
                    {
                        func(_url, obj, LOADSTATUS.LOAD_SUCCESS);
                    }
                }

                _cache.callbacks.Clear();
            }
            else
            {
                ///回收资源
                BundleCacheManager.returnCacheInfo(_cache.bdName);
            }

            unload();
        }

        private void onDepLoaded(string url, Object obj, LOADSTATUS result)
        {
            if (result == LOADSTATUS.LOAD_SUCCESS)
            {
                if (_depBundles == null || _depBundles.Count == 0)
                {
                    ///加载完成所有依赖资源了
                    /// todo: 这里需要使用协程加载资源
                    //GlobalObject.Instance.StartCoroutine(_loadSelfBundle());
                }
                else
                {
                    BundleInfo depInfo = _depBundles[0];
                    _depBundles.RemoveAt(0);
                    loadAsset(depInfo, onDepLoaded, true);
                }
            }
        }

        public void load()
        {
            _cache.loadType = ObjectCacheInfo.LD_TYPE_LOADING;

            /* 获取依赖资源 */
            int count = _info.dep_bundle_list.Count;

            if (count > 0)
            {
                _depBundles = new List<BundleInfo>();

                for (int i = 0; i < count; i++)
                {
                    BundleInfo depinfo = BundleManager.getBundleInfo((int)_info.dep_bundle_list[i]);
                    _depBundles.Add(depinfo);
                }
            }

            onDepLoaded("", null, LOADSTATUS.LOAD_SUCCESS);
        }

        private void unload()
        {
            _info = null;
            _cache.loader = null;
            _cache = null;
            _url = "";
            _denpendByOthers = false;
        }

        public void loadPack()
        {
            //todo: 需要使用一个 GlobalObject
            //GlobalObject.Instance.StartCoroutine(_loadSelfPackBundle());
        }

        private IEnumerator _loadSelfPackBundle()
        {
            WWW www = null;
            string path;

            path = FileUtils.PERSISTENT_FILE_PROTOCAL_ + PERSISTANT_BASE_PATH + _info.bundle_name + "_" + _info.bundle_md5 + BundleConfig.BUNDLESUFFIX;
            www = new WWW(path);
            yield return www;

            AssetBundle ab = null;

            ab = www.assetBundle;
            if (null == ab)
            {
                if (_callback != null)
                {
                    _callback(_url, null, LOADSTATUS.LOAD_ERROR);
                }
            }
            else
            {
                _cache.content = ab;
                if (_callback != null)
                {
                    _callback(_url, null, LOADSTATUS.LOAD_SUCCESS);
                }
            }
        }
    }
}
