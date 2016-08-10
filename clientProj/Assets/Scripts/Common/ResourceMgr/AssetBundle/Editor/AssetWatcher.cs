using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MoreFun;

public class AssetWatcher : AssetPostprocessor 
{
	public static bool Active = false;

	private static bool isFileLoaded = false;

	public static List<string> _file_list = new List<string> ();
	
	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
#if ENABLE_PROFILER
        Profiler.BeginSample("AssetWatcher");
#endif
		if(Active)
        {
            if(isFileLoaded == false)
            {
                LoadFromFile();
            }
            
            bool isNeedWrite = false;
            foreach(string asset in importedAssets)
            {
                #if LOG_DETAIL
                Debug.Log("Import Asset:" + asset);
                #endif
                if(_file_list.Contains(asset) == false)
                {
                    if(asset.Contains("Assets/StreamingAssets/") == true)
                    {
                        continue;
                    }
                    _file_list.Add(asset);
                    isNeedWrite = true;
                }
            }
            
            if(isNeedWrite == true)
            {
                WriteToFile();
            }
        }
        #if ENABLE_PROFILER
        Profiler.EndSample();
        #endif

	}

	static void WriteToFile ()
	{
		string xml = ImporterFileDataXml.encode (_file_list);

		FileUtils.SaveToFile (xml, BundleConfig.ASSETDEPDENCIES_FILEPATH, "FileList.xml");
	}

	public static void LoadFromFile ()
	{
		isFileLoaded = true;
		byte[] bytes = FileUtils.getFileBytes (BundleConfig.ASSETDEPDENCIES_FILEPATH + "FileList.xml");

		string xml = System.Text.Encoding.Default.GetString (bytes);

		List<string> filelist = ImporterFileDataXml.decode (xml);

		foreach(string asset in filelist)
		{
			if(_file_list.Contains(asset) == false)
			{
				if(asset.Contains("Assets/StreamingAssets/") == true)
				{
                    continue;
                }
				_file_list.Add(asset);
			}
		}
	}

}
