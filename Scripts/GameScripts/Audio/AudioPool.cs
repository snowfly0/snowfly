using System;
using System.Collections.Generic;
using UnityEngine;


public class AudioPool : TObjectPool<AudioSource, AudioPool>
{
    protected override void InitConfig()
    {
        _path = "Scene/SoundSource";
    }


    protected override void OnReturn(AudioSource comp)
    {
        base.OnReturn(comp);
        comp.clip = null;
    }
}

