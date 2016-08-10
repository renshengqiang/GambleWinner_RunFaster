using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

public class BuildIncrement 
{
	public static void BuildIncrementBundles(BuildTarget target)
	{
		/* 刷新一下数据 */
		//AssetWatcher.LoadFromFile ();

        /* 判断上次编的结果 */
        string bundlefile = BundleConfig.BUNDLE_CONFIG_FILE_PATH + BundleConfig.BUNDLE_CONFIG_FILE_NAME;
		/* 先判断bundle.xml存不存在 */
        if (File.Exists(bundlefile) == false)
		{
#if LOG_DETAIL
			Debug.Log("bundle.data不存在");
#endif
			return;
		}
		
		/* 打包要打包的资源 */
		List<string> file_new = new List<string> ();
		int count = AssetWatcher._file_list.Count;
		for(int i = 0; i < count; i++)
		{
			string filepath = AssetWatcher._file_list[i];
			/* 如果是meta文件继续 */
			bool b_ignore = BundleUtils.isIgnoreFile(filepath);
			if(b_ignore == true)
			{
				continue;
			}
			file_new.Add(filepath);
		}

		if(file_new.Count == 0)
		{
#if LOG_DETAIL
			Debug.Log("不存在资源更新");
#endif
			return;
		}

        /* decode上次编的结果 */
        string configPath = BundleConfig.BUNDLE_CONFIG_FILE_PATH + BundleConfig.BUNDLE_CONFIG_FILE_NAME;
        List<BundleInfo> list_bundle = new List<BundleInfo>();
        using (FileStream fileStream = File.OpenRead(configPath))
        {
            BundleInfoArray array = Serializer.Deserialize<BundleInfoArray>(fileStream);
            for (int i = 0; i < array.lstBundleInfo.Count; ++i)
            {
                BundleInfo bundle = array.lstBundleInfo[i];
                list_bundle.Add(bundle);
            }
        }

		/* 处理依赖关系 */
        List<string> file_list = new List<string>();
        for (int i = 0; i < BundleConfig.ASSET_PATH_RELATIVE_PATH.Length; ++i)
        {
            file_list.Add(Application.dataPath + "/Resources/" + BundleConfig.ASSET_PATH_RELATIVE_PATH[i].assetRelativePath);
        }
		AssetDepdenceTable deptable = new AssetDepdenceTable ();
        deptable.addAsset(AssetBundleTool.getFileList(file_list.ToArray()));

		List<string> asset_new = new List<string> ();
		foreach(string asset in file_new)
		{
			if(deptable.dicDep.ContainsKey(asset) == true)
			{
				asset_new.Add(asset);
			}
		}
            
		/* 保存合并前依赖关系 deptable保存的是当前完整的资源资源关系*/
		deptable.SaveAssetDepdenciesToFile(BundleConfig.ASSETDEPDENCIES_FILEPATH, BundleConfig.ASSETDEPDENCIES_FILENAME);
		/* 合并依赖关系 */
		deptable.MergeDependencies ();
		/* 保存合并后的依赖关系 */
		deptable.SaveAssetDepdenciesToFile(BundleConfig.ASSETDEPDENCIES_FILEPATH, BundleConfig.ASSETDEPDENCIES_AFTERMERAGE_FILENAME);
		
		BundleDepdenciesTable bundletable = new BundleDepdenciesTable ();
		bundletable.parseDepTable (deptable);

		List<string> list_asset = new List<string> ();
		deptable.getAssets (asset_new, list_asset);     /* 获取最新的更新资源和他们依赖的资源列表 */
		
        //foreach(string asset in list_asset)
        //{
        //    AssetBundleTool.BuildAssetBundle(asset, deptable, bundletable, target);
        //}
		
        //bundletable.renameBundles ();
		
		foreach(BundleInfo bundle in list_bundle)
		{
			bundletable.mergeBundleInfo(bundle);
		}

        bundletable.SaveBundleDepdenciesToFile(BundleConfig.BUNDLE_CONFIG_FILE_PATH, BundleConfig.BUNDLE_CONFIG_FILE_NAME);

		/* 清理工作 */
		AssetWatcher._file_list.Clear ();

        /* 清理多余的AB */
        bundletable.deleteUnusedAssetBundles(AssetBundleTool.getABList());
	}

    public static void ClearFileList()
    {
        System.DateTime time = System.DateTime.Now;
        string dest = BundleConfig.ASSETDEPDENCIES_FILEPATH + "FileList_" + time.Month.ToString("D2") + time.Day.ToString("D2") + "_" + time.Hour.ToString("D2") + time.Minute.ToString("D2") + ".xml";
        File.Move(BundleConfig.ASSETDEPDENCIES_FILEPATH + "FileList.xml", dest);
    }
}
