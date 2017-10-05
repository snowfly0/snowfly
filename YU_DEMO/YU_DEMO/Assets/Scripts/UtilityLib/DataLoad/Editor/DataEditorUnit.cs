using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Xml;
using DataEditorUtil;
/*需求
 * 显示path名
 * 打开所在路径
 * 打开窗口
*/
public class DataEditorUnit
{
    public string ModuleName { get; protected set; }
    public string UnitName { get; protected set; }
    string _fullpath;
    Type _dataType;
    Type _windowType;
    GameDataType _gameDataType = GameDataType.XMLResources;
    bool _isInitOK;

    public DataEditorUnit(Type windowType, Type dataType)
    {
        GDataModuleAttribute atb = GDataModuleAttribute.GetAttribute(dataType);
        if (atb != null)
        {
            ModuleName = atb.ModuleName;
        }
        else
        {
            ModuleName = "默认分组";
        }
        UnitName = GDataDescriptionAttribute.GetNameOrBaseDescpt(dataType);
        _isInitOK = true;
        try
        {
            _fullpath = (string)dataType.GetMethod("GetEditorFullPath", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, null);
        }
        catch (Exception ex)
        {
            _isInitOK = false;
            Debug.LogError(dataType.Name+"缺少路径声明，初始化失败"+ex.Message);
        }
        try
        {
            _gameDataType = (GameDataType)dataType.GetProperty("DataType", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).GetValue(null, null);
        }
        catch (Exception ex)
        {
            _isInitOK = false;
            Debug.LogError(dataType.Name+"缺少数据类型，声明初始化失败"+ex.Message);
        }
        _dataType = dataType;
        _windowType = windowType;
    }

    public void PaintGUI()
    {
        if (!_isInitOK) return;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(30);
        EditorGUILayout.LabelField(UnitName);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(30);
        EditorGUILayout.LabelField("存储路径：" + _fullpath);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(30);
        if (GUILayout.Button("访问目录", GUILayout.Width(80)))
        {
            AccessDirectory();
        }
        if (GUILayout.Button("打开窗口", GUILayout.Width(80)))
        {
            _windowType.GetMethod("ShowDataWindow", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, null);
        }
        if (GUILayout.Button("为XML文件写入注释", GUILayout.Width(170)))
        {
            CreateXmlComment();
        }
        EditorGUILayout.EndHorizontal();
    }

    void AccessDirectory()
    {
        try
        {
            System.Diagnostics.Process.Start(EditorUtil.GetFileDirectory(_fullpath));
        }
        catch (Exception ex)
        {
            Debug.Log("访问目录失败：" + ex.Message);
        }
    }

    void CreateXmlComment()
    {
        string comment = GameData.XmlParser.CreateXmlComment(_dataType);
        try
        {
            XmlDocument doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader reader = XmlReader.Create(_fullpath, settings);
            doc.Load(reader);
            reader.Close();
            doc.InsertBefore(doc.CreateComment(comment), doc.DocumentElement);
            doc.Save(_fullpath);
            EditorWindow.GetWindow<DataEditorWindow>().ShowNotification(new GUIContent("写入注释成功"));
        }
        catch (Exception ex)
        {
            Debug.LogError("写入注释失败：" + ex.Message);
        }
    }
}

