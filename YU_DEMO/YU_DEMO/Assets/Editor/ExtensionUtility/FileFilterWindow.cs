using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
/// <summary>
/// 文件过滤器，功能：
/// 1.将选定所有文件夹下指定后缀的文件转移到某一目录
/// 2.删除选定所有文件夹下指定后缀的文件
/// 3.删除选定所有文件夹的所有子文件夹
/// 4.替换更新资源到选定目录
/// </summary>
public class FileFilterWindow : EditorWindow
{
    [MenuItem("Tools/文件过滤器")]
    static void DoIt()
    {
        GetWindow<FileFilterWindow>();
    }
    string _filtersMove;
    string _filtersDelete;
    string _moveToFolder;
    bool _useSetPath;
    bool _useReplaceRegex;
    string _filterReplaceRegex;
    string _setPath;
    void OnEnable()
    {
        LoadPrefs();
    }

    void LoadPrefs()
    {
        FieldInfo[] infos = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        string firstName = "Editor.FileFilterWindow.";
        foreach (var info in infos)
        {
            if (!info.Name.StartsWith("_")) continue;
            string name = firstName + info.Name;
            if (info.FieldType == typeof(int))
            {
                info.SetValue(this, PlayerPrefs.GetInt(name));
            }
            else if (info.FieldType == typeof(string))
            {
                string value = PlayerPrefs.GetString(name);
                if (value != null)
                {
                    info.SetValue(this, value);
                }
            }
            else if (info.FieldType == typeof(bool))
            {
                info.SetValue(this, PlayerPrefs.GetInt(name) == 1);
            }
            else if (info.FieldType == typeof(float))
            {
                info.SetValue(this, PlayerPrefs.GetFloat(name));
            }
        }
    }


    void OnDisable()
    {
        SavePrefs();
    }

    void SavePrefs()
    {
        FieldInfo[] infos = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        string firstName = "Editor.FileFilterWindow.";
        foreach (var info in infos)
        {
            if (!info.Name.StartsWith("_")) continue;
            string name = firstName + info.Name;
            if (info.FieldType == typeof(int))
            {
                PlayerPrefs.SetInt(name, (int)info.GetValue(this));
            }
            else if (info.FieldType == typeof(bool))
            {
                int value = (bool)info.GetValue(this) == true ? 1 : 0;
                PlayerPrefs.SetInt(name, value);
            }
            else if (info.FieldType == typeof(string))
            {
                string value = (string)info.GetValue(this);
                if (value != null)
                {
                    PlayerPrefs.SetString(name, value);
                }
            }
            else if (info.FieldType == typeof(float))
            {
                PlayerPrefs.SetFloat(name, (float)info.GetValue(this));
            }
        }
        PlayerPrefs.Save();
    }

    void OnGUI()
    {
        UpdateFilterMove();
        EditorGUILayout.Space();
        UpdateFilterDelete();
        EditorGUILayout.Space();
        UpdateFileDelete();
        EditorGUILayout.Space();
        UpdateFileReplace();
    }


