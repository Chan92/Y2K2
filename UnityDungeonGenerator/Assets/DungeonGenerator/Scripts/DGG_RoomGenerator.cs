using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ObjectList {
	public int objectAmount;
	public Transform objectPrefab;
}

public class DGG_RoomGenerator : EditorWindow {
	private string roomNames = "Room";
	private int roomAmount = 1;
	private RoomType roomType;
	private int roomWidth = 1, roomLength = 1, roomHeight = 1;
	private Material groundMaterial, wallMaterial, ceilingMaterial;
	private GameObject baseRoom;
	private int doorAmount = 1;
	private const int minDoors = 1, maxDoors = 4;
	private Transform doorPrefab;
	private ObjectList[] objects;
	[Space(25)]
	private string savePath = "DungeonGenerator/Prefabs/Rooms/";

	[MenuItem("Window/DungeonGenerator")]
	public static void ShowWindow() {
		GetWindow<DGG_RoomGenerator>("DungeonGenerator");
	}

	private void OnGUI() {
		GUILayout.BeginHorizontal();
		roomNames = EditorGUILayout.TextField("Room Names", roomNames);
		roomAmount = EditorGUILayout.IntField("Amount", roomAmount);
		roomType = (RoomType)EditorGUILayout.EnumPopup("Type", roomType);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		roomWidth = EditorGUILayout.IntField("Width", roomWidth);
		roomLength = EditorGUILayout.IntField("Length", roomLength);
		roomHeight = EditorGUILayout.IntField("Height", roomHeight);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
#pragma warning disable CS0618 // Type or member is obsolete
		groundMaterial = (Material)EditorGUILayout.ObjectField("Ground Material", groundMaterial, objType: typeof(Material));
		wallMaterial = (Material) EditorGUILayout.ObjectField("Wall Material", wallMaterial, objType: typeof(Material));
		ceilingMaterial = (Material) EditorGUILayout.ObjectField("Ceiling Material", ceilingMaterial, objType: typeof(Material));
#pragma warning restore CS0618 // Type or member is obsolete
		GUILayout.EndHorizontal();

#pragma warning disable CS0618 // Type or member is obsolete
		baseRoom = (GameObject)EditorGUILayout.ObjectField("Base Room", baseRoom, objType: typeof(GameObject));
#pragma warning restore CS0618 // Type or member is obsolete

		GUILayout.BeginHorizontal();
		doorAmount = EditorGUILayout.IntSlider("Door Amount", doorAmount, minDoors, maxDoors);
#pragma warning disable CS0618 // Type or member is obsolete
		doorPrefab = (Transform) EditorGUILayout.ObjectField("Door", doorPrefab, objType: typeof(Transform));
#pragma warning restore CS0618 // Type or member is obsolete
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();

		//objects list


		EditorGUILayout.Space();
		savePath = EditorGUILayout.TextField("Save Path", savePath);
		if(GUILayout.Button("Generate rooms")) {
			Generate();
		}
	}



	public void Generate(){
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
		if (objects.Length < 1) {
			return;
		}
		
		for (int i = 0; i < objects.Length; i++) {
			if (objects[i].objectAmount >= 1) {
				for(int _object = 0; _object < objects[i].objectAmount; _object++) {
					Transform _newObject = Instantiate(objects[i].objectPrefab);
					_newObject.parent = _room;
				}
			}
		}
	}

}
