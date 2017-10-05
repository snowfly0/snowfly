using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using GameData;
using System;
using DataEditorUtil;

public class GenericDataWindow<T> : DataWindowBase<T> where T : GenericData<T>, new() 
{
    public class GDataPair
    {
        public int Key;
        public T TValue;
        public GDataPair(int key, T val)
        {
            Key = key;
            TValue = val;
        }
    }

    protected Dictionary<int, T> _dataMap;
    protected List<GDataPair> _dataPairs;
    string _windowName;
    string _windowDescpt;
    FieldInfo[] _fielfInfos;
    string[] _rowTags;
    string[] _rowDescpts;
    float[] _rowLengths;
    bool[] _isFoldouts;
    int _drawIndex;
    GUILayoutOption _heightOfRowOption;
    Vector2 _scrollPos;
    Vector2 _rowNameScrollPos;
    Vector3 _keysScrollPos;
    protected virtual void OnEnable()
    {
        _heightOfRowOption = GUILayout.Height(20);
        InitLoadData();
        InitTitleInfo();
        InitFieldInfos();
        this.titleContent = new GUIContent(_windowName);
    }

    void InitLoadData()
    {
        _dataMap = GenericData<T>.DataMap;
        _dataPairs = new List<GDataPair>();
        GDataPair gpair = null;
        foreach (var pair in _dataMap)
        {
            gpair = new GDataPair(pair.Key, pair.Value);
            _dataPairs.Add(gpair);
        }
    }

    void InitTitleInfo()
    {
        _windowName = GDataDescriptionAttribute.GetNameOrBaseDescpt(typeof(T));
        _windowDescpt = GDataDescriptionAttribute.GetDetailDescpt(typeof(T));
    }

    void InitFieldInfos()
    {
        Type type = typeof(T);
        _fielfInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        _rowTags = new string[_fielfInfos.Length + 1];
        _rowTags[0] = "标识符";
        _rowDescpts = new string[_fielfInfos.Length + 1];
        _isFoldouts = new bool[_fielfInfos.Length];
        _rowLengths = new float[_rowTags.Length];
        _rowLengths[0] = 140;
        for (int i = 0; i < _fielfInfos.Length; ++i)
        {
            GDataDescriptionAttribute attribute = null;
            attribute = Attribute.GetCustomAttribute(_fielfInfos[i], typeof(GDataDescriptionAttribute)) as GDataDescriptionAttribute;
            float rate = 10f;
            if (attribute != null)
            {
                _rowTags[i + 1] = attribute.BaseDescpt;
                _rowDescpts[i + 1] = attribute.DetailDescpt;
                rate = 15f;
            }
            else
            {
                _rowTags[i + 1] = _fielfInfos[i].Name;
            }
            float minlen = GetFieldMinWidth(_fielfInfos[i]);
            float rowlen = _rowTags[i + 1].Length * rate;
            _rowLengths[i + 1] = rowlen > minlen ? rowlen : minlen;
        }
    }

    protected virtual void OnGUI()
    {
        GUILayout.Label(new GUIContent(_windowName, _windowDescpt), EditorStyles.boldLabel);
        EditorGUILayout.LabelField("(勾选以应用批量修改)");
        DrawRowNames();
        DrawValues();
        DrawOperations();
    }


