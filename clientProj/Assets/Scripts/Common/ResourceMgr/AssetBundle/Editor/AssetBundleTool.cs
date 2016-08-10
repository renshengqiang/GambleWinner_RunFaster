using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MoreFun;
using System.Text;

public class AssetBundleTool 
{
    public struct PackBundleItem
    {
        public string packName;
        public List<string> assets;
        public PackBundleItem(string _packName, List<string> _assets)
        {
            packName = _packName;
            assets = _assets;
        }
    }
    /// <summary>
    /// 自动打包，参数见BundleConfig.
    /// </summary>
    /// <param name="target">Target.</param>
	public static void BuildBundlesAuto(BuildTarget target)
	{
        List<string> file_list = new List<string>();
        List<PackBundleItem> packBundleList = new List<PackBundleItem>();
        for (int i = 0; i < BundleConfig.ASSET_PATH_RELATIVE_PATH.Length; ++i)
        {
            if (BundleConfig.ASSET_PATH_RELATIVE_PATH[i].pack == false)
            {
                file_list.Add(Application.dataPath + "/Resources/" + BundleConfig.ASSET_PATH_RELATIVE_PATH[i].assetRelativePath);
            }
            else
            {
                string[] paths = {Application.dataPath + "/Resources/" + BundleConfig.ASSET_PATH_RELATIVE_PATH[i].assetRelativePath};
                List<string> assets = getFileList(paths);
                packBundleList.Add(new PackBundleItem("Assets/Resources/" + BundleConfig.ASSET_PATH_RELATIVE_PATH[i].assetRelativePath, assets));
            }
        }

        //for (int i = 0; i < BundleConfig.LANGUAGE_ASSET_RELATIVE_PATH.Length; ++i)
        //{
        //    file_list.Add(Application.dataPath + "/Resources/" + BundleConfig.LANGUAGE_ASSET_RELATIVE_PATH[i].dir);
        //}

        List<string> asset_without_dep = new List<string>();
        //foreach (KeyValuePair<MODULE_TYPE, string> item in KillerMainCfgManager.dicWndCfg)
        //{
        //    asset_without_dep.Add("Assets/Resources/" + item.Value + ".prefab");
        //}

        // 先编译整包资源，然后编译语言资源
        BuildBundles(getFileList(file_list.ToArray()),packBundleList,asset_without_dep,target);

        // 编译多语言包
        //FileUtils.ClearDir(BundleConfig.LAN_ZIP_BUNDLE_OUT_PATH);
        //BuildLanAssets(target);
	}

    /// <summary>
    /// 自动打包场景，参数见BundleConfig
    /// </summary>
    /// <param name="target"></param>
    public static void BuildStreamScenesAuto(BuildTarget target)
    {
        //List<StreamSceneInfo> scene_list = getSceneList();
        //BuildScenes(scene_list, target);
    }

    /// <summary>
    /// 打包场景
    /// </summary>
    /// <param name="lstStreamSceneInfo"></param>
    /// <param name="target"></param>
    public static void BuildScenes(List<StreamSceneInfo> lstStreamSceneInfo, BuildTarget target)
    {
        List<StreamSceneBuildState> lstSize = new List<StreamSceneBuildState>();
        foreach (StreamSceneInfo info in lstStreamSceneInfo)
        {
            string md5Name = BundleManager.parseAssetName2BundleName(info.stream_scene);
            List<string> levels = new List<string>();
            foreach (string scene in info.scene_list)
            {
                levels.Add(BundleConfig.SCENE_FILE_PREFIX + scene + BundleConfig.SCENE_FILE_SUFFIX);
            }

            string filePath = BundleConfig.STREAM_SCENE_OUT_PATH + md5Name + BundleConfig.BUNDLESUFFIX;
            BuildPipeline.BuildStreamedSceneAssetBundle(levels.ToArray(), BundleConfig.STREAM_SCENE_OUT_PATH + md5Name + BundleConfig.BUNDLESUFFIX, target);

            StreamSceneBuildState state = new StreamSceneBuildState();
            state.bundle_name = md5Name;
            state.size = FileUtils.GetFileSize(filePath);
            state.md5 = FileUtils.getFileMd5(filePath);
            lstSize.Add(state);
        }

        string xml = SceneBundleInfoXML.encode(lstSize);
        FileUtils.SaveToFile(xml, BundleConfig.STREAM_SCENE_OUT_PATH, BundleConfig.STREAM_SCENE_FILENAME);
    }

