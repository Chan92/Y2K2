using System.IO;
using UnityEngine;
using UnityEditor;

namespace Funtools.DungeonGenerator {
	public class DGG_RoomGeneratorFunctional :EditorWindow {
		private float groundHeight = 0.2f;
		private float wallThickness = 0.2f;
		private GameObject roomCopy;

		static DGG_RoomGeneratorFunctional instance;
		public static DGG_RoomGeneratorFunctional Instance {
			get {
				if(instance == null) {
					instance = new DGG_RoomGeneratorFunctional();
				}
				return instance;
			}
		}

		public void Generate() {
			//start with 1 as non-developers arent used to start counting by 0
			//restart with 1 each time u generate, so that deleted slots get refilled
			int _nameCounter = 1;

			//generates the amount of rooms requested
			for(int i = 0; i < RoomInfo.Instance.roomAmount; i++) {
				string _newName = RoomInfo.Instance.roomNames + _nameCounter.ToString();

				//if an object with the name already exist, update the counter
				while(File.Exists("Assets/" + RoomInfo.Instance.savePath + _newName + ".prefab")) {
					_nameCounter++;
					_newName = RoomInfo.Instance.roomNames + _nameCounter.ToString();
				}

				if(RoomInfo.Instance.baseRoom == null) {
					CreateBaseRoom();
				} else {
					//ModifyBaseRoom();
				}

				GameObject _newRoom = (GameObject) PrefabUtility.InstantiatePrefab(roomCopy);

				//GameObject _newRoom = (GameObject) PrefabUtility.InstantiatePrefab(baseRoom);
				//_newRoom.transform.localScale = new Vector3(roomWidth, roomHeight, roomLength);
				//AddRoomDoors(_newRoom.transform);
				FillRoom(_newRoom.transform);

				//save the room
				string path = Path.Combine(Application.dataPath, RoomInfo.Instance.savePath);
				path.Replace("\\", "/");
				PrefabUtility.SaveAsPrefabAsset(_newRoom, path + _newName + ".prefab");
				DestroyImmediate(_newRoom, false);
			}
		}

		private void AddRoomDoors(Transform _room) {

		}

		//add objects to the room
		private void FillRoom(Transform _room) {
			if(RoomInfo.Instance.objects == null) {
				Debug.Log("itsnull");
				return;
			}

			Debug.Log(RoomInfo.Instance.objects.Count);
			if(RoomInfo.Instance.objects.Count < 1) {
				return;
			}

			for(int i = 0; i < RoomInfo.Instance.objects.Count; i++) {
				if(RoomInfo.Instance.objects[i].objectPrefab != null && RoomInfo.Instance.objects[i].objectAmount >= 1) {
					for(int _object = 0; _object < RoomInfo.Instance.objects[i].objectAmount; _object++) {
						Transform _newObject = Instantiate(RoomInfo.Instance.objects[i].objectPrefab);
						_newObject.parent = _room;
						_newObject.localPosition = RandomPosition();
					}
				}
			}
		}

		private void CreateBaseRoom() {
			float _positionModifier = 1;
			GameObject newBaseRoom = new GameObject("BaseRoom");

			GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
			ground.name = "Ground";
			ground.transform.localScale = new Vector3(RoomInfo.Instance.roomWidth, groundHeight, RoomInfo.Instance.roomLength);
			ground.transform.parent = newBaseRoom.transform;

			for(int i = 0; i < 4; i++) {
				GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
				wall.name = "Wall " + (i + 1);
				wall.transform.parent = newBaseRoom.transform;

				if(i < 2) {
					wall.transform.localScale = new Vector3(RoomInfo.Instance.roomWidth, RoomInfo.Instance.roomHeight, wallThickness);
					wall.transform.localPosition = new Vector3(0, (RoomInfo.Instance.roomHeight + groundHeight) / 2, ((RoomInfo.Instance.roomLength - wallThickness) / 2) * _positionModifier);
				} else {
					wall.transform.localScale = new Vector3(wallThickness, RoomInfo.Instance.roomHeight, RoomInfo.Instance.roomLength);
					wall.transform.localPosition = new Vector3(((RoomInfo.Instance.roomWidth - wallThickness) / 2) * _positionModifier, (RoomInfo.Instance.roomHeight + groundHeight) / 2, 0);
				}

				_positionModifier *= -1;
			}

			Debug.Log("created");
			roomCopy = (GameObject) PrefabUtility.InstantiatePrefab(newBaseRoom);
		}

		private void ModifyBaseRoom() {
			float _positionModifier = 1;

			GameObject _baseRoomCopy = (GameObject) PrefabUtility.InstantiatePrefab(RoomInfo.Instance.baseRoom);

			Transform _groundObj = _baseRoomCopy.transform.Find("Ground");
			if(_groundObj) {
				Vector3 _oldGroundSize = _groundObj.localScale;
				_groundObj.localScale = new Vector3(_oldGroundSize.x * RoomInfo.Instance.roomWidth, groundHeight, _oldGroundSize.z * RoomInfo.Instance.roomLength);
			}

			for(int i = 0; i < 4; i++) {
				Transform _wallObj;

				_wallObj = _baseRoomCopy.transform.Find("Wall");
				if(!_wallObj)
					_wallObj = _baseRoomCopy.transform.Find("Wall " + i);

				if(_wallObj) {
					if(i < 2) {
						_wallObj.localScale = new Vector3(RoomInfo.Instance.roomWidth, RoomInfo.Instance.roomHeight, wallThickness);
						_wallObj.localPosition = new Vector3(0, (RoomInfo.Instance.roomHeight + groundHeight) / 2, ((RoomInfo.Instance.roomLength - wallThickness) / 2) * _positionModifier);
					} else {
						_wallObj.localScale = new Vector3(wallThickness, RoomInfo.Instance.roomHeight, RoomInfo.Instance.roomLength);
						_wallObj.localPosition = new Vector3(((RoomInfo.Instance.roomWidth - wallThickness) / 2) * _positionModifier, (RoomInfo.Instance.roomHeight + groundHeight) / 2, 0);
					}
				}

				_positionModifier *= -1;
			}

			Debug.Log("Scaled");
			roomCopy = _baseRoomCopy;
		}

		private Vector3 RandomPosition() {



			return Vector3.zero;
		}
	}
}
