using UnityEngine;
using System.Collections.Generic;

public class PathManager : MonoBehaviour {
    static Dictionary<string, CurvePath> _Pathes = new Dictionary<string, CurvePath>();
    void Awake()
    {
        foreach(var p in GetComponentsInChildren<CurvePath>())
        {
            _Pathes.Add(p.name,p);
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnDestroy()
    {
        _Pathes.Clear();
    }
    public static CurvePath GetPath(string pathName)
    {
        CurvePath path;
        _Pathes.TryGetValue(pathName, out path);
        return path;
    }
}
