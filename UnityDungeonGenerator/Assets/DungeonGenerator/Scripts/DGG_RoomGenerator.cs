using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
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
		int _nameCounter = 1;

		for(int i = 0; i < roomAmount; i++) {
			string _newName = roomNames + _nameCounter.ToString();

			while(File.Exists("Assets/" + savePath + _newName + ".prefab")) {
				_nameCounter++;
				_newName = roomNames + _nameCounter.ToString();
				print("new name");
			}

			GameObject _newRoom = (GameObject) PrefabUtility.InstantiatePrefab(baseRoom);
			_newRoom.name = _newName;

			string path = Path.Combine(Application.dataPath, savePath);
			path.Replace("\\", "/");
			PrefabUtility.SaveAsPrefabAsset(_newRoom, path + _newName +".prefab");
		}
    }

}
