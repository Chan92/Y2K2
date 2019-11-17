using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DGG_DungeonGenerator))]
public class DGG_DungeonGeneratorEditor : Editor{
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		DGG_DungeonGenerator myDGScript = (DGG_DungeonGenerator) target;
		if(GUILayout.Button("Build Object")) {
		}
	}
}
