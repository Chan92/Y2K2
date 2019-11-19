using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DGG_RoomGeneratorBackup))]
public class DGG_RoomGeneratorEditor: Editor{
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		DGG_RoomGeneratorBackup myRGScript = (DGG_RoomGeneratorBackup) target;
		if(GUILayout.Button("Build Object")) {
			myRGScript.Generate();
		}
	}
}
