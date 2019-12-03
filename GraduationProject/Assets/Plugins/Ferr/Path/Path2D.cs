using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Ferr {
	public enum PointType {
		Free = 0,
		Locked = 1,
		Auto = 2,
		AutoSymmetrical = 3,
		Sharp = 4,
		CircleCorner = 5
	}
	[Serializable]
	public class PointControl {
		public float     radius=1;
		public Vector2   controlNext;
		public Vector2   controlPrev;
		public PointType type;

		public PointControl(PointType aType, float aRadius, Vector2 aControlPrev, Vector2 aControlNext) {
			type        = aType;
			radius      = aRadius;
			controlNext = aControlNext;
			controlPrev = aControlPrev;
		}
		public PointControl(PointControl aCopy) {
			type        = aCopy.type;
			radius      = aCopy.radius;
			controlNext = aCopy.controlNext;
			controlPrev = aCopy.controlPrev;
		}

		public override string ToString() {
			if (type == PointType.Sharp) {
				return type.ToString();
			} else if (type == PointType.CircleCorner) {
				return string.Format("{0}:{1}", type.ToString(), radius.ToString());
			} else {
				return string.Format("{0}:{1},{2}", type.ToString(), controlPrev.ToString(), controlNext.ToString());
			}
		}
	}

	[System.Serializable]
	public class Path2D : ISVGElement {
		public enum Plane {
			XY,
			XZ
		}

		#region Fields
		[SerializeField] protected List<Vector2>      _points;
		[SerializeField] protected List<PointControl> _pointControls;
        [SerializeField] protected bool               _closed;
		[SerializeField] protected float              _splitDistance = 1;
		
		private List<int>        _mapToSmooth;
		private List<Vector2>    _smoothPoints;
		private PathDistanceMask _distanceMask;
		private bool             _controlsDirty = true;
		#endregion

		#region Overloads
		public Vector2 this[int aIndex] {
			get{return _points[PathUtil.WrapIndex(aIndex, _pointControls.Count, _closed)];}
			set{_points[PathUtil.WrapIndex(aIndex, _pointControls.Count, _closed)] = value; SetDirty();}
		}
		#endregion
		
		#region Properties
		public int     Count  { get { return _points.Count;} }
		public Rect    Bounds { get { return PathUtil.GetBounds(_points);} }
		public Vector2 Start  { get { return _points[0]; } }
		public Vector2 End    { get { return _closed ? _points[0] : _points[_points.Count-1]; } }
		public float   TotalLength { get { return DistanceMask.GetTotalDistance(); } }
		public float SmoothSplitDistance {
			get{return _splitDistance;}
			set{if (_splitDistance != value) {SetDirty();} _splitDistance = value;}
		}
        public bool  Closed {
            get{return _closed;}
	        set{if (_closed != value) {SetDirty();} _closed = value;}
        }

		protected List<Vector2>      SmoothPoints  { get { EnsureSmoothClean  (); return _smoothPoints;  } }
		protected List<PointControl> PointControls { get { EnsureControlsClean(); return _pointControls; } }
		public    PathDistanceMask   DistanceMask  { get { EnsureMaskClean    (); return _distanceMask;  } }
		#endregion

		#region Constructors
		public Path2D() {
			_points = new List<Vector2>();
			_pointControls = new List<PointControl>();
			SetDirty();
		}
		public Path2D(List<Vector2> aPoints) {
			_points = aPoints;
			_pointControls = new List<PointControl>();
			for (int i = 0; i < _points.Count; i++) {
				_pointControls.Add(new PointControl(PointType.Auto, 1, Vector2.zero, Vector2.zero));
			}
			SetDirty();
		}
		public Path2D(Path2D aCopyPath) {
			_points = new List<Vector2>(aCopyPath._points);
			_pointControls = new List<PointControl>(aCopyPath._points.Count);
			for (int i = 0; i < aCopyPath._pointControls.Count; i++) {
				_pointControls.Add(new PointControl( aCopyPath._pointControls[i] ));
			}
            CopySettings(aCopyPath);
		}
        public void CopySettings(Path2D aCopyPath) {
            _closed        = aCopyPath.Closed;
            _splitDistance = aCopyPath.SmoothSplitDistance;
            SetDirty();
        }
		#endregion

		#region Public Interface
		public virtual int Add(Vector2 aPt, PointType aType = PointType.Auto, float aRadius = 1, Vector2 aControlPointPrev = default(Vector2), Vector2 aControlPointNext = default(Vector2)) {
			return Add(aPt, new PointControl(aType, aRadius, aControlPointPrev, aControlPointNext));
		}
		public virtual int Add(Vector2 aPt, PointControl aControls) {
			_points.Add(aPt);
			_pointControls.Add(aControls);
			SetDirty();
			return _points.Count-1;
		}
		public void Add(Path2D aPath) {
			for (int i = 0; i < aPath.Count; i++) {
				PointControl p = aPath.GetControls(i);
				Add(aPath[i], new PointControl(p));
			}
		}
		public virtual void Insert(int aRawIndex, Vector2 aPt, PointType aType = PointType.Auto, float aRadius = 1, Vector2 aControlPointPrev = default(Vector2), Vector2 aControlPointNext = default(Vector2)) {
			Insert(aRawIndex, aPt, new PointControl(aType, aRadius, aControlPointPrev, aControlPointNext));
		}
		public virtual void Insert(int aRawIndex, Vector2 aPt, PointControl aControls) {
			_points       .Insert(aRawIndex, aPt  );
			_pointControls.Insert(aRawIndex, aControls);
			SetDirty();
		}
		public virtual void RemoveAt(int aRawIndex) {
			_points       .RemoveAt(aRawIndex);
			_pointControls.RemoveAt(aRawIndex);
			SetDirty();
		}
		public virtual void Clear() {
			_pointControls.Clear();
			_points.Clear();
			if (_smoothPoints != null)
				_smoothPoints.Clear();
			if (_distanceMask != null)
				_distanceMask.Clear();

			_controlsDirty = true;
		}
		public Vector2 Get(int aRawIndex) {
			return _points[aRawIndex];
		}
		public PointControl GetControls(int aRawIndex) {
			return PointControls[aRawIndex];
		}
		public List<PointControl> GetControls() {
			return PointControls;
		}
		public Vector2 Get(int aRawIndex, float aPercent) {
			return DistanceMask.GetPointAtPercent(SmoothPoints, aRawIndex, aPercent);
		}
        public virtual void Set(int aIndex, Vector2 aValue) {
            _points[aIndex] = aValue;
            SetDirty();
        }
		public float GetInteriorAngle(int aRawIndex) {
			return PathUtil.GetInteriorAngle(aRawIndex, _points, _closed);
		}
		public Vector2 GetNormal(int aIndex, float aPercent=0) {
			if (aPercent == 0)
				return PathUtil.GetPointNormal  (aIndex, _points, _closed);
			else {
				Debug.Log("Not Smoothed");
				return PathUtil.GetSegmentNormal(aIndex, _points, _closed);
			}
		}
		public Vector2 GetSegmentNormal(int aRawIndex) {
			return PathUtil.GetSegmentNormal(aRawIndex, _points, _closed);
		}
		public Vector2 GetTangent(int aRawIndex, float aPercent=0) {
			// TODO: Perhaps add a percentage based non-smoothed tangent?
			return PathUtil.GetPointTangent(aRawIndex, _points, _closed);
		}
		public List<Vector2> GetSubPathDistance(float aStart, float aLength) {
			PathDistanceMask mask = DistanceMask;
			float totalDist = mask.GetTotalDistance();
			float left  = Mathf.Clamp(aStart, 0, totalDist);
			float right = Mathf.Clamp(aStart + aLength, 0, totalDist);
				
			List<Vector2> source = GetFinalPath();
			float p1     = 0;
			int   index1 = mask.GetRawPointIndexAtDistance(left, out p1, _closed);
			float p2     = 0;
			int   index2 = mask.GetRawPointIndexAtDistance(right, out p2, _closed);
			
			List<Vector2> result = new List<Vector2>((index2-index1) + 2);
			if (p1 != 0)
				result.Add(PathUtil.GetBezierPoint(index1, p1, _points, PointControls, Closed));
			
			if (index2 - index1 > 0)
				result.AddRange(source.GetRange(index1 + 1, (index2 - (index1+1))));
			if (p2 != 0)
				result.Add(PathUtil.GetBezierPoint(index2, p2, _points, PointControls, Closed));
			
			return result;
		}
		public int GetClosestSegment(Vector2 aPoint) {
			int smoothSeg = PathUtil.GetClosestSegment(GetFinalPath(), aPoint, Closed);
			return smoothSeg == -1 ? -1 : DistanceMask[smoothSeg].index;
		}
		public int GetClosestControlPoint(Vector2 aPoint) {
			return PathUtil.GetClosestPoint(_points, aPoint);
		}
		public float GetDistanceFromPath(Vector2 aPoint) {
			return PathUtil.GetDistanceFromPath(GetFinalPath(), aPoint, Closed);
		}
		public List<Vector2> GetSubPathRaw(int aStart, int aLength)
		{
			return _points.GetRange(aStart, aLength);
		}
		public List<Vector2> GetPathRaw()
		{
			return _points;
		}
		public List<Vector2> GetPathRawCopy()
		{
			return new List<Vector2>(_points);
		}
		
		public bool Contains(Vector2 aPoint) {
			return PathUtil.IsInPoly(GetFinalPath(), aPoint);
		}
		public bool IsClockwise() {
			return PathUtil.IsClockwise(_points);
		}
        public virtual void ReverseSelf() {
			List<Vector2>      pts        = new List<Vector2>();
			List<PointControl> ptControls = new List<PointControl>();
			for (int i = _points.Count-1; i >= 0; i-=1) {
				PointControl c = _pointControls[i];
				c.controlNext = c.controlPrev;
				c.controlPrev = _pointControls[i].controlNext;

				pts.Add(_points[i]);
				ptControls.Add(c);
			}
			_points        = pts;
			_pointControls = ptControls;

			SetDirty();
		}
		public int GetSegment(Vector2 aClosestTo) {
			return PathUtil.GetClosestSegment(_points, aClosestTo, _closed);
		}
		
		public List<Vector2> GetFinalPath() {
			return SmoothPoints;
		}
		public List<Vector2> GetFinalPathCopy() {
			return new List<Vector2>(GetFinalPath());
		}
        public List<Vector2> GetFinalNormalsCopy() {
            return PathUtil.GetNormals(SmoothPoints, _closed);
        }
        public List<Vector2> GetFinalTangentsCopy() {
			return PathUtil.NormalsToTangents(PathUtil.GetNormals(SmoothPoints, _closed));
        }
		
		public float GetDistanceAt(int aRawIndex) {
			EnsureSmoothClean();
			return DistanceMask[_mapToSmooth[aRawIndex]].distance;
		}
		public float GetDistanceBetween(int aStartIndex, int aEndIndex) {
			float start = GetDistanceAt(aStartIndex);
			float end   = GetDistanceAt(aEndIndex);
			if (end < start) {
				float total = DistanceMask.GetTotalDistance();
				return (end + total) - start;
			}
			return end - start;
		}
		public int GetSegmentAtDistance(float aDist) {
			var data = DistanceMask.GetDataAtDistance(aDist, _closed);
			return data.percent >= 1 ? PathUtil.WrapIndex(data.index+1, _points.Count, _closed) : data.index;
        }
        public Vector2 GetPointAtDistance(float aDist) {
			return DistanceMask.GetPointAtDistance(SmoothPoints, aDist, _closed);
        }
        public Vector2 GetNormalAtDistance(float aDist) {
			return DistanceMask.GetNormAtDistance(SmoothPoints, aDist, _closed);
        }
		public Vector2 GetTangentAtDistance(float aDist) {
			return DistanceMask.GetTangentAtDistance(SmoothPoints, aDist, _closed);
		}
		public void RemoveDuplicates() {
			for (int i = 0; i < _points.Count; i++) {
				for (int s = 0; s < _points.Count; s++) {
					if (i!=s && _points[i] == _points[s]) {
						_points.RemoveAt(s);
						_pointControls.RemoveAt(s);
						s--;
					}
				}
			}
		}
		public bool SelfIntersecting() {
			int count = _closed ? _points.Count : _points.Count-1;

			for (int i = 0; i < _points.Count; i++) {
				for (int s = 0; s < _points.Count; s++) {
					if (i!=s && _points[i] == _points[s]) {
						Debug.Log("Duplicate point");
						return true;
					}
				}
			}

			for (int i = 0; i < count; i++) {
				int next = (i+1)%_points.Count;

				for (int s = 0; s < count; s++) {
					int sNext = (s+1)%_points.Count;
					if (s == i || next == s || sNext == i) continue;
					
					if (PathUtil.LineSegmentIntersection(_points[i], _points[next], _points[s], _points[sNext]))
						return true;
				}
			}

			return false;
		}

		public int GetSmoothIndex(int aRawIndex) {
			if (aRawIndex >= _mapToSmooth.Count)
				return SmoothPoints.Count-1;
			return _mapToSmooth[aRawIndex];
		}

		public Path2D BoolAdd(Path2D aOther) {
			if (!(Closed && aOther.Closed) || SelfIntersecting() || aOther.SelfIntersecting()) {
				if (!Closed || !aOther.Closed)
					Debug.LogWarning("Paths must be closed for boolean operations!");
				if (SelfIntersecting())
					Debug.LogWarning("This path is self-intersecting!");
				if (aOther.SelfIntersecting())
					Debug.LogWarning("Other path is self-intersecting!");
				Path2D r = new Path2D();
				r.Closed = true;
				return r;
			}

			if (!IsClockwise()) ReverseSelf();
			if (!aOther.IsClockwise()) aOther.ReverseSelf();

			// find a point that is not within the other path
			int id = -1;
			for (int i = 0; i < _points.Count; i++) {
				if (!aOther.Contains(_points[i])) {
					id = i;
					break;
				}
			}

			// if it's entirely enclosed, then the other path is the result
			if (id == -1)
				return new Path2D(aOther);

			int           currId = id;
			List<Vector2> currPath = _points;
			List<Vector2> otherPath= aOther._points;
			Vector2       start    = currPath[currId];
			Vector2       curr     = start;
			int           ignoreSegment = -1;
			Path2D        result   = new Path2D();
			result.Closed = true;
			result.Add(start);

			int escape = (currPath.Count + otherPath.Count) * 2;
			while (!(curr == start && result.Count > 1)) {
				int     nextId = (currId+1)%currPath.Count;
				Vector2 next   = currPath[nextId];
				float   dist   = float.MaxValue;
				Vector2 nextPt = next;
				bool    swapPaths = false;

				for (int i = 0; i < otherPath.Count; i++) {
					if (i==ignoreSegment) continue;

					Vector2 o1 = otherPath[i];
					Vector2 o2 = otherPath[(i+1)%otherPath.Count];

					if (PathUtil.LineSegmentIntersection(curr, next, o1, o2)) {
						Vector2 pt  = PathUtil.LineIntersectionPoint(curr, next, o1, o2);
						float   mag = (curr-pt).sqrMagnitude;
						if (mag < dist) {
							dist      = mag;
							nextPt    = pt;
							swapPaths = true;
							nextId    = i;
						}
					}
				}
				ignoreSegment = -1;

				result.Add(nextPt);
				if (swapPaths) {
					List<Vector2> t = currPath;
					currPath = otherPath;
					otherPath  = t;
					ignoreSegment = currId;
				}
				currId = nextId;
				curr   = nextPt;

				escape--;
				if (escape <= 0)
					break;
			}
			return result;
		}
		#endregion
		
		#region Protected Methods
		public virtual void SetDirty() {
			if (_smoothPoints != null)
				_smoothPoints.Clear();
			if (_mapToSmooth != null)
				_mapToSmooth.Clear();
			if (_distanceMask != null)
				_distanceMask.Clear();
			_controlsDirty = true;
		}
		protected virtual void EnsureControlsClean() {
			if (_controlsDirty) {
				UpdateControls();
			}
		}
		protected virtual void EnsureSmoothClean() {
			if (_smoothPoints == null || _smoothPoints.Count <= 0) {
				if (_smoothPoints == null)
					_smoothPoints = new List<Vector2>();
				_smoothPoints.Clear();
				if (_mapToSmooth == null)
					_mapToSmooth = new List<int>();
				_mapToSmooth.Clear();

				List<PointControl> controls = PointControls;
				int count = _closed ? _points.Count : _points.Count -1;
			
				for (int i = 0; i < count; ++i) {
					// If updating this, also update the correct PathDistanceMask constructor
					int next = (i+1) % _points.Count;

					_mapToSmooth.Add(_smoothPoints.Count);

					Vector2 p1 = _points[i];
					Vector2 p2 = _points[next];
					if (controls[i].type == PointType.CircleCorner)
						p1 = PathUtil.GetRoundedCornerEnd(i, _points, controls, _closed, controls[i].radius, false);
					if (controls[next].type == PointType.CircleCorner)
						p2 = PathUtil.GetRoundedCornerEnd(next, _points, controls, _closed, controls[next].radius, true);

					if ((controls[i   ].type == PointType.Sharp) && 
						(controls[next].type == PointType.Sharp || controls[next].type == PointType.CircleCorner) ||
						(!_closed && controls[i].type == PointType.CircleCorner && (i==0 || i==_points.Count-1))) {
						_smoothPoints.Add(_points[i]);
						continue;
					}

					if (controls[i].type == PointType.CircleCorner) {
						int addedPoints = _smoothPoints.Count;
						PathUtil.AddRoundCorners(i, _points, controls, _closed, ref _smoothPoints, controls[i].radius, _splitDistance*0.75f);
						addedPoints = _smoothPoints.Count-addedPoints;
						_mapToSmooth[_mapToSmooth.Count-1] += addedPoints/2;
						if (controls[next].type == PointType.CircleCorner || controls[next].type == PointType.Sharp)
							continue;
					} 
					Vector2 c1 = controls[i].controlNext + p1;
					Vector2 c2 = controls[next].controlPrev + p2;
					float dist   = PathUtil.BezierLength(p1, c1, c2, p2);
					int   slices = Mathf.Max(1,(int)(dist / _splitDistance));

					for (int s = 0; s < slices; ++s) {
						_smoothPoints.Add(PathUtil.BezierPoint(p1, c1, c2, p2, s/(float)slices));
					}
				}
				// if the first corner was a circle curve, we'll need to make some adjustments
				if (controls.Count>0 && controls[0].type == PointType.CircleCorner) {
					int moved = _mapToSmooth[0];
					_smoothPoints.AddRange(_smoothPoints.GetRange(0, moved));
					_smoothPoints.RemoveRange(0, moved);
					for (int i = 0; i < _mapToSmooth.Count; i++) {
						_mapToSmooth[i] -= moved;
					}
				}

				if (!_closed && _points.Count > 0) {
					_mapToSmooth.Add(_smoothPoints.Count);
					_smoothPoints.Add(_points[_points.Count-1]);
				}
			}
		}
		protected virtual void EnsureMaskClean() {
			if (_distanceMask == null || _distanceMask.Count <= 0) {
				_distanceMask = new PathDistanceMask(SmoothPoints, _mapToSmooth, _closed);
			}
		}
		protected void UpdateControls() {
			for (int i = 0; i < _pointControls.Count; i++) {
				PointControl p = _pointControls[i];
				if (p.type == PointType.Free) {
				} else if (p.type == PointType.Locked) {
					p.controlNext = -p.controlPrev;
				} else if (p.type == PointType.Sharp) {
					p.controlNext = Vector2.zero;
					p.controlPrev = Vector2.zero;
				} else if (p.type == PointType.CircleCorner) {
					p.controlNext = Vector2.zero;
					p.controlPrev = Vector2.zero;
				} else if (p.type == PointType.AutoSymmetrical) {
					if (!_closed && (i==0||i==_pointControls.Count-1)) {
						p.controlNext = Vector2.zero;
						p.controlPrev = Vector2.zero;
					} else {
						Vector2 n = PathUtil.GetPointNormal(i, _points, _closed);
						n = new Vector2(-n.y, n.x);

						float length = (PathUtil.GetSegmentLength(i, _points, _closed) + PathUtil.GetSegmentLength(i-1, _points, _closed)) / 4;
						n = n * length;
						p.controlNext = n;
						p.controlPrev = -n;
					}
				} else if (p.type == PointType.Auto) {
					if (!_closed && (i==0||i==_pointControls.Count-1)) {
						p.controlNext = Vector2.zero;
						p.controlPrev = Vector2.zero;
					} else {
						Vector2 n = PathUtil.GetPointNormal(i, _points, _closed);
						n = new Vector2(-n.y, n.x);

						float prevLength = PathUtil.GetSegmentLength(i-1, _points, _closed);
						float nextLength = PathUtil.GetSegmentLength(i, _points, _closed);
						p.controlNext =  (n*nextLength)/3;
						p.controlPrev = -(n*prevLength)/3;
					}
				}
			}
			_controlsDirty = false;
		}
		#endregion

		#region SVG
		public ISVGElement Reverse() {
			Path2D result = new Path2D();
			EnsureControlsClean();
			for (int i = _points.Count-1; i >= 0; i-=1) {
				PointControl c = new PointControl(_pointControls[i]); 
				c.controlNext = c.controlPrev;
				c.controlPrev = _pointControls[i].controlNext;

				result._points       .Add(_points[i]);
				result._pointControls.Add(c);
			}
			result._closed        = _closed;
			result._splitDistance = _splitDistance;

			return result;
		}
		public List<Vector2> GetPoints(int aResolution) {
			return GetFinalPath();
		}
		public string        ToSVGElement() {
			string result = "";
			List<Vector2> points = GetFinalPath();
			Vector2 start  = points[0] * SVG.Scale;
			int     count  = points.Count-1;

			result = string.Format("<path d='M{0},{1}", start.x, -start.y);
			for (int i = 0; i < count; i++) {
				Vector2 p = points[PathUtil.WrapIndex(i+1, points.Count, _closed)] * SVG.Scale;
				result += string.Format("L{0},{1}", p.x, -p.y);
			}
			if (_closed)
				result += 'z';

			result += "' fill='none' stroke-width='1' stroke='#000'/>";
			
			return result;
		}
		#endregion

		#region Debug Utilities
		public void GizmoDraw(float aNormSize = 0.4f, float aTanSize = 0.4f, bool aShowIndex = false) {
            List<Vector2> p = GetFinalPath();
			PathUtil.GizmoDraw(p, 0.04f, _closed);
			PathUtil.GizmoDraw(_points, 0.075f, _closed, false);
			
			if (aShowIndex) {
				for (int i=0; i<_points.Count; i+=1) {
					#if UNITY_EDITOR
					Vector3 labelPos = _points[i] + GetNormal(i);
					UnityEditor.Handles.Label(labelPos, i.ToString());
					#endif
				}
			}
			
			if (aNormSize != 0) {
	            List<Vector2> norms = GetFinalNormalsCopy();
				for (int i = 0; i < p.Count; i++) {
	                Gizmos.DrawRay(p[i], norms[i] * aNormSize);
	            }
			}
			
			if (aTanSize != 0) {
				List<Vector2> tans = GetFinalTangentsCopy();
				for (int i = 0; i < p.Count; i++) {
	                Gizmos.DrawRay(p[i], tans[i] * aTanSize);
	            }
			}
		}
		public void DistanceGizmoDraw(float aNormSize = 0.4f, float aTanSize = 0.4f) {
			if (_points.Count <= 0)
				return;
			
			float dist      = TotalLength;
			int   count     = (int)(dist / SmoothSplitDistance);
			float splitDist = dist / count;
			
			List<Vector2> p = new List<Vector2>();
			for (int i=0; i<=count; i+=1) {
				float d = i * splitDist;
				p.Add(GetPointAtDistance(d));
				Vector2 norm = GetNormalAtDistance (d);
				Vector2 tan  = GetTangentAtDistance(d);
				Gizmos.DrawRay(p[i], norm * aNormSize);
				Gizmos.DrawRay(p[i], tan  * aTanSize );
			}
			
			PathUtil.GizmoDraw(p, 0.04f, _closed);
			PathUtil.GizmoDraw(_points, 0.075f, _closed, false);
		}
		public override string ToString() {
			return string.Format("#{0}, length:{1} {2}", Count, TotalLength, Closed?"(c)":"");
		}
		#endregion

		#region Static Creation Methods
		public static Path2D CreateRect(Rect aRect) {
			Path2D result = new Path2D();
			result.Closed = true;
			result.Add(new Vector2(aRect.xMin, aRect.yMin), PointType.Sharp);
			result.Add(new Vector2(aRect.xMax, aRect.yMin), PointType.Sharp);
			result.Add(new Vector2(aRect.xMax, aRect.yMax), PointType.Sharp);
			result.Add(new Vector2(aRect.xMin, aRect.yMax), PointType.Sharp);
			
			return result;
		}
		public static Path2D CreateStrip(List<Vector2> aPoints, float aWidth) {
			Path2D result = new Path2D();
			result.Closed = true;
			for (int i = 0; i < aPoints.Count; i++) {
				result.Add(aPoints[i] + PathUtil.GetPointNormal(i, aPoints, false) * aWidth);
			}
			for (int i = aPoints.Count-1; i >= 0; i--) {
				result.Add(aPoints[i] - PathUtil.GetPointNormal(i, aPoints, false) * aWidth);
			}

			return result;
		}
		#endregion
	}
	
    [System.Serializable]
	public class Path2D<T> : Path2D {
		
		#region Fields
        [SerializeField] protected List<T> _data;
		#endregion
		
		#region Constructors
		public Path2D() : base() {
			_data = new List<T>();
		}
		public Path2D(List<Vector2> aPoints) : base(aPoints){
			_data = new List<T>(aPoints.Count);
			for (int i=0; i<aPoints.Count; i+=1) {
				_data.Add(default(T));
			}
		}
		public Path2D(List<Vector2> aPoints, List<T> aData) : base(aPoints){
			_data = aData;
		}
		public Path2D(Path2D<T> aOther) {
			this._closed         = aOther._closed;
			this._data           = new List<T>(aOther._data);
			this._points         = new List<Vector2>(aOther._points);
			this._pointControls  = new List<PointControl>(aOther._pointControls);
			this._splitDistance  = aOther._splitDistance;
		}
		#endregion
		
		#region Public Interface
		public          int  Add(Vector2 aPt, T aData, PointControl aControl) {
			_data.Add(aData);
			return base.Add(aPt, new PointControl(aControl));
		}
		public          int  Add(Vector2 aPt, T aData, PointType aType = PointType.Auto, float aRadius = 1, Vector2 aControlPointPrev = default(Vector2), Vector2 aControlPointNext = default(Vector2)) {
			_data.Add(aData);
			return base.Add(aPt, aType, aRadius, aControlPointPrev, aControlPointNext);
		}
        public override int  Add(Vector2 aPt, PointType aType = PointType.Auto, float aRadius = 1, Vector2 aControlPointPrev = default(Vector2), Vector2 aControlPointNext = default(Vector2)) {
            _data.Add(Activator.CreateInstance<T>());
            return base.Add(aPt, aType, aRadius, aControlPointPrev, aControlPointNext);
        }
        public          void Insert  (int aIndex, Vector2 aPt, T aData, PointControl aControl) {
            _data.Insert(aIndex, aData);
            base .Insert(aIndex, aPt, aControl);
		}
		public          void Insert  (int aIndex, Vector2 aPt, T aData, PointType aType = PointType.Auto, float aRadius = 1, Vector2 aControlPointPrev = default(Vector2), Vector2 aControlPointNext = default(Vector2)) {
            _data.Insert(aIndex, aData);
            base .Insert(aIndex, aPt, aType, aRadius, aControlPointPrev, aControlPointNext);
		}
        public override void Insert  (int aIndex, Vector2 aPt, PointType aType = PointType.Auto, float aRadius = 1, Vector2 aControlPointPrev = default(Vector2), Vector2 aControlPointNext = default(Vector2)) {
	        _data.Insert(aIndex, Activator.CreateInstance<T>());
            base .Insert(aIndex, aPt, aType, aRadius, aControlPointPrev, aControlPointNext);
        }
        public override void RemoveAt(int aIndex) {
			_data.RemoveAt(aIndex);
            base .RemoveAt(aIndex);
        }
		public override void Clear() {
			_data.Clear();
			base.Clear();
		}
		public          void SetData (int aIndex, T aData){
			_data[aIndex] = aData;
			SetDirty();
		}
        public          T    GetData (int aRawIndex) {
            return _data[aRawIndex];
        }
		public          T    GetData (int aRawIndex, float aPercent) {
			if      (aPercent == 0) return _data[aRawIndex];
			else if (aPercent == 1) return _data[PathUtil.WrapIndex(aRawIndex+1, Count, _closed)];

			var start = (ILerpable<T>)_data[aRawIndex];
            return start.Lerp(_data[PathUtil.WrapIndex(aRawIndex+1, Count, _closed)], aPercent);
        }
		public          T    GetDataAtDistance (float aDist) {
			if (typeof(ILerpable<T>).IsAssignableFrom(typeof(T))) {
				PathDMPoint pt   = DistanceMask.GetDataAtDistance( aDist, _closed );
				int         next = PathUtil.WrapIndex(pt.index+1, Count, _closed);
				return ((ILerpable<T>)_data[pt.index]).Lerp(_data[next], pt.percent);
			} else {
				return _data[DistanceMask.GetDataAtDistance(aDist, _closed).index];
			}
		}
        public          List<T> GetData    () {
            return _data;
        }
        public          List<T> GetDataCopy() {
            return new List<T>(_data);
        }
		public          void    GetSubPathRaw(int aStart, int aLength, out List<Vector2> aPath, out List<T> aData) {
			aPath = _points.GetRange(aStart, aLength);
			aData = _data  .GetRange(aStart, aLength);
		}
		public          Path2D<T> GetSubPath(int aStart, int aLength) {
			int extra  = (aStart + aLength) - _points.Count;
			int length = extra > 0 ? aLength - extra: aLength;
			
			List<Vector2> newPoints = _points.GetRange(aStart, length);
			List<T>       newData   = _data  .GetRange(aStart, length);
			for (int i=0; i<extra; i+=1) {
				newPoints.Add(_points[i]);
				newData  .Add(_data[i]);
			}
			
			Path2D<T> result = new Path2D<T>(newPoints, newData);
			result._splitDistance = _splitDistance;
			return result;
		}
		public override void ReverseSelf() {
			PathUtil.Reverse<T>(ref _data);
			base.ReverseSelf();
		}
		/*public List<Path2D<T>> SplitPath(List<PathSplitCriteria> aSplitCriteria) {
			List<int> overrides = null;
			if (typeof(IEdgeOverride).IsAssignableFrom(typeof(T))) {
				overrides = new List<int>(_data.Count);
				for (int i=0; i<_data.Count; i+=1) {
					overrides.Add(((IEdgeOverride)_data[i]).EdgeOverride);
				}
			}
			List<PathSplitter.SplitInfo> splits = PathSplitter.Split(GetPathRaw(), overrides, _closed, aSplitCriteria);
			List<Path2D<T>>              result = new List<Path2D<T>>();
			
			if (splits.Count == 1) {
				result.Add(this);
				return result;
			}
			
			int start = 0;
			int count = splits.Count-1;
			int last  = splits.Count-1;
			
			if (_closed && splits[0].edgeID == splits[last].edgeID) {
				start  = 1;
				result.Add(GetSubPath(splits[last].start, splits[last].length + splits[0].length - 1));
			}
			for (int i=start; i<count; i+=1) {
				result.Add(GetSubPath(splits[i].start, splits[i].length));
			}
			return result;
		}*/
		#endregion
    }
}