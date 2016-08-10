using System.Collections;
using UnityEngine;

public static class BundleConfig
{
    public static string            BUNDLE_CONFIG_RELATIVE_PATH = "/assetbundle/config/";
    public static string            BUNDLE_CONFIG_FILE_NAME = "bundle.data";

	/// <summary>
	/// AssetBundle存放路径
	/// </summary>
    public static string 			BUNDLE_OUT_PARENT_PATH = Application.dataPath + "/../assetbundle/";
	public static string 			BUNDLE_OUT_PATH = BUNDLE_OUT_PARENT_PATH + "assetbundle/";
    public static string            BUNDLE_OUT_PATH_TMP = Application.dataPath + "/../tmpbundle/";
    public static string            ASSET_BUNDLE_PATH = Application.streamingAssetsPath + "/assetbundle/";
    
    /// <summary>
    /// 多语言资源包的存储位置
    /// </summary>
    public static string            LAN_BUNDLE_OUT_PATH = Application.dataPath + "/../";            // 多语言包打包 assetbundle 的时候，assetbundle 的存储位置
    public static string            LAN_ZIP_BUNDLE_OUT_PATH = Application.dataPath + "/../zip/";    // zip 打包时的位置
    public static string            LAN_RES_SAVE_PATH = Application.persistentDataPath;             // zip 下载回来保存的位置

    /* CDN 资源路径 */
    public static string LAN_RES_CDN_PATH
    {
        get
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return "http://cdntest.zsh.qq.com/kl/ios_publish/zip/";
            }
            else
            {
                return "http://cdntest.zsh.qq.com/kl/android_publish/zip/";
            }
        }
    }

    /// <summary>
    /// 依赖的配置存放地址
    /// </summary>
    public static string            BUNDLE_CONFIG_FILE_PATH = BUNDLE_OUT_PARENT_PATH + "assetbundle/config/";

    /// <summary>
    /// 场景文件的配置
    /// </summary>
    public static string            SCENE_FILE_PREFIX = "assets/scenes/battle/";
    public static string            STREAM_SCENE_OUT_PATH = Application.dataPath + "/streamscenes/";
    public static string            STREAM_SCENE_FILENAME = "scenes.xml";
    public static string            SCENE_FILE_SUFFIX = ".unity";

	/// <summary>
	/// 资源依赖关系表存放路径.
	/// </summary>
	public static string 			ASSETDEPDENCIES_FILEPATH = Application.dataPath + "/../BundlesConfig/";
	public static string 			ASSETDEPDENCIES_FILENAME = "asset_dep.xml";
	public static string 			ASSETDEPDENCIES_AFTERMERAGE_FILENAME = "asset_merage_dep.xml";

    public struct AssetItem
    {
        public string assetRelativePath;       // 资源相对路径，相对于Applicatin.dataPath + "/Resources/";
        public bool pack;                      // 是否一起打包，一起打包的包命名按照assetRelativePath进行命名
        public AssetItem(string path, bool ifPack)
        {
            assetRelativePath = path;
            pack = ifPack;
        }

    }

    /// <summary>
    /// 打包资源相对Resources的相对路径
    /// </summary>
    public static AssetItem[]       ASSET_PATH_RELATIVE_PATH = new AssetItem[] {
                                                        new AssetItem("Game/Battle/Scene/", false),                     /* 场景行为树 */
                                                        new AssetItem("Game/Icons/", false),                            /* 物品icons */
                                                        new AssetItem("Game/Battle/AI/", false),                        /* Game/Battle/AI */
                                                        new AssetItem("Game/UI/Activity/", false),                      /* 活动图片 */
                                                        new AssetItem("Game/UI/LoginActivity/", false),                 /* 登录强弹活动图片 */
                                                        new AssetItem("Local/translate/", false),                       /* 翻译资源 */
                                                        new AssetItem("Game/Recharge/", false),                         /* vip 充值图片 */
                                                };

    public class LanguageResItem
    {
        public string languageCode;         /* 语言代码 */
        public string dir;                  /* 资源路径 */
        public string[] audio_banks;        /* 音效资源 */
        public bool includeInApp;           /* 是否编在安装包内 */
        public LanguageResItem(string languageCode, string dir, string[] audio_banks, bool includeInApp = false)
        {
            this.languageCode = languageCode;
            this.dir = dir;
            this.audio_banks = audio_banks;
        }
    }

    public static LanguageResItem[] LANGUAGE_ASSET_RELATIVE_PATH = new LanguageResItem[]{
            new LanguageResItem("en-US", "Local/en", null, true),
            new LanguageResItem("tr", "Local/tr", null),
            new LanguageResItem("ko", "Local/ko", null),
            new LanguageResItem("zh-TW", "Local/zh-TW", null),
    };

    public static string            SRPITE_PATH = Application.dataPath + "/KillerRoot";
    
    /// <summary>
    /// 忽略打包的文件列表
    /// 在上面需要打包的路径的基础上，去除其中的某些资源
    /// </summary>
    public static string[]          ASSET_IGNORE_ARR = new string[]{
                                    "Game/UI/UILogoBG",
                                    "GameSettings/QualityGameSetting",
                                    "Game/Avatar/Skeleton",
                                    "Game/Avatar/Skeleton_female",
                                    "Game/Avatar/Weapon"

    };

    public static bool              USE_ASSET_BUNDLE_ONLY = false;
	/// <summary>
	/// 检索文件类型.
	/// </summary>
	public static string 			SEARCH_FILE_TYPE = "*.*";

	/// <summary>
	/// AssetBundle后缀.
	/// </summary>
	public static string			BUNDLESUFFIX = ".assetbundle";

	/// <summary>
	/// 忽略打包文件后缀.
	/// </summary>
	public static string[] 			IGNORE_FILE_SUFFIX_ARR = new string[]{
		".meta",
		".cs",
		".cg",
		".js",
	};

	#region 打包参数

	/// <summary>
	/// 打包是是否压缩.
	/// </summary>
	public static bool BUNDLE_COMPRESS = true;

	public static bool BUNDLE_DETERMINISTIC = true;

	#endregion
}
