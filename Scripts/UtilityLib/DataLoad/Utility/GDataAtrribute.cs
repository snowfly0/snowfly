using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class NoDataWindowAttribute : Attribute
{

}


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class GDataModuleAttribute : Attribute
{
    public string ModuleName;

    public static GDataModuleAttribute GetAttribute(Type type)
    {
        GDataModuleAttribute attribute = null;
        object[] objs = type.GetCustomAttributes(false);
        foreach (var o in objs)
        {
            attribute = o as GDataModuleAttribute;
            if (attribute != null)
            {
                return attribute;
            }
        }
        return null;
    }


}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class GDataDescriptionAttribute : Attribute
{
    public string BaseDescpt;
    public string DetailDescpt;

    public static GDataDescriptionAttribute GetAttribute(Type type)
    {
        GDataDescriptionAttribute attribute = null;
        object[] objs = type.GetCustomAttributes(false);
        foreach (var o in objs)
        {
            attribute = o as GDataDescriptionAttribute;
            if (attribute != null)
            {
                return attribute;
            }
        }
        return null;
    }

    public static GDataDescriptionAttribute GetAttribute(MemberInfo info)
    {
        return Attribute.GetCustomAttribute(info, typeof(GDataDescriptionAttribute)) as GDataDescriptionAttribute;
    }

    public static string GetNameOrBaseDescpt(Type t)
    {
        GDataDescriptionAttribute attribute = GetAttribute(t);
        if(attribute!=null)
        {
            return attribute.BaseDescpt;
        }
        return t.Name;
    }

    public static string GetNameOrBaseDescpt(MemberInfo info)
    {
        GDataDescriptionAttribute attribute = GetAttribute(info);
        if (attribute != null)
        {
            return attribute.BaseDescpt;
        }
        return info.Name;
    }

    public static string GetDetailDescpt(Type t)
    {
        GDataDescriptionAttribute attribute = GetAttribute(t);
        if (attribute != null)
        {
            return attribute.DetailDescpt;
        }
        return null;
    }


    public static string GetDetailDescpt(MemberInfo t)
    {
        GDataDescriptionAttribute attribute = GetAttribute(t);
        if (attribute != null)
        {
            return attribute.DetailDescpt;
        }
        return null;
    }
}

