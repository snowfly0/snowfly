using System;
using System.Reflection;
using UnityEngine;


public class SettingData<T>:BaseData<T>
{

    static SettingData()
    {
        LoadSetting();
    }

    public static void LoadSetting()
    {
        switch(DataType)
        {
            case GameDataType.XMLResources:
                {
                    GameData.XmlParser.LoadXmlSetting<T>(DataPath);
                    break;
                }
            default:
                {
                    GameData.XmlParser.LoadXmlSettingExternal<T>(ExternalPath);
                    break;
                }
        }
    }

    public static void SaveSetting()
    {
        GameData.XmlParser.SaveXmlSetting<T>(ExternalPath);
    }

 
}