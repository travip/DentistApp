using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class MenuAction 
{
	public enum SelectAction
	{
		SUB_MENU,
		IMAGE_VIEW,
		WEBCAM
	}

	//public delegate void SelectActionEvent ();

	//public static event SelectActionEvent subMenu, imageView, webcam;

	public static Action subMenu, imageView, webcam;


	public static Action GetDelegate (SelectAction onSelect)
	{
		switch (onSelect) {
			case SelectAction.SUB_MENU:
				return subMenu;

			case SelectAction.IMAGE_VIEW:
				return imageView;

			case SelectAction.WEBCAM:
				return webcam;

			default:
				return null;
		}
	}
}
