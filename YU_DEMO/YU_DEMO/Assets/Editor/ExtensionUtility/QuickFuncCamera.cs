using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

public class QuickFuncCamera : ScriptableObject
{
    [MenuItem("Quick/显示场景中所有摄像机层级")]
    public static void DoIt()
    {
        Camera[] cams = new Camera[30];
        int count= Camera.GetAllCameras(cams);
        if (count == 0)
        {
            SceneView.focusedWindow.ShowNotification(new GUIContent("当前场景没摄像机"));
        }
        StringBuilder sb = new StringBuilder();
        List<Camera> camList = new List<Camera>();
        foreach (Camera c in cams)
        {
            if (c == null) continue;
            bool isInsert=false;
            for (int i = 0; i < camList.Count;++i )
            {
                if(camList[i].depth<c.depth)
                {
                    isInsert = true;
                    camList.Insert(i, c);
                    break;
                }
            }
            if (!isInsert)
            {
                camList.Add(c);
            }
        }
        for (int i = 0; i < camList.Count; i++)
        {
            Camera c=camList[i];
            sb.AppendLine(c.name + "--Depth:" + c.depth);
        }
        Debug.Log("场景所有摄像机：" + sb.ToString());
    }
}

