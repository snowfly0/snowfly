using System;
using System.Collections.Generic;
using UnityEngine;



public class ActorPool : XRSingleton<ActorPool>
{
    Dictionary<int, Queue<ActorEntity>> m_npcObjectMap;
    Dictionary<int, GameObject> m_prefabs;
    List<ActorEntity> m_usingNpcs;
    Transform m_actorRoot;
    public static void Initialize()
    {
        Instance._Initialize();
    }

    void _Initialize()
    {
        InitializePrefabs();
    }


    void InitializePrefabs()
    {
        m_actorRoot = new GameObject("[ActorsRoot]").transform;
        GameObject.DontDestroyOnLoad(m_actorRoot);
        m_npcObjectMap = new Dictionary<int, Queue<ActorEntity>>();
        m_prefabs = new Dictionary<int, GameObject>();
        m_usingNpcs = new List<ActorEntity>();
        var dataMap = ActorData.DataMap;

        foreach (var pair in dataMap)
        {
            Queue<ActorEntity> npcGroup = new Queue<ActorEntity>();
            GameObject prefab = null;
            ActorData data=pair.Value;;
            string prefabName=data.PrefabName;
            prefab = Resources.Load<GameObject>("ActorPrefabs/"+prefabName);
            if(!prefab)
            {
                Debug.LogError(string.Format("Can't find prefab {0}", prefabName));
                continue;
            }
            m_prefabs.Add(data.Id, prefab);
            for (int i = 0; i < data.MaxCount; ++i)
            {
                GameObject npc = GameObject.Instantiate<GameObject>(prefab);
                npc.SetActive(false);
                ActorEntity parent = Create(data, npc);
                npcGroup.Enqueue(parent);
            }
            m_npcObjectMap.Add(pair.Key, npcGroup);
        }
        Debug.LogError("FishPoolInitPrefabEnd");
    }

    ActorEntity Create(ActorData data,GameObject npc)
    {
        ActorEntity parent = npc.AddComponent<ActorEntity>();
        npc.transform.SetParent(m_actorRoot);
        parent.Init(data);
        parent.DOReset();
        return parent;
    }

    /// <summary>
    /// 从Npc池获取一个Npc对象
    /// </summary>
    /// <param name="typeName">Npc类型名</param>
    /// <returns></returns>
    public static ActorEntity GetOne(int type)
    {
        return Instance._GetOne(type);
    }

    ActorEntity _GetOne(int type)
    {
        Queue<ActorEntity> value;
        ActorEntity npc = null;
        if (m_npcObjectMap.TryGetValue(type, out value))
        {
            if (value.Count > 0)
            {
                npc = value.Dequeue();
            }
            else
            {
                npc = Create(ActorData.GetData(type), GameObject.Instantiate(m_prefabs[type]));
            }
        }
        if (npc != null)
        {
            m_usingNpcs.Add(npc);
        }
        else
        {
            Debug.LogError("Not enough fish in pool:" + type);
        }
        return npc;
    }

    /// <summary>
    /// 返还Npc到Npc池（会自动SetActive为false）
    /// </summary>
    /// <param name="npc"></param>
    public static void Return(ActorEntity npc)
    {
        Instance._Return(npc);
    }



    void _Return(ActorEntity npc)
    {
        Queue<ActorEntity> value;
        if (Instance.m_npcObjectMap.TryGetValue(npc.TypeId, out value))
        {
            if (value.Contains(npc))
            {
                Debug.LogError("NpcPool已经存在" + npc.name + "但是被重复返还");
            }
            value.Enqueue(npc);
        }
        m_usingNpcs.Remove(npc);
        GameObject go = npc.gameObject;
        go.SetActive(false);
        npc.transform.SetParent(m_actorRoot);
        npc.transform.localPosition = Vector3.zero;
        npc.transform.localRotation = Quaternion.identity;
        npc.DOReset();
    }

    /// <summary>
    /// 获取场景中所有用到的鱼
    /// </summary>
    /// <returns></returns>
    public static List<ActorEntity> GetActiveActors()
    {
        return Instance.m_usingNpcs;
    }

    public static void Reset()
    {
        List<ActorEntity> actors = GetActiveActors();
        for (int i = actors.Count-1; i>=0;--i)
        {
            Return(actors[i]);
        }
    }
}

