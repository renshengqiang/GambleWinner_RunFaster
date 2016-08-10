using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class BundleDepdenciesTable
{
    private Dictionary<string, BundleInfo> dicBuildState = new Dictionary<string, BundleInfo>();

	private Dictionary<string, int> dicBundleId = new Dictionary<string, int> ();

	private AssetDepdenceTable _deptable;

	private int Id;

	public void parseDepTable (AssetDepdenceTable deptable)
	{
		dicBuildState.Clear ();
		_deptable = deptable;
		Id = 0;
		addState (deptable.listAsset);
    }

    // 加一个整体打包的资源列表序列
    public void addPackResources(AssetBundleTool.PackBundleItem packItem)
    {
        BundleInfo state = new BundleInfo();
        dicBuildState.Add(packItem.packName, state);
        state.bundle_id = (uint)getBundleId(packItem.packName);
        state.bundle_name = UrlUtil.parseAssetPathToBundleName(packItem.packName);
    }

	private int getBundleId(string assetpath)
	{
		int bundle_id;
		if(dicBundleId.ContainsKey(assetpath) == false)
		{
			bundle_id = Id ++;
			dicBundleId.Add(assetpath, bundle_id);
		}
		else
		{
			bundle_id = dicBundleId[assetpath];
		}
		return bundle_id;
	}

    private void addState(List<string> asset_list)
    {
        foreach (string asset in asset_list)
        {
            BundleInfo state;
            if (dicBuildState.ContainsKey(asset) == false)
            {
                state = new BundleInfo();
                dicBuildState.Add(asset, state);
            }
            else
            {
                /* 已经处理过了，没有必要再处理了 */
                continue;
                //dicBuildState.TryGetValue(asset, out state);
            }
            state.bundle_id = (uint)getBundleId(asset);
            state.bundle_name = UrlUtil.parseAssetPathToBundleName(asset);

            List<string> depAssetList;
            _deptable.dicDep.TryGetValue(asset, out depAssetList);
            if (depAssetList == null)
            {
#if LOG_DETAIL
                Debug.LogError(asset + "没有在依赖表里面");
#endif
                throw new UnityException();
            }
            foreach (string depasset in depAssetList)
            {
                int depID = getBundleId(depasset);
                if (state.dep_bundle_list.Contains((uint)depID) == false)
                {
                    state.dep_bundle_list.Add((uint)depID);
                }
            }
            addState(depAssetList);
        }
    }

    public void SaveBundleDepdenciesToFile(string configpath, string filename)
    {
        List<BundleInfo> list = new List<BundleInfo>();
        BundleInfoArray array = new BundleInfoArray();
        foreach (BundleInfo info in dicBuildState.Values)
        {
            list.Add(info);
        }
        array.lstBundleInfo.AddRange(list);
        string strAbXml = BundleListXML.encode(list);
        list.Clear();

        /* 把xml存储到文件中 */
        FileUtils.SaveToFile(strAbXml, configpath, filename + ".xml");
        
        /* 保存proto文件 */
        using (MemoryStream messageStream = new MemoryStream())
        {
            //todo:保存文件
            //Serializer.Serialize<BundleInfoArray>(messageStream, array);
            byte[] messageArr = messageStream.ToArray();
            FileUtils.SaveToFile(messageArr, configpath, filename);
        }
        
    }

    public void renameBundles(string languageCode)
	{
		foreach(KeyValuePair<string, BundleInfo> pair in dicBuildState)
		{
			BundleInfo state = pair.Value;
			try{
                state.bundle_md5 = BuildHelper.getBundleMd5(state.bundle_name, languageCode);
                state.size = (uint)BuildHelper.getBundleSize(state.bundle_name, languageCode);

                BuildHelper.renameBundle(state.bundle_name, state.bundle_md5, languageCode);
			}
			catch
			{
#if LOG_DETAIL
				Debug.LogWarning(pair.Key + "没有AssetBundle或者本来已经存在");
#endif
			}
		}
	}


    /// <summary>
    /// 删除多余的AssetBundle
    /// </summary>
    /// <param name="files">当前工程中的所有AssetBundle列表，只包含文件名，不包含具体路径</param>
    public void deleteUnusedAssetBundles(List<string> files)
    {
        Dictionary<string, bool> dicBundles = new Dictionary<string, bool>();

        foreach (BundleInfo state in dicBuildState.Values)
        {
            dicBundles.Add(state.bundle_name + "_" + state.bundle_md5 + BundleConfig.BUNDLESUFFIX, true);
        }

        foreach (string file in files)
        {
            if (!dicBundles.ContainsKey(file))
            {
                string realName = BundleConfig.BUNDLE_OUT_PATH + file;
                try
                {
                    File.Delete(realName);
                }
                catch
                {
#if LOG_DETAIL
                    Debug.LogWarning("文件: " + realName + "不存在");
#endif
                }
            }
        }
    }


	public BundleInfo getBuildState(string assetpath)
	{
        BundleInfo state;
		dicBuildState.TryGetValue (assetpath, out state);
		return state;
	}

    public void mergeBundleInfo(BundleInfo bundle)
	{
        foreach (BundleInfo info in dicBuildState.Values)
		{
			if(info.bundle_name == bundle.bundle_name)
			{
				if(info.size <= 0)
				{
					info.size = bundle.size;
					info.bundle_md5 = bundle.bundle_md5;
				}
				break;
			}
		}
	}
}
