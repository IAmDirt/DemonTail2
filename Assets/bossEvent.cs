using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class bossEvent : MonoBehaviour
{

    public UnityEvent event1;
    public UnityEvent event2;
    public UnityEvent event3;


    public void bossEvent1()
    {
        event1.Invoke();
    }
    public void bossEvent2()
    {
        event2.Invoke();

    }
    public void bossEvent3()
    {
        event3.Invoke();

    }
}
