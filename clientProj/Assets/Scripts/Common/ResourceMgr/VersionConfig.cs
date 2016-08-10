using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class VersionConfig
{
    /* StreamAsset目录中的Version文件 */
    public static string STREAM_VERSION_FILE = FileUtils.STREAM_FILE_PROTOCAL_ + Application.streamingAssetsPath + "/Config/version";

    public static string ASSET_BUNDLE_EXTRACT_PATH = Application.persistentDataPath + "/assetbundle";
    
    /* IIPS数据配置字符串
     * 需要配置的占位符包括：
     * 0：tversion 服务器地址
     * 1：serverid
     * 2: version str
     * 3: IFS存放在本地的地址
     * 4：IFS解压地址
     */
    public static string DATA_PLUGIN_FORMAT_STRING = @"{{
				""m_update_type"":5,
				""basic_version"":
				{{
					""m_protocol_version"":9,
                    ""m_retry_count"":10,
					""m_server_url_list"":[{0}],
                    ""m_app_id"" : 1,
					""m_service_id"":{1},
					""m_retry_interval_ms"":10,
					""m_connect_timeout_ms"":1000,
					""m_update_type"":5,
                    ""m_current_version_str"" : ""{2}""
				}},

				""full_diff"":
				{{
					""m_ifs_save_path"":""{3}"",
					""m_file_extract_path"":""{4}""
				}}	
			}}";

    /*IIPS数据配置字符串:需要解压数据
     * 需要配置的占位符包括：
     * 0：tversion 服务器地址
     * 1：serverid
     * 2: version str
     * 3: IFS存放在本地的地址
     * 4：IFS解压地址
     * 5：安装包内的IFS地址
     * 6: 解压地址
     * 以@开头的字符串表示忽略其中的特殊字符
     * 例如：@"c:\temp" = "c:\\temp"
     */
    public static string DATA_PLUGIN_FORMAT_STRING_EXTRACT_DATA = @"{{
				""m_update_type"":5,
				""basic_version"":
				{{
					""m_protocol_version"":9,
                    ""m_retry_count"":10,
					""m_server_url_list"":[{0}],
                    ""m_app_id"" : 1,
					""m_service_id"":{1},
					""m_retry_interval_ms"":10,
					""m_connect_timeout_ms"":1000,
					""m_update_type"":5,
                    ""m_current_version_str"" : ""{2}""
				}},
                ""first_extract"":
                {{
                    ""m_ifs_extract_path"":""{6}"",
                    ""m_ifs_res_save_path"":""{4}"",
                    ""filelist"":
                    [
                        {{
                            ""filepath"":""{5}"",
                            ""filename"":""first_source.png""
                        }}
                    ]
                }},
				""full_diff"":
				{{
					""m_ifs_save_path"":""{3}"",
					""m_file_extract_path"":""{4}""
				}}	
			}}";

    /* IIPS程序配置字符串
     * 需要配置的占位符包括：
     * 0：tversion 服务器地址
     * 1：serverid
     * 2: version str
     * 3: apk存放地址
     * 4：原始apk存放位置
     */
    public static string APP_PLUGIN_FORMAT_STRING = @"{{
				""m_update_type"":4,
                ""basic_version"":
                {{
					""m_protocol_version"":9,
                    ""m_retry_count"":10,
					""m_server_url_list"":[{0}],
                    ""m_app_id"" : 1,
					""m_service_id"":{1},
					""m_retry_interval_ms"":10,
					""m_connect_timeout_ms"":1000,
					""m_update_type"":4,
                    ""m_current_version_str"" : ""{2}""
                }},
                ""basic_diffupdata"":
                {{
                    ""m_diff_config_save_path"" : ""{3}"",
                    ""m_diff_temp_path"" : ""{3}"",
                    ""m_nMaxDownloadSpeed"" : 10240000,
                    ""m_apk_abspath"" : ""{4}""
                }}
            }}";

    /* tversion服务器地址 */
    public static string TEST_TVERSION_URL = "\"tcp://testb.mtcls.qq.com:10001\",\"tcp://testb.mtcls.qq.com:10003\",\"tcp://testb.mtcls.qq.com:10005\"";
    public static string MIDDLE_TVERSION_URL = "\"tcp://middle2.mtcls.qq.com:30001\",\"tcp://middle2.mtcls.qq.com:30003\",\"tcp://middle2.mtcls.qq.com:30005\"";
