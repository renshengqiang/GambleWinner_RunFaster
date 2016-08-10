using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class AssetDepdenceTable 
{
	/* 依赖关系表 */
	public Dictionary<string, List<string>> dicDep = new Dictionary<string, List<string>> ();
	/* 被依赖关系表 */
	public Dictionary<string, List<string>> dicDepBy = new Dictionary<string, List<string>> ();
	
	/* 用来存储遍历到的资源 */
	public List<string> listAsset = new List<string> ();

	#region 添加资源
	/// <summary>
	/// Adds the asset.
	/// </summary>
	/// <param name="asset_list">Asset_list.</param>
	public void addAsset(List<string> asset_list)
	{
        if (asset_list == null) return;
		foreach(string assetpath in asset_list)
		{
			addAsset(assetpath);
		}
	}

    /// <summary>
    /// 这个函数添加的资源是不需要考虑依赖关系的
    /// </summary>
    /// <param name="asset_list"></param>
    public void assAssetWithoutDependency(List<string> asset_list)
    {
        if (asset_list == null) return;

        foreach (string asset in asset_list)
        {
            string guid = AssetDatabase.AssetPathToGUID(asset);
            if (!string.IsNullOrEmpty(guid))
            {
                List<string> listDep;
                dicDep.TryGetValue(asset, out listDep);
                if (listDep == null)
                {
                    listDep = new List<string>();
                    dicDep.Add(asset, listDep);
                }
                listAsset.Add(asset);
            }
        }
    }

	/// <summary>
	/// Adds the asset and.
	/// </summary>
	/// <param name="assetpath">Assetpath.</param>
	public void addAsset(string assetpath)
	{
		/* 获取资源的依赖关系 */
		string[] depPath = AssetDatabase.GetDependencies(new string[]{assetpath});
		
		List<string> listDep;
		dicDep.TryGetValue(assetpath, out listDep);
		if(listDep == null)
		{
			listDep = new List<string>();
			dicDep.Add(assetpath, listDep);
		}
		
		/* 过滤依赖的资源，并存到依赖表对应的list中 */
		foreach (var path in depPath)
		{
			/* 如果依赖是自己继续 */
			if(path == assetpath)
			{
				continue;
			}

			/* 如果依赖的是忽略的文件，如脚本、Shader等 */
			bool b_ignore = BundleUtils.isIgnoreFile(path);
			if(b_ignore == true)
			{
				continue;
			}

			/* 添加到依赖表 */			
			listDep.Add(path);
			
			/* 添加到被依赖表 */
			List<string> listDepBy;
			dicDepBy.TryGetValue(path, out listDepBy);
			if(listDepBy == null)
			{
				listDepBy = new List<string>();
				dicDepBy.Add(path, listDepBy);
			}
			listDepBy.Add(assetpath);
		}

		string[] arr_listDep = listDep.ToArray();
		foreach(var dep_path in arr_listDep)
        {
			parseToLineDepdencies (assetpath, listDep, dep_path);
        }
		listAsset.Add (assetpath);
    }

	/// <summary>
	/// 转化为单线依赖.
	/// </summary>
	public void parseToLineDepdencies(string assetpath, List<string> dep_list, string dep_path)
	{
		/* 当前资源依赖的资源 */
		string[] arrDep = AssetDatabase.GetDependencies(new string[]{dep_path});
		
		/* 当前资源所依赖的资源列表 */
		List<string> listDep;
		dicDep.TryGetValue(dep_path, out listDep);
		if(listDep == null)
		{
			listDep = new List<string>();
			dicDep.Add(dep_path, listDep);
		}		
		
		foreach(string asset in arrDep)
		{
			/* 如果依赖是自己继续 */
			if(dep_path == asset)
			{
				continue;
			}
			
			/* 如果以来的是脚本或者Shader继续 */
			bool b_ignore = BundleUtils.isIgnoreFile(asset);
			if(b_ignore)
			{
				continue;
			}

			/* 添加到依赖表 */
			if(listDep.Contains(asset) == false)
			{
				listDep.Add(asset);
			}
			
			/* 添加到被依赖表 */
			List<string> listDepBy;
			dicDepBy.TryGetValue(asset, out listDepBy);
			if(listDepBy == null)
			{
				listDepBy = new List<string>();
				dicDepBy.Add(asset, listDepBy);
#if LOG_DETAIL
				Debug.LogError("这里应该是存在的，出了什么问题呢？");
#endif
			}
			if(listDepBy.Contains(dep_path) == false)
			{
				listDepBy.Add(dep_path);
			}
			
			/* 从原来依赖关系中解除 */
			dep_list.Remove(asset);
			listDepBy.Remove(assetpath);
		}

		/* 继续处理依赖关系 */
		string[] parent = listDep.ToArray();
		if(parent.Length > 0)
		{
			foreach(string parentasset in parent)
			{
				parseToLineDepdencies(dep_path, listDep, parentasset);
			}
		}
	}

	#endregion

	#region 合并依赖关系
	/// <summary>
	/// 合并依赖关系.
	/// </summary>
	public void MergeDependencies ()
	{
		/* 遍历所有资源 */
		for(int i = 0; i < listAsset.Count; i++)
		{
			string assetpath = listAsset[i];
			/* 遍历所有依赖的资源 */
			List<string> listAssetDep;
			dicDep.TryGetValue(assetpath, out listAssetDep);

			mergeAssetDep(assetpath, listAssetDep);
		}
	}
	
	private void mergeAssetDep(string curAsset, List<string> depAssetList)
	{
		/* 如果有依赖资源才处理，如果没有就没有必要找下去了，有才处理 */
		if (depAssetList != null && depAssetList.Count >= 0) 
		{
			/* 继承依赖关系 */
			int j = 0;
			while (j < depAssetList.Count) 
			{
				string assetDep = depAssetList [j];
				List<string> listDep_depBy;
				dicDepBy.TryGetValue (assetDep, out listDep_depBy);

                /* 在要打包的路径中*/
                bool bContains = false;
                for (int i = 0; i < BundleConfig.ASSET_PATH_RELATIVE_PATH.Length; ++i)
                {
                    if (assetDep.Contains("/Resources/" + BundleConfig.ASSET_PATH_RELATIVE_PATH[i].assetRelativePath))
                    {
                        bContains = true;
                        break;
                    }
                }

                /* 如果依赖的资源的被依赖资源只有assetpath这一个，那么就合并依赖 */
                if (bContains == false && listDep_depBy != null && listDep_depBy.Count == 1)
				{
					/* 删除curAsset原来对assetDep的依赖 */
					depAssetList.RemoveAt (j);

					/* 把assetDep依赖的资源添加到listAssetDep中 */
					List<string> tmpListDep;
					dicDep.TryGetValue (assetDep, out tmpListDep);
					if (tmpListDep != null) 
					{
						foreach (var tmpath in tmpListDep)
						{
							/* 缩短依赖关系 */
							depAssetList.Add (tmpath);
							/* 被依赖关系要变化 */
							List<string> tmpListDepby;
							dicDepBy.TryGetValue (tmpath, out tmpListDepby);
							/* 这里一定不会为空，判断处理一下吧，如果为空为异常 */
							if (tmpListDepby != null)
							{
								tmpListDepby.Remove (assetDep);
								if(tmpListDepby.Contains(curAsset) == false)
								{
									tmpListDepby.Add(curAsset);
								}
							}
							else
							{
#if LOG_DETAIL
								Debug.LogError (tmpath + "没有资源被依赖了，怎么可能？");
#endif
							}
						}

						dicDep.Remove(assetDep);
                        j = 0;          /* 强制从头开始 */
					}
				}
				else
				{
					j++;
					mergeAssetDep(assetDep, dicDep[assetDep]);
				}
			}
		}
	}

	#endregion


	#region 保存配置文件
	/// <summary>
    /// 保存打包关系设置文件.
    /// </summary>
	public void SaveAssetDepdenciesToFile (string dirpath, string filename)
    {
        /* 遍历依赖表，并转化为XML格式字符串 */
		string bundlesettingxml = encodeDepTable (this);
        string bundlemd5xml = encodeDepTableMD5(this);
//        string bundlesettingxml = "<assets version=\"0\">";
//        foreach(KeyValuePair<string,List<string>> pair in dicDep)
//        {
//            string strDepAssets = "";
//            int len = pair.Value.Count;
//            for(int i = 0; i < len; i ++)
//            {
//                string asset_dep = pair.Value[i];
//                strDepAssets += asset_dep;
//                if(i < len - 1)
//                {
//                    strDepAssets += ";";
//                }
//            }
//            bundlesettingxml += "<asset path=\"" + pair.Key + "\" depassets=\"" + strDepAssets + "\" />";
//        }
//        bundlesettingxml += "</assets>";
        
        /* 把xml存储到文件中 */
		FileUtils.SaveToFile (bundlesettingxml, dirpath, filename);
        FileUtils.SaveToFile(bundlemd5xml, dirpath, "md5_" + filename);
    }
	#endregion

	#region 序列化和反序列化

	public static string encodeDepTable(AssetDepdenceTable assettable)
	{
		XmlDocument doc = new XmlDocument();
		XmlElement root = doc.CreateElement("assets");
		
		doc.AppendChild(root);
		
		foreach(KeyValuePair<string,List<string>> pair in assettable.dicDep)
		{
			XmlElement itemxml = doc.CreateElement("asset");
			string strDepAssets = "";
			int len = pair.Value.Count;
			for(int i = 0; i < len; i ++)
			{
				string asset_dep = pair.Value[i];
				strDepAssets += asset_dep;
				if(i < len - 1)
				{
					strDepAssets += ";";
				}
			}
			itemxml.SetAttribute("path", pair.Key);
			itemxml.SetAttribute("depassets", strDepAssets);

			root.AppendChild(itemxml);
		}
		
		return doc.OuterXml;
	}

    public static string encodeDepTableMD5(AssetDepdenceTable assettable)
    {
        XmlDocument doc = new XmlDocument();
        XmlElement root = doc.CreateElement("assets");

        doc.AppendChild(root);

        foreach (KeyValuePair<string, List<string>> pair in assettable.dicDep)
        {
            XmlElement itemxml = doc.CreateElement("asset");
            itemxml.SetAttribute("path", pair.Key);
            itemxml.SetAttribute("depassets", UrlUtil.parseAssetPathToBundleName(pair.Key));

            root.AppendChild(itemxml);
        }

        return doc.OuterXml;
    }

	#endregion

	#region 获取关系
	
	/// <summary>
	/// Gets the assets.获取所有相关的资源集合
	/// </summary>
	/// <param name="assetlist">需要遍历的资源列表.</param>
	/// <param name="list_asset">返回资源列表存放位置.</param>
	public void getAssets (List<string> assetlist, List<string> list_asset)
	{
		foreach(string asset in assetlist)
		{
			if(dicDep.ContainsKey(asset))
			{
				if(list_asset.Contains(asset) == false)
				{
					list_asset.Add(asset);
				}
				getAssets(dicDep[asset], list_asset);
			}
			else
			{
#if LOG_DETAIL
				Debug.LogWarning(asset);
#endif
            }
		}
	}
	#endregion
}
