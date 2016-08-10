using System.Xml;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using ProtoBuf;
using System;

namespace MoreFun
{
    /// <summary>
    /// BundleManager负责Bundle的统一管理
    /// </summary>
    public class BundleManager
    {
        /* 单例 */
        private static BundleManager _Instance;
        private bool initialized = false;

        static public BundleManager getInstance()
        {
            if (_Instance == null)
            {
                _Instance = new BundleManager();
            }

            return _Instance;
        }

        public static string parseAssetName2BundleName(string assetname)
        {
            if (assetname == null) return "";

            string[] filenames = assetname.Split('.');

            MD5 md5 = new MD5CryptoServiceProvider(); 

            byte[] input = System.Text.Encoding.Default.GetBytes(filenames[0]);

            byte[] bMd5 = md5.ComputeHash(input);

            md5.Clear();

            string str = "";

            for (int i = 0; i < bMd5.Length; i++)
            {
                str += bMd5[i].ToString("x").PadLeft(2, '0');
            }

            return str;
        }

        public static string CONFIG_PATH = Application.streamingAssetsPath + "/Config/bundle.xml";
        static private Dictionary<string, BundleInfo> _dicBundles = new Dictionary<string, BundleInfo>();
        static private Dictionary<int, string> _dicBundleID = new Dictionary<int, string>();

        public bool Initialize()
        {
            bool ret = true;
            if (false == initialized)
            {
                ret = loadConfig();
                initialized = true;
            }
            return ret;
        }

        bool loadConfig()
        {
            try
            {
                string configPath = Application.persistentDataPath + BundleConfig.BUNDLE_CONFIG_RELATIVE_PATH + BundleConfig.BUNDLE_CONFIG_FILE_NAME;
                using (FileStream fileStream = File.OpenRead(configPath))
                {
                    BundleInfoArray array = Serializer.Deserialize<BundleInfoArray>(fileStream);
                    for (int i = 0; i < array.lstBundleInfo.Count; ++i)
                    {
                        BundleInfo bundle = array.lstBundleInfo[i];

                        if (!_dicBundleID.ContainsKey((int)bundle.bundle_id))
                            _dicBundleID.Add((int)bundle.bundle_id, bundle.bundle_name);
                        else
                        {
#if LOG_DETAIL
                            Debug.LogWarning("BundleManager::decodeConfig: " + bundle.bundle_id + "already exist");
#endif
                        }

                        if (!_dicBundles.ContainsKey(bundle.bundle_name))
                        {
                            _dicBundles.Add(bundle.bundle_name, bundle);
                        }
                        else
                        {
                            _dicBundles[bundle.bundle_name] = bundle;
#if LOG_DETAIL
                            Debug.LogWarning("BundleManager::decodeConfig: " + bundle.bundle_name + "already exist");
#endif
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                //this.MustLogError(e);
                return false;
            }
        }

        void decodeConfig(string bundleXML)
        {
            if (bundleXML != null && !bundleXML.Equals(""))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(bundleXML);
                XmlElement root = doc.DocumentElement;
                XmlNodeList list = root.GetElementsByTagName("asset");
                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    BundleInfo bundle = new BundleInfo();
                    bundle.bundle_id = uint.Parse(((XmlElement)list.Item(i)).GetAttribute("id"));
                    bundle.bundle_name = ((XmlElement)list.Item(i)).GetAttribute("name");
                    bundle.bundle_md5 = ((XmlElement)list.Item(i)).GetAttribute("md5");
                    string strDepBundle = ((XmlElement)list.Item(i)).GetAttribute("dep");

                    if (strDepBundle.Length > 0)
                    {
                        string[] dep_bundles = strDepBundle.Split(';');
                        foreach (string depbundlename in dep_bundles)
                        {
                            if (depbundlename != "")
                            {
                                int bundle_id = int.Parse(depbundlename);
                                bundle.dep_bundle_list.Add((uint)bundle_id);
                            }
                        }
                    }

                    if (!_dicBundleID.ContainsKey((int)bundle.bundle_id))
                        _dicBundleID.Add((int)bundle.bundle_id, bundle.bundle_name);
                    else
                    {
#if LOG_DETAIL
                        Debug.LogWarning("BundleManager::decodeConfig: " + bundle.bundle_id + "already exist");
#endif
                    }

                    if (!_dicBundles.ContainsKey(bundle.bundle_name))
                    {
                        _dicBundles.Add(bundle.bundle_name, bundle);
                    }
                    else
                    {
                        _dicBundles[bundle.bundle_name] = bundle;
#if LOG_DETAIL
                        Debug.LogWarning("BundleManager::decodeConfig: " + bundle.bundle_name + "already exist");
#endif
                    }
                }
            }
        }

        static public BundleInfo getBundleInfoByAsset(string assetpath)
        {
            string bundle_name = parseAssetName2BundleName(assetpath);
            return getBundleInfo(bundle_name);
        }

        static public BundleInfo getBundleInfo(string bundle_name)
        {
            BundleInfo info;
            _dicBundles.TryGetValue(bundle_name, out info);
            return info;
        }

        static public BundleInfo getBundleInfo(int bundle_id)
        {
            string bundle_name = _dicBundleID[bundle_id];
            BundleInfo info = null;
            _dicBundles.TryGetValue(bundle_name, out info);
            return info;
        }
    }
}
