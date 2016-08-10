using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BundleMenu 
{
    [MenuItem("Killer/BundleTool/CreateBundles Windows", false, 0)]
    public static void BuildWindows()
    {
        System.DateTime start_time = System.DateTime.Now;

        AssetBundleTool.BuildBundlesAuto(BuildTarget.StandaloneWindows);

        string usingtime = (System.DateTime.Now - start_time).ToString();
        EditorUtility.DisplayDialog("提示", "构建AssetBundle——windows完成，耗时：" + usingtime, "确定");
    }

    [MenuItem("Killer/BundleTool/CreateBundles Android", false, 0)]
	public static void BuildAndroid()
	{
		System.DateTime start_time = System.DateTime.Now;

		AssetBundleTool.BuildBundlesAuto (BuildTarget.Android);
		
		string usingtime = (System.DateTime.Now - start_time).ToString();
		EditorUtility.DisplayDialog ("提示", "构建AssetBundle——android完成，耗时：" + usingtime, "确定");
	}

    [MenuItem("Killer/BundleTool/CreateBundles IOS", false, 1)]
	public static void BuildIOS()
	{
		System.DateTime start_time = System.DateTime.Now;

		AssetBundleTool.BuildBundlesAuto (BuildTarget.iPhone);

		string usingtime = (System.DateTime.Now - start_time).ToString();
		EditorUtility.DisplayDialog ("提示", "构建AssetBundle——iphone完成，耗时：" + usingtime, "确定");
	}


    [MenuItem("Killer/BundleTool/CreateBundles Webplayer", false, 2)]
    public static void BuildWebPlayer()
    {
        System.DateTime start_time = System.DateTime.Now;

        AssetBundleTool.BuildBundlesAuto(BuildTarget.WebPlayer);

        string usingtime = (System.DateTime.Now - start_time).ToString();
        EditorUtility.DisplayDialog("提示", "构建AssetBundle——Webplayer完成，耗时：" + usingtime, "确定");
    }

    [MenuItem("Killer/BundleTool/CreateBundles Increasement(Android)", false, 3)]
    public static void BuildIncreasementAndroid()
    {
        System.DateTime start_time = System.DateTime.Now;

        BuildIncrement.BuildIncrementBundles(BuildTarget.Android);

        string usingtime = (System.DateTime.Now - start_time).ToString();
        EditorUtility.DisplayDialog("提示", "构建AssetBundle——Android完成，耗时：" + usingtime, "确定");
    }

    [MenuItem("Killer/BundleTool/CreateBundles Increasement(IOS)", false, 4)]
    public static void BuildIncreasementIOS()
    {
        System.DateTime start_time = System.DateTime.Now;

        BuildIncrement.BuildIncrementBundles(BuildTarget.iPhone);

        string usingtime = (System.DateTime.Now - start_time).ToString();
        EditorUtility.DisplayDialog("提示", "构建AssetBundle——iphone完成，耗时：" + usingtime, "确定");
    }

    [MenuItem("Killer/BundleTool/CreateBundles By Selection(Android)", false, 5)]
    public static void BuildBySelectionAndroid()
    {
        System.DateTime start_time = System.DateTime.Now;

        CreateBySelection.BuildSelection(BuildTarget.Android);

        string usingtime = (System.DateTime.Now - start_time).ToString();
        EditorUtility.DisplayDialog("提示", "构建AssetBundle——iphone完成，耗时：" + usingtime, "确定");
    }

    [MenuItem("Killer/BundleTool/CreateBundles By Selection(IOS)", false, 6)]
    public static void BuildBySelectionIOS()
    {
        System.DateTime start_time = System.DateTime.Now;


        CreateBySelection.BuildSelection(BuildTarget.iPhone);

        string usingtime = (System.DateTime.Now - start_time).ToString();
        EditorUtility.DisplayDialog("提示", "构建AssetBundle——iphone完成，耗时：" + usingtime, "确定");
    }

    [MenuItem("Killer/BundleTool/BuildStreamScene Android", false, 7)]
    public static void BuildStreamSceneAndroid()
    {
        System.DateTime start_time = System.DateTime.Now;
        AssetBundleTool.BuildStreamScenesAuto(BuildTarget.Android);
        string usingtime = (System.DateTime.Now - start_time).ToString();
        EditorUtility.DisplayDialog("提示", "构建StreamScene——Android完成，耗时：" + usingtime, "确定");
    }

    [MenuItem("Killer/BundleTool/BuildStreamScene IOS", false, 8)]
    public static void BuildStreamSceneIOS()
    {
        System.DateTime start_time = System.DateTime.Now;
        AssetBundleTool.BuildStreamScenesAuto(BuildTarget.iPhone);
        string usingtime = (System.DateTime.Now - start_time).ToString();
        EditorUtility.DisplayDialog("提示", "构建StreamScene——iPhone完成，耗时：" + usingtime, "确定");
    }

    [MenuItem("Killer/BundleTool/Clearn Rename", false, 9)]
    public static void CleanRename()
    {
        System.DateTime start_time = System.DateTime.Now;

        List<string> file_list = FileUtils.getFileList(BundleConfig.BUNDLE_OUT_PATH, "*.assetbundle");

        foreach (string bundlename in file_list)
        {
            string[] tmp = bundlename.Split('\\');
            string filename = tmp[tmp.Length - 1];
            if (filename.Contains(BundleConfig.BUNDLESUFFIX) && filename.Contains("_"))
            {
                int index = filename.IndexOf('_');
                int tmplen = filename.Length - index;
                string name = bundlename.Substring(0, bundlename.Length - tmplen);

                FileUtils.renameFile(bundlename, name + BundleConfig.BUNDLESUFFIX);
            }
        }

        string usingtime = (System.DateTime.Now - start_time).ToString();
        EditorUtility.DisplayDialog("提示", "Clearn Rename完成，耗时：" + usingtime, "确定");
    }

    [MenuItem("Killer/BundleTool/CleanBuildEnv", false, 10)]
    public static void CleanBuildEnv()
    {
        CleanBuild.PreBuild();
    }

    [MenuItem("Killer/BundleTool/UndoCleanBuildEnv", false, 11)]
    public static void UndoCleanBuildEnv()
    {
        CleanBuild.PostBuild();
    }

    [MenuItem("Killer/BundleTool/CheckSprite", false, 12)]
    public static void ParseSprite()
    {
        SpritePackerTool.ParseSprites();
    }
}
