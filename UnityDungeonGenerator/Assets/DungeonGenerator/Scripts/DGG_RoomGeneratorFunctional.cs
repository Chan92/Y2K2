using System.IO;
using UnityEngine;
using UnityEditor;

namespace Funtools.DungeonGenerator {
	public class DGG_RoomGeneratorFunctional :EditorWindow {
		private GameObject newRoom;
		private float roomSizeMultiplier = 10f;
		private float groundHeight = 0.2f;
		private float wallThickness = 0.2f;

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
			for(int i = 0; i < DGG_RoomInfo.Instance.roomAmount; i++) {
				string _newName = DGG_RoomInfo.Instance.roomNames + _nameCounter.ToString();

				//if an object with the name already exist, update the counter
				while(File.Exists("Assets/" + DGG_RoomInfo.Instance.savePath + _newName + ".prefab")) {
					_nameCounter++;
					_newName = DGG_RoomInfo.Instance.roomNames + _nameCounter.ToString();
				}

				if(DGG_RoomInfo.Instance.baseRoom == null) {
					CreateBaseRoom();
				} else {
					ModifyBaseRoom();
				}

				AddRoomDoors(newRoom.transform);
				FillRoom(newRoom.transform);

				string path = Path.Combine(Application.dataPath, DGG_RoomInfo.Instance.savePath);
				path.Replace("\\", "/");
				PrefabUtility.SaveAsPrefabAsset(newRoom, path + _newName + ".prefab");
				DestroyImmediate(newRoom, false);				
			}

			Debug.Log("Generated new room(s).");
		}

		private void SetRoomType(GameObject _obj) {
			System.Type t = System.Type.GetType("Funtools.DungeonGenerator.DGG_RoomType");
			_obj.AddComponent(t);
			_obj.GetComponent<DGG_RoomType>().SetType(DGG_RoomInfo.Instance.roomType);
		}

		private void AddRoomDoors(Transform _room) {
			//not yet inplemented
		}

		//add objects to the room
		private void FillRoom(Transform _room) {
			if(DGG_RoomInfo.Instance.objects == null) {
				return;
			}

			if(DGG_RoomInfo.Instance.objects.Count < 1) {
				return;
			}

			for(int i = 0; i < DGG_RoomInfo.Instance.objects.Count; i++) {
				if(DGG_RoomInfo.Instance.objects[i].objectPrefab != null && DGG_RoomInfo.Instance.objects[i].objectAmount >= 1) {
					for(int _object = 0; _object < DGG_RoomInfo.Instance.objects[i].objectAmount; _object++) {
						Transform _newObject = Instantiate(DGG_RoomInfo.Instance.objects[i].objectPrefab);
						_newObject.parent = _room;
						_newObject.localPosition = RandomPosition(_newObject.localScale, _room);
					}
				}
			}
		}

		private void CreateBaseRoom() {
			float _positionModifier = 1;
			GameObject _newBaseRoom = new GameObject("BaseRoom");
			SetRoomType(_newBaseRoom);

			GameObject _ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
			_ground.name = "Ground";
			_ground.transform.localScale = new Vector3(DGG_RoomInfo.Instance.roomWidth * roomSizeMultiplier, groundHeight, DGG_RoomInfo.Instance.roomLength * roomSizeMultiplier);
			_ground.transform.parent = _newBaseRoom.transform;
			SetMaterial(_ground, DGG_RoomInfo.Instance.groundMaterial);

			for(int i = 0; i < 4; i++) {
				GameObject _wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
				_wall.name = "Wall " + (i + 1);
				_wall.transform.parent = _newBaseRoom.transform;
				SetMaterial(_wall, DGG_RoomInfo.Instance.wallMaterial);
				Repositioning(_wall.transform, i, _positionModifier);
				_positionModifier *= -1;
			}

			newRoom = _newBaseRoom;
		}

		private void ModifyBaseRoom() {
			float _positionModifier = 1;
			GameObject _baseRoomCopy = (GameObject) PrefabUtility.InstantiatePrefab(DGG_RoomInfo.Instance.baseRoom.gameObject);
			SetRoomType(_baseRoomCopy);

			Transform _groundObj = _baseRoomCopy.transform.Find("Ground");
			if(_groundObj) {
				Vector3 _oldGroundSize = _groundObj.localScale;
				_groundObj.localScale = new Vector3(roomSizeMultiplier * DGG_RoomInfo.Instance.roomWidth, groundHeight, roomSizeMultiplier * DGG_RoomInfo.Instance.roomLength);
				SetMaterial(_groundObj.gameObject, DGG_RoomInfo.Instance.groundMaterial);
			}

			for(int i = 0; i < _baseRoomCopy.transform.childCount; i++) {
				Transform _wallObj;

				_wallObj = _baseRoomCopy.transform.Find("Wall");
				if(!_wallObj) {
					_wallObj = _baseRoomCopy.transform.Find("Wall " + i);
				}

				if(_wallObj) {
					_wallObj.name = ("Wall " + i);
					SetMaterial(_wallObj.gameObject, DGG_RoomInfo.Instance.wallMaterial);
					Repositioning(_wallObj, i, _positionModifier);
				}

				_positionModifier *= -1;
			}

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
			Vector3 _size = Vector3.one;
			_size.x = DGG_RoomInfo.Instance.roomWidth;
			_size.y = DGG_RoomInfo.Instance.roomHeight;
			_size.z = DGG_RoomInfo.Instance.roomLength;
			_size *= roomSizeMultiplier;

			if(_counter < 2) {
				_obj.localScale = new Vector3(_size.x, _size.y, wallThickness);
				_obj.localPosition = new Vector3(0, (_size.y + groundHeight) / 2, ((_size.z - wallThickness) / 2) * _positionModifier);
			} else {
				_obj.localScale = new Vector3(wallThickness, _size.y, _size.z);
				_obj.localPosition = new Vector3(((_size.x - wallThickness) / 2) * _positionModifier, (_size.y + groundHeight) / 2, 0);
			}
		}

		//generates a random position in the room for the spawned objects
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
