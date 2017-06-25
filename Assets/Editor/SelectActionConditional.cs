using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CylinderMenu.TransitionItem))]
public class SelectActionConditional : Editor
{
	CylinderMenu.TransitionItem script;

	void OnEnable () {
		script = (CylinderMenu.TransitionItem)target;
	}

	public override void OnInspectorGUI () {
		// Show default values
		DrawDefaultInspector();

		// If the enum "SelectAction" is currently set to imageView, show the 'full sized pic' Texture attribute
		if (script.selectAction == CylinderMenu.MenuManager.SelectAction.imageView) {
			script.FullSizedPic = (Texture)EditorGUILayout.ObjectField("Full sized picture", script.FullSizedPic, typeof(Texture), false);
		}

		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
}