    /// <summary>
    /// 打包assetlist资源.
    /// </summary>
    /// <param name="assetlist">单个资源单独打包的列表</param>
    /// <param name="sepratePackLst">sepratePackLst. 单独需要整体打包的列表</param>
    /// /// <param name="assetsWithoutDependency">assetsWithoutDependency 单独打包的 prefab，不考虑依赖关系</param>
    /// <param name="target">Target.</param>
    public static void BuildBundles(List<string> assetlist, List<PackBundleItem> sepratePackLst, List<string> assetsWithoutDependency, BuildTarget target, string languageCode = null)
	{
        AssetDepdenceTable deptable = new AssetDepdenceTable ();
        /* 添加依赖关系 */
		deptable.addAsset(assetlist);
        /* 添加不考虑依赖关系的资源 */
        deptable.assAssetWithoutDependency(assetsWithoutDependency);
        /* 保存合并前依赖关系 */
		deptable.SaveAssetDepdenciesToFile(BundleConfig.ASSETDEPDENCIES_FILEPATH, BundleConfig.ASSETDEPDENCIES_FILENAME);
        /* 合并依赖关系 */
		deptable.MergeDependencies ();
		/* 保存合并后的依赖关系 */
		deptable.SaveAssetDepdenciesToFile(BundleConfig.ASSETDEPDENCIES_FILEPATH, BundleConfig.ASSETDEPDENCIES_AFTERMERAGE_FILENAME);

		BundleDepdenciesTable bundletable = new BundleDepdenciesTable ();
		bundletable.parseDepTable (deptable);

		/* 创建AssetBundle */
		BuildAssetBundle (deptable, bundletable, target, languageCode);
		
        /* 加入整体打包Item */
        for (int i = 0; sepratePackLst != null && i < sepratePackLst.Count; ++i)
        {
            bundletable.addPackResources(sepratePackLst[i]);
        }

        /* 每个整体打包的资源直接重新打包 */
        for (int i = 0; sepratePackLst != null && i < sepratePackLst.Count; ++i)
        {
            BuildAssetBundle(sepratePackLst[i], target);
        }
        
        /* 重命名最后生成的ab */
        bundletable.renameBundles(languageCode);

        if (languageCode == null)
        {
            /* 保存依赖关系 只在编译全包的时候才保存这个文件*/
            bundletable.SaveBundleDepdenciesToFile(BundleConfig.BUNDLE_CONFIG_FILE_PATH, BundleConfig.BUNDLE_CONFIG_FILE_NAME);

            /* 删除多余的ab，只保留这次编出来的AB */
            bundletable.deleteUnusedAssetBundles(getABList());
        }
	}

	#region 遍历文件

	/// <summary>
	/// 获取文件列表.
    /// 这里获取的文件名是 Assets/path.suffix 这样的路径名
	/// </summary>
	/// <returns>The file list.</returns>
	/// <param name="directory_list">Directory_list.</param>
	public static List<string> getFileList(string[] directory_list)
	{
		List<string> file_list = new List<string> ();

		int dir_len = directory_list.Length;
		for(int i = 0; i < dir_len; i++)
		{
			string dirpath = directory_list[i];
			/* 遍历所有文件 枚举所有依赖 */
			DirectoryInfo directory = new DirectoryInfo(dirpath);
			FileInfo[] dirs = directory.GetFiles(BundleConfig.SEARCH_FILE_TYPE, SearchOption.AllDirectories);

			/* 遍历所有Prefab */
			foreach (FileInfo info in dirs)
			{
				/* 如果是meta文件继续 */
                string fullname = info.FullName.Replace('\\', '/');
                bool b_ignore = BundleUtils.isIgnoreFile(fullname);
				if(b_ignore == true)
				{
					continue;
				}

				string assetTemp = "Assets" + info.FullName.Substring(Application.dataPath.Length);
				
				string assetpath = assetTemp.Replace("\\", "/");
				
				/* 把遍历到的资源路径存起来 */
				file_list.Add(assetpath);
			}
		}

		return file_list;
	}

