using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DGG_RoomGenerator))]
public class DGG_RoomGeneratorEditor: Editor{
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		DGG_RoomGenerator myRGScript = (DGG_RoomGenerator) target;
		if(GUILayout.Button("Build Object")) {
			myRGScript.Generate();
		}
	}
}
