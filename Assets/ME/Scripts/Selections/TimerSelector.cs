using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerSelector : MonoBehaviour, iSelectable
{

    public UnityEvent trigger;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Select()
    {
        trigger.Invoke();
    }
}
