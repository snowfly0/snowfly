using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataWindowBase:EditorWindow
{

}

public class DataWindowBase<T> : DataWindowBase 
    where T : BaseData<T>
{


    protected virtual void OnDestroy()
    {
        EditorApplication.ExecuteMenuItem("Window/Hierarchy");//清除页面缓存
    }
}