    /// <summary>
    /// 获取所有的AB以备删除多余
    /// </summary>
    /// <returns></returns>
    public static List<string> getABList()
    {
        List<string> file_list = new List<string>();
        DirectoryInfo dir = new DirectoryInfo(BundleConfig.BUNDLE_OUT_PATH);
        FileInfo[] files = dir.GetFiles("*" + BundleConfig.BUNDLESUFFIX, SearchOption.TopDirectoryOnly);
        foreach (FileInfo info in files)
        {
            file_list.Add(info.Name);
        }
        return file_list;
    }

    /// <summary>
    /// 获取要打包的场景列表
    /// </summary>
    /// <returns></returns>
    //public static List<StreamSceneInfo> getSceneList()
    //{
    //    List<StreamSceneInfo> lstScene = new List<StreamSceneInfo>();
    //    SCENE_CONF_ARRAY array = DataConfigManager.GetOneDataConfig<SCENE_CONF_ARRAY>();

    //    Dictionary<string, List<string> > dicScene = new Dictionary<string,List<string>>();
    //    foreach(SCENE_CONF conf in array.items)
    //    {
    //        if (0 == conf.builtin)
    //        {
    //            if (!dicScene.ContainsKey(conf.bundle))
    //            {
    //                StreamSceneInfo info = new StreamSceneInfo();
    //                info.stream_scene = conf.bundle;
    //                info.scene_list = new List<string>();
    //                lstScene.Add(info);
    //                dicScene.Add(info.stream_scene, info.scene_list);
    //            }
    //            List<string> lst = dicScene[conf.bundle];
    //            lst.Add(conf.scene_name);
    //        }
    //    }

    //    return lstScene;
    //}
	#endregion

	#region 创建AssetBundle

	private static Dictionary<string, bool> dicAssetBundleBuilded = new Dictionary<string, bool> ();

    private static List<BundleInfo> list_buildstate = new List<BundleInfo>();

    public static void BuildAssetBundle(AssetDepdenceTable deptable, BundleDepdenciesTable bundletable, BuildTarget target, string languageCode)
	{
        if (Directory.Exists(BundleConfig.BUNDLE_OUT_PARENT_PATH))
        {
            Directory.Delete(BundleConfig.BUNDLE_OUT_PARENT_PATH, true);
        }
        if (false == Directory.Exists(BundleConfig.BUNDLE_OUT_PARENT_PATH))
        {
            Directory.CreateDirectory(BundleConfig.BUNDLE_OUT_PARENT_PATH);
        }
        if (Directory.Exists(BundleConfig.BUNDLE_OUT_PATH))
        {
            Directory.Delete(BundleConfig.BUNDLE_OUT_PATH, true);
        }
		if(Directory.Exists(BundleConfig.BUNDLE_OUT_PATH) == false)
		{
			Directory.CreateDirectory(BundleConfig.BUNDLE_OUT_PATH);
		}
        if (Directory.Exists(BundleConfig.BUNDLE_CONFIG_FILE_PATH))
        {
            Directory.Delete(BundleConfig.BUNDLE_CONFIG_FILE_PATH, true);
        }
        if (Directory.Exists(BundleConfig.BUNDLE_CONFIG_FILE_PATH) == false)
        {
            Directory.CreateDirectory(BundleConfig.BUNDLE_CONFIG_FILE_PATH);
        }
        if (Directory.Exists(BundleConfig.BUNDLE_OUT_PATH_TMP) == false)
        {
            Directory.CreateDirectory(BundleConfig.BUNDLE_OUT_PATH_TMP);
        }
		foreach(string asset in deptable.dicDep.Keys)
		{
			BuildAssetBundle(asset, deptable, bundletable, target, languageCode);
		}
	}

