using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
[CustomEditor(typeof(CurvePath))]
public class CurvePathEditor : Editor
{
    CurvePath _path;
    void OnEnable()
    {
        _path = target as CurvePath;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("ReversePath"))
        {
            ReversePath();
        }
        if (GUILayout.Button("StartPointGoHead"))
        {
            StartPointGoHead();
        }
    }

    void OnSceneGUI()
    {
        ShowHandles();
    }

    public void ShowHandles()
    {
        if (_path.transform.childCount < 1) return;
        if (_path.IsEditMode)
        {
            foreach (Transform t in _path.transform)
            {
                Undo.RecordObject(t, "");
                t.position = Handles.PositionHandle(t.position, t.rotation);
                //t.rotation = Handles.RotationHandle(t.rotation, t.position);//暂不需要旋转量
            }
        }
    }

    void ReversePath()
    {
        Transform transf=_path.transform;
        int childCount=transf.childCount;
        if (childCount < 3) return;
        List<Transform> childs=new List<Transform>();
        for (int i = 1; i < childCount; ++i)
        {
            Transform child = transf.GetChild(1);
            child.SetParent(null);
            childs.Add(child);
        }
        for(int i=childs.Count-1;i>=0;--i)
        {
            childs[i].SetParent(transf);
        }
    }

    void StartPointGoHead()
    {
        Transform transf = _path.transform;
        int childCount = transf.childCount;
        if (childCount < 2) return;
        transf.GetChild(0).SetAsLastSibling();
    }
}