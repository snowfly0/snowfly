using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 根据选择的场景更改物体的激活状态
/// </summary>
public class ActiveByScene : MonoBehaviour
{
    public List<int> SceneIndexs;

    void Awake()
    {
        SceneManager.Instance.OnSceneChange += OnSceneChange;
    }


    void OnSceneChange(int index)
    {
        if(!SceneIndexs.Contains(index))
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if(!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
    }
}

