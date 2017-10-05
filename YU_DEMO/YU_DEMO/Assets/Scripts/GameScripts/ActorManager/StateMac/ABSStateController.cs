using System;
using System.Collections.Generic;

using System.Text;

/// <summary>
/// 状态机控制器抽象类
/// </summary>
public abstract class ABSStateController
{

    public ActorEntity Owner { get; protected set; }

    protected Dictionary<ActorState, ABSStateMac> _stateMacs = new Dictionary<ActorState, ABSStateMac>();

    protected ABSStateController()
    {

    }

    /// <summary>
    /// 根据角色类型创建状态机
    /// </summary>
    /// <param name="actorType"></param>
    /// <returns></returns>
    public static ABSStateController Create(ActorType actorType)
    {
        switch (actorType)
        {
            case ActorType.ScanFish:
                {
                    return new ScanFishController();
                }
            case ActorType.SceneFish:
                {
                    return new SceneFishController();
                }
        }
        return null;
    }
    //public ABSStateController(ActorEntity actor)
    //{

    //}

    /// <summary>
    /// 获取状态机接口
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public ABSStateMac GetStateMac(ActorState state)
    {
        ABSStateMac mac;
        _stateMacs.TryGetValue(state, out mac);
        return mac;
    }


    public virtual bool CanChangeState(ActorState state)
    {
        return true;
    }

    public void ChangeState(ActorState state)
    {
        ABSStateMac mac;
        if(_stateMacs.TryGetValue(state, out mac))
        {
            Owner.ChangeState(mac);
        }
        else
        {
            UnityEngine.Debug.LogError(string.Format("It is no {0} state in {1}",state,this.GetType().Name));
        }
    }

    void AddStateMac(ABSStateMac mac)
    {
        if(mac==null)
        {
            UnityEngine.Debug.LogError("Try to add a null StateMac to the controller:"+GetType().Name);
            return;
        }
        if (_stateMacs.ContainsKey(mac.Type))
        {
            UnityEngine.Debug.LogError("The same StateMac is already in the dictionary");
            return;
        }
        _stateMacs.Add(mac.Type, mac);
    }

    /// <summary>
    /// 注册状态机接口
    /// </summary>
    /// <param name="states"></param>
    public void AddStateMac(params ActorState[] states)
    {
        foreach (ActorState s in states)
        {
            AddStateMac(ABSStateMac.Create(s));
        }
    }

    public virtual void Init(ActorEntity actor)
    {
        Owner = actor;
        foreach (ABSStateMac mac in _stateMacs.Values)
        {
            mac.Init(actor);
            mac.SetController(this);
        }
    }

    /// <summary>
    /// 在驱动角色时被调用
    /// </summary>
    public abstract void Start();

    public virtual void Reset()
    {
        foreach (ABSStateMac mac in _stateMacs.Values)
        {
            mac.Reset();
        }
    }
}

