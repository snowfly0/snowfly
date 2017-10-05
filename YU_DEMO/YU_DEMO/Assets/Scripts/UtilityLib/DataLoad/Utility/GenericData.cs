using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using GameData;
using System;

public enum GameDataType
{
    /// <summary>
    /// 内部数据（只读）
    /// </summary>
    XMLResources,
    /// <summary>
    /// 持久外部数据，手机平台只读不可写
    /// </summary>
    XMLStreaming,
    /// <summary>
    /// 持久外部数据
    /// </summary>
    XMLPersistent,
    /// <summary>
    /// 临时数据
    /// </summary>
    XMLTempCache,

}


public class GenericData<T> : BaseData<T> where T : class,new()
{
    public int Id { get; set; }

    protected static Dictionary<int, T> _DataMap;
    public static Dictionary<int, T> DataMap
    {
        get
        {
            if (_DataMap == null)
            {
                switch (DataType)
                {
                    case GameDataType.XMLResources:
                        {
                            _DataMap = XmlParser.LoadIntDictionary<T>(DataPath);
                            break;
                        }
                    case GameDataType.XMLPersistent:
                        {
                            string persisiPath = ExternalPath;
                            if (Application.platform == RuntimePlatform.Android)
                            {
                                persisiPath = "file://" + ExternalPath;
                            }
                            _DataMap = XmlParser.LoadIntDictionaryExternal<T>(persisiPath);
                            break;
                        }
                    default:
                        {
                            _DataMap = XmlParser.LoadIntDictionaryExternal<T>(ExternalPath);
                            break;
                        }
                }
            }
            return _DataMap;
        }
    }


    public static T GetData(int id)
    {
        if (DataMap == null) return null;
        T t;
        _DataMap.TryGetValue(id, out t);
        return t;
    }

    public static void UnLoadDataMap()
    {
        _DataMap = null;
    }

    public T Clone()
    {
        return MemberwiseClone() as T;
    }


    public static void Save(Dictionary<int, T> dataMap)
    {
        _DataMap = dataMap;
        Save();
    }

    public static void Save()
    {
        if (_DataMap == null)
        {
            Debug.LogError("No data for save:" + typeof(T).Name);
            return;
        }
        XmlParser.SaveXmlIntDictionary<T>(ExternalPath, _DataMap);
    }

}


public class GenericDataEx<T> : BaseData<T> where T : class,new()
{
    public string Id { get; set; }

    protected static Dictionary<string, T> _DataMap;
    public static Dictionary<string, T> DataMap
    {
        get
        {
            if (_DataMap == null)
            {

                switch (DataType)
                {
                    case GameDataType.XMLResources:
                        {
                            _DataMap = XmlParser.LoadStringDictionary<T>(DataPath);
                            break;
                        }
                    case GameDataType.XMLPersistent:
                        {
                            string persisiPath = ExternalPath;
                            if(Application.platform==RuntimePlatform.Android)
                            {
                                persisiPath ="file://"+ExternalPath;
                            }
                            _DataMap = XmlParser.LoadStringDictionaryExternal<T>(persisiPath);
                            break;
                        }
                    default:
                        {
                            _DataMap = XmlParser.LoadStringDictionaryExternal<T>(ExternalPath);
                            break;
                        }
                }
            }
            return _DataMap;
        }
    }

    public static T GetData(string id)
    {
        if (DataMap == null) return null;
        T t;
        _DataMap.TryGetValue(id, out t);
        return t;
    }


    public static void UnLoadDataMap()
    {
        _DataMap = null;
    }

    public T Clone()
    {
        return MemberwiseClone() as T;
    }


    public static void Save(Dictionary<string, T> dataMap)
    {
        _DataMap = dataMap;
        Save();
    }

    public static void Save()
    {
        if (_DataMap == null)
        {
            Debug.LogError("No data for save:" + typeof(T).Name);
            return;
        }
        XmlParser.SaveXmlStringDictionary<T>(ExternalPath, _DataMap);
    }

}


public class GenericDataList<T> : BaseData<T> where T : class,new()
{
    protected static Dictionary<string, List<T>> _DataMap;
    public static Dictionary<string, List<T>> DataMap
    {
        get
        {
            if (_DataMap == null)
            {
                switch (DataType)
                {
                    case GameDataType.XMLResources:
                        {
                            _DataMap = _DataMap = XmlParser.LoadDataMap<T>(DataPath);
                            break;
                        }
                    default:
                        {
                            _DataMap = XmlParser.LoadDataMapExternal<T>(ExternalPath);
                            break;
                        }
                }
            }
            return _DataMap;
        }
    }

    public static void UnLoadDataMap()
    {
        _DataMap = null;
    }

    public static void Save()
    {
        if (_DataMap == null)
        {
            Debug.LogError("No data for save:" + typeof(T).Name);
            return;
        }

        XmlParser.SaveXmlDictionaryList<T>(ExternalPath, _DataMap);
    }

    public T Clone()
    {
        return MemberwiseClone() as T;
    }
}
