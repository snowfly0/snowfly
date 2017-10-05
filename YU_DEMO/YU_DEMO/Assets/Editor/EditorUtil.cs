using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System;
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

    public static Transform GetChildByDeep(int deep, Transform parent)
    {
        if (deep == 0)
        {
            return parent;
        }
        Transform result = parent;
        while (deep-- > 0)
        {
            if (result != null)
            {
                result = result.GetChild(0);
            }
            else
            {
                result = null;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// 使用默认相对量设置父子关系
    /// </summary>
    /// <param name="child"></param>
    /// <param name="parent"></param>
    /// <param name="setLayerOfParent"></param>
    public static void SetParent(GameObject child, GameObject parent, bool setLayerOfParent = false)
    {
        child.transform.SetParent(parent.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localEulerAngles = Vector3.zero;
        if (setLayerOfParent)
        {
            child.layer = parent.layer;
        }
    }

    public static void SetParent(Transform child, Transform parent, bool setLayerOfParent = false)
    {
        child.SetParent(parent);
        child.localPosition = Vector3.zero;
        child.localEulerAngles = Vector3.zero;
        if (setLayerOfParent)
        {
            child.gameObject.layer = parent.gameObject.layer;
        }
    }


    /// <summary>
    /// 根据路径获取Transform组件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Transform GetComponent(string path)
    {
        Transform comp = null;
        string[] Nodes = path.Split('/');
        try
        {
            GameObject go = GameObject.Find(Nodes[0]);
            Transform t = go.transform;
            for (int i = 1; i < Nodes.Length; i++)
            {
                t = t.FindChild(Nodes[i]);
            }
            comp = t;
        }
        catch
        {
            Debug.LogError(path + "can't find");
        }
        return comp;
    }

    /// <summary>
    /// 获取目标物体的路径
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static string GetGameObjectPath(GameObject go)
    {
        string name = go.name;
        Transform t = go.transform.parent;
        while (t != null)
        {
            name = name.Insert(0, t.name + "/");
            t = t.parent;
        }
        return name;
    }

    /// <summary>
    /// 使用字符串分割字符串
    /// </summary>
    /// <param name="sourceString"></param>
    /// <param name="splitString"></param>
    /// <returns></returns>
    public static string[] SplitWithString(string sourceString, string splitString)
    {
        List<string> arrayList = new List<string>();
        string s = string.Empty;
        while (sourceString.IndexOf(splitString) > -1)
        {
            s = sourceString.Substring(0, sourceString.IndexOf(splitString));
            sourceString = sourceString.Substring(sourceString.IndexOf(splitString) + splitString.Length);
            arrayList.Add(s);
        }
        arrayList.Add(sourceString);
        return arrayList.ToArray();
    }


    /// <summary>
    /// 获取当前编辑的场景名
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentEditSceneName()
    {
        string[] names = EditorApplication.currentScene.Split('/');
        string fullName = names[names.Length - 1];
        return fullName.Replace(".unity", "");
    }


    /// <summary>
    /// 检测根物体下的每个子物体，具有TCHECK组件的物体是否有TADD类型组件，如果没有则添加TADD组件
    /// </summary>
    /// <typeparam name="TCHECK">检测类型</typeparam>
    /// <typeparam name="TADD">附加类型</typeparam>
    /// <param name="parent">检测的根物体</param>
    /// <param name="includeInactiveObjs">是否包含未激活的子物体</param>
    public static List<TADD> AttachComponentInChildren<TCHECK, TADD>(GameObject parent, bool includeInactiveObjs)
        where TCHECK : Component
        where TADD : Component
    {
        TCHECK[] checks = parent.GetComponentsInChildren<TCHECK>(includeInactiveObjs);
        List<TADD> adds = new List<TADD>();
        foreach (var c in checks)
        {
            if (!c.gameObject.GetComponent<TADD>())
            {
                TADD ad = c.gameObject.AddComponent<TADD>();
                adds.Add(ad);
            }
        }
        return adds;
    }


    public static List<T> GetComponentsInChildrenOfSelectdOBJ<T>(bool includeInactiveObjs = true)
    {
        GameObject[] gos = Selection.gameObjects;
        List<T> rets = new List<T>();
        foreach (var go in gos)
        {
            rets.AddRange(go.GetComponentsInChildren<T>(includeInactiveObjs));
        }
        return rets;

    }

    public static XmlDocument CreateOrOverrideXmlFile(string path)
    {
        XmlDocument xmldoc = new XmlDocument();    //新建XML文件
        xmldoc.AppendChild(xmldoc.CreateXmlDeclaration("1.0", "utf-8", null));
        XmlElement root = xmldoc.CreateElement("Root");
        xmldoc.AppendChild(root);
        xmldoc.Save(path);
        return xmldoc;
    }

    /// <summary>
    /// 获取在物体的相对于父物体的下一个节点的物体，如果没有则返回null
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static GameObject GetNextNodeOfGameObject(GameObject go)
    {
        Transform parent = go.transform.parent;
        if (parent == null)
        {
            return null;
        }
        int childCount = parent.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            if (parent.GetChild(i).gameObject != go) continue;
            else if (i < childCount - 1)
            {
                return parent.GetChild(i + 1).gameObject;
            }
        }
        return null;
    }


    /// <summary>
    /// 获取目录下的文件
    /// </summary>
    /// <param name="path">目录</param>
    /// <param name="includeChildren">是否包含子文件夹</param>
    /// <param name="lowerTypeName">指定类型后缀</param>
    /// <returns></returns>
    public static List<FileInfo> GetFiles(string path, bool includeChildren = false,string[] lowerTypeName=null)
    {
        List<FileInfo> retInfos = new List<FileInfo>();
        if (!Directory.Exists(path)) return retInfos;
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        FileInfo[] infos = dirInfo.GetFiles();
        foreach (var info in infos)
        {
            if(lowerTypeName==null||lowerTypeName.Length==0)
            {
                retInfos.Add(info);
                continue;
            }
            foreach (var endStr in lowerTypeName)
            {
                if (info.Name.ToLower().EndsWith(endStr))
                {
                    retInfos.Add(info);
                    break;
                }
            }
        }
        if (includeChildren)
        {
            foreach (var dir in dirInfo.GetDirectories())
            {
                retInfos.AddRange(GetFiles(dir.FullName, true, lowerTypeName));
            }
        }
        return retInfos;
    }

    /// <summary>
    /// 获取选择的所有文件夹的完全路径
    /// </summary>
    /// <returns></returns>
    public static string[] GetSelectedFoldersPathes(bool getFull = true)
    {
        List<string> pathes = new List<string>();
        foreach (string guid in Selection.assetGUIDs)
        {
            string relativePath = AssetDatabase.GUIDToAssetPath(guid);
            if (!getFull)
            {
                pathes.Add(relativePath);
                continue;
            }
            string fullpath = Directory.GetCurrentDirectory() + "/" + relativePath;
            if (!File.Exists(fullpath))//如果不是文件，就是文件夹
            {
                pathes.Add(fullpath);
            }
        }
        return pathes.ToArray();
    }


    /// <summary>
    /// 在文件夹的所有子路径中查找所有某个类型的资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">路径</param>
    /// <param name="endString">要查找的文件后缀名(小写)</param>
    /// <returns></returns>
    public static T[] LoadAllAssetsInFolderChildren<T>(string path, string endString) where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        LoadAsset<T>(dirInfo, assets, endString);
        return assets.ToArray();
    }

    static void LoadAsset<T>(DirectoryInfo dirInfo, List<T> assets, string endString) where T : UnityEngine.Object
    {
        foreach (var file in dirInfo.GetFiles())
        {
            string path = file.FullName.Replace(Directory.GetCurrentDirectory() + "\\", "");
            UnityEngine.Object[] assetArray = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var asset in assetArray)
            {
                T tAsset = asset as T;
                if (tAsset && !tAsset.name.StartsWith("__preview"))
                {
                    assets.Add(tAsset);
                }
            }
        }
        foreach (var dir in dirInfo.GetDirectories())
        {
            LoadAsset<T>(dir, assets, endString);
        }
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