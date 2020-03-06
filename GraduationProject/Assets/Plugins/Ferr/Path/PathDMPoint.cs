using UnityEngine;
using System.Collections;

namespace Ferr {
	[System.Serializable]
	public struct PathDMPoint {
		public float distance;
		public float percent;
		public int   index;
		
		public PathDMPoint(int aIndex, float aDistance, float aPercent) {
			distance = aDistance;
			percent  = aPercent;
			index    = aIndex;
		}
	}
}