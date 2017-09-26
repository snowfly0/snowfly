using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class _fishBirthPoint : MonoBehaviour
{

    public Transform _LU;
    public Transform _RU;
    public Transform _LD;
    public Transform Top;
    private System.Random ram01 = new System.Random();

    void Start()
    {
        // print(_birthPoint());
    }
    public Vector3 _birthPoint()
    {

        float _bpX = ram01.Next((int)_LU.position.x, (int)_RU.position.x);
        float _bpZ = ram01.Next((int)_LD.position.z, (int)_LU.position.z);
        //   print(new Vector3(_bpX, transform.position.y, _bpZ));
        return new Vector3(_bpX, transform.position.y, _bpZ);
    }
    public Vector3 TargetPointPosi()
    {
        float _bpX = ram01.Next((int)_LU.position.x, (int)_RU.position.x);
        float _bpZ = ram01.Next((int)_LD.position.z, (int)_LU.position.z);
        float _bpY = ram01.Next((int)Top.position.y, (int)_LU.position.y);
        return new Vector3(_bpX, _bpY, _bpZ);
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!_LU || !_RU || !_LD || !Top) return;
        if(Selection.gameObjects.Length>0)
        {
            foreach(GameObject go in Selection.gameObjects)
            {
                if(go==gameObject)
                {
                    Gizmos.color = Color.red;
                }
            }
        }
        Gizmos.DrawLine(_LU.position, _RU.position);
        Gizmos.DrawLine(_LU.position, _LD.position);
        Vector3 offset = _LU.position - _RU.position;
        Vector3 nextPoint=_LD.position-offset;
        Gizmos.DrawLine(_LD.position, nextPoint);
        Gizmos.DrawLine(nextPoint, _RU.position);
    }
#endif
}
