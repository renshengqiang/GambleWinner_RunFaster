using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using System.Text;
using System.Text.RegularExpressions;

public class FileUtils
{
    /* 文件协议 */
#if UNITY_EDITOR
    public static string STREAM_FILE_PROTOCAL_ = "file:///";
#elif UNITY_IPHONE
    public static string STREAM_FILE_PROTOCAL_ = "file://";
#else
    public static string STREAM_FILE_PROTOCAL_ = "";
#endif

#if UNITY_EDITOR
    public static string PERSISTENT_FILE_PROTOCAL_ = "file:///";
#else
    public static string PERSISTENT_FILE_PROTOCAL_ = "file://";
#endif

    /* 获取stream文件路径前缀*/
    public static string GetStreamingFilePath()
    {
#if UNITY_EDITOR
        return "file:///" + Application.streamingAssetsPath;
#elif UNITY_IPHONE
        return "file://" + Application.streamingAssetsPath;
#else
        return Application.streamingAssetsPath;
#endif

    }

    /// <summary>
    /// 文件是否存在
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool FileExist(string name)
    {
        return File.Exists(name);
    }

	/// <summary>
	/// 保存文件.
	/// </summary>
	/// <param name="content">Content.</param>
	/// <param name="filename">Filename.</param>
	public static void SaveToFile(string content, string dirname, string filename)
	{
		byte[] byte_xml = System.Text.Encoding.Default.GetBytes(content);
		SaveToFile (byte_xml, dirname, filename);
	}

	public static void SaveToFile(byte[] bytes, string dirname, string filename)
	{
		if(!Directory.Exists(dirname))
		{
			Directory.CreateDirectory(dirname);
		}

		string filepath = dirname + filename;
		
		/* 把xml存储到文件中 */
		using (FileStream fileStream = new FileStream(filepath, File.Exists(filepath) ? FileMode.Truncate : FileMode.OpenOrCreate))
		{
			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Close();
			fileStream.Dispose();
		}
	}

    // 读取文件对应的byte数组
	public static byte[] getFileBytes(string filepath)
	{
		if(File.Exists(filepath) == false)
		{
#if LOG_DETAIL
            Debug.LogError("FileUtils: getFileBytes: " + filepath + " 文件不存在");
#endif
			return new byte[0]{};
		}
		byte[] bytes = null;
		using (FileStream fileStream = new FileStream(filepath, FileMode.Open))
		{
			bytes = new byte[fileStream.Length];
			fileStream.Read(bytes, 0, int.Parse(fileStream.Length+""));

			fileStream.Close();
			fileStream.Dispose();
		}
		return bytes;
	}

