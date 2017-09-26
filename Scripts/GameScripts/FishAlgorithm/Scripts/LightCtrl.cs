using UnityEngine;
using System.Collections;

public class LightCtrl : MonoBehaviour {


	public float _speed=0.5f;
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0, _speed * Time.deltaTime,Space.Self);
	}
}
