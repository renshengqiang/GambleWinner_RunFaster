using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

public class BundleUtils 
{

	/// <summary>
	/// 是否忽略.
    /// 如果是指定类型的文件如.cs,.shader等需要忽略；
    /// 如果在忽略白名单中的文件也需要忽略。
	/// </summary>
	/// <returns><c>true</c>, if escape was ised, <c>false</c> otherwise.</returns>
	/// <param name="filepath">Filepath.</param>
	public static bool isIgnoreFile(string filepath)
	{
		bool b_ignore = false;
		foreach(string ignore in BundleConfig.IGNORE_FILE_SUFFIX_ARR)
		{
            if (filepath.EndsWith(ignore) == true)
			{
				b_ignore = true;
				break;
			}
		}

        foreach (string ignore in BundleConfig.ASSET_IGNORE_ARR)
        {
            if (ignore.EndsWith("/"))
            {
                if (filepath.Contains(ignore))
                {
                    b_ignore = true;
                    break;
                }
            }
            else
            {
                int length = filepath.LastIndexOf('.');
                if (-1 == length) length = filepath.Length;
                string file = filepath.Substring(0, length);
                if(file.EndsWith(ignore))
                {
                    b_ignore = true;
                    break;
                }
            }
        }
		return b_ignore;
	}
}
