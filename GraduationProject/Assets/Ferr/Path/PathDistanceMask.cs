using UnityEngine;
using System.Collections.Generic;

namespace Ferr {
	[System.Serializable]
	public class PathDistanceMask : List<PathDMPoint> {
		int _indexCount;
		public int IndexCount { get { return _indexCount; } }

		#region Constructors
		public PathDistanceMask(List<Vector2> aPath, bool aClosed) : base(aPath.Count + 1) {
			if (aPath.Count <= 0) {
				Add(new PathDMPoint(0, 0, 0));
				return;
			}

			int     count    = aClosed ? aPath.Count : aPath.Count -1;
			float   distance = 0;
			Vector2 prev     = aPath[0];

			_indexCount = aPath.Count;
			for (int i = 0; i < count; ++i) {
				Vector2 p1   = aPath[i];
				float   dist = Vector2.Distance(p1, aPath[(i + 1) % aPath.Count]);
				Add(new PathDMPoint(i, distance, 0));
				distance += dist;
				prev = p1;
			}

			if (aClosed) Add(new PathDMPoint(count - 1,       distance + Vector2.Distance(aPath[0], prev), 1));
			else         Add(new PathDMPoint(aPath.Count - 2, distance, 1));
		}
		public PathDistanceMask(List<Vector3> aPath, bool aClosed) : base(aPath.Count + 1) {
			if (aPath.Count <= 0) {
				Add(new PathDMPoint(0, 0, 0));
				return;
			}

			int     count    = aClosed ? aPath.Count : aPath.Count -1;
			float   distance = 0;
			Vector3 prev     = aPath[0];

			_indexCount = aPath.Count;
			for (int i = 0; i < count; ++i) {
				Vector3 p1   = aPath[i];
				float   dist = Vector3.Distance(p1, aPath[PathUtil.WrapIndex(i + 1, aPath.Count, aClosed)]);
				Add(new PathDMPoint(i, distance, 0));
				distance += dist;
				prev = p1;
			}

			if (aClosed) Add(new PathDMPoint(count - 1,       distance, 1));
			else         Add(new PathDMPoint(aPath.Count - 2, distance, 1));
		}
		public PathDistanceMask(List<Vector2> aPath, List<int> aMapping, bool aClosed) {
			if (aPath.Count <= 0) {
				Add(new PathDMPoint(0, 0, 0));
				return;
			}

			int     count    = aClosed ? aPath.Count : aPath.Count -1;
			float   distance = 0;
			Vector2 prev     = aPath[0];

			_indexCount = aPath.Count;
			int   currId     = 0;
			float idDistance = 0;
			for (int i = 0; i < count; ++i) {
				if (currId < aMapping.Count-1 && i==aMapping[currId+1]) {
					currId += 1;
					idDistance = distance;
				}

				Vector2 p1   = aPath[i];
				float   dist = Vector2.Distance(p1, aPath[(i + 1) % aPath.Count]);
				Add(new PathDMPoint(currId, distance, idDistance));
				distance += dist;
				prev = p1;
			}

			if (aClosed) {
				Add(new PathDMPoint(aMapping.Count-1, distance, 1));
			} else Add(new PathDMPoint(aPath.Count - 2, distance, 1));

			currId = aMapping.Count-1;
			float nextDistance = GetTotalDistance();
			for (int i=Count-2; i>=0; i-=1) {
				if (aMapping[currId] > i) {
					currId -= 1;
					nextDistance = this[i+1].distance;
				}

				var pt = this[i];
				pt.percent = (pt.distance - pt.percent)/(nextDistance - pt.percent);
				this[i] = pt;
			}
		}
		#endregion

		public float GetTotalDistance() {
			int c = Count;
			if (c < 1)
				return 0;
			
			return this[c-1].distance;
		}

		public float DistanceBetweenSmoothIndices(int aSmoothVertStart, int aSmoothVertEnd) {
			if (aSmoothVertEnd>=aSmoothVertStart)
				return this[aSmoothVertEnd].distance-this[aSmoothVertStart].distance;
			else
				return this[aSmoothVertStart].distance + (GetTotalDistance()-this[aSmoothVertStart].distance);
		}
		public float DistanceBetweenDistances(float aStartDistance, float aEndDistance) {
			if (aEndDistance>=aStartDistance)
				return aEndDistance-aStartDistance;
			else
				return aEndDistance + (GetTotalDistance()-aStartDistance);
		}

