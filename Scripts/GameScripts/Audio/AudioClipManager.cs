using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 声音资源管理类，资源放在Resources/Sounds/文件夹下，每增加一类枚举，在此文件夹下新增一个和枚举名一样的文件夹，将声音资源放在此文件夹下即可
/// </summary>
public class AudioClipManager:XRSingleton<AudioClipManager>
{
    Dictionary<int, AudioClip>[] _allClips;

    public void Init()
    {
        Array array= Enum.GetValues(typeof(ClipType));
        int len= array.GetLength(0);
        _allClips = new Dictionary<int, AudioClip>[len];
        for(int i=0;i<len;++i)
        {
            _allClips[i] = new Dictionary<int, AudioClip>();
        }
    }

    public AudioClip GetClip(ClipType type,int id)
    {
        if(_allClips==null)
        {
            Init();
        }
        int index=(int)type;
        Dictionary<int, AudioClip> clips = _allClips[index];
        AudioClip clip;
        if(!clips.TryGetValue(id,out clip))
        {
            clip = Resources.Load<AudioClip>("Sounds/" +type.ToString()+"/"+ id);
            clips.Add(id, clip);
        }
        return clip;
    }


    public void Collect()
    {
        if (_allClips == null) return;
        for (int i = 0; i < _allClips.Length; ++i)
        {
            _allClips[i].Clear();
        }
    }

    public void Collect(ClipType type)
    {
        if (_allClips == null) return;
        _allClips[(int)type].Clear();
    }
}



public enum ClipType
{
    MyFish=0,
    TaskGuide,
    SeaBook,
}


//public class ClipType
//{
//    static int _CurrentIndex;
//    public string Path { get; protected set; }
//    public int Type { get; protected set; }

//    public ClipType(string loadPath)
//    {
//        Path = loadPath;
//        Type = _CurrentIndex++;
//    }

//    public static readonly ClipType TaskGuide = new ClipType("Sounds/TaskGuide");
//    public static readonly ClipType MyFish = new ClipType("Sounds/MyFish");
//}

