using UnityEngine;

/// <summary>
/// 泛型脚本单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class XRUnitySingleton<T> : MonoBehaviour where T:class
{
    public virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    protected XRUnitySingleton() { }

    private static T m_Singleton;
    public static T Instance
    {
        get
        {
            if (m_Singleton == null)
            {
                T[] tons = GameObject.FindObjectsOfType(typeof(T)) as T[];
                if (tons == null||tons.Length==0)
                {
                    //Debug.LogError("Can't not find instance of SingleType:" + typeof(T).Name);
                }
                else if (tons.Length > 1)
                {
                    //Debug.LogError("More than one instance of SingleType:" + typeof(T).Name + ",is not allow");
                }
                else
                {
                    m_Singleton = tons[0];
                }
            }
            return m_Singleton;
        }
    }

    public virtual void OnDestroy()
    {
        m_Singleton = null;
    }
}

/// <summary>
/// 泛型单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class XRSingleton<T> where T:new ()
{
    private static T m_T;
    protected XRSingleton(){}
    public static T Instance
    {
        get
        {
            if (m_T == null)
            {
                    m_T = new T();
            }
            return m_T;
        }
    }
}

