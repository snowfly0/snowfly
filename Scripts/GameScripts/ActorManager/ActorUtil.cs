using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Text;
public static class ActorUtil
{
    static System.Diagnostics.Stopwatch ms_sw = new System.Diagnostics.Stopwatch();
    
    public static void BeginSW()
    {
        ms_sw.Reset();
        ms_sw.Start();
    }

    public static string EndSW()
    {
        ms_sw.Stop();
        return ms_sw.Elapsed.ToString();
        //NpcUtil.BeginSW();
        //string message=NpcUtil.EndSW();
        //Debug.Log(NpcTypeName + isOn+message);
    }

    public static string GetNpcInfo(ActorEntity npc)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("NpcDebugInfo:");
        var fieldInfos= npc.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach(var info in fieldInfos)
        {
            try
            {
                sb.AppendLine(info.Name + ":" + info.GetValue(npc).ToString());
            }
            catch { }
        }
        var propertyInfos = npc.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (var info in propertyInfos)
        {
            try
            {
                sb.AppendLine(info.Name + ":" + info.GetValue(npc, null).ToString());
            }
            catch { }
        }
        return sb.ToString();
    }

    public static void DrawParabola(float a,float b,float c)
    {
#if UNITY_EDITOR

#endif
    }


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
    /// 在所有下层节点中查找符合前缀名的物体
    /// </summary>
    /// <param name="parent">父物体</param>
    /// <param name="startStr">前缀名</param>
    /// <param name="results">返回结果</param>
    /// <param name="includeInactive">是否包含不激活的物体</param>
    public static void FindChildrenNameStartWith(Transform parent, string startStr,List<Transform> results,bool includeInactive=false)
    {
        if (results == null)
        {
            results = new List<Transform>();
        }
        for(int i=0;i<parent.childCount;++i)
        {
            Transform child = parent.GetChild(i);
            if (!child.gameObject.activeSelf&&!includeInactive) continue;
            if(child.name.StartsWith(startStr))
            {
                results.Add(child);
            }
            FindChildrenNameStartWith(child, startStr, results,includeInactive);
        }
    }

    /// <summary>
    /// 在子物体中查找第一个符合名称的组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="parent">父物体</param>
    /// <param name="nameStr">名称</param>
    /// <param name="includeInactive">是否包含不激活的物体</param>
    /// <returns></returns>
    public static T FindChildName<T>(Transform parent, string nameStr, bool includeInactive = false)where T:UnityEngine.Object
    {
        T result = null;
        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform child = parent.GetChild(i);
            if (!child.gameObject.activeSelf&&!includeInactive) continue;
            if (child.name==nameStr)
            {
                result = child.GetComponent<T>();
            }
            if(!result)
            {
                result = FindChildName<T>(child, nameStr, includeInactive);
            }
            if(result)
            {
                return result;
            }
        }
        return null;
    }

    /// <summary>
    /// 在子物体中查找T类型的组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="parent">父物体</param>
    /// <param name="results">查找结果</param>
    /// <param name="deep">查找树深度（-1为不限制深度，0为只查找下一层的子物体，0以上为查到多层物体）</param>
    /// <param name="includeInactive">是否包含自身不处于激活状态的物体</param>
    public static void FindComponent<T>(Transform parent, List<T> results, int deep = -1, bool includeInactive = false) 
    {
        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform child = parent.GetChild(i);
            if (!child.gameObject.activeSelf && !includeInactive) continue;
            T t=child.GetComponent<T>();
            if (t!=null)
            {
                results.Add(t);
            }
            if(deep>0)
            {
                FindComponent<T>(child, results, deep-1,includeInactive);
            }
            else if(deep<0)
            {
                FindComponent<T>(child, results, deep, includeInactive);
            }
        }
    }

    /// <summary>
    /// 在所有子物体中查找第一个T类型的组件,不按树遍历方式查找，按层次查找，适用于查层次比较浅的物体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parent"></param>
    /// <param name="includeParent"></param>
    /// <returns></returns>
    public static T FindComponentInChildren<T>(Transform parent,bool includeParent=false)where T:UnityEngine.Object
    {
        T result = null;
        if(includeParent)
        {
            result = parent.GetComponent<T>();
            if (result != null)
            {
                return result;
            }
        }
        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform child = parent.GetChild(i);
             result= child.GetComponent<T>();
            if (result!=null) return result;
        }
        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform child = parent.GetChild(i);
            result = FindComponentInChildren<T>(child,false);
            if (result != null) return result;
        }
        return result;
    }
}