    public static void BuildAssetBundle(PackBundleItem packItem, BuildTarget target)
    {
        string filepath = BundleConfig.BUNDLE_OUT_PATH + UrlUtil.parseAssetPathToBundleName(packItem.packName) + BundleConfig.BUNDLESUFFIX;
        List<Object> lstObjs = new List<Object>();
        List<string> lstPaths = new List<string>();
        uint crc = 0;

        for (int i = 0; i < packItem.assets.Count; ++i)
        {
            Object obj = AssetDatabase.LoadAssetAtPath(packItem.assets[i], typeof(Object));
            lstObjs.Add(obj);

            // 去除文件前路径和后缀
            string path = packItem.assets[i];
            path = UrlUtil.RemoveFileSuffix(path);
            path = path.Replace("Assets/Resources/", "");
            lstPaths.Add(path);
        }
        BuildHelper.BuildAssetBundle(lstObjs, lstPaths, filepath, out crc, target);
    }

    public static void BuildAssetBundle(string asset, AssetDepdenceTable deptable, BundleDepdenciesTable bundletable, BuildTarget target, string langaugeCode)
	{
		int pushcount = 0;
		dicAssetBundleBuilded.Clear();
        pushcount = BuildBundle(asset, deptable, bundletable, target, true, langaugeCode);
		for(int i = 0; i < pushcount; i++)
		{
			BuildPipeline.PopAssetDependencies();
#if LOG_DETAIL
			Debug.Log("/*-----------PopAssetDependencies()");
#endif
		}
	}

    private static int BuildBundle(string assetpath, AssetDepdenceTable deptable, BundleDepdenciesTable bundletable, BuildTarget target, bool firstcall, string languageCode)
	{

		int pushcount = 0;
		string filename = "";
		string depBundles = "";

		if(assetpath != null && assetpath.Length != 0)
		{
			List<string> assetDepList;
			deptable.dicDep.TryGetValue(assetpath, out assetDepList);

            BundleInfo state = bundletable.getBuildState(assetpath);
			
			uint crc = 0;
			Object obj = AssetDatabase.LoadAssetAtPath(assetpath, typeof(Object));
			
			if(obj == null)
			{
#if LOG_DETAIL
				Debug.LogError(assetpath+"没有资源！");
#endif
				return 0 ;
			}
			
			filename = UrlUtil.parseAssetPathToBundleName(assetpath);
            string formalPath = languageCode == null ? BundleConfig.BUNDLE_OUT_PATH : BundleConfig.LAN_BUNDLE_OUT_PATH + languageCode + "/";
			string filepath = (firstcall ? BundleConfig.BUNDLE_OUT_PATH : BundleConfig.BUNDLE_OUT_PATH_TMP) + filename + BundleConfig.BUNDLESUFFIX;
			
			if(assetDepList == null || assetDepList.Count == 0)
			{
				/* 如果没有依赖的资源了，那么就直接打包 */
				BuildPipeline.PushAssetDependencies();
#if LOG_DETAIL
				Debug.Log("/*-----------PushAssetDependencies()");
#endif
				pushcount ++;

                bool needExtra = false;
                if(needExtra)
                {
                    BuildHelper.BuildAssetBundleExtra(obj, filepath, out crc, target);
                }
                else
                {
                    BuildHelper.BuildAssetBundle(obj, filepath, out crc, target);
                }

#if LOG_DETAIL
				Debug.Log("/*-----------BuildAssetBundle(" + assetpath + ")");
#endif
				//dicAssetBundleBuilded.Add(filename, true);
			}
			else
			{
				//Debug.LogWarning ("-*-CreateBundle:PushAssetDependencies");
				/* 先递归调用打包 */
				for(int i = 0; i < assetDepList.Count; i++)
				{
					if(i > 0)
					{
						depBundles += ";";
					}
					pushcount += BuildBundle(assetDepList[i], deptable, bundletable, target, false, languageCode);
				}

				BuildPipeline.PushAssetDependencies();
#if LOG_DETAIL
				Debug.Log("/*-----------PushAssetDependencies()");
#endif
				pushcount ++;

				BuildHelper.BuildAssetBundle(obj, filepath, out crc, target);
#if LOG_DETAIL
				Debug.Log("/*-----------BuildAssetBundle(" + assetpath + ")");
#endif
            }

            obj = null;
			EditorUtility.UnloadUnusedAssets();
        }
        else
        {
#if LOG_DETAIL
            Debug.LogError("空资源不可以打包！");
#endif
        }
        
		return pushcount;
	}

	#endregion
}
