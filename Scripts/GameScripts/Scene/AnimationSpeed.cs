using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation))]
public class AnimationSpeed : MonoBehaviour
{
    public float Speed=1.0f;
    // Use this for initialization
    void Awake()
    {
        var anima = GetComponent<Animation>();
        string name = anima.clip.name;
        anima[name].speed = Speed;
    }

}
