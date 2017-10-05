#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 深圳，瑞立夫
//
// 模块名：TimerService
// 创建者：陈长源
// 修改者列表：
// 创建日期：2015.12.4
// 模块描述：时间事件托管服务类
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TimerServiceInternal;

public class TimerService:XRUnitySingleton<TimerService>
{

    #region 时间管理服务
    static  List<BaseTimer> m_CNTs = new List<BaseTimer>();
    static List<BaseTimer> m_RealTimeCNTs = new List<BaseTimer>();
    static float m_fRealTime;
    static readonly object m_Locker=new object();

    static Action m_UpdateDelegates;

    static Action m_OtherThreadSignal;



    static public void AddOtherThreadSignal(Action call)
    {
        lock(m_Locker)
        {
            m_OtherThreadSignal += call;
        }
    }



    /// <summary>
    /// 以时间管理器为委托，延迟一定时间后触发委托的事件(无法中断)
    /// </summary>
    /// <param name="delayTime">延迟事件</param>
    /// <param name="callback">委托</param>
    static public BaseTimer AddTimer(float delayTime, Action callback,bool isRealTime=false)
    {
        BaseTimer timer = new BaseTimer(callback, delayTime);
        if(isRealTime)
        {
            m_RealTimeCNTs.Insert(0, timer);
        }
        else
        m_CNTs.Insert(0, timer);
        return timer;
    }

    static public BaseTimer AddTimer<T>(float delayTime, Action<T> callback, T arg, bool isRealTime = false)
    {
        BaseTimer timer = new BaseTimer<T>(callback, delayTime, arg);
        if (isRealTime)
        {
            m_RealTimeCNTs.Insert(0, timer);
        }
        else
        m_CNTs.Insert(0, timer);
        return timer;
    }

    static public BaseTimer AddTimer<T1, T2>(float delayTime, Action<T1, T2> callback, T1 arg0, T2 arg1, bool isRealTime = false)
    {
        BaseTimer timer = new BaseTimer<T1, T2>(callback, delayTime, arg0, arg1);
        if (isRealTime)
        {
            m_RealTimeCNTs.Insert(0, timer);
        }
        else
        m_CNTs.Insert(0, timer);
        return timer;
    }

    static public BaseTimer AddTimer<T1, T2, T3>(float delayTime, Action<T1, T2, T3> callback, T1 arg0, T2 arg1, T3 arg2, bool isRealTime = false)
    {
        BaseTimer timer = new BaseTimer<T1, T2, T3>(callback, delayTime, arg0, arg1, arg2);
        if (isRealTime)
        {
            m_RealTimeCNTs.Insert(0, timer);
        }
        else
        m_CNTs.Insert(0, timer);
        return timer;
    }

    static public BaseTimer AddTimer<T1, T2, T3, T4>(float delayTime, Action<T1, T2, T3, T4> callback, T1 arg0, T2 arg1, T3 arg2, T4 arg3, bool isRealTime = false)
    {
        BaseTimer timer = new BaseTimer<T1, T2, T3, T4>(callback, delayTime, arg0, arg1, arg2, arg3);
        if (isRealTime)
        {
            m_RealTimeCNTs.Insert(0, timer);
        }
        else
        m_CNTs.Insert(0,timer);
        return timer;
    }

    static public void AddUpdateDelegate(Action action)
    {
        m_UpdateDelegates += action;
    }

    static public void RemoveTimer(BaseTimer timer)
    {
        if(!m_CNTs.Remove(timer))
        {
            m_RealTimeCNTs.Remove(timer);
        }
    }
    #endregion


    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        if (CheckInit()) return;
        GameObject timerServerObj = new GameObject("[TimerService]");
        timerServerObj.AddComponent<TimerService>();
    }


    static bool CheckInit()
    {
        TimerService[] finds = GameObject.FindObjectsOfType<TimerService>();
        if (finds.Length > 1)
        {
            UnityEngine.Debug.LogError("More than one TimerService instance,remove others");
            for (int i = 1; i < finds.Length; ++i)
            {
                GameObject.DestroyImmediate(finds[i]);
            }
            return true;
        }
        else if (finds.Length == 1) return true;
        return false;
    }

    public  override void Awake()
    {
        base.Awake();
        CheckInit();
    }


    void Start()
    {

    }


    void Update()
    {
        lock(m_Locker)
        {
            if(m_OtherThreadSignal!=null)
            {
                m_OtherThreadSignal();
                m_OtherThreadSignal = null;
            }
        }
        if (m_UpdateDelegates != null)
        {
            m_UpdateDelegates();
        }
        float deltaTime = Time.deltaTime;
        for (int i = m_CNTs.Count - 1; i >= 0; i--)
        {
            var CNT = m_CNTs[i];
            if (CNT.Update(deltaTime))
            {
                m_CNTs.Remove(CNT);
            }
        }
        float readDelta = Time.realtimeSinceStartup - m_fRealTime;
        m_fRealTime = Time.realtimeSinceStartup;
        for (int i = m_RealTimeCNTs.Count - 1; i >= 0; i--)
        {
            var CNT = m_RealTimeCNTs[i];
            if (CNT.Update(readDelta))
            {
                m_RealTimeCNTs.Remove(CNT);
            }
        }
        AddSpeed();
    }








    //用于加速测试
    //public bool IsAddSpeedExport;
    public static bool IsAddSpeedRestrict { get; set; }
    void AddSpeed()
    {
//#if !UNITY_EDITOR
//                if (!IsAddSpeedExport) return;
//#endif
        if (Input.GetKey(KeyCode.RightArrow) && !IsAddSpeedRestrict)
        {
            Time.timeScale = 15f;
        }
        else if (!IsAddSpeedRestrict)
        {
            Time.timeScale = 1;
        }
    }






}


