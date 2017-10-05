using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 扫描出的鱼的行为控制器
/// </summary>
public class ScanFishController : ABSStateController
{
    public ScanFishController()
    {
        AddStateMac(ActorState.Birth, ActorState.Leaving, ActorState.Showing, ActorState.Wandering,ActorState.EatFood,ActorState.RunAway);
    }



    public override void Start()
    {
        ChangeState(ActorState.Birth);
    }


    public override bool CanChangeState(ActorState state)
    {
        return Owner.CurrentState == ActorState.Wandering;
    }
}
