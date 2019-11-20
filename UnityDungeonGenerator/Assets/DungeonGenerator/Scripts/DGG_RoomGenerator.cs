using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Events;

[System.Serializable]
public class ObjectList {
	[SerializeField]
	public int objectAmount;
	[SerializeField]
	public Transform objectPrefab;
}

public class DGG_RoomGenerator :EditorWindow {
	private string roomNames = "Room";
	private int roomAmount = 1;
	private RoomType roomType;
	private int roomWidth = 1, roomLength = 1, roomHeight = 1;
	private Material groundMaterial, wallMaterial, ceilingMaterial;
	private GameObject baseRoom;
	private int doorAmount = 1;
	private const int minDoors = 1, maxDoors = 4;
	private Transform doorPrefab;
	[SerializeField]
	private List<ObjectList> objects;
	private string savePath = "DungeonGenerator/Prefabs/Rooms/";
	private bool isInitialized = false;
	private ReorderableList _myList;
	private SerializedObject _serializedObject;
	[MenuItem("Window/DungeonGenerator")]
	public static void ShowWindow() {
		GetWindow<DGG_RoomGenerator>("DungeonGenerator");
	}

	//visuals
	private void OnGUI() {
		EditorGUILayout.Space();
		GUILayout.BeginHorizontal();
		roomNames = EditorGUILayout.TextField("Room Names", roomNames);
		roomAmount = EditorGUILayout.IntField("Amount", roomAmount);
		roomType = (RoomType) EditorGUILayout.EnumPopup("Type", roomType);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		roomWidth = EditorGUILayout.IntField("Width", roomWidth);
		roomLength = EditorGUILayout.IntField("Length", roomLength);
		roomHeight = EditorGUILayout.IntField("Height", roomHeight);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
#pragma warning disable CS0618 // Type or member is obsolete
		groundMaterial = (Material) EditorGUILayout.ObjectField("Ground Material", groundMaterial, objType: typeof(Material));
		wallMaterial = (Material) EditorGUILayout.ObjectField("Wall Material", wallMaterial, objType: typeof(Material));
		ceilingMaterial = (Material) EditorGUILayout.ObjectField("Ceiling Material", ceilingMaterial, objType: typeof(Material));
#pragma warning restore CS0618 // Type or member is obsolete
		GUILayout.EndHorizontal();

#pragma warning disable CS0618 // Type or member is obsolete
		baseRoom = (GameObject) EditorGUILayout.ObjectField("Base Room", baseRoom, objType: typeof(GameObject));
#pragma warning restore CS0618 // Type or member is obsolete

		GUILayout.BeginHorizontal();
		doorAmount = EditorGUILayout.IntSlider("Door Amount", doorAmount, minDoors, maxDoors);
#pragma warning disable CS0618 // Type or member is obsolete
		doorPrefab = (Transform) EditorGUILayout.ObjectField("Door", doorPrefab, objType: typeof(Transform));
#pragma warning restore CS0618 // Type or member is obsolete
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		DrawObjectList();
		EditorGUILayout.Space();

		savePath = EditorGUILayout.TextField("Save Path", savePath);
		if(GUILayout.Button("Generate rooms")) {
			Generate();
		}
	}

	private void DrawObjectList() {
		if(!isInitialized) {
			_serializedObject = new SerializedObject(this);

			_myList = new ReorderableList(_serializedObject, _serializedObject.FindProperty("objects"), true, true, true, true) {
				drawHeaderCallback = (Rect rect) => {
					EditorGUI.LabelField(rect, "Objects to spawn");
				}
			};
			isInitialized = true;
		}

		if(_myList.count > 0) {
			_myList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
				var element = _myList.serializedProperty.GetArrayElementAtIndex(index);
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

		_serializedObject.Update();
		_myList.DoLayoutList();
		_serializedObject.ApplyModifiedProperties();
	}

	//functionals
	public void Generate() {
		//start with 1 as non-developers arent used to start counting by 0
		//restart with 1 each time u generate to fill deleted slots
		int _nameCounter = 1;

		//generates the amount of rooms requested
		for(int i = 0; i < roomAmount; i++) {
			string _newName = roomNames + _nameCounter.ToString();

			//if an object with the name already exist, update the counter
			while(File.Exists("Assets/" + savePath + _newName + ".prefab")) {
				_nameCounter++;
				_newName = roomNames + _nameCounter.ToString();
			}

			GameObject _newRoom = (GameObject) PrefabUtility.InstantiatePrefab(baseRoom);
			_newRoom.transform.localScale = new Vector3(roomWidth, roomHeight, roomLength);
			AddRoomDoors(_newRoom.transform);
			FillRoom(_newRoom.transform);

			//save the room
			string path = Path.Combine(Application.dataPath, savePath);
			path.Replace("\\", "/");
			PrefabUtility.SaveAsPrefabAsset(_newRoom, path + _newName + ".prefab");
			DestroyImmediate(_newRoom, false);
		}
	}

	private void AddRoomDoors(Transform _room) {

	}

	//add objects to the room
	private void FillRoom(Transform _room) {

		Debug.Log(objects.Count);
		if(objects.Count < 1) {
			return;
		}

		for(int i = 0; i < objects.Count; i++) {
			if(objects[i].objectPrefab != null && objects[i].objectAmount >= 1) {
				for(int _object = 0; _object < objects[i].objectAmount; _object++) {
					Transform _newObject = Instantiate(objects[i].objectPrefab);
					_newObject.parent = _room;
				}
			}
		}

	}

}
