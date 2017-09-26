using UnityEngine;
using System.Collections;
using Actor.StateMac;

public enum ActorState
{
    Null,
    /// <summary>
    /// 出生
    /// </summary>
    Birth,
    /// <summary>
    /// 做展示
    /// </summary>
    Showing,
    /// <summary>
    /// 游荡
    /// </summary>
    Wandering,

    SceneWandering,
    /// <summary>
    /// 离场
    /// </summary>
    Leaving,
    /// <summary>
    /// 抢食
    /// </summary>
    EatFood,
}

/// <summary>
/// 抽象状态机类
/// </summary>
public abstract class ABSStateMac {
    public ABSStateController Controller;
    public static ABSStateMac Create(ActorState type)
    {
        ABSStateMac mac = null;
        switch (type)
        {
            case ActorState.Birth:
                {
                    mac = new FishBirthMac();
                    break;
                }
            case ActorState.Leaving:
                {
                    mac = new FishLeaveMac();
                    break;
                }
            case ActorState.Null:
                {
                    mac = new FishNullMac();
                    break;
                }
            case ActorState.Showing:
                {
                    mac = new FishShowMac();
                    break;
                }
            case ActorState.Wandering:
                {
                    mac = new FishWanderMac();
                    break;
                }
            case ActorState.SceneWandering:
                {
                    mac = new SceneFishWanderMac();
                    break;
                }
            case ActorState.EatFood:
                {
                    mac = new FishEatMac();
                    break;
                }
        }
        if(mac!=null)
        {
            mac.Type = type;
        }
        return mac;
    }


    public ActorState Type;

	public abstract void Init (ActorEntity actor);
	
    /// <summary>
    /// 为状态机设置控制器
    /// </summary>
    /// <param name="controller"></param>
    public void SetController(ABSStateController controller)
    {
        Controller = controller;
    }

    public void ChangeState(ActorState state)
    {
        if(Controller!=null)
        {
            Controller.ChangeState(state);
        }
    }

    /// <summary>
    /// 被其它脚本每帧更新
    /// </summary>
	public abstract void Update();


    public abstract void Reset();
  
    /// <summary>
    /// 在状态机进入时触发
    /// </summary>
    public virtual void OnStateEnter()
    {

    }

    /// <summary>
    /// 在状态机离开时触发
    /// </summary>
    public virtual void OnStateExit()
    {

    }


    //public virtual void OnStatePause()
    //{

    //}

    //public virtual void OnStateResume()
    //{

    //}
}
