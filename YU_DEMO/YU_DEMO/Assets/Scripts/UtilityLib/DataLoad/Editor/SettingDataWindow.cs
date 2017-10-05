using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using DataEditorUtil;

public class SettingDataWindow<T> : DataWindowBase<T>
    where T : SettingData<T>
{
    FieldInfo[] _fieldInfos;
    string[] _rowTags;
    string[] _rowDescpts;
    string _windowName;
    string _windowDescpt;
    Vector2 _scrollPos;
    protected virtual void OnEnable()
    {
        SettingData<T>.LoadSetting();
        _fieldInfos = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static);
        //List<FieldInfo> infos = new List<FieldInfo>(_fieldInfos);
        //for (int i = infos.Count - 1; i >= 0; --i)
        //{
        //    if (infos[i].IsLiteral)
        //        infos.RemoveAt(i);
        //}
        //_fieldInfos = infos.ToArray();
        _rowTags = new string[_fieldInfos.Length];
        _rowDescpts = new string[_fieldInfos.Length];
        for (int i = 0; i < _fieldInfos.Length; ++i)
        {
            FieldInfo info = _fieldInfos[i];
            _rowTags[i] = GDataDescriptionAttribute.GetNameOrBaseDescpt(info)+":";
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
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        int index = 0;
        foreach (var info in _fieldInfos)
        {
            //EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(_rowTags[index], _rowDescpts[index]));
            EditorGUIUtil.DrawField(info, null, "", GUILayout.MaxWidth(300));
            GUILayout.Space(20);
            // EditorGUILayout.EndHorizontal();
            index++;
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("保存数据", GUILayout.Width(80)))
        {
            SaveSetting();
        }
        if (GUILayout.Button("还原数据", GUILayout.Width(80)))
        {
            SettingData<T>.LoadSetting();
        }
    }





    void SaveSetting()
    {
        try
        {
            SettingData<T>.SaveSetting();
            AssetDatabase.Refresh();
            //this.ShowNotification(new GUIContent("保存成功"));
        }
        catch (Exception ex)
        {
            //this.ShowNotification(new GUIContent("保存失败"));
            Debug.LogError(typeof(T).Name + ex.Message);
        }
    }
}