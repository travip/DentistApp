using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CylinderMenu
{
	public class MenuSelection : MonoBehaviour, iSelectable
	{
		private TransitionItem parentItem;

		void Start()
		{
			parentItem = transform.parent.parent.GetComponent<TransitionItem>();
		}

		public void Select()
		{
			MenuManager.instance.SelectMenuItem(parentItem);
		}
	}
}
