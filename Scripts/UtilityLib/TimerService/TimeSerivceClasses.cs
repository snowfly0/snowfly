#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 深圳，瑞立夫
//
// 模块名：TimerServiceClasses
// 创建者：陈长源
// 修改者列表：
// 创建日期：2015.12.4
// 模块描述：时间服务内部类
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseTimer
{
    public float TimeScale = 1.0f;
    protected float delayTime;
    public bool IsPause { get; set; }
    Action m_action;
    public BaseTimer()
    {

    }
    public BaseTimer(Action action, float delayTime)
    {
        this.delayTime = delayTime;
        this.m_action = action;
    }

    public bool Update(float delta)
    {
        if (IsPause) return false;
        delayTime -= delta * TimeScale;
        if (delayTime <= 0)
        {
            Complete();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 销毁计时器
    /// </summary>
    /// <param name="completeOnKilled">是否在销毁后完成委托</param>
    public void Kill(bool completeOnKilled=false)
    {
        TimerService.RemoveTimer(this);
        if (completeOnKilled)
        {
            Complete();
        }
    }

    public virtual void Complete()
    {
        if (m_action != null)
        {
            m_action();
        }
    }
}


namespace TimerServiceInternal
{
    #region 时间管理类

    internal class BaseTimer<T> : BaseTimer
    {
        Action<T> m_action;
        T args0;
        public BaseTimer(Action<T> action, float delayTime,T args0)
        {
            this.delayTime = delayTime;
            this.m_action = action;
            this.args0 = args0;
        }



        public override void Complete()
        {
            if (m_action != null)
            {
                m_action(args0);
            }
        }
    }

    internal class BaseTimer<T1,T2> : BaseTimer
    {
        Action<T1,T2> m_action;
        T1 args0;
        T2 args1;
        public BaseTimer(Action<T1,T2> action, float delayTime, T1 args0,T2 args1)
        {
            this.delayTime = delayTime;
            this.m_action = action;
            this.args0 = args0;
            this.args1 = args1;
        }


        public override void Complete()
        {
            if (m_action != null)
            {
                m_action(args0, args1);
            }
        }
    }

    internal class BaseTimer<T1, T2,T3> : BaseTimer
    {
        Action<T1, T2,T3> m_action;
        T1 args0;
        T2 args1;
        T3 args2;
        public BaseTimer(Action<T1, T2,T3> action, float delayTime, T1 args0, T2 args1,T3 args2)
        {
            this.delayTime = delayTime;
            this.m_action = action;
            this.args0 = args0;
            this.args1 = args1;
            this.args2 = args2;
        }


        public override void Complete()
        {
            if (m_action != null)
            {
                m_action(args0, args1, args2);
            }
        }
    }


    internal class BaseTimer<T1, T2, T3,T4> : BaseTimer
    {
        Action<T1, T2, T3,T4> m_action;
        T1 args0;
        T2 args1;
        T3 args2;
        T4 args3;
        public BaseTimer(Action<T1, T2, T3,T4> action, float delayTime, T1 args0, T2 args1, T3 args2,T4 args3)
        {
            this.delayTime = delayTime;
            this.m_action = action;
            this.args0 = args0;
            this.args1 = args1;
            this.args2 = args2;
            this.args3 = args3;
        }


        public override void Complete()
        {
            if (m_action != null)
            {
                m_action(args0, args1, args2, args3);
            }
        }
    }
  #endregion



}
