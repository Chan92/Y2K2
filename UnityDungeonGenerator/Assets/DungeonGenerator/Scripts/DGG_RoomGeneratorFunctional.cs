using System.IO;
using UnityEngine;
using UnityEditor;

namespace Funtools.DungeonGenerator {
	public class DGG_RoomGeneratorFunctional :EditorWindow {
		private float groundHeight = 0.2f;
		private float wallThickness = 0.2f;
		private GameObject newRoom;

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
					ModifyBaseRoom();
				}

				AddRoomDoors(newRoom.transform);
				FillRoom(newRoom.transform);

				string path = Path.Combine(Application.dataPath, RoomInfo.Instance.savePath);
				path.Replace("\\", "/");
				PrefabUtility.SaveAsPrefabAsset(newRoom, path + _newName + ".prefab");
				DestroyImmediate(newRoom, false);
			}
		}

		private void AddRoomDoors(Transform _room) {

		}

		//add objects to the room
		private void FillRoom(Transform _room) {
			if(RoomInfo.Instance.objects == null) {
				return;
			}

			if(RoomInfo.Instance.objects.Count < 1) {
				return;
			}

			for(int i = 0; i < RoomInfo.Instance.objects.Count; i++) {
				if(RoomInfo.Instance.objects[i].objectPrefab != null && RoomInfo.Instance.objects[i].objectAmount >= 1) {
					for(int _object = 0; _object < RoomInfo.Instance.objects[i].objectAmount; _object++) {
						Transform _newObject = Instantiate(RoomInfo.Instance.objects[i].objectPrefab);
						_newObject.parent = _room;
						_newObject.localPosition = RandomPosition(_newObject.localScale, _room);
					}
				}
			}
		}

		private void CreateBaseRoom() {
			float _positionModifier = 1;
			GameObject _newBaseRoom = new GameObject("BaseRoom");

			GameObject _ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
			_ground.name = "Ground";
			_ground.transform.localScale = new Vector3(RoomInfo.Instance.roomWidth, groundHeight, RoomInfo.Instance.roomLength);
			_ground.transform.parent = _newBaseRoom.transform;
			SetMaterial(_ground, RoomInfo.Instance.groundMaterial);

			for(int i = 0; i < 4; i++) {
				GameObject _wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
				_wall.name = "Wall " + (i + 1);
				_wall.transform.parent = _newBaseRoom.transform;
				SetMaterial(_wall, RoomInfo.Instance.wallMaterial);
				Repositioning(_wall.transform, i, _positionModifier);
				_positionModifier *= -1;
			}

			Debug.Log("created");
			newRoom = _newBaseRoom;
		}

		private void ModifyBaseRoom() {
			float _positionModifier = 1;

			GameObject _baseRoomCopy = (GameObject) PrefabUtility.InstantiatePrefab(RoomInfo.Instance.baseRoom.gameObject);

			Transform _groundObj = _baseRoomCopy.transform.Find("Ground");
			if(_groundObj) {
				Vector3 _oldGroundSize = _groundObj.localScale;
				_groundObj.localScale = new Vector3(_oldGroundSize.x * RoomInfo.Instance.roomWidth, groundHeight, _oldGroundSize.z * RoomInfo.Instance.roomLength);
				SetMaterial(_groundObj.gameObject, RoomInfo.Instance.groundMaterial);
			}

			for(int i = 0; i < _baseRoomCopy.transform.childCount; i++) {
				Transform _wallObj;

				_wallObj = _baseRoomCopy.transform.Find("Wall");
				if(!_wallObj) {
					_wallObj = _baseRoomCopy.transform.Find("Wall " + i);
				}

				if(_wallObj) {
					_wallObj.name = ("Wall " + i);
					SetMaterial(_wallObj.gameObject, RoomInfo.Instance.wallMaterial);
					Repositioning(_wallObj, i, _positionModifier);
				}

				_positionModifier *= -1;
			}

			Debug.Log("Scaled");
			newRoom = _baseRoomCopy;
		}

		private void SetMaterial(GameObject _obj, Material _mat) {
			Renderer renderer = _obj.GetComponent<Renderer>();

			if (renderer && _mat) {
				renderer.material = _mat;
			}
		}

		//setting the object  on the correct scale and position
		private void Repositioning(Transform _obj, int _counter, float _positionModifier) {
			if(_counter < 2) {
				_obj.localScale = new Vector3(RoomInfo.Instance.roomWidth, RoomInfo.Instance.roomHeight, wallThickness);
				_obj.localPosition = new Vector3(0, (RoomInfo.Instance.roomHeight + groundHeight) / 2, ((RoomInfo.Instance.roomLength - wallThickness) / 2) * _positionModifier);
			} else {
				_obj.localScale = new Vector3(wallThickness, RoomInfo.Instance.roomHeight, RoomInfo.Instance.roomLength);
				_obj.localPosition = new Vector3(((RoomInfo.Instance.roomWidth - wallThickness) / 2) * _positionModifier, (RoomInfo.Instance.roomHeight + groundHeight) / 2, 0);
			}
		}

		private Vector3 RandomPosition(Vector3 _objSize, Transform _room) {
			Vector3 _roomSize = _room.Find("Ground").localScale /2;

			_roomSize.x -= _objSize.x/2;			 
			_roomSize.z -= _objSize.z/2;
			_roomSize.y += _objSize.y/2;

			float _randomX = Random.Range(-_roomSize.x, _roomSize.x);
			float _randomZ = Random.Range(-_roomSize.z, _roomSize.z);
			return new Vector3(_randomX, _roomSize.y, _randomZ);
		}
	}
}
