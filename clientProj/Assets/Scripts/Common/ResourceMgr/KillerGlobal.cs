using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class KillerGlobal
{
    // 这些变量需要在Build的过程中动态修改
    public static bool isDevBuild = true;

    // 是否使用动态更新
    public static bool useDynamicUpdate = false;

    // 是否是在测试CDN上进行更新测试
    public static bool dynamicUpdateUseTestCdn = false;

    public static bool updateUseDolphin = false;

    public static string buildVersion = "";

    public static string version = "1.7.2.0";

    public static readonly Vector2 referenceSize = new Vector2(1136.0f, 640.0f);

    public static bool isAutoMonitorFPS = false;

    public static bool isMultivariateTest = false;

    public static bool isAppleCheckVersion = false;
    public static string apolloSdk
    {
        get
        {
            string ret = "msdk";

#if COSDK
                ret = "cosdk";
#elif IMSDK
                ret = "imsdk";
#else
#endif

            return ret;
        }
    }
    public static bool isCosdk
    {
        get
        {
            bool ret = false;

#if COSDK
                ret = true;
#endif

            return ret;
        }
    }
    public static bool isMsdk
    {
        get
        {
            bool ret = false;

#if MSDK
                ret = true;
#endif

            return ret;
        }
    }
    public static bool isIMsdk
    {
        get
        {
            bool ret = false;

#if IMSDK
                ret = true;
#endif

            return ret;
        }
    }

    // 运营资源是否使用测试环境(测试 CDN 还是 正式 CDN)
    public static bool isOPDev = false;

    // 是否使用 Editor 本地资源进行测试
    public static bool isOPUseLocal
    {
        get
        {
            //return false;
            if (Application.isEditor) return true;
            else return isOPDev;
        }
    }

    //构建版本是只有微信，还是只有手Q，还是都有
    //IOS版本无论怎么样都有游客模式
    public enum BuildPlatformType
    {
        All = 0,
        QQ_ONLY,
        WX_ONLY
    }
    public static BuildPlatformType targetPlatformType = BuildPlatformType.All;

    public static bool IsLogoutWillKillTheApplication
    {
        get
        {
            //return !isDevBuild;
            return false;
        }
    }

    // For guest mode.
    public static bool isGuestMode = false;

    public enum GuestSkipNewbieNode
    {
        GSNN_PVP = 2048,                     // PK对战的引导节点
        GSNN_LEGENDSHOOTER = 65536,         // 枪神大赛的引导节点
        GSNN_MAX
    }

    // 是否在模拟Release环境，设置为true则所有流程都是和手机上一致的，PC上使用QQ登录
    public static bool isSimulateReleaseEnvironment = false;

    public static bool releaseModule = true;

    public static bool pvpReceivedToActionAndPrediction = false;

    // 多语言资源是否使用本地资源
    public static bool localiztionResUseLocal = true;

    /// <summary>
    /// 和 UTC 标准时间的小时差
    /// 请不要拿时间戳和这个运算,时间戳是没有时区的概念的!!!
    /// </summary>
    /// <returns></returns>
    public static int GetTimeUTCOffset()
    {
        return TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
    }

    public static List<string> supportedLans = new List<string> { "en-US", "tr", "fr" };

    // 默认的语言：其实要根据玩家的手机语言进行判断
    public static string currentLan
    {
        get
        {
            if (PlayerPrefs.HasKey(KillerGlobal.lanSettingKey))
            {
                return PlayerPrefs.GetString(KillerGlobal.lanSettingKey);
            }
            else
            {
                //string languageCode = KillerOSExtra.me.GetCountry();
                //if (languageCode.ToUpper().Equals("TR"))
                //{
                //    return "tr";
                //}
                //else if(languageCode.ToUpper().Equals("FR"))
                //{
                //    return "fr";
                //}
                return "en-US";
            }
        }
    }

    public static string lanSettingKey = "klp_language";

    public static bool battleSceneUseAB = false;
}