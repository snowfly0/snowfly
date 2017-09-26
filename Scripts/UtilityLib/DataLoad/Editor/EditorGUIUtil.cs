using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System;
namespace DataEditorUtil
{
    public class EditorGUIUtil
    {
        public static void DrawField(FieldInfo info, object obj, string label = "", params GUILayoutOption[] option)
        {
            string fieldName = info.Name;
            if (info.IsLiteral)
            {
                EditorGUILayout.LabelField(label, info.GetValue(null).ToString(), option);
                return;
            }
            if (info.FieldType == typeof(int))
            {
                int value = (int)info.GetValue(obj);
                int newVal = EditorGUILayout.IntField(label, value, option);
                if (value != newVal)
                {
                    info.SetValue(obj, newVal);
                }
            }
            else if (info.FieldType == typeof(float))
            {
                float value = (float)info.GetValue(obj);
                float newVal = EditorGUILayout.FloatField(label, value, option);
                if (value != newVal)
                {
                    info.SetValue(obj, newVal);
                }
            }
            else if (info.FieldType == typeof(string))
            {
                object strobj = info.GetValue(obj);
                string value = strobj == null ? "" : (string)strobj;
                string newVal = EditorGUILayout.TextField(label, value, option);
                if (value != newVal)
                {
                    info.SetValue(obj, newVal);
                }
            }
            else if (info.FieldType == typeof(bool))
            {
                bool value = (bool)info.GetValue(obj);
                bool newVal = EditorGUILayout.Toggle(label, value, option);
                if (value != newVal)
                {
                    info.SetValue(obj, newVal);
                }
            }
            else if (info.FieldType.BaseType == typeof(Enum))
            {
                Enum value = (Enum)info.GetValue(obj);
                Enum newVal = (Enum)EditorGUILayout.EnumPopup(label, value, option);
                if (value != newVal)
                {
                    info.SetValue(obj, newVal);
                }
            }
            else if (info.FieldType == typeof(Color))
            {
                Color value = (Color)info.GetValue(obj);
                Color newVal = EditorGUILayout.ColorField(label, value, option);
                if (value != newVal)
                {
                    info.SetValue(obj, newVal);
                }
            }
            else if (info.FieldType == typeof(Vector3))
            {
                Vector3 value = (Vector3)info.GetValue(obj);
                Vector3 newVal = EditorGUILayout.Vector3Field(label, value, option);
                if (value != newVal)
                {
                    info.SetValue(obj, newVal);
                }
            }
            else if (info.FieldType == typeof(Vector4))
            {
                Vector4 value = (Vector4)info.GetValue(obj);
                Vector4 newVal = EditorGUILayout.Vector4Field(label, value, option);
                if (value != newVal)
                {
                    info.SetValue(obj, newVal);
                }
            }

            else if (info.FieldType == typeof(Vector2))
            {
                Vector2 value = (Vector2)info.GetValue(obj);
                Vector2 newVal = EditorGUILayout.Vector2Field(label, value, option);
                if (value != newVal)
                {
                    info.SetValue(obj, newVal);
                }
            }
            else
            {
                EditorGUILayout.LabelField("暂不支持编辑", option);
            }
        }
    }

}