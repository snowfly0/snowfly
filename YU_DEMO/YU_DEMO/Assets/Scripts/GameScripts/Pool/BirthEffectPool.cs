using System;
using System.Collections.Generic;

//出生特效池
public class BirthEffectPool:GameObjectPool<BirthEffectPool>
{
    protected override void InitConfig()
    {
        _path = "Pool/BirthEffect";
        _initCount = 3;
    }

    [UnityEngine.RuntimeInitializeOnLoadMethod]
    static void OnLoadInit()
    {
        Init();
    }
}
