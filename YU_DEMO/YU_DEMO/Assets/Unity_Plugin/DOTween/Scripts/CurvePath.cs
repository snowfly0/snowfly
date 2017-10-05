using UnityEngine;
using System.Collections;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CurvePath : MonoBehaviour
{
    public PathType PathType = PathType.CatmullRom;
    public Ease EaseType = Ease.Linear;
    public float ElaspedTime = 5.0f;
    public int DetailLevel = 5;
    public int LoopTimes = -1;
    public bool IsClosedPath;
    public LoopType LoopType;
    public Transform DemoObj;

    public AxisConstraint LockPosition = AxisConstraint.None;

    public AxisConstraint LockRotation = AxisConstraint.None;
    public bool IsEditMode;

    Tweener kTweener;
    void Start()
    {

    }

    protected virtual void OnEnable()
    {
        if (Application.isPlaying && DemoObj && transform.childCount > 2)
        {
            if (kTweener != null)
            {
                kTweener.Kill();
            }
            kTweener = FollowPath(DemoObj, false);
        }
    }

    public Vector3 GetStartPoint()
    {
        return transform.GetChild(0).position;
    }

    /// <summary>
    /// 物体移动到路径起点
    /// </summary>
    /// <param name="target">目标物体</param>
    /// <param name="time">消耗时间</param>
    /// <param name="call">完成时回调</param>
    /// <returns></returns>
    public Tweener MoveToPath(Transform target, float time, TweenCallback call = null)
    {
        Vector3[] points = new Vector3[2];
        points[0] = target.position;
        points[1] = transform.GetChild(0).position;
        var ret = target.DOPath(points, time, PathType, PathMode.Full3D, DetailLevel, GetLineColor())
      .SetOptions(false, LockPosition, LockRotation);//SetLookAt(0.1f);
        ret.SetEase(EaseType);
        target.DOLookAt(points[1], 1.0f);
        if (call != null)
        {
            ret.OnComplete(call);
        }
        return ret;
    }

    /// <summary>
    /// 物体跟随路径
    /// </summary>
    /// <param name="target">目标物体</param>
    /// <param name="isRelative">是否为相对路径</param>
    /// <returns></returns>
    public Tweener FollowPath(Transform target, bool isRelative = true, bool setlookAt = true)
    {
        Vector3[] points = null;
        if (isRelative)
        {
            Vector3 offset = target.position - transform.GetChild(0).position;
            points = GetRelativePoints(transform, offset);
        }
        else
        {
            target.position = transform.GetChild(0).position;
            points = GetPoints(transform);
        }
        var ret = target.DOPath(points, ElaspedTime, PathType, PathMode.Full3D, DetailLevel, GetLineColor())
      .SetOptions(IsClosedPath, LockPosition, LockRotation);
        if (setlookAt)
        {
            ret.SetLookAt(0.001f);
        }
        //target.DORotate(points[0], Mathf.Clamp(ElaspedTime, ElaspedTime, 0.5f));
        ret.SetEase(EaseType).SetLoops(LoopTimes, LoopType);
        return ret;
    }

    // Update is called once per frame
    void Update()
    {

    }
#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if (Application.isPlaying || transform.childCount < 2) return;

        var ret = transform.GetChild(0).DOPath(GetPoints(transform), 1.0f, PathType, PathMode.Full3D, DetailLevel, GetLineColor())
       .SetOptions(IsClosedPath)
       .SetLookAt(0.001f);

        ret.SetEase(EaseType).SetLoops(0, LoopType);

        DG.Tweening.Plugins.PathPlugin.Get().SetChangeValue(ret);
        ret.endValue.Draw();
        ret.Kill(false);
        if (!IsEditMode) return;
        Vector3[] points = GetPoints(transform);
        Handles.color = Color.white;
        Handles.ArrowCap(0, points[0], Quaternion.LookRotation(points[1] - points[0]), 5.0f);
    }



 

    bool IsSelectThisPath()
    {
        return (Selection.gameObjects.Length == 1 && Selection.gameObjects[0].transform == transform);
    }
#endif
    Color GetLineColor()
    {
        if (IsEditMode)
        {
            return Color.red;
        }
        return Color.white;
    }
    //    Vector3[] GetPoints(Transform[] tranforms)
    //    {
    //        Vector3[] points = new Vector3[tranforms.Length];
    //        int index = 0;
    //        foreach (var t in tranforms)
    //        {
    //            if (t)
    //            {
    //                points[index] = t.position;
    //            }
    //            index++;
    //        }
    //        return points;
    //    }

    Vector3[] GetPoints(Transform parent)
    {
        Vector3[] points = new Vector3[parent.childCount];
        for (int i = 0; i < points.Length; ++i)
        {
            points[i] = parent.GetChild(i).position;
        }
        return points;
    }

    Vector3[] GetRelativePoints(Transform parent, Vector3 offset)
    {
        Vector3[] points = new Vector3[parent.childCount];
        for (int i = 0; i < points.Length; ++i)
        {
            points[i] = parent.GetChild(i).position + offset;
        }
        return points;
    }
}

