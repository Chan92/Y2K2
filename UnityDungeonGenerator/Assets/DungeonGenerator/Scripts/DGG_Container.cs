﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Funtools.DungeonGenerator {
	[System.Serializable]
	public struct DGG_ObjectList {
		public int objectAmount;
		public Transform objectPrefab;
	}

	public class DGG_RoomInfo :ScriptableObject {
		static DGG_RoomInfo instance;
		public static DGG_RoomInfo Instance {
			get {
				if(instance == null) {
					instance = new DGG_RoomInfo();
				}
				return instance;
			}
		}

		public string roomNames = "Room";
		public float roomWidth = 1, roomLength = 1, roomHeight = 1;
		public int roomAmount = 1, doorAmount = 1;

		public Material groundMaterial, wallMaterial, ceilingMaterial;
		public Transform baseRoom, doorPrefab;
		public ReorderableList myList;
		private SerializedObject serializedObject;
		public SerializedObject SerializedObject {
			get {
				if(serializedObject == null) {
					serializedObject = new SerializedObject(this);
				}

				return serializedObject;
			} set {
				serializedObject = value;
			}
		}
		public bool isInitialized = false;

		[SerializeField]
		public List<DGG_ObjectList> objects;
		public RoomType roomType;

		public string savePath = "DungeonGenerator/Prefabs/Rooms/";
		public const int minDoors = 1, maxDoors = 4;
	}
}