    void DrawValues()
    {
    
        _keysScrollPos.y = _scrollPos.y;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginScrollView(_keysScrollPos, true, false, GUILayout.Width(_rowLengths[0]));
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < _dataPairs.Count; ++i)
        {
            var pair = _dataPairs[i];
            pair.Key = EditorGUILayout.IntField(string.Empty, pair.Key, GUILayout.Width(_rowLengths[0]), _heightOfRowOption);
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        for (int i = 0; i < _dataPairs.Count; ++i)
        {
            _drawIndex = i;
            DrawData(_dataPairs[i]);
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }

    void DrawOperations()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加新项", GUILayout.Width(80)))
        {
            T newItem = new T();
            var pair = new GDataPair(0, newItem);
            _dataPairs.Add(pair);
        }
        if (GUILayout.Button("移除全部", GUILayout.Width(80)))
        {
            if (EditorUtil.DisplayNotify("删除后无法恢复，是否确认"))
            {
                _dataPairs.Clear();
            }
        }
        if (GUILayout.Button("保存数据", GUILayout.Width(80)))
        {
            _dataMap.Clear();
            bool isSameKey = false;
            for (int i = 0; i < _dataPairs.Count; ++i)
            {
                var pair = _dataPairs[i];
                try
                {
                    _dataMap.Add(pair.Key, pair.TValue);
                }
                catch
                {
                    isSameKey = true;
                }
            }
            string notifyMsg = isSameKey ? "部分数据的标识符冲突，保存失败" : "保存成功";
            if (!isSameKey)
            {
                try
                {
                    GenericData<T>.Save();
                }
                catch (System.Exception ex)
                {
                    notifyMsg = "数据格式有误，保存失败";
                    Debug.LogError(ex.Message);
                }
            }
            AssetDatabase.Refresh();
            this.ShowNotification(new GUIContent(notifyMsg));
        }
        if (GUILayout.Button("访问所在目录", GUILayout.Width(120)))
        {
            string dir = EditorUtil.GetFileDirectory(GenericData<T>.GetEditorFullPath());
            try
            {
                System.Diagnostics.Process.Start(dir);
            }
            catch
            {
                this.ShowNotification(new GUIContent("访问失败，目录错误"));
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawRowNames()
    {
        _rowNameScrollPos.x = _scrollPos.x;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(string.Empty, GUILayout.Width(_rowLengths[0]));
        EditorGUILayout.BeginScrollView(_rowNameScrollPos, GUIStyle.none, GUIStyle.none, GUILayout.Height(30));
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < _isFoldouts.Length; ++i)
        {
            _isFoldouts[i] = EditorGUILayout.Toggle(string.Empty, _isFoldouts[i], GUILayout.Width(_rowLengths[i + 1]));
        }
        EditorGUILayout.LabelField(string.Empty, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent(_rowTags[0], _rowDescpts[0]), GUILayout.Width(_rowLengths[0]));
        EditorGUILayout.BeginScrollView(_rowNameScrollPos, GUIStyle.none, GUIStyle.none, GUILayout.Height(30));
        EditorGUILayout.BeginHorizontal();
        for (int i = 1; i < _rowTags.Length; ++i)
        {
            EditorGUILayout.LabelField(new GUIContent(_rowTags[i], _rowDescpts[i]), GUILayout.Width(_rowLengths[i]));
        }
        EditorGUILayout.LabelField(string.Empty, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
        
    }

    protected virtual void DrawData(GDataPair pair)
    {
        EditorGUILayout.BeginHorizontal();
        int fieldIndex = 0;
        foreach (var info in _fielfInfos)
        {
            GUILayoutOption option = GUILayout.Width(_rowLengths[fieldIndex + 1]);
            if(_isFoldouts[fieldIndex])
            {
                EditorGUI.BeginChangeCheck();
            }
            EditorGUIUtil.DrawField(info, pair.TValue,string.Empty,option,_heightOfRowOption);
            if(_isFoldouts[fieldIndex]&&EditorGUI.EndChangeCheck())
            {
                object value = info.GetValue(pair.TValue);
                foreach (var data in _dataPairs)
                {
                    info.SetValue(data.TValue,value);
                }
            }
            fieldIndex++;
        }
        if (GUILayout.Button(new GUIContent("C", "复制并在最后插入"), GUILayout.Width(25)))
        {
            _dataPairs.Add(new GDataPair(pair.Key, pair.TValue.Clone()));
        }
        if (GUILayout.Button(new GUIContent("I", "复制并在下方插入"), GUILayout.Width(25)))
        {
            _dataPairs.Insert(_drawIndex, new GDataPair(pair.Key, pair.TValue.Clone()));
        }
        if (GUILayout.Button(new GUIContent("X", "删除"), GUILayout.Width(25)))
        {
            _dataPairs.RemoveAt(_drawIndex);
        }
        EditorGUILayout.EndHorizontal();
    }




    float GetFieldMinWidth(FieldInfo info)
    {
        if (info.FieldType == typeof(int))
        {
            return 70;
        }
        else if (info.FieldType == typeof(float))
        {
            return 70;
        }
        else if (info.FieldType == typeof(string))
        {
            return 140;
        }
        else if (info.FieldType == typeof(bool))
        {
            return 30;
        }
        else if (info.FieldType.BaseType == typeof(Enum))
        {
            return 100;
        }
        else if (info.FieldType == typeof(Color))
        {
            return 70;
        }
        else if (info.FieldType == typeof(Vector3))
        {
            return 210;
        }
        else if (info.FieldType == typeof(Vector2))
        {
            return 140;
        }
        else if (info.FieldType == typeof(Vector4))//|| info.FieldType == typeof(Quaternion))
        {
            return 280;
        }
        return 150;
    }


    void OnDisable()
    {

    }

    protected override void OnDestroy()
    {
        GenericData<T>.UnLoadDataMap();
        base.OnDestroy();
    }
}