#if UNITY_ANDROID
    public static string RELEASE_TVERSION_URL = "\"203.205.151.237:20011\",\"203.205.151.237:20013\",\"203.205.151.237:20015\",\"203.205.166.139:20013\",\"203.205.166.139:20015\",\"203.205.166.139:20013\",\"203.205.147.178:20011\",\"203.205.147.178:20013\",\"203.205.147.178:20015\",\"203.205.167.178:20013\",\"203.205.167.178:20015\",\"203.205.167.178:20013\"";
#else
    public static string RELEASE_TVERSION_URL = "\"tcp://mtcls.qq.com:20011\",\"203.205.151.237:20013\",\"203.205.151.237:20015\",\"203.205.166.139:20013\",\"203.205.166.139:20015\",\"203.205.166.139:20013\",\"203.205.147.178:20011\",\"203.205.147.178:20013\",\"203.205.147.178:20015\",\"203.205.167.178:20013\",\"203.205.167.178:20015\",\"203.205.167.178:20013\"";
#endif
    public static string TEST_TVERSION_IP = "101.226.87.24";
    public static string MIDDLE_TVERSION_IP = "101.226.141.88";
    public static string RELEASE_TVERSION_IP = "61.151.224.100";

    /*
     * 注意这个是给 IIPS 服务使用的
     */
    //public static int ANDROID_APP_SERVICE_ID = 0x9e00069;
	//public static int ANDROID_DATA_SERVICE_ID = 0x9e00070;
	//public static int IOS_APP_SERVICE_ID = 0x9e00073;
	//public static int IOS_DATA_SERVICE_ID = 0x9e00074;

    public static int ANDROID_APP_SERVICE_ID = 0x9e00071;
    public static int ANDROID_DATA_SERVICE_ID = 0x9e00072;
    public static int IOS_APP_SERVICE_ID = 0x9e00075;
    public static int IOS_DATA_SERVICE_ID = 0x9e00076;

    /* Dolphin 更新服务器地址 */
    public static uint DOLPHIN_GAME_ID = 1626353557;
    public static string DOLPHIN_GAME_KEY = "a1fcc5fa93e91db1a8775cb9493821e6";
    public static string RELEASE_UPDATE_ENV_URL = "download.1.1626353557.agcloudcs.com";
    public static string PREPUBLISH_UPDATE_ENV_URL = "pre-download.2.1626353557.agcloudcs.com";

    /* Dolphin 更新渠道 */
    public static uint GetChannelID()
    {
        // 目前所有渠道暂时都用渠道3
        return 489;
    }

    /* kl 服务器地址 */
    public static string KLP1_TEST_IP = "2.2.2.2";
    public static string AWS_TEST_IP = "4.4.4.4";
    public static string TEST_IP = "6.6.6.6";
    public static string PRE_PUBLISH_IP = "8.8.8.8";
    public static string APPLE_CHECK_IP = "10.10.10.10";
    public static string RELEASE_IP_SG = "12.12.12.12";
    public static string RELEASE_IP_US = "14.14.14.14";
    public static string KLP1_TEST_URL = "tcp://101.227.153.17:8080";
    public static string AWS_TEST_URL = "tcp://52.76.32.66:8080";
    public static string TEST_URL = "test.tconnd.fusionwar.37.com:8080";
    public static string PRE_PUBLISH_URL = "pre.tconnd.fusionwar.37.com:8080";
    public static string APPLE_CHECK_URL = "apple.tconnd.fusionwar.37.com:8080";
    public static string RELEASE_URL_SG = "sg.tconnd.fusionwar.37.com:8080";
    public static string RELEASE_URL_US = "us.tconnd.fusionwar.37.com:8080";
    public static string RELEASE_URL_UR = "ge.tconnd.fusionwar.37.com:8080";

    /* version 服务器地址 */
    public static int ANDROID_DNS_SERVICE_ID = 165675113;
    public static int IOS_DNS_SERVICE_ID = 165675113;
    public static string HK_DEV_IP = "2.2.2.2";
    public static string TEST_VERSION_IP = "4.4.4.4";
    public static string PRE_PUBLISH_VERSION_IP = "6.6.6.6";
    public static string APPLE_CHECK_VERSION_IP = "8.8.8.8";
    public static string RELEASE_VERSION_IP_SG = "10.10.10.10";
    public static string RELEASE_VERSION_IP_US = "12.12.12.12";
    public static string RELEASE_VERSION_IP_UR = "16.16.16.16";
    public static string HK_DEV_URL = "http://101.227.160.19:8002";
    public static string TEST_VERSION_URL = "http://101.227.160.19:8002";
    public static string PRE_PUBLISH_VERSION_URL = "http://pre.version.fusionwar.37.com:8003";
    public static string APPLE_CHECK_VERSION_URL = "http://apple2.version.fusionwar.37.com:8003";
