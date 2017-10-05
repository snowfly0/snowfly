using System;
using System.Collections.Generic;
using UnityEngine;







public interface ISingleObject
{
    int ObjectId { get; set; }
}



public class GameObjectPoolRoot
{
    public static GameObject Root
    {
        get
        {
            if (!_Root)
            {
                _Root = new GameObject("[GameObjectPool]");
                GameObject.DontDestroyOnLoad(_Root);
            }
            return _Root;
        }
    }

    static GameObject _Root;
}


public class GameObjectPool<T> : XRSingleton<T> where T : GameObjectPool<T>, new()
{
    protected string _path;
    protected int _initCount = 20;
    Stack<GameObject> _gameObjects = new Stack<GameObject>();
    GameObject _prefab;
    GameObject _root;
    public static void Init(GameObject root = null)
    {
        Instance._Init(root);
    }

    public static GameObject GetOne()
    {
        return Instance._GetOne();
    }




    public static void Return(GameObject go)
    {
        if (go.activeSelf)
        {
            go.transform.SetParent(Instance._root.transform, false);
            go.SetActive(false);
        }
        Instance._gameObjects.Push(go);
    }

    protected virtual void InitConfig()
    {

    }

    void _Init(GameObject root)
    {
        if (_prefab) return;//已经初始化不再初始化
        if (root != null)
        {
            _root = root;
        }
        else
        {
            _root = GameObjectPoolRoot.Root;
        }
        InitConfig();
        _prefab = Resources.Load<GameObject>(_path);
        if (_prefab == null)
        {
            UnityEngine.Debug.LogError(string.Format("No prefab type at path:{0} for pool:{1}", _path, GetType().Name));
            return;
        }

        for (int i = 0; i < _initCount; ++i)
        {
            GameObject go = CreateNew();
            _gameObjects.Push(go);
        }
    }

    GameObject CreateNew()
    {
        GameObject parent = _root;
        GameObject go = GameObject.Instantiate<GameObject>(_prefab);
        go.SetActive(false);
        go.transform.SetParent(parent.transform,false);
        InitObject(go);
        return go;
    }


    protected virtual void InitObject(GameObject go)
    {

    }

    GameObject _GetOne()
    {
        if (_gameObjects.Count < 1)
        {
            if (!_prefab)
            {
                _Init(null);
            }
            GameObject go = CreateNew();
            return go;
        }
        else
            return _gameObjects.Pop();
    }
}



public class TObjectPool<T, TPool> where T : Component where TPool:TObjectPool<T, TPool>,new()
{
    protected string _path;
    protected int _initCount = 20;
    static TPool _Instance;
    protected static TPool  Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance =new TPool();
            }
            return _Instance;
        }
    }

    Stack<T> _objects = new Stack<T>();
    GameObject _prefab;
    GameObject _root;



    public static void Init(GameObject root = null)
    {
        Instance._Init(root);
    }

    public static T GetOne()
    {
        return Instance._GetOne();
    }




    public static void Return(T go)
    {
        Instance.OnReturn(go); 
    }



    protected virtual void InitConfig()
    {

    }



    void _Init(GameObject root)
    {
        if (_prefab) return;//已经初始化不再初始化
        if (root != null)
        {
            _root = root;
        }
        else
        {
            _root = GameObjectPoolRoot.Root;
        }
        InitConfig();
        _prefab = Resources.Load<GameObject>(_path);
        if (_prefab == null)
        {
            UnityEngine.Debug.LogError(string.Format("No prefab type at path:{0} for pool:{1}", _path, GetType().Name));
            return;
        }
        for (int i = 0; i < _initCount; ++i)
        {
            T comp = CreateNew();
            _objects.Push(comp);
            InitObject(comp);
        }
    }


    T CreateNew()
    {
        GameObject parent = _root;
        GameObject go = GameObject.Instantiate<GameObject>(_prefab);
        go.SetActive(false);
        go.transform.SetParent(parent.transform,false);
        T comp = go.GetComponent<T>();
        InitObject(comp);
        return comp;
    }

    protected virtual void InitObject(T comp)
    {

    }

    T _GetOne()
    {
        if (_objects.Count < 1)
        {
            if (!_prefab)
            {
                _Init(null);
            }
            T comp = CreateNew();
            return comp;
        }
        else
            return _objects.Pop();
    }

    protected virtual void OnReturn(T comp)
    {
        if (comp.gameObject.activeSelf)
        {
            comp.transform.SetParent(_root.transform, false);
            comp.gameObject.SetActive(false);
        }
        Instance._objects.Push(comp);
    }
}

