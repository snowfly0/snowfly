using UnityEngine;
using System.Collections.Generic;

public class BirthArea : XRUnitySingleton<BirthArea> {

    Dictionary<string, _fishBirthPoint> _points = new Dictionary<string, _fishBirthPoint>();

    public override void Awake()
    {
        foreach(Transform child in transform)
        {
            _fishBirthPoint area=child.GetComponent<_fishBirthPoint>();
            if (area)
            {
                _points.Add(area.name, area);
            }
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public _fishBirthPoint GetArea(string name)
    {
        _fishBirthPoint value;
        _points.TryGetValue(name, out value);
        return value;
    }
}
