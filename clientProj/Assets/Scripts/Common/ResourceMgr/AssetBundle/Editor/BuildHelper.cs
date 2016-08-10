using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Security.Cryptography;
using System.Collections.Generic;

public class BuildHelper 
{
	/// <summary>
	/// Gets the build asset opts.
	/// </summary>
	/// <value>The build asset opts.</value>
	private static BuildAssetBundleOptions BuildAssetOpts
	{
		get
		{
			return	(BundleConfig.BUNDLE_COMPRESS ? 0 : BuildAssetBundleOptions.UncompressedAssetBundle) |
				(BundleConfig.BUNDLE_DETERMINISTIC ? BuildAssetBundleOptions.DeterministicAssetBundle : 0) |
					BuildAssetBundleOptions.CollectDependencies |
					BuildAssetBundleOptions.CompleteAssets;
		}
	}

    public static string getOutBundleFilePath(string filename, string languageCode)
	{
        if (null == languageCode)
            return BundleConfig.BUNDLE_OUT_PATH + filename + BundleConfig.BUNDLESUFFIX;
        else
            return BundleConfig.LAN_BUNDLE_OUT_PATH + languageCode + "/" + filename + BundleConfig.BUNDLESUFFIX;
	}

    public static long getBundleSize(string filename, string langaungeCode)
	{
		string bundlepath = getOutBundleFilePath (filename, langaungeCode);
		long bundlesize = FileUtils.GetFileSize (bundlepath);
		return bundlesize;
	}

    public static string getBundleMd5(string filename, string languageCode)
    {
        string bundlepath = getOutBundleFilePath(filename, languageCode);
        string md5 = FileUtils.getFileMd5(bundlepath);
        return md5;
    }

    public static void renameBundle(string filename, string md5, string languageCode)
    {
        string bundlename = null;
        if (null == languageCode)
            bundlename = BundleConfig.BUNDLE_OUT_PATH + filename;
        else
            bundlename = BundleConfig.LAN_BUNDLE_OUT_PATH + languageCode + "/" + filename;

        string bundlepath = bundlename + BundleConfig.BUNDLESUFFIX;
        string distination = bundlename + "_" + md5 + BundleConfig.BUNDLESUFFIX;
        try
        {
            if (File.Exists(distination))
            {
                File.Delete(distination);
            }
            FileUtils.renameFile(bundlepath, distination);
        }
        catch (System.Exception ex)
        {
#if LOG_DETAIL
            Debug.LogError(bundlepath + "Error:" + ex.ToString());
#endif
        }
    }

    public static bool BuildAssetBundle(Object assetobj, string filename, out uint crc, BuildTarget target)
	{
		bool result = false;
        result = BuildPipeline.BuildAssetBundle(assetobj, null, filename, out crc, BuildAssetOpts, target); 
		return result;
	}

    public static bool BuildAssetBundle(List<Object> assetObjs, List<string> paths, string filename, out uint crc, BuildTarget target)
    {
        bool result = false;
        result = BuildPipeline.BuildAssetBundleExplicitAssetNames(assetObjs.ToArray(), paths.ToArray(), filename, out crc, BuildAssetOpts, target);
        return result;
    }

    /// <summary>
    /// 编 assetbundle 之前将依赖的 KLUIListView 组件转换为直接拖拽的依赖
    /// 编 assetbundle 完成以后将该 prefab 和它所依赖的 prefab 资源从 Resource 目录中删除
    /// 编 assetbundle 之前将依赖的 sprite 设置成相同的 tag，编完之后将 Tag 清空，防止资源被误入其他界面中
    /// </summary>
    /// <param name="assetobj"></param>
    /// <param name="filename"></param>
    /// <param name="crc"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static bool BuildAssetBundleExtra(Object assetobj, string filename, out uint crc, BuildTarget target)
    {
        bool result = false;
        string assetpath = AssetDatabase.GetAssetPath(assetobj);
        //FontTool.TranformListViewToDirectlyDepend(assetpath);
        //FontTool.TransformTextToMoretext(assetpath);
        //FontTool.SetDependencySpritesTag(assetpath);
        result = BuildPipeline.BuildAssetBundle(assetobj, null, filename, out crc, BuildAssetOpts, target);
        string[] dependencies = AssetDatabase.GetDependencies(new string[] { assetpath });
        Debug.Log("BuildAssetBundleExtra: " + assetpath);
        foreach (string asset in dependencies)
        {
            if (asset.StartsWith("Assets/Resources/") && asset.EndsWith(".prefab"))
            {
                Debug.Log("Delete " + assetpath + " " + asset);

                AssetDatabase.DeleteAsset(asset);
            }
            Debug.Log(asset);
        }
        //FontTool.ClearDependencySpritesTag(assetpath);
        AssetDatabase.Refresh();
        return result;
    }
}
