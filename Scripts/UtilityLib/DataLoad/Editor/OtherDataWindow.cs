using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using DataEditorUtil;
public class OtherDataWindow<T>  : DataWindowBase<T> where T : BaseData<T>
{
    FieldInfo[] _fieldInfos;
    string[] _rowTags;
    string[] _rowDescpts;
    string _windowName;
    string _windowDescpt;
    Vector2 _scrollPos;
    protected virtual void OnEnable()
    {
        _fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static|BindingFlags.Instance);
        _rowTags = new string[_fieldInfos.Length];
        _rowDescpts = new string[_fieldInfos.Length];
        for (int i = 0; i < _fieldInfos.Length; ++i)
        {
            FieldInfo info = _fieldInfos[i];
            _rowTags[i] = GDataDescriptionAttribute.GetNameOrBaseDescpt(info);
            _rowDescpts[i] = GDataDescriptionAttribute.GetDetailDescpt(info);
        }
        InitTitleInfo();
        this.titleContent = new GUIContent(_windowName);
    }

    void InitTitleInfo()
    {
        _windowName = GDataDescriptionAttribute.GetNameOrBaseDescpt(typeof(T));
        _windowDescpt = GDataDescriptionAttribute.GetDetailDescpt(typeof(T));
    }

    void OnGUI()
    {       
        GUILayout.Label(new GUIContent(_windowName, _windowDescpt), EditorStyles.boldLabel);
        GUILayout.Label("数据说明：");
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        int index = 0;
        foreach(var info in _fieldInfos)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(info.Name+":",GUILayout.Width(250));
            EditorGUILayout.LabelField(_rowTags[index]+"--"+_rowDescpts[index]);
            EditorGUILayout.EndHorizontal();
            index++;
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("访问文件夹",GUILayout.Width(100)))
        {
            string dir = EditorUtil.GetFileDirectory(BaseData<T>.GetEditorFullPath());
            try
            {
                System.Diagnostics.Process.Start(dir);
            }
            catch
            {
                this.ShowNotification(new GUIContent("访问失败，目录错误"));
            }
        }
    }

 

}