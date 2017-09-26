using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class BaseData
{

}

public class BaseData<T> : BaseData
{
    public static GameDataType DataType
    {
        get
        {
            return (GameDataType)(typeof(T).GetField("_DataType", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null));
        }
    }

    public static string DataPath
    {
        get
        {
            string path = (string)(typeof(T).GetField("_DataPath", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).GetValue(null));
            if (DataType == GameDataType.XMLResources)
            {
                if (path.EndsWith(".xml"))
                {
                    //path = path.Replace(".xml", "");
                    path = path.Remove(path.Length - 4, 4);
                }
				if (path.StartsWith("/"))
				{
					path = path.Remove(0,1);
				}
            }
            else
            {
                if (!path.StartsWith("/"))
                {
                    path = path.Insert(0, "/");
                }
            }
            return path;
        }
    }

    /// <summary>
    /// 完全路径,如果不存在返回null
    /// </summary>
    public static string ExternalPath
    {
        get
        {
            switch (DataType)
            {
                case GameDataType.XMLResources:
                    {
#if UNITY_EDITOR
                        return Application.dataPath + "/Resources/" + DataPath + ".xml";
#endif
                        Debug.LogError(string.Format("{0}类型数据不存在外部路径", typeof(T).Name));
                        break;
                    }
                case GameDataType.XMLPersistent:
                    {
                        return Application.persistentDataPath + DataPath;
                    }
                case GameDataType.XMLStreaming:
                    {
                        string path= Application.streamingAssetsPath + DataPath;
                        return path;
                    }
                case GameDataType.XMLTempCache:
                    {
                        return Application.temporaryCachePath + DataPath;
                    }
            }
            return null;
        }
    }


    public static string GetEditorFullPath()//为Editor原有功能保留的函数
    {
        return ExternalPath;
    }
}

