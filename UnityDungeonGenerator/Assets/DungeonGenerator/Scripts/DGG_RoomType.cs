using UnityEngine;

namespace Funtools.DungeonGenerator {
	public class DGG_RoomType:MonoBehaviour {
		public RoomType roomType;

		public void SetType(RoomType _newType) {
			roomType = _newType;
		}
	}
}
