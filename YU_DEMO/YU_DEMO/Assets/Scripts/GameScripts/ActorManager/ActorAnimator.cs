using System;
using System.Collections.Generic;
using UnityEngine;



public class ActorAnimator
{
    enum ActionLayer
    {
        None = 0,
        Base = 1,
        ExpandLayer1 = 2,
        ExpandLayer2 = 4,
        ExpandLayer3 = 8,
        ExpandLayers = 14,
    }

    Animator m_animator;
    public AnimatorStateInfo m_stateInfo;
    ActionLayer m_runingLayer;
    bool m_bIsBaseStart;
    bool[] m_IsMutiStart;
    string[] m_stateNames;
    int m_nCount;

    public float Speed { get { return m_animator.speed; } set { m_animator.speed = value; } }

    /// <summary>
    /// 获取基本层动画播放的进度
    /// </summary>
    public float NormalizeTime { get; protected set; }
    /// <summary>
    /// 委托：动画结束时触发
    /// </summary>
    public Action OnActionEnd;


    /// <summary>
    /// 委托：在其它层动画刚开始时触发
    /// </summary>
    public Action<int, string> OnMutiLayerActionStart;
    /// <summary>
    /// 委托：获取其他层动画播放的进度
    /// </summary>
    public Action<int, float> OnMutiLayerProcess;

    public string CurrentStateName { get { return m_stateNames[0]; } }


    Action m_endCall;
    bool m_isLoop;
    public ActorAnimator(Animator animator)
    {
        if (animator == null)
        {
            Debug.LogError("Parameter animator can't be null");
            throw new Exception("Parameter animator can't be null");
        }
        m_animator = animator;
        m_stateNames = new string[4];
        m_IsMutiStart = new bool[3];
    }

    /// <summary>
    /// 播放动作
    /// </summary>
    /// <param name="stateName">动作状态机名</param>
    /// <param name="layerIndex">动画层</param>
    /// <returns>是否播放成功</returns>
    public bool Play(string stateName, int layerIndex = 0)
    {
        if(!m_animator.enabled)
        {
            m_animator.enabled = true;
        }
        int stateShortHash = Animator.StringToHash(stateName);
        if (!m_animator.HasState(layerIndex, stateShortHash))
        {
            Debug.LogError("Miss State:" + stateName + " in " + m_animator.name);
            OnAnimaEnd();
            return false;
        }
        m_endCall = null;
        if (m_stateNames[layerIndex] != null)
        {
            m_animator.SetBool(m_stateNames[layerIndex], false);
        }
        m_animator.SetBool(stateName,true);
        if (layerIndex < 1)
        {
            m_runingLayer |= ActionLayer.Base;
            m_bIsBaseStart = false;
        }
        else if (layerIndex >= 1)
        {
            int layer = 1 << layerIndex;
            m_runingLayer |= (ActionLayer)layer;
            m_IsMutiStart[layerIndex - 1] = false;
        }
        NormalizeTime = 0f;
        m_nCount = 1;
        m_stateNames[layerIndex] = stateName;
        return true;
    }


    public bool Play(string stateName, Action endCall)
    {
        if (Play(stateName, 0))
        {
            m_endCall = endCall;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 设置循环，只能用于Play之后，使用此项不能监听结束事件
    /// </summary>
    /// <param name="isLoop"></param>
    public void SetLoop(bool isLoop)
    {
        m_isLoop = isLoop;
    }


    /// <summary>
    /// 更新Animator的信息
    /// </summary>
    public void UpdateAnimator()
    {
        if ((m_runingLayer & ActionLayer.Base) != ActionLayer.None)
        {
            _UpdateBaseLayer();
        }
        if ((m_runingLayer & ActionLayer.ExpandLayers) != ActionLayer.None)
        {
            _UpdateMutiLayers();
        }
    }

    void _UpdateBaseLayer()
    {
        m_stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        if (!m_bIsBaseStart)
        {
            if (--m_nCount < 0 && m_stateInfo.normalizedTime < 1)
            {
                if (m_stateInfo.IsName(m_stateNames[0]))
                {
                    //Logger<LNPC>.Debug("Animation begin,stateName:" + m_stateNames[0] + ",Time:" + m_stateInfo.normalizedTime);
                    m_bIsBaseStart = true;
                    NormalizeTime = m_stateInfo.normalizedTime;
                    m_animator.SetBool(m_stateNames[0], false);
                }
            }
        }
        else
        {
            NormalizeTime = m_stateInfo.normalizedTime;
            if (NormalizeTime >= 1.0f)
            {
                m_bIsBaseStart = false;
                if (m_isLoop)
                {
                    Play(m_stateNames[0]);
                }
                else
                {
                    OnAnimaEnd();
                }
            }
        }
    }


    void OnAnimaEnd()
    {
        if (OnActionEnd != null)
        {
            OnActionEnd();
        }
        if (m_endCall != null)
        {
            Action temp = null;
            temp = m_endCall;
            m_endCall = null;
            temp();
        }
    }

    void _UpdateMutiLayers()
    {
        for (int i = 1; i <= 3; ++i)
        {
            ActionLayer layer = (ActionLayer)(1 << i);
            if ((m_runingLayer & layer) != ActionLayer.None)
            {
                _UpdateMutiLayer(i);
            }
        }
    }

    void _UpdateMutiLayer(int layerIndex)
    {
        m_stateInfo = m_animator.GetCurrentAnimatorStateInfo(layerIndex);
        if (!m_IsMutiStart[layerIndex - 1])
        {
            if (m_stateInfo.IsName(m_stateNames[layerIndex]))
            {
                m_IsMutiStart[layerIndex - 1] = true;
                _NotifyMutiActionStart(layerIndex, m_stateNames[layerIndex]);
            }
        }
        else
        {
            float time = m_stateInfo.normalizedTime;
            _MutiProcessCall(layerIndex, time);
            if (time >= 1.0f)
            {
                m_IsMutiStart[layerIndex - 1] = false;
                if (m_stateInfo.IsName(m_stateNames[layerIndex]))
                {
                    ActionLayer layer = (ActionLayer)(1 << layerIndex);
                    m_runingLayer &= (~layer);
                }
            }
        }
    }



    void _NotifyMutiActionStart(int layerIndex, string stateName)
    {
        m_animator.SetBool(stateName, false);
        if (OnMutiLayerActionStart != null)
        {
            OnMutiLayerActionStart(layerIndex, stateName);
        }
    }


    void _MutiProcessCall(int layer, float process)
    {
        if (OnMutiLayerProcess != null)
        {
            OnMutiLayerProcess(layer, process);
        }
    }

    public Animator Animator
    {
        get
        {
            return m_animator;
        }
    }

    public void Reset()
    {
        m_runingLayer = ActionLayer.None;
        m_bIsBaseStart = false;
        m_isLoop = false;
        for (int i = 0; i < m_IsMutiStart.Length; ++i)
        {
            m_IsMutiStart[i] = false;
        }

        for (int i = 0; i < m_stateNames.Length; ++i)
        {
            if (m_stateNames[i] != null)
            {
                if (m_animator.isInitialized)
                {
                    m_animator.SetBool(m_stateNames[i], false);
                }
                m_stateNames[i] = null;
            }
        }
        m_animator.Stop();
        m_animator.enabled = false;
    }
}

