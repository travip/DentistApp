using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
	public class BackSelection : MonoBehaviour, iSelectable
	{
		public void Select ()
		{
			MenuManager.instance.ToPreviousRow();
		}
	}
}
