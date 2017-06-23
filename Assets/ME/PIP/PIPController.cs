using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIPController : TransitionableObject
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

    public void RightHanded()
    {
        PIPPointer.localRotation = Quaternion.Euler(0, 0, 0);
    }
    
    public void LeftHanded()
    {
        PIPPointer.localRotation = Quaternion.Euler(0, -180f, 0);
    }

	// Transitions

	override protected IEnumerator TransitionIn () {
		InputManager.instance.ToggleViewMode();
		InputManager.instance.goDown.AddListener(StartTransitionOut);
		yield return null;
	}

	override protected IEnumerator TransitionOut () {
		OverlayTransitioner.instance.TransitionScreenNotCo(ScreenType.MainMenu);
		InputManager.instance.goDown.RemoveListener(StartTransitionOut);

		yield return new WaitForSeconds(Constants.Transitions.FadeTime);

		InputManager.instance.ToggleViewMode();
		gameObject.SetActive(false);
		CylinderMenu.MenuManager.instance.ExitPIP();
	}
}
