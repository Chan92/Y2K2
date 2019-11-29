using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Funtools.DungeonGenerator {
	public class DGG_RoomGeneratorVisual :EditorWindow {
		[MenuItem("Window/DungeonGenerator")]
		public static void ShowWindow() {
			GetWindow<DGG_RoomGeneratorVisual>("DungeonGenerator");
		}

		//Draws elements to the window
		private void OnGUI() {
			EditorGUILayout.Space();
			DrawGeneralSettings();

			DrawSizeSettings();
			DrawMaterialSettings();
			DrawBaseRoomSettings();
			
			//disabled cause its not yet implemented
			//DrawDoorSettings();
			EditorGUILayout.Space();
			DrawObjectList();
			EditorGUILayout.Space();
			DrawSaveSettings();
		}

		private void DrawGeneralSettings() {
			GUILayout.BeginHorizontal();
			DGG_RoomInfo.Instance.roomNames = EditorGUILayout.TextField("Room Names", DGG_RoomInfo.Instance.roomNames);
			DGG_RoomInfo.Instance.roomAmount = EditorGUILayout.IntField("Amount", DGG_RoomInfo.Instance.roomAmount);
			DGG_RoomInfo.Instance.roomType = (RoomType) EditorGUILayout.EnumPopup("Type", DGG_RoomInfo.Instance.roomType);
			GUILayout.EndHorizontal();
		}

		private void DrawSizeSettings() {
			GUILayout.BeginHorizontal();
			DGG_RoomInfo.Instance.roomWidth = EditorGUILayout.FloatField("Width", DGG_RoomInfo.Instance.roomWidth);
			DGG_RoomInfo.Instance.roomHeight = EditorGUILayout.FloatField("Height", DGG_RoomInfo.Instance.roomHeight);
			DGG_RoomInfo.Instance.roomLength = EditorGUILayout.FloatField("Length", DGG_RoomInfo.Instance.roomLength);
			GUILayout.EndHorizontal();
		}

		private void DrawMaterialSettings() {
#pragma warning disable CS0618 // Type or member is obsolete
			GUILayout.BeginHorizontal();
			DGG_RoomInfo.Instance.groundMaterial = (Material) EditorGUILayout.ObjectField("Ground Material", DGG_RoomInfo.Instance.groundMaterial, typeof(Material));
			DGG_RoomInfo.Instance.wallMaterial = (Material) EditorGUILayout.ObjectField("Wall Material", DGG_RoomInfo.Instance.wallMaterial, typeof(Material));
			/*disabled cause its not yet implemented
			DGG_RoomInfo.Instance.ceilingMaterial = (Material) EditorGUILayout.ObjectField("Ceiling Material", DGG_RoomInfo.Instance.ceilingMaterial, typeof(Material));
			*/
			GUILayout.EndHorizontal();
#pragma warning restore CS0618 // Type or member is obsolete
		}

		private void DrawBaseRoomSettings() {
#pragma warning disable CS0618 // Type or member is obsolete
			DGG_RoomInfo.Instance.baseRoom = (Transform) EditorGUILayout.ObjectField("Base Room", DGG_RoomInfo.Instance.baseRoom, typeof(Transform));
#pragma warning restore CS0618 // Type or member is obsolete
		}

		private void DrawDoorSettings() {
#pragma warning disable CS0618 // Type or member is obsolete
			GUILayout.BeginHorizontal();
			DGG_RoomInfo.Instance.doorAmount = EditorGUILayout.IntSlider("Door Amount", DGG_RoomInfo.Instance.doorAmount, DGG_RoomInfo.minDoors, DGG_RoomInfo.maxDoors);
			DGG_RoomInfo.Instance.doorPrefab = (Transform) EditorGUILayout.ObjectField("Door", DGG_RoomInfo.Instance.doorPrefab, typeof(Transform));
			GUILayout.EndHorizontal();			
#pragma warning restore CS0618 // Type or member is obsolete
		}

		private void DrawObjectList() {
			if(!DGG_RoomInfo.Instance.isInitialized) {
				DGG_RoomInfo.Instance.myList = new ReorderableList(DGG_RoomInfo.Instance.SerializedObject, DGG_RoomInfo.Instance.SerializedObject.FindProperty("objects"), true, true, true, true) {
					drawHeaderCallback = (Rect rect) => {
						EditorGUI.LabelField(rect, "Objects to spawn");
					}
				};
				DGG_RoomInfo.Instance.isInitialized = true;
			}

			if(DGG_RoomInfo.Instance.myList.count > 0) {
				DGG_RoomInfo.Instance.myList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
					var element = DGG_RoomInfo.Instance.myList.serializedProperty.GetArrayElementAtIndex(index);
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

			DGG_RoomInfo.Instance.SerializedObject.Update();
			DGG_RoomInfo.Instance.myList.DoLayoutList();
			DGG_RoomInfo.Instance.SerializedObject.ApplyModifiedProperties();
		}

		private void DrawSaveSettings() {
			DGG_RoomInfo.Instance.savePath = EditorGUILayout.TextField("Save Path", DGG_RoomInfo.Instance.savePath);
			if(GUILayout.Button("Generate rooms")) {
				DGG_RoomGeneratorFunctional.Instance.Generate();
			}
		}
	}
}