using UnityEngine;
using System.Collections.Generic;
using System;
public class SceneManager : XRUnitySingleton<SceneManager>
{
    public GameObject[] Scenes;
    public static AudioSource audios;
    public AudioSource myAudios;
    public bool ShowSceneFish = true;
    public bool ShowAllFish;
    public int GetCurrentScene { get { return _sceneIndex; } }

    public float EatFoodMinDistance = 20;

    public Vector3 FoodPosition { get; protected set; }
    public Action<int> OnSceneChange;
    int _sceneIndex;
    // Use this for initialization
    public override void Awake()
    {
       
    }

    void Start()
    {
        LoadScene();
        LoadSceneFishes();
    }

    void LoadScene()
    {
        _sceneIndex = PlayerPrefs.GetInt("SceneIndex", 0);
        if (_sceneIndex >= Scenes.Length)
        {
            PlayerPrefs.SetInt("SceneIndex", 0);
            _sceneIndex = 0;
        }
        for (int i = 0; i < Scenes.Length; ++i)
        {
            if (i != _sceneIndex)
            {
                Scenes[i].gameObject.SetActive(false);
            }
            else
            {
                Scenes[i].gameObject.SetActive(true);
                SceneChange(i);
            }
        }
        audios = myAudios;
    }

    void LoadSceneFishes()
    {
        if (!ShowSceneFish) return;
        foreach (var data in SceneFishData.DataMap.Values)
        {
            if (!SceneManager.Instance.ShowAllFish&&!data.Enable) continue;
            ActorEntity actor = ActorPool.GetOne(data.TypeId);
            if (!actor)
            {
                UnityEngine.Debug.LogError("Miss sceneFish,TypeId:" + data.TypeId);
                continue;
            }
            actor.Create(data);
            actor.gameObject.SetActive(true);
            actor.Run();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>当前场景</returns>
    public int ChangeScene()
    {
        Scenes[_sceneIndex].SetActive(false);
        if (_sceneIndex < Scenes.Length - 1)
        {
            Scenes[++_sceneIndex].SetActive(true);
            SceneChange(_sceneIndex);
        }
        else
        {
            Scenes[0].SetActive(true);
            SceneChange(0);
            _sceneIndex = 0;
        }
        PlayerPrefs.SetInt("SceneIndex", _sceneIndex);
        return _sceneIndex;
    }

    void SceneChange(int index)
    {
        if(OnSceneChange!=null)
        {
            OnSceneChange(index);
        }
    }

    public void NofityFeedFishes(Vector3 foodPosition)
    {
        FoodPosition = foodPosition;
        List<ActorEntity> actors = ActorPool.GetActiveActors();
        for (int i = actors.Count - 1; i >= 0; --i)
        {
            var actor = actors[i];
            if (Vector3.Distance(FoodPosition, actor.transform.position) < EatFoodMinDistance)
            {
                actor.NotifyFeed();
            }
        }
    }
}
