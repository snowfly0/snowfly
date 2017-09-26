using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;

namespace DataEditorUtil
{
    public class EditorUtil
    {

        public static T GetClassAttribute<T>(Type type) where T : System.Attribute
        {
            T attribute = null;
            object[] objs = type.GetCustomAttributes(false);
            foreach (var o in objs)
            {
                attribute = o as T;
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return null;
        }

        /// <summary>
        /// 显示是否选择窗口
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool DisplayNotify(string message)
        {
            return EditorUtility.DisplayDialog("提示", message, "Yes", "No");
        }

        /// <summary>
        /// 获取文件的所在目录
        /// </summary>
        /// <param name="fileFullname">文件完全路径</param>
        /// <returns></returns>
        public static string GetFileDirectory(string fileFullname)
        {
            string[] strs = fileFullname.Split('/');
            return fileFullname.Replace(strs[strs.Length - 1], "");
        }
    }

}