﻿// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using System.Runtime.CompilerServices;
using Assets.Scripts.GameScripts.FishAlgorithm;
using UnityEngine;

/// <summary>
/// 主控类，游戏运行时候自动生成，无需添加脚本
/// </summary>
public class MainManager : XRUnitySingleton<MainManager>
{
    public override void Awake()
    {
        //[Scaner] 开启线程检测二维码扫描
        base.Awake();
        //初始化角色池
        ActorPool.Initialize();
    }

    void Start()
    {

    }

    [RuntimeInitializeOnLoadMethod]
    static void InitOnLoad()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("[MainManager]");
            go.AddComponent<MainManager>();
        }
    }



    void Update()
    {
    }


    //public void Reset()
    //{
    //    ActorPool.Reset();
    //}
}


