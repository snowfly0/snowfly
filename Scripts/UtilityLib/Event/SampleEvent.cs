using UnityEngine;
using System.Collections;

/// <summary>
///暂时不太有用 
/// </summary>
public class SampleEvent : MonoBehaviour {

    public class TestEvent
    {
        public static readonly EventInfo T001=new EventInfo();
        public static readonly EventInfo T002=new EventInfo();

        public class UIEvent1
        {
            public static readonly EventInfo Login = new EventInfo();
            public static readonly EventInfo LoginCallback = new EventInfo();
        }
        public class UIEvent2
        {

        }
    }
	// Use this for initialization
	void Awake () {
        EventDispatcher.AddEventListener(TestEvent.T001, Log1);
        EventDispatcher.AddEventListener<GameObject>(TestEvent.T002, Log2);
        EventDispatcher.AddEventListener(TestEvent.T001, Log3);
        EventDispatcher.AddEventListener(TestEvent.T002, Log4);
        EventDispatcher.AddEventListener(TestEvent.UIEvent1.Login, Log4);
	}
	
    void Start()
    {
        EventDispatcher.TriggerEvent(TestEvent.T002);
        EventDispatcher.TriggerEvent<GameObject>(TestEvent.T002,new GameObject());
        EventDispatcher.TriggerEvent(TestEvent.T001);
    }
	// Update is called once per frame
	void Update () {

	}

    void Log1()
    {
        Debug.Log("SampleLog1");
    }

    void Log2(GameObject b)
    {
        Debug.Log("SampleLog2");
    }

    void Log3()
    {
        Debug.Log("SampleLog3");
    }

    void Log4()
    {
        Debug.Log("SampleLog4");
    }
}
