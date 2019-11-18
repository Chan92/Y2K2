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

public class DGG_RoomGenerator : MonoBehaviour {
	[SerializeField]
	private string roomNames = "Room";
	[SerializeField]
	private int roomAmount = 1;
	[SerializeField]
	private int roomWidth = 1, roomLength = 1, roomHeight = 1;
	[SerializeField][Range(1, 4)]
	private int doorAmount = 1;
	[SerializeField]
	private RoomType roomType;
	[SerializeField]
	private Material groundMaterial, wallMaterial, ceilingMaterial;
	[SerializeField]
	private GameObject baseRoom;
	[SerializeField]
	private Transform doorPrefab;
	[SerializeField]
	private ObjectList[] objects;
	[Space(25)]
	[SerializeField]
	private string savePath = "DungeonGenerator/Prefabs/Rooms/";

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