    void UpdateFilterMove()
    {
        EditorGUILayout.LabelField("1.过滤文件到指定文件夹");
        EditorGUILayout.LabelField("目标文件夹");
        EditorGUILayout.BeginHorizontal();
        _moveToFolder = EditorGUILayout.TextField(_moveToFolder, GUILayout.Width(300));
        if (GUILayout.Button("浏览", GUILayout.Width(80)))
        {
            string p = EditorUtility.OpenFolderPanel("选择文件夹", Directory.GetCurrentDirectory(), "");
            if (!string.IsNullOrEmpty(p))
            {
                _moveToFolder = p;
            }
        }
        EditorGUILayout.EndHorizontal();
        _useSetPath = EditorGUILayout.Toggle("使用指定输入目录", _useSetPath);
        if (_useSetPath)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(_setPath, GUILayout.Width(300));
            if (GUILayout.Button("浏览", GUILayout.Width(80)))
            {
                string p = EditorUtility.OpenFolderPanel("选择文件夹", Directory.GetCurrentDirectory(), "");
                if (!string.IsNullOrEmpty(p))
                {
                    _setPath = p;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        _useReplaceRegex = EditorGUILayout.Toggle("使用正则表达式更名", _useReplaceRegex);
        if (_useReplaceRegex)
        {
            _filterReplaceRegex = EditorGUILayout.TextField(_filterReplaceRegex, GUILayout.Width(300));
        }
        EditorGUILayout.LabelField("过滤类型，小写后缀，用分号隔开(如“.jpg;.png”)");
        EditorGUILayout.BeginHorizontal();
        _filtersMove = EditorGUILayout.TextField(_filtersMove, GUILayout.Width(300));
        if (GUILayout.Button("过滤", GUILayout.Width(60)))
        {
            FilterCopyFiles();
        }
        EditorGUILayout.EndHorizontal();
    }

    void FilterCopyFiles()
    {
        if (string.IsNullOrEmpty(_filtersMove))
        {
            this.ShowNotification(new GUIContent("过滤字符不能为空"));
            return;
        }
        string[] endStrs = _filtersMove.Split(';');
        List<FileInfo> infos = _useSetPath ? EditorUtil.GetFiles(_setPath, true, endStrs) : GetSelectedPathesFiles(endStrs);
        foreach (var info in infos)
        {
            string destName = string.Empty;
            if (!_useReplaceRegex)
            {
                destName = info.Name;
            }
            else
            {
                Regex reg = new Regex(@_filterReplaceRegex);
                Match macth = reg.Match(info.Name);
                for (int i = 0; i < macth.Groups.Count; ++i)
                {
                    destName += macth.Groups[i].Value;
                }
                //destName = Regex.Replace(info.Name, @_filterReplaceRegex,"");
                string[] values = info.Name.Split('.');
                destName += "." + values[values.Length - 1];
            }
            string destpath = _moveToFolder + "/" + destName;
            info.CopyTo(destpath, true);
            Debug.Log("复制文件：" + info.FullName + " 到:" + destpath);
            //临时代码生成
            //bool isFind = false;
            //string targetName = info.Name.Replace(".mp3","");
            //foreach (TaskData data in TaskData.DataMap.Values)
            //{
            //    if (data.AudioName != targetName) continue;
            //    isFind = true;
            //    destName = data.Id + "、" + info.Name;
            //    string destpath = _moveToFolder + "/" + destName;
            //    info.CopyTo(destpath, true);
            //    Debug.Log("复制文件：" + info.FullName + " 到:" + destpath);
            //}
            //if(!isFind)
            //{
            //    Debug.LogError("没有对应的任务:" + info.Name);
            //}
        }
        AssetDatabase.Refresh();
    }


    void UpdateFilterDelete()
    {
        EditorGUILayout.LabelField("2.删除选定文件夹下的指定类型文件");
        EditorGUILayout.LabelField("过滤类型，小写后缀，用分号隔开(如“.jpg;.png”)");
        EditorGUILayout.BeginHorizontal();
        _filtersDelete = EditorGUILayout.TextField(_filtersDelete, GUILayout.Width(300));
        if (GUILayout.Button("删除", GUILayout.Width(60)) && EditorUtil.DisplayNotify("删除后无法恢复"))
        {
            FilterDeleteFiles();
        }
        EditorGUILayout.EndHorizontal();
    }

    void FilterDeleteFiles()
    {
        if (string.IsNullOrEmpty(_filtersDelete))
        {
            this.ShowNotification(new GUIContent("过滤字符不能为空"));
            return;
        }
        string[] endStrs = _filtersDelete.Split(';');
        List<FileInfo> infos = GetSelectedPathesFiles(endStrs);
        foreach (var info in infos)
        {
            info.Delete();
            string metaPath = info.FullName + ".meta";
            if (File.Exists(metaPath))
            {
                File.Delete(metaPath);
            }
            Debug.Log("删除文件：" + info.FullName);
        }
        AssetDatabase.Refresh();
    }


    void UpdateFileDelete()
    {
        EditorGUILayout.LabelField("3.删除选定文件夹下的子文件夹");
        if (GUILayout.Button("删除子文件夹") && EditorUtil.DisplayNotify("删除后无法恢复"))
        {
            foreach (string path in EditorUtil.GetSelectedFoldersPathes(true))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                foreach (DirectoryInfo info in dirInfo.GetDirectories())
                {
                    info.Delete(true);
                    string metaPath = info.FullName + ".meta";
                    if (File.Exists(metaPath))
                    {
                        File.Delete(metaPath);
                    }
                    Debug.Log("删除目录：" + info.FullName);
                }
            }
            AssetDatabase.Refresh();
        }
    }
    List<FileInfo> GetSelectedPathesFiles(string[] endStrs)
    {
        List<FileInfo> infos = new List<FileInfo>();
        foreach (string path in EditorUtil.GetSelectedFoldersPathes(true))
        {
            infos.AddRange(EditorUtil.GetFiles(path, true, endStrs));
        }
        return infos;
    }


    string _replaceSourcePath;
    bool _isIncludeChildren;
    string _filterStringReplace;
    bool _isConfirmEachReplace;
    void UpdateFileReplace()
    {
        EditorGUILayout.LabelField("4.替换文件到指定文件夹的子目录中的同名文件");
        EditorGUILayout.LabelField("源文件夹");
        EditorGUILayout.BeginHorizontal();
        _replaceSourcePath = EditorGUILayout.TextField(_replaceSourcePath, GUILayout.Width(300));
        if (GUILayout.Button("浏览", GUILayout.Width(80)))
        {
            string p = EditorUtility.OpenFolderPanel("选择文件夹", Directory.GetCurrentDirectory(), "");
            if (!string.IsNullOrEmpty(p))
            {
                _replaceSourcePath = p;
            }
        }
        EditorGUILayout.EndHorizontal();
        _isIncludeChildren = EditorGUILayout.Toggle("是否包含源文件夹的子目录", _isIncludeChildren);
        _isConfirmEachReplace = EditorGUILayout.Toggle("是否逐一确认替换", _isConfirmEachReplace);
        EditorGUILayout.LabelField("过滤类型，小写后缀，用分号隔开(如“.jpg;.png”)");
        _filterStringReplace = EditorGUILayout.TextField(_filterStringReplace, GUILayout.Width(300));
        if (GUILayout.Button("替换", GUILayout.Width(80)) && EditorUtil.DisplayNotify("替换后无法恢复"))
        {
            BeginReplace();
        }
    }

    void BeginReplace()
    {
        string[] filters = null;
        if (!string.IsNullOrEmpty(_filterStringReplace))
        {
            filters = _filterStringReplace.Split(';');
        }
        if (!Directory.Exists(_replaceSourcePath))
        {
            this.ShowNotification(new GUIContent("目录不存在"));
            return;
        }
        List<FileInfo> sourceInfos = EditorUtil.GetFiles(_replaceSourcePath, _isIncludeChildren, filters);
        List<FileInfo> targetInfos = GetSelectedPathesFiles(filters);
        int count = 0;
        foreach (FileInfo info in sourceInfos)
        {
            FileInfo find = targetInfos.Find(x => x.Name == info.Name);
            if (find!=null)
            {
                bool isReplace = true;
                if(_isConfirmEachReplace)
                {
                    isReplace = EditorUtil.DisplayNotify(string.Format("源文件：{0}，目标文件{1}，是否确认替换", info.FullName, find.FullName));
                }
                if (isReplace)
                {
                    info.CopyTo(find.FullName, true);
                    count++;
                }
            }
        }
        Debug.Log("替换了" + count + "个文件");
    }
}