		public Vector2 GetPointAtPercent(List<Vector2> aSmoothPoints, int aRawIndex, float aPercent) {
			Debug.Log("Untested code");
			int start = GetSmoothIndexAt(aRawIndex);
			int end   = start;
			float startPercent = this[start].percent;
			float endPercent   = this[start].percent;
			for (int i = start+1; i < Count; i++) {
				end = i;
				if (this[i].index != aRawIndex) {
					endPercent = 1;
					break;
				} else if (this[i].percent >= aPercent) {
					endPercent = this[i].percent;
					break;
				}
				startPercent = this[i].percent;
			}

			return Vector2.Lerp(aSmoothPoints[end-1], aSmoothPoints[end], (aPercent-startPercent)/(endPercent-startPercent));
		}
		public Vector2 GetPointAtDistance(List<Vector2> aPoints, float aDistance, bool aClosed) {
			float p     = 0;
			int   index = GetSmoothPointIndexAtDistance(aDistance, out p, aClosed);
			return Vector2.LerpUnclamped(aPoints[index], aPoints[PathUtil.WrapIndex(index+1, aPoints.Count, aClosed)], p);
        }
		public Vector3 GetPointAtDistance(List<Vector3> aPoints, float aDistance, bool aClosed) {
			float p     = 0;
			int   index = GetSmoothPointIndexAtDistance(aDistance, out p, aClosed);
			return Vector3.LerpUnclamped(aPoints[index], aPoints[PathUtil.WrapIndex(index + 1, aPoints.Count, aClosed)], p);
		}
		public Vector2 GetNormAtDistance(List<Vector2> aPath, float aDistance, bool aClosed) {
			float p     = 0;
			int   index = GetSmoothPointIndexAtDistance(aDistance, out p, aClosed);
			//return Vector2.LerpUnclamped(PathUtil.GetPointNormal(index, aPath, aClosed), PathUtil.GetPointNormal(index+1, aPath, aClosed), p).normalized;
			return PathUtil.GetSegmentNormal(index, aPath, aClosed);
        }
		public Vector2 GetNormWeightedAtDistance(List<Vector2> aPath, float aDistance, bool aClosed) {
			float p     = 0;
			int   index = GetSmoothPointIndexAtDistance(aDistance, out p, aClosed);
			return Vector2.LerpUnclamped(PathUtil.GetPointNormalWeighted(index, aPath, aClosed), PathUtil.GetPointNormalWeighted(index+1, aPath, aClosed), p);
        }
		public Vector2 GetTangentAtDistance(List<Vector2> aPath, float aDistance, bool aClosed) {
			float p     = 0;
			int   index = GetSmoothPointIndexAtDistance(aDistance, out p, aClosed);
			//return Vector2.LerpUnclamped(PathUtil.GetPointTangent(index, aPath, aClosed), PathUtil.GetPointTangent(index+1, aPath, aClosed), p);
			return PathUtil.GetSegmentTangent(index, aPath, aClosed);
        }

		public float GetDistanceAtIndex(int aRawIndex) {
			for (int i = 0; i < Count; i++) {
				if (this[i].index == aRawIndex)
					return this[i].distance;
			}
			return GetTotalDistance();
		}
		public int GetSmoothIndexAt(int aRawIndex) {
			for (int i = 0; i < Count; i++) {
				if (this[i].index == aRawIndex)
					return i;
			}
			return -1;
		}
		public int GetRawPointIndexAtDistance(float aDistance, out float aPercent, bool aWrap) {
			float maxDist = GetTotalDistance();
			if (aWrap) {
				aDistance = ((aDistance % maxDist) + maxDist) % maxDist;
			}
			aPercent = 0;
			if (aDistance >= maxDist) {
				aPercent = 1;
				return IndexCount-1;
			}
			if (aDistance <= 0) {
				aDistance = 0;
				return 0;
			}
				
			
            // binary search
			int left  = 0;
			int right = Count - 1;
			while (left != right && left+1 != right) {
				int mid = left + (right-left) / 2;
				
				if (this[mid].distance < aDistance) {
					left = mid;
				} else {
					right = mid;
				}
			}
			
			if (this[left].distance <= aDistance && this[right].distance >= aDistance) {
				float p = (aDistance - this[left].distance) / (this[right].distance - this[left].distance);
				
				aPercent = p;
				return this[left].index;
			} else {
				Debug.LogFormat("Oops {0}/{1}, {2}|{3}", aDistance, GetTotalDistance(), this[left].distance, this[right].distance );
			}
			return -1;
		}
		public int GetSmoothPointIndexAtDistance(float aDistance, out float aPercent, bool aWrap) {
			float maxDist = GetTotalDistance();
			if (aWrap) {
				aDistance = ((aDistance % maxDist) + maxDist) % maxDist;
			}
			aPercent = 0;
			if (aDistance >= maxDist) {
				aPercent = 1;
				return IndexCount-1;
			}
			if (aDistance <= 0) {
				aDistance = 0;
				return 0;
			}
				
			
            // binary search
			int left  = 0;
			int right = Count - 1;
			while (left != right && left+1 != right) {
				int mid = left + (right-left) / 2;
				
				if (this[mid].distance < aDistance) {
					left = mid;
				} else {
					right = mid;
				}
			}
			
			if (this[left].distance <= aDistance && this[right].distance >= aDistance) {
				float p = (aDistance - this[left].distance) / (this[right].distance - this[left].distance);
				
				aPercent = p;
				return left;
			} else {
				Debug.LogFormat("Oops {0}/{1}, {2}|{3}", aDistance, GetTotalDistance(), this[left].distance, this[right].distance );
			}
			return -1;
		}
		public bool IsRawPoint(int aSmoothIndex, bool aClosed) {
			if (aSmoothIndex == 0)
				return true;
			return this[aSmoothIndex].index != this[aSmoothIndex-1].index;
		}
        public PathDMPoint GetDataAtDistance(float aDistance, bool aWrap) {
			float maxDist = GetTotalDistance();
			if (aWrap) {
				aDistance = ((aDistance % maxDist) + maxDist) % maxDist;
			}

            // binary search
            int left  = 0;
            int right = Count - 1;
            while (left != right && left + 1 != right) {
                int mid = left + (right - left) / 2;

                if (this[mid].distance < aDistance) {
                    left = mid;
                } else {
                    right = mid;
                }
            }

            float p = (aDistance - this[left].distance) / (this[right].distance - this[left].distance);
            if (this[left].index == this[right].index) {
                return new PathDMPoint(this[left].index, aDistance, Mathf.Lerp(this[left].percent, this[right].percent, p));
            } else {
                float lP = 1- this[left ].percent;
                float rP = this[right].percent;

                return new PathDMPoint(this[left].index, aDistance, this[left].percent + p * (lP + rP));
            }
        }
	}
}