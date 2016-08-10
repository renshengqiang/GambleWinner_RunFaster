using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MoreFun
{
    public class ResourceCallback
    {
        public string path;
        public List<ObjectCacheInfo.LoadCallBack> lstCallback = new List<ObjectCacheInfo.LoadCallBack>();
        public bool clearResource;
        public ResourceCallback(string _path, ObjectCacheInfo.LoadCallBack _CB, bool _clearResource = true)
        {
            path = _path;
            lstCallback.Add(_CB);
            clearResource = _clearResource;
        }
    };

    /// <summary>
    /// ResourceManager负责资源的统一加载
    /// 资源有两种加载方式：一种是通过Resource.Load，二是通过WWW来进行Load。
    /// 前者是在安装包中即加载的资源，后者是通过网络更新的资源。
    /// ResourceManager还负责网络资源的更新工作，在进入游戏前提示用户进行资源下载。
    /// </summary>
	public class ResourceManager
	{
        public static ResourceManager mainGroup = new ResourceManager();
        private static Dictionary<string, ResourceManager> ms_dicResourceManager = new Dictionary<string, ResourceManager>();
        private Dictionary<string, Object> m_dicResourceDownloaded = new Dictionary<string, Object>();
        private Dictionary<string, Object[]> m_dicPackResourceDownloaded = new Dictionary<string, Object[]>();
        private static List<ResourceCallback> ms_lstResourceToLoad = new List<ResourceCallback>();  /*当前正在加载的资源和他们的回调函数列表*/
        private static bool ms_loading = false;                                                     /*是否正在加载资源的过程当中*/
        private static ResourceManager ms_loadingManager = null;                                    /*当前正在加载资源的ResourceManager(多个ResourceManager一定不会在同时工作)*/
        private static Dictionary<string, string> dicPlaceHolder = new Dictionary<string, string>();

        public T GetResource<T>(string path) where T : Object
        {
            return getResource<T>(path);
        }

        public T GetPackedResource<T>(string pack, string path) where T : Object
        {
            return getPackedResource<T>(pack, path);
        }

        public static ResourceManager GetResourceManager(string type)
        {
            ResourceManager ret = null;
            ms_dicResourceManager.TryGetValue(type, out ret);
            if(null == ret)
            {
                ret = new ResourceManager();
                ms_dicResourceManager[type] = ret;
            }

            return ret;
        }

        private ResourceManager() {}

        public static void InitUpdate()
        {
            GlobalObject.Instance.AddFixedUpdate(FixedUpdate);
            GlobalObject.Instance.AddUpdate(FixedUpdate);/*FIXEDUPDATE会受TimeScale影响，因此需要使用Update也来执行这段逻辑*/
        }

        public static string GetPlaceHolder(string resouceName)
        {
            foreach (KeyValuePair<string, string> item in dicPlaceHolder)
            {
                if (Regex.IsMatch(resouceName, item.Key))
                {
                    return item.Value;
                }
            }
            return null;
        }

        private static void FixedUpdate()
        {
            if (false == ms_loading && ms_lstResourceToLoad.Count > 0)
            {
                string path = ms_lstResourceToLoad[0].path;
                List<ObjectCacheInfo.LoadCallBack> lstCallback = ms_lstResourceToLoad[0].lstCallback;
                bool clearResource = ms_lstResourceToLoad[0].clearResource;
                ms_lstResourceToLoad.RemoveAt(0);
                ms_loading = true;
                BundleLoader.loadAsset(path,
                            (url, obj, result) =>
                            {
                                ms_loading = false;
                                if (obj != null && ms_loadingManager != null &&
                                        !ms_loadingManager.m_dicResourceDownloaded.ContainsKey(path))
                                {
                                    ms_loadingManager.m_dicResourceDownloaded[path] = obj;
                                }

                                if (obj != null)
                                {
                                    for (int i = 0; i < lstCallback.Count; ++i)
                                    {
                                        if (MoreGlobal.IsValidHandler(lstCallback[i]))
                                            lstCallback[i](path, obj, result);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < lstCallback.Count; ++i)
                                    {
                                        if (MoreGlobal.IsValidHandler(lstCallback[i]))
                                            lstCallback[i](path, obj, LOADSTATUS.LOAD_ERROR);
                                    }
                                }
                                if (true == clearResource)
                                {
                                    BundleLoader.unloadAssetResource(path);
                                }
                            });
            }
        }

		public Object GetResource(string path)
		{
#if ENABLE_PROFILER
            Profiler.BeginSample("GetResource.load");
#endif
            Object ob = getResource(path);			
#if ENABLE_PROFILER
            Profiler.EndSample();
#endif 
            return ob;
		}

        public Object GetResourceAll<T>(string packPath, string childName) where T : Object 
        {
            Object[] objs;
            m_dicPackResourceDownloaded.TryGetValue(packPath, out objs);

            if(null == objs)
            {
                objs = Resources.LoadAll<T>(packPath);
                if (objs != null)
                {
                    m_dicPackResourceDownloaded[packPath] = objs;
                }
            }
            if (objs == null) return null;
            for (int i = 0; i < objs.Length; ++i)
            {
                if (objs[i].name == childName) return objs[i];
            }
            return null;
        }

        public Object GetResourceAll<T>(string packPath, int index) where T : Object
        {
            Object[] objs;
            m_dicPackResourceDownloaded.TryGetValue(packPath, out objs);

            if (null == objs)
            {
                objs = Resources.LoadAll<T>(packPath);
                if (objs != null)
                {
                    m_dicPackResourceDownloaded[packPath] = objs;
                }
            }
            if (objs == null) return null;
            if (index < objs.Length) return objs[index];
            return null;
        }

		public bool UnloadResource(string path)
		{
			return unloadResource(path);
		}

        public void LoadResourceAsync<T>(string path, ObjectCacheInfo.LoadCallBack loaded) where T : Object
        {
            loadRes<T>(path, loaded);
        }

		public void LoadResourceAsync(string path, ObjectCacheInfo.LoadCallBack loaded)
		{
            loadRes(path, loaded);
		}

        /* 直接加载资源，不考虑依赖关系，用于加载icon等无依赖的资源 */
        public void LoadResourceAsyncDirectly<T>(string path, ObjectCacheInfo.LoadCallBack loaded) where T : Object
        {
            loadRes<T>(path, loaded, true);
        }

        /* 直接加载资源，不需要考虑依赖关系，用于加载icon等无依赖的资源 */
        public void LoadResourceAsyncDirectly(string path, ObjectCacheInfo.LoadCallBack loaded)
        {
            loadResDirectly(path, loaded, true);
        }
        
        /* 加载打包资源，没有考虑依赖关系 */
        public void LoadPackedResourceAsyncDirectly(string path, ObjectCacheInfo.PackLoadCallBack loaded)
        {
            if(KillerGlobal.useDynamicUpdate)
            {
                BundleLoader.loadAsset(path, (url, objs, result) =>
                {
                    if (MoreGlobal.IsValidHandler(loaded))
                        loaded(url, objs, result);
                });
            }
            else
            {
                if (MoreGlobal.IsValidHandler(loaded))
                    loaded(path, null, LOADSTATUS.LOAD_SUCCESS);
            }
        }

        public void LoadResourceAsync(List<string> paths, ObjectCacheInfo.LoadProcess loaded)
        {
#if LOG_DETAIL
            for (int i = 0; paths != null && i < paths.Count; ++i)
            {
                Debug.Log("Loading " + i + " " + paths[i]);
            }
#endif
            loadRes(paths, loaded);
        }

        public void LoadPreloadResourceAsync(List<string> paths, ObjectCacheInfo.LoadProcess loaded)
        {
            loadPreloadRes(paths, loaded);
        }

        /// <summary>
        /// 清理加载资源时使用的资源文件（当前有引用的Texture等不会销毁）
        /// </summary>
        public void UnloadResources()
        {
            List<string> lstPath = new List<string>(m_dicResourceDownloaded.Keys);
            for (int i = 0; i < lstPath.Count; ++i)
            {
                string path = lstPath[i];
                if (!builtin(path))
                {
                    BundleLoader.unloadAssetResource(path);
                }
            }
        }
        
        /// <summary>
        /// 清理使用ResourceManager创建的所有资源（所有通过该ResourceManager创建的资源都会被销毁，包括正在使用的有引用计数的资源）
        /// without UnloadUnusedAssets nor GC()
        /// </summary>
        public void ClearSelf(List<string> lstDontClear = null)
        {
            List<string> lstPath = new List<string>(m_dicResourceDownloaded.Keys);
            for(int i=0; i<lstPath.Count; ++i)
            {
                string path = lstPath[i];
                if (builtin(path))
                {
                    if (m_dicResourceDownloaded.ContainsKey(path))
                    {
                        m_dicResourceDownloaded.Remove(path);
                    }
                    else if (m_dicPackResourceDownloaded.ContainsKey(path))
                    {
                        m_dicPackResourceDownloaded.Remove(path);
                    }
                }
                else
                {
                    bool ignore = false;
                    if (lstDontClear != null)
                    {
                        for (int j = 0; j < lstDontClear.Count; ++j)
                        {
                            if (path.Equals(lstDontClear[j]))
                            {
                                ignore = true;
                                break;
                            }
                        }
                    }
                    if (false == ignore)
                    {
                        BundleLoader.unloadAsset(path);
                        m_dicResourceDownloaded.Remove(path);
                    }
                }
            }
        }

        /// <summary>
        /// 清理使用ResourceManager创建的所有资源（所有通过该ResourceManager创建的资源都会被销毁，包括正在使用的有引用计数的资源）
        /// </summary>
        public void Clear(List<string> lstDontClear = null)
        {
            ClearSelf(lstDontClear);
            GlobalObject.Instance.StartCoroutine(ReleaseUnusedAssets());
        }

        private IEnumerator ReleaseUnusedAssets()
        {
            yield return null;
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        // wenhuo 这里只是做缓冲资源 引用清理 目前只用在quality level 切换
        public void DoClearWenhuo(string[] clearList)
        {
            if (null != clearList)
            {
                for (int i = 0; i < clearList.Length; ++i)
                {
                    if (null != clearList[i])
                    {
                        m_dicResourceDownloaded.Remove(clearList[i]);
                    }
                }
            }

            List<string> rlist = new List<string>();
            foreach(KeyValuePair<string,Object> kv in m_dicResourceDownloaded)
            {
                Sprite sp = kv.Value as Sprite;
                if (null != sp)
                {
                    rlist.Add(kv.Key);
                }
            }
            for (int i = 0; i < rlist.Count; ++i)
            {
                m_dicResourceDownloaded.Remove(rlist[i]);
            }
            rlist.Clear();            

            //m_dicResourceDownloaded.Clear();
        }

		/// <summary>
		/// 获取对象用，需要预加载后使用.
		/// </summary>
		/// <returns>The load resource.</returns>
		/// <param name="path">Path.</param>
		private Object getResource(string path)
		{
			Object assetobj = null;
            m_dicResourceDownloaded.TryGetValue(path, out assetobj);

            if (false == assetobj.IsValid())
            {
                if (false == KillerGlobal.useDynamicUpdate)
                {
                    assetobj = Resources.Load(path);
                    if (null == assetobj)
                    {
                        assetobj = getDefaultResource<GameObject>(path);
                    }

                    if (assetobj != null)
                    {
                        m_dicResourceDownloaded[path] = assetobj;
                    }
                }
                else
                {
                    if (builtin(path))
                    {
                        assetobj = Resources.Load(path);
                        if (null == assetobj)
                        {
                            assetobj = getDefaultResource<GameObject>(path);
                        }

                        if (assetobj != null)
                        {
                            m_dicResourceDownloaded[path] = assetobj;
                        }

                    }
                    else
                    {
                        assetobj = BundleLoader.getLoadedObj(path);
                    }
                }
            }
			
			return assetobj;
		}

        private T getResource<T>(string path) where T : Object
        {
            T assetobj = null;

            Object temp = null;
            m_dicResourceDownloaded.TryGetValue(path, out temp);

            if (null != temp)
            {
                assetobj = temp as T;
            }
            else
            {
                if (false == KillerGlobal.useDynamicUpdate)
                {
                    assetobj = Resources.Load<T>(path);
                    if (null == assetobj)
                    {
                        assetobj = getDefaultResource<T>(path);
                    }

                    if (assetobj != null)
                    {
                        m_dicResourceDownloaded[path] = assetobj;
                    }
                }
                else
                {
                    if (builtin(path))
                    {
                        assetobj = Resources.Load<T>(path);
                        if (assetobj != null)
                        {
                            m_dicResourceDownloaded[path] = assetobj;
                        }
                    }
                    else
                    {
                        assetobj = BundleLoader.getLoadedObj(path) as T;
                    }
                }
            }

            return assetobj;
        }

        /// <summary>
        /// 加载打包文件内部资源使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private T getPackedResource<T>(string pack, string path) where T : Object
        {
            Object ret;
            if (true == KillerGlobal.useDynamicUpdate)
            {
                m_dicResourceDownloaded.TryGetValue(path, out ret);

                if (null == ret)
                {
                    BundleLoader.GetPackedAsset(pack, path, (url, obj, result) =>
                        {
                            if (result == LOADSTATUS.LOAD_SUCCESS)
                            {
                                m_dicResourceDownloaded[path] = obj;
                                ret = obj;
                            }
                            else
                            {
                                ret = null;
                            }
                        });
                }
            }
            else
            {
                ret = getResource<T>(path);
            }
            return ret as T;
        }

		/// <summary>
		/// 归还对象用
		/// </summary>
		/// <returns><c>true</c>, if unload resource was khed, <c>false</c> otherwise.</returns>
		/// <param name="path">Path.</param>
		private bool unloadResource(string path)
		{
            unloadRes(path);
			return false;
		}

        /// <summary>
        /// 增加到待下载队列中去
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loaded"></param>
        /// <param name="clearResource"></param>
        private void addToLoadingLst(string path, ObjectCacheInfo.LoadCallBack loaded, bool clearResource)
        {
            for (int i = 0; i < ms_lstResourceToLoad.Count; ++i)
            {
                if (ms_lstResourceToLoad[i].path.Equals(path))
                {
                    ms_lstResourceToLoad[i].lstCallback.Add(loaded);
                    ms_lstResourceToLoad[i].clearResource = clearResource;
                    return;
                }
            }
            ms_lstResourceToLoad.Add(new ResourceCallback(path, loaded, clearResource));
        }

		/// <summary>
		/// 加载资源用，引用计数会加一.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="loaded">Loaded.</param>
		private void loadRes(string path, ObjectCacheInfo.LoadCallBack loaded, bool clearResource = true)
		{
			Object assetobj = null;
            m_dicResourceDownloaded.TryGetValue(path, out assetobj);

            if (null != assetobj)
            {
                loaded(path, assetobj, LOADSTATUS.LOAD_SUCCESS);
            }
            else
            {
                if (false == KillerGlobal.useDynamicUpdate)
                {
                    assetobj = Resources.Load(path);
                    if (null == assetobj)
                    {
                        assetobj = getDefaultResource<GameObject>(path);
                    }

                    if (assetobj != null)
                    {
                        m_dicResourceDownloaded[path] = assetobj;
                    }
                    if (MoreGlobal.IsValidHandler(loaded))
                        loaded(path, assetobj, LOADSTATUS.LOAD_SUCCESS);
                }
                else
                {
                    if (builtin(path))
                    {
                        assetobj = Resources.Load(path);
                        if (assetobj != null)
                        {
                            m_dicResourceDownloaded[path] = assetobj;
                        }

                        // todo: 需要判断回调的对象是否存在了
                        if (assetobj != null && loaded != null)
                        {
                            loaded(path, assetobj, LOADSTATUS.LOAD_SUCCESS);
                        }
                        else if (loaded != null)
                        {
                            loaded(path, assetobj, LOADSTATUS.LOAD_ERROR);
                        }
                    }
                    else
                    {
                        if (false == BundleLoader.tryGetAsset(path, loaded))
                        {
                            ms_loadingManager = this;
                            addToLoadingLst(path, loaded, clearResource);
                        }
                    }
                }
            }
		}

        /// <summary>
        /// 预加载资源用，引用计数会加一(直接去加载，不会考虑资源共享的问题，load完之后直接unload资源)
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="loaded">Loaded.</param>
        private void loadResDirectly(string path, ObjectCacheInfo.LoadCallBack loaded, bool clearResource = false)
        {
            Object assetobj = null;
            m_dicResourceDownloaded.TryGetValue(path, out assetobj);

            if (null != assetobj)
            {
                loaded(path, assetobj, LOADSTATUS.LOAD_SUCCESS);
            }
            else
            {
                if (false == KillerGlobal.useDynamicUpdate)
                {
                    assetobj = Resources.Load(path);
                    if (assetobj != null)
                    {
                        m_dicResourceDownloaded[path] = assetobj;
                    }
                    if(MoreGlobal.IsValidHandler(loaded))
                        loaded(path, assetobj, LOADSTATUS.LOAD_SUCCESS);
                }
                else
                {
                    if (builtin(path))
                    {
                        assetobj = Resources.Load(path);
                        if (null == assetobj)
                        {
                            assetobj = getDefaultResource<GameObject>(path);
                        }

                        if (assetobj != null)
                        {
                            m_dicResourceDownloaded[path] = assetobj;
                        }

                        if (MoreGlobal.IsValidHandler(loaded))
                        {
                            if (assetobj != null)
                            {
                                loaded(path, assetobj, LOADSTATUS.LOAD_SUCCESS);
                            }
                            else
                            {
                                loaded(path, assetobj, LOADSTATUS.LOAD_ERROR);
                            }
                        }
                    }
                    else
                    {
                        BundleLoader.loadAsset(path,
                            (url, obj, result) =>
                            {
                                if (obj != null && !m_dicResourceDownloaded.ContainsKey(path))
                                {
                                    m_dicResourceDownloaded[path] = obj;
                                    if (true == clearResource)
                                    {
                                        BundleLoader.unloadAssetResource(path);
                                    }
                                }
                                loaded(path, obj, result);
                            });
                    }
                }
            }
        }

        /// <summary>
        /// 预加载资源用，引用计数会加一.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <param name="loaded">Loaded.</param>
        private void loadRes<T>(string path, ObjectCacheInfo.LoadCallBack loaded, bool directly = false) where T:Object
        {
            Object temp = null;
            m_dicResourceDownloaded.TryGetValue(path, out temp);

            if (null != temp)
            {
                loaded(path, temp, LOADSTATUS.LOAD_SUCCESS);
            }
            else
            {
                T assetobj = null;

                if (false == KillerGlobal.useDynamicUpdate)
                {
                    assetobj = Resources.Load<T>(path);
                    if (null == assetobj)
                    {
                        assetobj = getDefaultResource<T>(path);
                    }

                    if (assetobj != null)
                    {
                        m_dicResourceDownloaded[path] = assetobj;
                    }
                    if (MoreGlobal.IsValidHandler(loaded))
                        loaded(path, assetobj, LOADSTATUS.LOAD_SUCCESS);
                }
                else
                {
                    if (builtin(path))
                    {
                        assetobj = Resources.Load<T>(path);

                        if (null == assetobj)
                        {
                            assetobj = getDefaultResource<T>(path);
                        }

                        if (assetobj != null)
                        {
                            m_dicResourceDownloaded[path] = assetobj;
                        }
                        if (MoreGlobal.IsValidHandler(loaded))
                        {
                            if (assetobj != null)
                            {
                                loaded(path, assetobj, LOADSTATUS.LOAD_SUCCESS);
                            }
                            else
                            {
                                loaded(path, assetobj, LOADSTATUS.LOAD_ERROR);
                            }
                        }
                        
                    }
                    else
                    {
                        if (false == directly)
                        {
                            if (false == BundleLoader.tryGetAsset(path, loaded))
                            {
                                ms_loadingManager = this;
                                addToLoadingLst(path, loaded, true);
                            }
                        }
                        else
                        {
                            BundleLoader.loadAsset(path,
                            (url, obj, result) =>
                            {
                                loaded(path, obj, result);
                                if (obj != null && !m_dicResourceDownloaded.ContainsKey(path))
                                {
                                    m_dicResourceDownloaded[path] = obj;
                                    BundleLoader.unloadAssetResource(path);
                                }
                            });
                        }
                    }
                }
            }
        }

        private void loadRes(List<string> path_list, ObjectCacheInfo.LoadProcess loadprocess)
        {
            int count = path_list.Count;
            int process = 0;
            for (int i = 0; i < count; i++)
            {
                LOADSTATUS loadStatus = LOADSTATUS.LOAD_SUCCESS;
                loadRes(path_list[i], (url, obj, result) =>
                {
                    process++;
                    float curpercent = (float)process / (float)count;

                    if (Mathf.Abs(curpercent - 1.0f) < Mathf.Epsilon)
                    {
                        for (int j = 0; j < count; ++j)
                        {
                            BundleLoader.unloadAssetResource(path_list[j]);
                        }
                    }
                    if (null == obj)
                    {
#if LOG_DETAIL
                        Debug.LogWarning("Load " + url + " error");
#endif
                        loadStatus = LOADSTATUS.LOAD_ERROR;
                    }

                    if (MoreGlobal.IsValidHandler(loadprocess))
                    {
                        loadprocess(curpercent, loadStatus);
                    }
                }, false);
            }
        }

        private void loadPreloadRes(List<string> path_list, ObjectCacheInfo.LoadProcess loadprocess)
        {
            int count = path_list.Count;
            int process = 0;
            for (int i = 0; i < count; i++)
            {
                LOADSTATUS loadStatus = LOADSTATUS.LOAD_SUCCESS;
                loadResDirectly(path_list[i], (url, obj, result) =>
                {
                    process++;
                    float curpercent = (float)process / (float)count;

                    if (Mathf.Abs(curpercent - 1.0f) < Mathf.Epsilon)
                    {
                        for (int j = 0; j < count; ++j)
                        {
                            BundleLoader.unloadAssetResource(path_list[j]);
                        }
                    }
                    if (null == obj)
                    {
#if LOG_DETAIL
                        Debug.LogWarning("Load " + url + " error");
#endif
                        loadStatus = LOADSTATUS.LOAD_ERROR;
                    }

                    if (MoreGlobal.IsValidHandler(loadprocess))
                    {
                        loadprocess(curpercent, loadStatus);
                    }
                });
            }
        }

        /// <summary>
        /// 释放资源用，引用计数会减一.
        /// </summary>
        /// <param name="path">Path.</param>
        private void unloadRes(string path)
		{
            if (builtin(path))
            {
                if (m_dicResourceDownloaded.ContainsKey(path))
                {
                    m_dicResourceDownloaded.Remove(path);
                }
                Resources.UnloadUnusedAssets();
            }
            else
            {
                BundleLoader.unloadAsset(path);
            }
		}

        /// <summary>
        /// 判断path路径对应的资源是否是内置编在首包中的
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool builtin(string path)
        {
#if ENABLE_PROFILER
            Profiler.BeginSample("ResourceManager.builtin");
#endif
            bool ret = true;

            if (KillerGlobal.useDynamicUpdate)
            {
                for (int i = 0; i < BundleConfig.ASSET_PATH_RELATIVE_PATH.Length; ++i)
                {
                    if (path.Contains(BundleConfig.ASSET_PATH_RELATIVE_PATH[i].assetRelativePath))
                    {
                        ret = false;
                    }
                }
                //foreach (KeyValuePair<MODULE_TYPE, string> item in KillerMainCfgManager.dicWndCfg)
                //{
                //    if (path == item.Value)
                //    {
                //        ret = false;
                //    }
                //}
                if(false == ret)
                {
                    for (int i = 0; i < BundleConfig.ASSET_IGNORE_ARR.Length; ++i)
                    {
                        if (path.EndsWith(BundleConfig.ASSET_IGNORE_ARR[i]))
                        {
                            ret = true;
                        }
                    }
                }
            }
            
            #if ENABLE_PROFILER
            Profiler.EndSample();
            #endif

            return ret;
        }

        private T getDefaultResource<T>(string url) where T:Object
        {
            string placeHolder = GetPlaceHolder(url);
            Object obj = Resources.Load<T>(placeHolder);
#if LOG_DETAIL
            Debug.LogWarning(url + "对应的资源不存在，使用 " + placeHolder + " 代替");
#endif
            return obj as T;
        }
    }
}


