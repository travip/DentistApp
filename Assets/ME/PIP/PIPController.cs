using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIPController : MonoBehaviour
{
    public PIPController instance { get; private set; }

    public Transform PIPPointer;

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }

    // Update is called once per frame
    void Update () {
		
	}

	public void StartPIP() {
		InputManager.instance.ToggleViewMode();
		InputManager.instance.goDown.AddListener(Back);
	}

	private void Back() {
		InputManager.instance.goDown.RemoveListener(Back);
		CylinderMenu.MenuManager.instance.ExitPIP();
        InputManager.instance.ToggleViewModeDelayed(0.5f);
    }

    public void RightHanded()
    {
        PIPPointer.localRotation = Quaternion.Euler(0, 0, 0);
    }
    
    public void LeftHanded()
    {
        PIPPointer.localRotation = Quaternion.Euler(0, -180f, 0);
    }
}
