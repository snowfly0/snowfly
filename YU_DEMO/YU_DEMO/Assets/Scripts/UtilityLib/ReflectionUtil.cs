using UnityEngine;
using System.Collections;
using System.Text;
using System.Reflection;
using System;
public class ReflectionUtil 
{
    public static string GetInfo(Type npc,object obj)
    {
        StringBuilder sb = new StringBuilder();
        var fieldInfos = npc.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (var info in fieldInfos)
        {
            try
            {
                object value=info.GetValue(obj);
                string valStr=value==null?"Null":value.ToString();
                sb.AppendLine(info.Name + ":" + valStr);
            }
            catch { }
        }
        //var propertyInfos = npc.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        //foreach (var info in propertyInfos)
        //{
        //    try
        //    {
        //        sb.AppendLine(info.Name + ":" + info.GetValue(npc, null).ToString());
        //    }
        //    catch { }
        //}
        return sb.ToString();
    }


    public static string GetPropertyInfo(Type npc, object obj)
    {
        StringBuilder sb = new StringBuilder();
        var propertyInfos = npc.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (var info in propertyInfos)
        {
            try
            {
                object value = info.GetValue(obj, null);
                string valStr = value == null ? "Null" : value.ToString();
                sb.AppendLine(info.Name + ":" + valStr);
            }
            catch { }
        }
        return sb.ToString();
    }
}
