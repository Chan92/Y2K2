using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Funtools.DungeonGenerator {
	public class DGG_RoomGeneratorVisual :EditorWindow {
		[MenuItem("Window/DungeonGenerator")]
		public static void ShowWindow() {
			GetWindow<DGG_RoomGeneratorVisual>("DungeonGenerator");
		}

		//visuals
		private void OnGUI() {
			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			RoomInfo.Instance.roomNames = EditorGUILayout.TextField("Room Names", RoomInfo.Instance.roomNames);
			RoomInfo.Instance.roomAmount = EditorGUILayout.IntField("Amount", RoomInfo.Instance.roomAmount);
			RoomInfo.Instance.roomType = (RoomType) EditorGUILayout.EnumPopup("Type", RoomInfo.Instance.roomType);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			RoomInfo.Instance.roomWidth = EditorGUILayout.IntField("Width", RoomInfo.Instance.roomWidth);
			RoomInfo.Instance.roomHeight = EditorGUILayout.IntField("Height", RoomInfo.Instance.roomHeight);
			RoomInfo.Instance.roomLength = EditorGUILayout.IntField("Length", RoomInfo.Instance.roomLength);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
#pragma warning disable CS0618 // Type or member is obsolete
			RoomInfo.Instance.groundMaterial = (Material) EditorGUILayout.ObjectField("Ground Material", RoomInfo.Instance.groundMaterial, objType: typeof(Material));
			RoomInfo.Instance.wallMaterial = (Material) EditorGUILayout.ObjectField("Wall Material", RoomInfo.Instance.wallMaterial, objType: typeof(Material));
			RoomInfo.Instance.ceilingMaterial = (Material) EditorGUILayout.ObjectField("Ceiling Material", RoomInfo.Instance.ceilingMaterial, objType: typeof(Material));

			GUILayout.EndHorizontal();

			RoomInfo.Instance.baseRoom = (Transform) EditorGUILayout.ObjectField("Base Room", RoomInfo.Instance.baseRoom, objType: typeof(Transform));

			GUILayout.BeginHorizontal();
			RoomInfo.Instance.doorAmount = EditorGUILayout.IntSlider("Door Amount", RoomInfo.Instance.doorAmount, RoomInfo.minDoors, RoomInfo.maxDoors);
			RoomInfo.Instance.doorPrefab = (Transform) EditorGUILayout.ObjectField("Door", RoomInfo.Instance.doorPrefab, objType: typeof(Transform));
#pragma warning restore CS0618 // Type or member is obsolete
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();

			DrawObjectList();
			EditorGUILayout.Space();

			RoomInfo.Instance.savePath = EditorGUILayout.TextField("Save Path", RoomInfo.Instance.savePath);
			if(GUILayout.Button("Generate rooms")) {
				DGG_RoomGeneratorFunctional.Instance.Generate();
			}
		}

		private void DrawObjectList() {
			if(!RoomInfo.Instance.isInitialized) {
				RoomInfo.Instance.myList = new ReorderableList(RoomInfo.Instance.SerializedObject, RoomInfo.Instance.SerializedObject.FindProperty("objects"), true, true, true, true) {
					drawHeaderCallback = (Rect rect) => {
						EditorGUI.LabelField(rect, "Objects to spawn");
					}
				};
				RoomInfo.Instance.isInitialized = true;
			}

			if(RoomInfo.Instance.myList.count > 0) {
				RoomInfo.Instance.myList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
					var element = RoomInfo.Instance.myList.serializedProperty.GetArrayElementAtIndex(index);
					rect.y += 2;


					EditorGUI.LabelField(
						new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), "Amount");
					EditorGUI.PropertyField(
						new Rect(rect.x + 55, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("objectAmount"), GUIContent.none);

					EditorGUI.LabelField(
						new Rect(rect.x + 135, rect.y, 50, EditorGUIUtility.singleLineHeight), "Object");
					EditorGUI.PropertyField(
						new Rect(rect.x + 195, rect.y, rect.width - 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("objectPrefab"), GUIContent.none);
				};
			}

			RoomInfo.Instance.SerializedObject.Update();
			RoomInfo.Instance.myList.DoLayoutList();
			RoomInfo.Instance.SerializedObject.ApplyModifiedProperties();
		}
	}
}