using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;
    public MyFEvent e_hit;
    [System.Serializable]
    public class MyFEvent : UnityEvent<float>
    {
        //event that can pass variables
        //scriptable objects (enums) are also good for stuff like gamestate
    }
    public void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(e_hit != null)
        {
            e_hit = new MyFEvent();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
