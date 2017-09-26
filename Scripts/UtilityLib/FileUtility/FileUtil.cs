using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class FileUtility
{
    public static string GetMD5HashFromBytes(byte[] bytes)
    {
        try
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("GetMD5HashFromFile() fail,error" + ex.Message);
        }
    }

    public static void SaveImage(string path, byte[] bytes)
    {
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
    }

    public static void StartUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return;
        }
        try
        {
            Application.OpenURL(url);
        }
        catch (Exception ex)
        {
            Debug.LogError("打开url:"+url+" 失败:" + ex.Message);
        }
    }
}

