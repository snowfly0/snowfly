using UnityEngine;
using System.Collections;

/// <summary>
/// 不会动的海洋生物控制器
/// </summary>
public class StaticFishController : ABSStateController
{
    public StaticFishController()
    {
        AddStateMac(ActorState.Null);
    }

    public override void Start()
    {
        ChangeState(ActorState.Null);
    }


}
