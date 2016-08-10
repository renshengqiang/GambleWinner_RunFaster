using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CreateBySelection 
{
	public static void BuildSelection (BuildTarget target)
	{
		List<BundleInfo> list_bundle = null;

		List<string> assetlist = GetSelectionAssetPathList ();
		if(assetlist.Count == 0)
		{
			EditorUtility.DisplayDialog("错误", "必须选择要构建的文件", "确定");
			return;
		}

		/* 检查原来bundle.xml的文件存不存在 */
		string bundlexmlfile = BundleConfig.BUNDLE_CONFIG_FILE_PATH + BundleConfig.BUNDLE_CONFIG_FILE_NAME;
		if(File.Exists(bundlexmlfile) == false)
		{
			EditorUtility.DisplayDialog("错误", "bundle.xml文件不存在", "确定");
			return;
		}
		else
		{
			byte[] listbytes = FileUtils.getFileBytes (bundlexmlfile);

			if(listbytes.Length > 0)
			{
				string strlistxml = System.Text.Encoding.ASCII.GetString(listbytes);
				list_bundle = BundleListXML.decode(strlistxml);
			}
			else
			{
#if LOG_DETAIL
				Debug.LogWarning("bundle.xml文件存在，但是长度为0");
#endif
			}
		}


        List<string> file_list = new List<string>();
        for (int i = 0; i < BundleConfig.ASSET_PATH_RELATIVE_PATH.Length; ++i)
        {
            file_list.Add(Application.dataPath + "/Resources/" + BundleConfig.ASSET_PATH_RELATIVE_PATH[i].assetRelativePath);
        }
        file_list = AssetBundleTool.getFileList(file_list.ToArray());

		/* 处理依赖关系 */
		AssetDepdenceTable deptable = new AssetDepdenceTable ();
		deptable.addAsset (file_list);
		/* 保存合并前依赖关系 */
		deptable.SaveAssetDepdenciesToFile(BundleConfig.ASSETDEPDENCIES_FILEPATH, BundleConfig.ASSETDEPDENCIES_FILENAME);
		/* 合并依赖关系 */
		deptable.MergeDependencies ();
		/* 保存合并后的依赖关系 */
		deptable.SaveAssetDepdenciesToFile(BundleConfig.ASSETDEPDENCIES_FILEPATH, BundleConfig.ASSETDEPDENCIES_AFTERMERAGE_FILENAME);
		
		BundleDepdenciesTable bundletable = new BundleDepdenciesTable ();
		bundletable.parseDepTable (deptable);

		if(Directory.Exists(BundleConfig.BUNDLE_OUT_PATH) == true)
		{
			//Directory.Delete(BundleConfig.BUNDLE_OUT_PATH, true);
		}
		Directory.CreateDirectory(BundleConfig.BUNDLE_OUT_PATH);

		List<string> list_asset = new List<string> ();
		deptable.getAssets (assetlist, list_asset);

        //foreach(string asset in list_asset)
        //{
        //    AssetBundleTool.BuildAssetBundle(asset, deptable, bundletable, target);
        //}

        //bundletable.renameBundles ();


		foreach(BundleInfo bundle in list_bundle)
		{
			bundletable.mergeBundleInfo(bundle);
		}
		
		bundletable.SaveBundleDepdenciesToFile (BundleConfig.BUNDLE_CONFIG_FILE_PATH, BundleConfig.BUNDLE_CONFIG_FILE_NAME);
	}

	public static List<string> GetSelectionAssetPathList()
	{
		List<string> list = new List<string>();

		foreach(Object obj in Selection.objects)
		{
			string path = AssetDatabase.GetAssetPath(obj);

			list.Add(path);
		}

		return list;
	}
}