#if UNITY_EDITOR
    public static string RELEASE_VERSION_URL_SG = "http://101.227.160.19:8002";
    public static string RELEASE_VERSION_URL_US = "http://101.227.160.19:8002";
    public static string RELEASE_VERSION_URL_UR = "http://101.227.160.19:8002";
#else
    public static string RELEASE_VERSION_URL_SG = "http://sg.version.fusionwar.37.com:8003";
    public static string RELEASE_VERSION_URL_US = "http://us.version.fusionwar.37.com:8003";
    public static string RELEASE_VERSION_URL_UR = "http://ge.version.fusionwar.37.com:8003";
#endif

    /* kl 服务器端口 */
    public static int KL_SERVER_PORT = 8080;

    /* DNS是否是测试版本的配置文件 */
    public static string DNS_TEST_FILE_PATH = Application.persistentDataPath + "/dns_test";
    /* 支付是否是强制沙箱的配置文件 */
    public static string PAY_FORCE_TO_SANDBOX_FILE_PATH = Application.persistentDataPath + "/midas_pay_test";

    /* 本地配置文件 */
    public static string CONFIG_DATA_PATH = Application.persistentDataPath + "/assetbundle/config/";
    public static string CACHE_DATA_PATH = Application.persistentDataPath + "/AssetBundle/";

    public static string LOCAL_VERSION_FILE = Application.persistentDataPath + "/Config/version";
    public static string LOCAL_BUNDLE_FILE = Application.persistentDataPath + "/assetbundle/config/bundle.data";
    public static string LOCAL_SCENE_FILE = Application.persistentDataPath + "/Config/scenes.xml";


    /* VERSION JSON标签 */
    public static string ANDROID_VALUE_KEY = "ANDROID";
    public static string ANDROID_VERSION_MD5_KEY = "ANDROID_MD5";
    public static string IOS_VALUE_KEY = "IOS";
    public static string IOS_VERSION_MD5_KEY = "IOS_MD5";
    public static string ANDROID_OP_VALUE_KEY = "ANDROID_OP";
    public static string ANDROID_OP_MD5_KEY = "ANDROID_OP_MD5";
    public static string IOS_OP_VALUE_KEY = "IOS_OP";
    public static string IOS_OP_MD5_VALUE = "IOS_OP_MD5";
    public static string LOGIN_ANNOUNCEMENT_KEY = "LOGIN_ANNOUNCEMENT";
    public static string ANDROID_SERVER_STOP = "ANDROID_SERVER_STOP";
    public static string IOS_SERVER_STOP = "IOS_SERVER_STOP";
    public static string NEWBIE_PASSED = "REGISTERED";
    public static string CLIENT_NEW_VERSION_REWARD = "CLIENT_NEW_VERSION_REWARD";

    /* 配置文件路径 */
    public static string GetRemoteAssetPath()
    {
        if (KillerGlobal.isOPDev)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return "http://cdntest.zsh.qq.com/kl/ios_publish/";
            }
            else
            {
                return "http://cdntest.zsh.qq.com/kl/android_publish/";
            }
        }
        else
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return "http://image.dlfx.qq.com/kl/ios_publish/";
            }
            else
            {
                return "http://image.dlfx.qq.com/kl/android_publish/";
            }
        }
    }

    /* 各种文件所在目录 */
    public static string ASSET_BUNDLE_DIR = "AssetBundle/";
    public static string SCENE_BUNDLE_DIR = "Scenes/";
    public static string CONFIG_DIR = "Config/";
    public static string ACTIVITY_DIR = "Game/UI/Activity/";
    public static string LOGIN_ACTIVITY_DIR = "Game/UI/LoginActivity/";

    /* 场景配置 */
    public static string BATTLE_SCENE_PATH_ROOT = "Assets/Scenes/Battle/";
    public static string SCENES_PERSISTENT_DATA_PATH = Application.persistentDataPath + "/AssetBundle/Scenes/";
    public static string WWW_REMOTE_SCENE_DATA_PATH = GetRemoteAssetPath() + "Scenes/";

    public static string serverConnectionFile = ASSET_BUNDLE_EXTRACT_PATH + "/serverUrl";

