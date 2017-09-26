using UnityEngine;
using System.Collections;

public class PointChangePosition : MonoBehaviour {
    public _fishBirthPoint FBPs;
    public float Speed = 1.0f;
    private bool StarMove = false;
    public bool isVector3 = true;
    float _fCurTime;
	// Use this for initialization
	void Start () {

	}

    void Update()
    {
        _fCurTime += Time.deltaTime;
        if(_fCurTime>=Speed)
        {
            _fCurTime = 0f;
            _changePosition();
        }
    }
    void _changePosition() {
        StarMove = true;

        if (isVector3) {
            iTween.MoveTo(gameObject, iTween.Hash("position", FBPs.TargetPointPosi(), "time", Speed + 0.1f, "EaseType", "linear"));//立体空间移动；
         }else{
             iTween.MoveTo(gameObject, iTween.Hash("position",FBPs._birthPoint(), "time", Speed + 0.1f, "EaseType", "linear"));//平面空间移动；
         }
    }

}
