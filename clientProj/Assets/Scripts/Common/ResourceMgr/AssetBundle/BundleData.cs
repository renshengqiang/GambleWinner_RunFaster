using UnityEngine;
using System.Collections.Generic;

public class StreamSceneInfo
{
    public string       stream_scene;     /* 场景前缀名 */
    public List<string> scene_list;       /* 需要一起打包的场景 */
}

public class StreamSceneBuildState
{
    public string       bundle_name;      /* 加密文件名*/
    public long         size;             /* 文件size */
    public string       md5;              /* 场景MD5 */
}