#if UNITY_EDITOR
    public static string countryServerFile = Application.dataPath + "/Resources/CountryServer";
#else
    public static string countryServerFile = ASSET_BUNDLE_EXTRACT_PATH + "/CountryServer";                                                                                                                                                                                                                                                                                               
#endif

    /* 服务器地址 */
    public static Dictionary<string, List<string>> serverConnectionCfg = new Dictionary<string, List<string>>();

    public static Dictionary<string, string> versionConnectionCfg = new Dictionary<string,string>();


    public static string GetFirstExtractIfsPath()
    {
#if UNITY_ANDROID
        string configifspath = Application.streamingAssetsPath + "/first_source.png";
        configifspath = configifspath.Replace("jar:file://", "apk://");
        configifspath = configifspath.Replace("!/", "?");
        return configifspath;
#elif UNITY_IOS
		string configifspath = Application.streamingAssetsPath + "/first_source.png";
		if(File.Exists(configifspath))
		{
			System.Console.WriteLine ("ifs exist,configapkpath:{0}",configifspath);
		}
		else
		{
			System.Console.WriteLine ("ifs not exist,configapkpath:{0}",configifspath);
		}
		return configifspath;
#else
        return Application.streamingAssetsPath + "/first_source.png";
#endif
    }

    public static string GetCurrentVersionApkPath()
    {
#if UNITY_EDITOR
        return Application.streamingAssetsPath + "/killer.apk";
#elif UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        if (jc == null)
        {
            return null;
        }
        AndroidJavaObject m_jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        if (m_jo == null)
        {
            return null;
        }
        AndroidJavaObject jo = new AndroidJavaObject("cu_iipsmobile.CuIIPSMobile");
        if (jo == null)
        {
            return null;
        }
        string apkpath = jo.Call<string>("GetApkAbsPath", m_jo);
        if (apkpath == "error")
        {
            System.Console.WriteLine("getpath failed");
            return null;
        }
        else
        {
            System.Console.WriteLine("getpath success  path:{0}", apkpath);
            return apkpath;
        }
#else
		return null;
#endif
    }
}
