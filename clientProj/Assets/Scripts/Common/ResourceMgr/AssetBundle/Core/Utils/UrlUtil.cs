using UnityEngine;
using System.Security.Cryptography;

#if MSDK
using Apollo.Plugins.Msdk;
#elif IMSDK
using Tencent.iMSDK;
#endif
using System;

public class UrlUtil
{

    /// <summary>
    /// 资源路径通过MD5方式变为文件名.
    /// </summary>
    /// <returns>The file name.</returns>
    /// <param name="assetpath">Assetpath.</param>
    static public string parseAssetPathToBundleName(string assetpath)
    {

        string filename = "";
        if (assetpath.Contains("Resources"))
        {
            string[] filenames = assetpath.Split('.');
            filename = filenames[0];
        }
        else
        {
            filename = assetpath;
        }

        MD5 md5 = MD5.Create();

        byte[] input = System.Text.Encoding.Default.GetBytes(filename);

        byte[] bMd5 = md5.ComputeHash(input);

        md5.Clear();

        string str = "";

        for (int i = 0; i < bMd5.Length; i++)
        {
            str += bMd5[i].ToString("x").PadLeft(2, '0');
        }

        //Debug.Log (filenames [0] + ":" + str);

        return str;
    }

    static public string parseSceneNameToBundleName(string sceneName)
    {
        MD5 md5 = new MD5CryptoServiceProvider();

        byte[] input = System.Text.Encoding.Default.GetBytes(sceneName);

        byte[] bMd5 = md5.ComputeHash(input);

        md5.Clear();

        string str = "";

        for (int i = 0; i < bMd5.Length; i++)
        {
            str += bMd5[i].ToString("x").PadLeft(2, '0');
        }

        return str;
    }

    /// <summary>
    /// 调用msdk打开内嵌浏览器
    /// </summary>
    /// <param name="url"></param>
    public static void OpenUrl(string url)
    {
        if (null != url && "" != url)
        {
            bool opened = false;
            if (Application.isMobilePlatform)
            {
#if MSDK
                IApolloCommonService common = IApollo.Instance.GetService(ApolloServiceType.Common) as IApolloCommonService;
                if (null != common)
                {
                    opened = true;
                    common.OpenUrl(url);
                }
#elif IMSDK
                IMSDKApi.WebViewLite.OpenUrl(url, true, false);
                opened = true;
#endif

            }

            if(false == opened)
            {
                Application.OpenURL(url);
            }
        }
    }

    /// <summary>
    /// 从url中解析出FileName
    /// </summary>
    /// <param name="url">url的格式是类似http://10.143.147.24/data1/208/kl/android_publish/app/killer_1_2_5_136.apk这样的字符串</param>
    /// <returns></returns>
    public static string GetFileNameFromUrl(string url)
    {
        return System.IO.Path.GetFileName(url);
    }

    /// <summary>
    /// 从文件名中去除后缀名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string RemoveFileSuffix(string path)
    {
        int length = path.LastIndexOf('.');
        if (-1 == length)
        {
            length = path.Length;
        }
        return path.Substring(0, length);
    }
	
	
    /// <summary>
    /// 从 url 中解析出不带后缀的文件名
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string GetFileNameWithoutSuffixFromUrl(string url)
    {
        string fullName = GetFileNameFromUrl(url);
        string name = "";
        if (!String.IsNullOrEmpty(fullName))
        {
            name = fullName.Substring(0, fullName.LastIndexOf('.'));
        }
        return name;
    }
}
