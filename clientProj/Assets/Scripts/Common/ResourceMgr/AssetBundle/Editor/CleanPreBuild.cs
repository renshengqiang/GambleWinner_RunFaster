using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CleanBuild 
{
    // 清空Build环境
    public static void CleanBuildEnv()
    {
        checkResourcesAssetDir("Assets/tmpResources/");
        List<string> file_list = new List<string>();
        for (int i = 0; i < BundleConfig.ASSET_PATH_RELATIVE_PATH.Length; ++i)
        {
            file_list.Add(Application.dataPath + "/Resources/" + BundleConfig.ASSET_PATH_RELATIVE_PATH[i].assetRelativePath);
        }
        file_list = AssetBundleTool.getFileList(file_list.ToArray());
        for (int i = 0; i < file_list.Count; i++)
        {
            string assetpath = file_list[i];
            copyAssetFileToTemp(assetpath);
            //AssetDatabase.MoveAsset(assetpath, getMoveTargetAssetPath(assetpath));
        }
        AssetDatabase.Refresh();
#if LOG_DETAIL
        Debug.Log("Move File To TempDir Over!");
#endif
    }

    // 创建版本文件
    public static void CreateVersionFile(string version)
    {
        string versionFile = Application.streamingAssetsPath + "/Config/version";

        if (File.Exists(versionFile)) File.Delete(versionFile);
        FileUtils.CreateAndWriteToFile(versionFile, version);
    }

	public static void PreBuild()
	{
		checkResourcesAssetDir ("Assets/tmpResources/");

        List<string> file_list = new List<string>();
        for (int i = 0; i < BundleConfig.ASSET_PATH_RELATIVE_PATH.Length; ++i)
        {
            if (BundleConfig.ASSET_PATH_RELATIVE_PATH[i].assetRelativePath != "Local/translate/")
            {
                file_list.Add(Application.dataPath + "/Resources/" + BundleConfig.ASSET_PATH_RELATIVE_PATH[i].assetRelativePath);
            }
        }
        file_list = AssetBundleTool.getFileList(file_list.ToArray());

        for (int i = 0; i < file_list.Count; i++)
        {
            string assetpath = file_list[i];
            copyAssetFileToTemp(assetpath);
        }
        //copyAssetBundleToStreamAssets();

		AssetDatabase.Refresh ();
#if LOG_DETAIL
		Debug.Log ("Move File To TempDir Over!");
#endif
	}
	
	public static void PostBuild()
	{
		/* 遍历所有文件 枚举所有依赖 */
		DirectoryInfo directory = new DirectoryInfo(Application.dataPath + "/../tmpResources/");
		FileInfo[] dirs = directory.GetFiles("*.*", SearchOption.AllDirectories);

		/* 遍历所有Prefab */
		foreach (FileInfo info in dirs)
		{
			string assetpath = info.FullName;
			copyAssetFileToResources(assetpath);
		}

        deleteStreamAssetsAssetBundles();
		AssetDatabase.Refresh ();
    }
    
    public static string getMoveTargetAssetPath(string path)
	{
		return path.Replace ("Resources", "tmpResources");
	}

	public static string getResourcesAssetPath(string path)
	{
		return path.Replace ("tmpResources", "Resources");
	}

	public static void checkResourcesAssetDir(string path)
	{
		string targetpath = path.Replace ("Assets/Resources/", Application.dataPath + "/../tmpResources/");
		int index = targetpath.LastIndexOf ('/');
		string dir = targetpath.Substring (0, index);
        try
        {
            if (false == Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        catch
        {
#if LOG_DETAIL
            Debug.LogWarning("Create directory " + dir + " Error");
#endif
        }
	}

	static void copyAssetFileToTemp (string assetpath)
	{
		checkResourcesAssetDir(assetpath);
		string targetpath = assetpath.Replace ("Assets/Resources/", Application.dataPath + "/../tmpResources/");
        try
        {
            if (File.Exists(targetpath))
            {
                File.Delete(targetpath);
            }
            if (File.Exists(targetpath + ".meta"))
            {
                File.Delete(targetpath + ".meta");
            }
            File.Move(Application.dataPath + "/../" + assetpath, targetpath);
            File.Move(Application.dataPath + "/../" + assetpath + ".meta", targetpath + ".meta");
        }
        catch
        {
#if LOG_DETAIL
            Debug.Log("Move " + assetpath + " error");
#endif
        }
	}

    static void deleteStreamAssetsAssetBundles()
    {
        DirectoryInfo directory = new DirectoryInfo(Application.streamingAssetsPath + "/AssetBundle");
        if (directory != null)
        {
            directory.Delete(true);
        }
        Directory.CreateDirectory(Application.streamingAssetsPath + "/AssetBundle");
        Directory.CreateDirectory(Application.streamingAssetsPath + "/AssetBundle/Config");
    }

    static void copyAssetBundleToStreamAssets()
    {
        deleteStreamAssetsAssetBundles();

        // 移动 bundles
        DirectoryInfo bundle_directory = new DirectoryInfo(BundleConfig.BUNDLE_OUT_PATH);
        FileInfo[] dirs = bundle_directory.GetFiles("*", SearchOption.AllDirectories);

        foreach (FileInfo info in dirs)
        {
            string assetpath = info.FullName;
#if LOG_DETAIL
            Debug.Log(assetpath);
#endif
            copyBundleFileToStreamAssets(assetpath);
        }
    }

    static void copyBundleFileToStreamAssets(string assetpath)
    {
        string pathTemp = assetpath.Replace("\\", "/");
        string targetpath = pathTemp.Replace("/assetbundle/", "/Assets/StreamingAssets/assetbundle/");
        try
        {
            File.Copy(assetpath, targetpath);
        }
        catch
        {
#if LOG_DETAIL
            Debug.LogWarning("Copy " + assetpath + " Error");
#endif
        }
    }

	static void copyAssetFileToResources (string assetpath)
	{
        string pathTemp = assetpath.Replace("\\", "/");
        string targetpath = pathTemp.Replace("/tmpResources/", "/Assets/Resources/");

        try
        {
            File.Move(assetpath, targetpath);
        }
        catch
        {
#if LOG_DETAIL
            Debug.LogWarning("Copy " + assetpath + " Error");
#endif
        }
	}
}
