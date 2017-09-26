using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using GameData;
using System;
using System.IO;
using System.Text;
using DataEditorUtil;



public class DataEditorWindow : EditorWindow
{
    [MenuItem("Tools/数据编辑器")]
    static void DoIt()
    {

        InitWindows();
        var window = EditorWindow.GetWindow<DataEditorWindow>();
        window.titleContent = new GUIContent("数据编辑器");
        window.position = new Rect(50, 50, Screen.currentResolution.width * 0.8f, Screen.currentResolution.height * 0.8f);
    }
    Vector2 _scrollPos;
    List<DataEditorModule> _editorModules;
    void OnEnable()
    {
        _editorModules = new List<DataEditorModule>();
        Assembly assembly = Assembly.GetAssembly(GetType());
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsGenericTypeDefinition)
            {
                DataEditorModule module = DataEditorModule.CreateModule(t);
                if (module != null)
                {
                    _editorModules.Add(module);
                }
            }
        }
    }

    void OnGUI()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        for (int i = 0; i < _editorModules.Count; ++i)
        {
            _editorModules[i].PaintGUI();
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("关闭所有窗口", GUILayout.Width(100)))
        {
            Assembly assembly = Assembly.GetAssembly(GetType());
            foreach (var t in assembly.GetTypes())
            {
                if (t.IsSubclassOf(typeof(DataWindowBase)) && !t.IsGenericType)
                {
                    EditorWindow.FocusWindowIfItsOpen(t);
                    if (focusedWindow.GetType().IsSubclassOf(typeof(DataWindowBase)))
                    {
                        focusedWindow.Close();
                    }
                }
            }
        }
    }

    static void InitWindows()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(BaseData<>));
        Assembly editorAssembly = Assembly.GetAssembly(typeof(DataEditorWindow));
        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsSubclassOf(typeof(BaseData))) continue;
            if (type.IsGenericTypeDefinition) continue;
            if (EditorUtil.GetClassAttribute<NoDataWindowAttribute>(type) != null) continue;
            Type baseType = type.BaseType;
            if (baseType != null && baseType.IsGenericType)
            {
                Type winType = null;
                if (baseType.GetGenericTypeDefinition() == typeof(GenericData<>))
                {
                    winType = typeof(GenericDataWindow<>).MakeGenericType(type);
                }
                else if (baseType.GetGenericTypeDefinition() == typeof(GenericDataEx<>))
                {
                    winType = typeof(GenericDataExWindow<>).MakeGenericType(type);
                }
                else if (baseType.GetGenericTypeDefinition() == typeof(SettingData<>))
                {
                    winType = typeof(SettingDataWindow<>).MakeGenericType(type);
                }
                else if (baseType.GetGenericTypeDefinition() == typeof(GenericDataList<>))
                {
                    winType = typeof(GenericDataListWindow<>).MakeGenericType(type);
                }
                else
                {
                    winType = typeof(OtherDataWindow<>).MakeGenericType(type);
                }
                if (!IsExistParentType(editorAssembly, winType))
                {
                    if (!IsExistParentType(editorAssembly, typeof(OtherDataWindow<>).MakeGenericType(type)))
                    {
                        CreateWindowCode(winType, type);
                    }
                }
            }
        }
        AssetDatabase.Refresh();
    }

    static bool IsExistParentType(Assembly asb, Type type)
    {
        foreach (var t in asb.GetTypes())
        {
            if (t.IsSubclassOf(type)) return true;
        }
        return false;
    }

    static void CreateWindowCode(Type winType, Type dataType)
    {
        string dirpath = Directory.GetCurrentDirectory() + "/Assets/Editor/DataWindows/";
        if (!Directory.Exists(dirpath))
        {
            Directory.CreateDirectory(dirpath);
        }
        string dataTypeName = dataType.FullName.Replace("+", ".");
        string winTypeName = dataType.FullName.Replace("+", "_").Replace(".", "__") + "DataWindow";
        string filepath = dirpath + winTypeName + ".cs";
        if (File.Exists(filepath))
        {
            if (!EditorUtility.DisplayDialog("提示", "已存在cs文件" + filepath + "，是否覆盖", "Yes", "No"))
            {
                return;
            }
            else
            {
                File.Delete(filepath);
            }
        }
        FileStream fs = new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
        StreamWriter sw = new StreamWriter(fs);
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("//this file is created auto");
        sb.AppendLine("//create time:" + DateTime.Now);
        sb.AppendLine("namespace DataWindow");
        sb.AppendLine("{");
        sb.AppendLine("public  class " + winTypeName + ":" + winType.Name.Split('`')[0] + "<" + dataTypeName + ">");
        sb.AppendLine("{");
        sb.AppendLine("public static void ShowDataWindow()");
        sb.AppendLine("{");
        sb.AppendLine("GetWindow<" + winTypeName + ">" + "(typeof(DataEditorWindow));");
        sb.AppendLine("}");
        sb.AppendLine("}");
        sb.AppendLine("}");
        sw.Write(sb.ToString());
        sw.Close();
        fs.Close();
    }


}