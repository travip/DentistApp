using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerSelector : MonoBehaviour, iSelectable
{

    public UnityEvent trigger;

    public void Select()
    {
        trigger.Invoke();
    }
}
