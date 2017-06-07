using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour {

    public static InputManager instance { get; private set; }

    public UnityEvent goRight;
    public UnityEvent goLeft;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            goRight.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            goLeft.Invoke();
        }	
	}
}
