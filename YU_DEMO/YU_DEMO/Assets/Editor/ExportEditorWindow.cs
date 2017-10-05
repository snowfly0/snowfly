using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class ExportDirManageWindow : EditorWindow
{
    [MenuItem("Tools/ExportPackage")]
    static void DoIt()
    {
        var window = ExportDirManageWindow.GetWindow<ExportDirManageWindow>();
    }
    List<XRDirInfo> m_dirInfos;
    int m_iSelectIndex;
    string m_sCreateDirName;
    Vector2 m_vScrollPos= new Vector2(20, 150);
    void OnEnable()
    {
        XRDirInfo.LoadDataMap();
        Debug.Log("开启目录管理");
    }

    void OnGUI()
    {
        GUILayout.Label("目录", EditorStyles.boldLabel);
        if (XRDirInfo.Keys == null || XRDirInfo.Keys.Length == 0)
        {
            GUILayout.Label("当前没有目录");
            m_dirInfos = null;
        }
        else
        {
            m_iSelectIndex = EditorGUILayout.Popup(m_iSelectIndex, XRDirInfo.Keys);
            m_dirInfos = XRDirInfo.GetDirInfos(XRDirInfo.Keys[m_iSelectIndex]);
        }
        m_sCreateDirName = EditorGUILayout.TextField("输入创建目录名", m_sCreateDirName, GUILayout.MinWidth(100));
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("创建目录"))
        {
            CreateDir(false);
        }
        if (GUILayout.Button("复制当前目录"))
        {
            CreateDir(true);
        }
        if (GUILayout.Button("移除当前目录"))
        {
            RemoveDir();
        }
        if (GUILayout.Button("保存所有"))
        {
            XRDirInfo.Save();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("目录管理", EditorStyles.boldLabel);
        m_vScrollPos= EditorGUILayout.BeginScrollView(m_vScrollPos);
        if (m_dirInfos != null)
        {
            for (int i = 0; i < m_dirInfos.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                m_dirInfos[i].IsSelected = EditorGUILayout.Toggle(m_dirInfos[i].IsSelected,GUILayout.MaxWidth(25));
                EditorGUILayout.LabelField(m_dirInfos[i].Path);
                if (GUILayout.Button("X",GUILayout.MaxWidth(25)))
                {
                    m_dirInfos.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("添加选定目录", GUILayout.MaxWidth(100)))
        {
            AddDirs();
        }
        if (GUILayout.Button("导出当前目录", GUILayout.MaxWidth(100)))
        {
            TempExport();
        }
        EditorGUILayout.EndHorizontal();
    }

    void CreateDir(bool isCopy)
    {
        if (string.IsNullOrEmpty(m_sCreateDirName))
        {
            this.ShowNotification(new GUIContent("目录名不能为空"));
            return;
        }
        if (XRDirInfo.AddDir(m_sCreateDirName,isCopy?m_dirInfos:null))
        {
            this.ShowNotification(new GUIContent("创建成功"));
            XRDirInfo.Save();
        }
        else
        {
            this.ShowNotification(new GUIContent("创建失败"));
        }
    }

    void RemoveDir()
    {
        if (XRDirInfo.Keys == null || XRDirInfo.Keys.Length == 0)
        {
            return;
        }
        if (m_iSelectIndex < XRDirInfo.Keys.Length)
        {
            XRDirInfo.RemoveDir(XRDirInfo.Keys[m_iSelectIndex]);
            m_iSelectIndex = 0;
        }
    }

    void AddDirs()
    {
        if (m_dirInfos == null) return;
        string[] pathes = EditorUtil.GetSelectedFoldersPathes(false);
        foreach (string str in pathes)
        {
            bool isAdd = false;
            for (int i = 0; i < m_dirInfos.Count; ++i)
            {
                if (m_dirInfos[i].Path == str)
                {
                    isAdd = true;
                    break;
                }
            }
            if(isAdd)
            {
                continue;
            }
            XRDirInfo info = new XRDirInfo { Path = str };
            m_dirInfos.Add(info);
        }
    }


    void TempExport()
    {
        if(m_dirInfos==null||m_dirInfos.Count==0)
        {
            this.ShowNotification(new GUIContent("目录为空"));
            return;
        }
        List<string> exportDirs = new List<string>();
        for(int i=0;i<m_dirInfos.Count;++i)
        {
            if (m_dirInfos[i].IsSelected)
            {
                exportDirs.Add(m_dirInfos[i].Path);
            }
        }
        string[] projectDirArray = Directory.GetCurrentDirectory().Split('/');
        string exportFileName=projectDirArray[projectDirArray.Length-1]+DateTime.Now.ToString("yy-MM-dd")+XRDirInfo.Keys[m_iSelectIndex];
        AssetDatabase.ExportPackage(exportDirs.ToArray(), exportFileName+".unityPackage",ExportPackageOptions.Default|ExportPackageOptions.Recurse|ExportPackageOptions.Interactive);
    }

    void OnDisable()
    {
        XRDirInfo.Save();
        XRDirInfo.UnLoadDataMap();
        Debug.Log("关闭目录管理");
    }
}
[Serializable]
public class XRDirInfo
{
    static string relativepath = "/ExportInfo.xr";
    public static string[] Keys;
    public bool IsSelected=true;
    public static bool ContaisDir(string keyName)
    {
        if (m_dirDataMap == null)
        {
            LoadDataMap();
        }
        return m_dirDataMap.ContainsKey(keyName);
    }

    public static bool AddDir(string keyName,List<XRDirInfo> listInfo=null)
    {
        if (!ContaisDir(keyName))
        {
            List<XRDirInfo> infos = new List<XRDirInfo>();
            if (listInfo != null)
            {              
                foreach(var info in listInfo)
                {
                    infos.Add(new XRDirInfo {Path=info.Path });
                }
            }
            m_dirDataMap.Add(keyName, infos);
            Keys = new List<string>(m_dirDataMap.Keys).ToArray();
            return true;
        }
        return false;
    }

    public static bool RemoveDir(string keyName)
    {
        if (ContaisDir(keyName))
        {
            m_dirDataMap.Remove(keyName);
            Keys = new List<string>(m_dirDataMap.Keys).ToArray();
            return true;
        }
        return false;
    }

    public static List<XRDirInfo> GetDirInfos(string keyName)
    {
        if (m_dirDataMap == null)
        {
            LoadDataMap();
        }
        List<XRDirInfo> infos;
        if (!m_dirDataMap.TryGetValue(keyName, out infos))
        {
            infos = new List<XRDirInfo>();
        }
        return infos;
    }
    private static Dictionary<string, List<XRDirInfo>> m_dirDataMap;
    public string Path;



    public static void LoadDataMap()
    {
        m_dirDataMap = new Dictionary<string, List<XRDirInfo>>();
        string dirpath = Directory.GetCurrentDirectory();
        string[] splits = dirpath.Split('/');
        string filepath = dirpath.Replace(splits[splits.Length - 1], "") + relativepath;
        bool isExistFile = File.Exists(filepath);
        if (!isExistFile) return;
        FileStream fs = null;
        try
        {
            fs = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            m_dirDataMap = bf.Deserialize(fs) as Dictionary<string, List<XRDirInfo>>;
            Debug.Log("加载信息成功");
        }
        catch (Exception ex)
        {
            Debug.LogError("加载信息失败" + ex);
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
            }
        }
        Keys = new List<string>(m_dirDataMap.Keys).ToArray();
    }

    public static void Save()
    {
        string dirpath = Directory.GetCurrentDirectory();
        string[] splits = dirpath.Split('/');
        string filepath = dirpath.Replace(splits[splits.Length - 1], "") + relativepath;
        FileStream fs = null;
        try
        {
            fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, m_dirDataMap);
            Debug.Log("保存信息成功");
        }
        catch (Exception ex)
        {
            Debug.LogError("保存信息失败" + ex);
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
            }
        }
    }

    public static void UnLoadDataMap()
    {
        m_dirDataMap = null;
        Keys = null;
    }
}
