using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 场景鱼的行为控制器(暂时不用)
/// </summary>
public class SceneFishController : ABSStateController
{
    public SceneFishController()
    {
        AddStateMac(ActorState.SceneWandering,ActorState.EatFood);
    }

    public override void Start()
    {
        ChangeState(ActorState.SceneWandering);
    }

    public override bool CanChangeState(ActorState state)
    {
        return Owner.CurrentState == ActorState.SceneWandering;
    }
}
