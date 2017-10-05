using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

public class DataEditorModule
{
    Dictionary<string, List<DataEditorUnit>> _dataUnits;
    public string ModuleName { get; protected set; }

    bool _isDrawUnits = true;

    private DataEditorModule(Type genericType)
    {
        Initialize(genericType);
    }

    public static DataEditorModule CreateModule(Type genericType)
    {
        DataEditorModule module = null;
        if (genericType == typeof(GenericDataWindow<>))
        {
            module = new DataEditorModule(genericType);
            module.ModuleName = "通用字典数据(键值Int)";
        }
        else if (genericType == typeof(GenericDataExWindow<>))
        {
            module = new DataEditorModule(genericType);
            module.ModuleName = "通用字典数据(键值String)";
        }
        else if (genericType == typeof(SettingDataWindow<>))
        {
            module = new DataEditorModule(genericType);
            module.ModuleName = "配置数据";
        }
        else if (genericType == typeof(GenericDataListWindow<>))
        {
            module = new DataEditorModule(genericType);
            module.ModuleName = "通用字典链表数据";
        }
        else if (genericType == typeof(OtherDataWindow<>))
        {
            module = new DataEditorModule(genericType);
            module.ModuleName = "其它数据";
        }
        return module;
    }

    void Initialize(Type genericType)
    {
        _dataUnits = new Dictionary<string, List<DataEditorUnit>>();
        Assembly assembly = Assembly.GetAssembly(genericType);
        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsSubclassOf(typeof(DataWindowBase))) continue;
            Type baseType = type.BaseType;
            if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == genericType)
            {
                AddUnit(new DataEditorUnit(type, baseType.GetGenericArguments()[0]));
            }
        }
    }

    void AddUnit(DataEditorUnit unit)
    {
        List<DataEditorUnit> units;
        if (!_dataUnits.TryGetValue(unit.ModuleName, out units))
        {
            units = new List<DataEditorUnit>();
            _dataUnits.Add(unit.ModuleName, units);
        }
        units.Add(unit);
    }


    public virtual void PaintGUI()
    {
        Color color = GUI.color;
        GUI.color = Color.green;
        _isDrawUnits = EditorGUILayout.Foldout(_isDrawUnits, ModuleName);
        GUI.color = color;
        if (!_isDrawUnits) return;
        foreach (var pair in _dataUnits)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            GUI.color = Color.green;
            EditorGUILayout.LabelField(pair.Key);
            GUI.color = color;
            EditorGUILayout.EndHorizontal();
            foreach (var unit in pair.Value)
            {
                unit.PaintGUI();
                GUILayout.Space(5);
            }
            GUILayout.Space(10);
        }
        GUILayout.Space(20);
    }
}