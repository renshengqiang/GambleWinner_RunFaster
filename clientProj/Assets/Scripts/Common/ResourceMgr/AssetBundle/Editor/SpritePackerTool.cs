using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class SpritePackerTool
{
    private static Dictionary<string, List<string> > dicPack = new Dictionary<string, List<string> >();

    /// <summary>
    /// 分析所有的Sprite找出其中的打包关系
    /// </summary>
    public static void ParseSprites()
    {
        List<string> files = getSpritesList();

        foreach (string path in files)
        {
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter != null)
            {
                if (textureImporter.spritePackingTag != null && !textureImporter.spritePackingTag.Equals(""))
                {
                    string tag = textureImporter.spritePackingTag;

                    if (!dicPack.ContainsKey(tag))
                    {
                        dicPack.Add(tag, new List<string>());                       
                    }
                    List<string> list = dicPack[tag];
                    if (null == list) list = new List<string>();
                    if (!list.Contains(path)) list.Add(path);
                }
            }
        }

        //foreach(KeyValuePair<string, List<string> > item in dicPack)
        //{
        //    Debug.Log(item.Key + ":");
        //    foreach (string sprite in item.Value)
        //    {
        //        Debug.Log(sprite);
        //    }
        //}
    }

    /// <summary>
    /// 是否是经过sprite打包过的资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsSpritePack(string path)
    {
         TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

         if (textureImporter != null)
         {
             if (textureImporter.spritePackingTag != null && !textureImporter.spritePackingTag.Equals("")) return true;
         }
         return false;
    }

    /// <summary>
    /// 获取经过Sprite打包过的资源的leading sprite，所有其他资源对Sprite的依赖都转换为对leading spriete的依赖
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetLeadingSprite(string path)
    {
          TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

          if (textureImporter != null)
          {
              if (textureImporter.spritePackingTag != null && !textureImporter.spritePackingTag.Equals(""))
              {
                  string tag = textureImporter.spritePackingTag;

                  if (dicPack.ContainsKey(tag) && dicPack[tag].Count > 0)
                  {
                      return dicPack[tag][0];
                  }
              }
          }
          return null;
    }

    /// <summary>
    /// 上述函数的重载形式
    /// </summary>
    /// <param name="path"></param>
    /// <param name="leading"></param>
    /// <returns></returns>
    public static bool GetLeadingSprite(string path, out string leading)
    {
        bool ret = false;
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter != null)
        {
            if (textureImporter.spritePackingTag != null && !textureImporter.spritePackingTag.Equals(""))
            {
                string tag = textureImporter.spritePackingTag;

                if (dicPack.ContainsKey(tag) && dicPack[tag].Count > 0)
                {
                    leading = dicPack[tag][0];
                    return true;
                }
            }
        }
        leading = null;
        return false;
    }

    /// <summary>
    /// 获取KillerRoot下面的所有列表
    /// </summary>
    /// <returns></returns>
    private static List<string> getSpritesList()
    {
        List<string> file_list = new List<string>();

        string dirpath = BundleConfig.SRPITE_PATH;
        /* 遍历所有文件 枚举所有依赖 */
        DirectoryInfo directory = new DirectoryInfo(dirpath);
        FileInfo[] dirs = directory.GetFiles(BundleConfig.SEARCH_FILE_TYPE, SearchOption.AllDirectories);

        /* 遍历所有文件 */
        foreach (FileInfo info in dirs)
        {
            /* 如果是meta文件继续 */
            string fullname = info.FullName.Replace('\\', '/');
            if (fullname.EndsWith(".meta")) continue;

            string assetTemp = "Assets" + info.FullName.Substring(Application.dataPath.Length);

            string assetpath = assetTemp.Replace("\\", "/");

            /* 把遍历到的资源路径存起来 */
            file_list.Add(assetpath);
        }

        return file_list;
    }
}
