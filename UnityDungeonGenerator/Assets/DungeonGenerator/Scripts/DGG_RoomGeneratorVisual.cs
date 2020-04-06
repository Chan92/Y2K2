using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Funtools.DungeonGenerator {
	public class DGG_RoomGeneratorVisual :EditorWindow {
		private const float widthCheck = 500;
		private Vector2 scrollpos;

		[MenuItem("Window/DungeonGenerator")]
		public static void ShowWindow() {
			GetWindow<DGG_RoomGeneratorVisual>("DungeonGenerator");
		}



		//Draws elements to the window
		private void OnGUI() {
			//Debug.Log("size: " + position.width + " x " + position.height);
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
			DGG_RoomInfo.Instance.roomNames = EditorGUILayout.TextField("Room Name", DGG_RoomInfo.Instance.roomNames);
			Rect rect = (Rect) EditorGUILayout.BeginHorizontal();

				float _edgeGab = 5;
				float _labelSize = 35;
				float _inputSize = position.width * 0.2f;

				DGG_RoomInfo.Instance.roomAmount = EditorGUILayout.IntField("Amount", DGG_RoomInfo.Instance.roomAmount, GUILayout.Width(200));

				EditorGUI.LabelField(
					new Rect(position.width - _inputSize - _edgeGab - _labelSize, rect.y, _labelSize, EditorGUIUtility.singleLineHeight), "Type");
				DGG_RoomInfo.Instance.roomType = (RoomType) EditorGUI.EnumPopup(
					new Rect(position.width - _inputSize - _edgeGab, rect.y, _inputSize, EditorGUIUtility.singleLineHeight), DGG_RoomInfo.Instance.roomType);

			GUILayout.EndHorizontal();
		}

		private void DrawSizeSettings() {
			Rect rect = (Rect) EditorGUILayout.BeginHorizontal();
				float _edgeGab = 5;

				float _labelSize = 15;
				float _inputSize = position.width * 0.15f;
				float _inputGab = (position.width * 0.25f) + _edgeGab;

				string _width = "X";
				string _height = "Y";
				string _length = "Z";

				if(position.width > widthCheck) {
					_labelSize = 45;

					_width = "Width";
					_height = "Height";
					_length = "Length";
				}

				EditorGUILayout.LabelField("Size");

				//Width
				EditorGUI.LabelField(
					new Rect(position.width - (_inputGab * 2) - _inputSize - _labelSize, rect.y, _labelSize, EditorGUIUtility.singleLineHeight), _width);
				DGG_RoomInfo.Instance.roomWidth = EditorGUI.FloatField(
					new Rect(position.width - (_inputGab * 2) - _inputSize, rect.y, _inputSize, EditorGUIUtility.singleLineHeight), DGG_RoomInfo.Instance.roomWidth);

				//Height
				EditorGUI.LabelField(
					new Rect(position.width - _inputGab - _inputSize - _labelSize, rect.y, _labelSize, EditorGUIUtility.singleLineHeight), _height);
				DGG_RoomInfo.Instance.roomHeight = EditorGUI.FloatField(
					new Rect(position.width - _inputGab - _inputSize, rect.y, _inputSize, EditorGUIUtility.singleLineHeight), DGG_RoomInfo.Instance.roomHeight);

				//Length
				EditorGUI.LabelField(
					new Rect(position.width - _inputSize - _edgeGab - _labelSize, rect.y, _labelSize, EditorGUIUtility.singleLineHeight), _length);
				DGG_RoomInfo.Instance.roomLength = EditorGUI.FloatField(
					new Rect(position.width - _inputSize - _edgeGab, rect.y, _inputSize, EditorGUIUtility.singleLineHeight), DGG_RoomInfo.Instance.roomLength);

			GUILayout.EndHorizontal();
		}

		private void DrawMaterialSettings() {
#pragma warning disable CS0618 // Type or member is obsolete
			Rect rect = (Rect) EditorGUILayout.BeginHorizontal();
			float _edgeGab = 5;
			float _labelSize = 80;
			float _inputSize = position.width * 0.15f;

			DGG_RoomInfo.Instance.groundMaterial = (Material) EditorGUILayout.ObjectField("Ground Material", DGG_RoomInfo.Instance.groundMaterial, typeof(Material), GUILayout.Width (250));

			EditorGUI.LabelField(
					new Rect(position.width - _inputSize - _edgeGab - _labelSize, rect.y, _labelSize, EditorGUIUtility.singleLineHeight), "Wall Material");
			DGG_RoomInfo.Instance.wallMaterial = (Material) EditorGUI.ObjectField(
				new Rect(position.width - _inputSize - _edgeGab, rect.y, _inputSize, EditorGUIUtility.singleLineHeight), DGG_RoomInfo.Instance.wallMaterial, typeof(Material));


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
			float _scrollSize = 250;

			if(position.width > widthCheck) {
				_scrollSize = 100;
			}

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

			scrollpos = EditorGUILayout.BeginScrollView(scrollpos, false, false, GUILayout.Height(_scrollSize));
			DGG_RoomInfo.Instance.SerializedObject.Update();
			DGG_RoomInfo.Instance.myList.DoLayoutList();
			DGG_RoomInfo.Instance.SerializedObject.ApplyModifiedProperties();
			GUILayout.EndScrollView();
		}

		private void DrawSaveSettings() {
			DGG_RoomInfo.Instance.savePath = EditorGUILayout.TextField("Save Path", DGG_RoomInfo.Instance.savePath);
			if(GUILayout.Button("Generate rooms")) {
				DGG_RoomGeneratorFunctional.Instance.Generate();
			}
		}
	}
}