    // 读取文件对应的字符串
    public static string getFileString(string filepath)
    {
        if (File.Exists(filepath) == false)
        {
            return "";
        }
        byte[] bytes = null;
        using (FileStream fileStream = new FileStream(filepath, FileMode.Open))
        {
            bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, int.Parse(fileStream.Length + ""));

            fileStream.Close();
            fileStream.Dispose();
        }
        return System.Text.Encoding.ASCII.GetString(bytes);
    }

	/// <summary>
	/// 获取文件大小.
	/// </summary>
	/// <returns>The file size.</returns>
	/// <param name="filepath">Filepath.</param>
	public static long GetFileSize(string filepath)
	{
		FileInfo bundleinfo = new FileInfo (filepath);
		long bundlesize = bundleinfo.Length;
		return bundlesize;
	}
	
	/// <summary>
	/// 获取文件的 md5.
	/// </summary>
	/// <returns>The file md5.</returns>
	/// <param name="fileurl">Fileurl.</param>
    public static string getFileMd5(string fileurl)
    {
        string strmd5 = "";
        if (File.Exists(fileurl) == true)
        {
            using (FileStream fileStream = new FileStream(fileurl, FileMode.Open))
            {
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, int.Parse(fileStream.Length + ""));

                MD5 md5 = MD5.Create();
                byte[] bMd5 = md5.ComputeHash(bytes);

                md5.Clear();

                for (int i = 0; i < bMd5.Length; i++)
                {
                    strmd5 += bMd5[i].ToString("x").PadLeft(2, '0');
                }

                fileStream.Close();
                fileStream.Dispose();
            }
        }
        else
        {
#if LOG_DETAIL
            Debug.LogError(fileurl + "文件不存在");
#endif
        }
        return strmd5;
    }

    public static string getBytesMd5(byte[] bytes)
    {
        string strmd5 = "";

        if (bytes != null)
        {
            MD5 md5 = MD5.Create();
            byte[] bMd5 = md5.ComputeHash(bytes);

            md5.Clear();

            for (int i = 0; i < bMd5.Length; i++)
            {
                strmd5 += bMd5[i].ToString("x").PadLeft(2, '0');
            }
        }

        return strmd5;
    }

	public static void renameFile(string source, string destination, bool bDeleteIfExit = false)
	{
		if(File.Exists(destination) == true)
		{
			if(bDeleteIfExit)
			{
				File.Delete(destination);
			}
			else
			{
				return;
			}
		}
		File.Move(source, destination);
    }

    public static void copyFile(string source, string destination)
    {
        if(File.Exists(destination) == true)
        {
            File.Delete(destination);
        }
        try
        {
            File.Copy(source, destination);
        }
        catch
        {
#if LOG_DETAIL
            Debug.Log("拷贝文件" + source + "发生错误，请检查是否存在");
#endif
        }
    }
	
	/// <summary>
	/// 获取文件列表.
	/// </summary>
	/// <returns>The file list.</returns>
	/// <param name="directory_list">Directory_list.</param>
	public static List<string> getFileList(string dirpath, string cont = "*.*")
	{
		List<string> file_list = new List<string> ();

		/* 遍历所有文件 枚举所有依赖 */
		DirectoryInfo directory = new DirectoryInfo(dirpath);
		FileInfo[] dirs = directory.GetFiles(cont, SearchOption.AllDirectories);
		
		/* 遍历所有Prefab */
		foreach (FileInfo info in dirs)
		{
			/* 把遍历到的资源路径存起来 */
			file_list.Add(info.FullName);
		}
		
		return file_list;
	}

    // 清空一个文件的content
    public static void Clear(string fileName)
    {
        if (File.Exists(fileName) == true)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                fileStream.SetLength(0);
                fileStream.Close();
            }
        }
    }

    // 清空一个文件夹
    public static void ClearDir(string dir)
    {
        try
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
        }
        catch
        {
#if LOG_DETAIL
            Debug.LogError("清空文件夹" + dir + "出错");
#endif
        }
    }

    public static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    public static string GetString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    public static void CreateAndWriteToFile(string fileName, byte[] bytes)
    {
        FileStream versionFile = new FileStream(fileName, FileMode.Create);
        BinaryWriter versionBinary = new BinaryWriter(versionFile);
        versionBinary.Write(bytes);
        versionFile.Close();
        //File.WriteAllBytes(fileName, bytes);
    }

    public static void CreateAndWriteToFile(string fileName, string content)
    {
        File.WriteAllText(fileName, content);
    }

    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Get the subdirectories for the specified directory.
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        // If the destination directory doesn't exist, create it. 
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        // Get the files in the directory and copy them to the new location.
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            if (File.Exists(temppath))
                File.Delete(temppath);
            file.MoveTo(temppath);
        }

        // If copying subdirectories, copy them and their contents to new location. 
        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }
    }

    // 获取length对应的字符串
    public static string GetSizeString(long length)
    {
        if (length < 1024)
        {
            return length + "B";
        }
        else if (length < 1048576)
        {
            float num = (float)length / 1024;
            return num.ToString("F1") + "K";
        }
        else
        {
            float num = (float)length / 1048576;
            return num.ToString("F1") + "M";
        }
    }

    // 获取指定长度的字符，如果是中文则取一半长度
    // 如果有截取以..结束
    private static readonly Regex regexEmoji = new Regex(@"\ud83c[\udf00-\udfff]|\ud83d[\udc00-\udeff]|[\u2600-\u26ff]");
    public static string CutStr(string str, int len, string ending = "..")
    {
        try {
            if (str == null || str.Length == 0 || len <= 0)
            {
                return string.Empty;
            }

            str = regexEmoji.Replace(str, "");

            if (str.Contains("\ud83c") || str.Contains("\ud83d"))
            {
                int maxlen = str.Length < len ? str.Length : len;
                return str.Substring(0, maxlen);
            }

            int l = str.Length;

            #region 计算长度
            int clen = 0;
            while (clen < len && clen < l)
            {
                //每遇到一个中文，则将目标长度减一。
                if ((int)str[clen] > 128) { len--; }
                clen++;
            }
            #endregion

            if (clen - len > 0) clen--;     // 如果刚好是最后遇到一个中文，则减去一个字符
            if (clen < l)
            {
                string a = str.Substring(0, clen);
                string b = a + ending;  //".."; william modified
                return b;
            }
            else
            {
                return str;
            }
        }
        catch(Exception)
        {
            return str;
        }
    }

    /// <summary>
    /// 创建一个目录，如果原来的目录已经存在或者创建成功则返回true
    /// 否则返回 false
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool CreateDirectory(string path)
    {
        if (Directory.Exists(path)) return true;
        else
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 删除一个本地文件
    /// </summary>
    /// <param name="file"></param>
    public static void DeleteFile(string file)
    {
        if (File.Exists(file))
        {
            File.Delete(file);
        }
    }
}
