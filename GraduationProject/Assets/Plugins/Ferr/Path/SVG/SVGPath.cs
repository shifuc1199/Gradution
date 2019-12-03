using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr {
	[Serializable]
	public class SVGPath : ISVGElement {
		public enum PointType {
			Free,
			Locked,
			Auto,
			Sharp
		}

		[Serializable]
		public class Point {
			public Vector2 point;
			public Vector2 controlNext;
			public Vector2 controlPrev;
			public PointType type;
		}

		[SerializeField] List<Point> _points = new List<Point>();
		[SerializeField] bool _closed = false;

		public Rect    Bounds {
			get {
				Rect bounds = new Rect(_points[0].point, Vector2.zero);
				for (int i = 0; i < _points.Count; i++) {
					Vector2 p = _points[i].point;
					if (p.x < bounds.xMin) bounds.xMin = p.x;
					if (p.x > bounds.xMax) bounds.xMax = p.x;
					if (p.y < bounds.yMin) bounds.yMin = p.y;
					if (p.y > bounds.yMax) bounds.yMax = p.y;
				}
				return bounds;
			}
		}
		public Vector2 Start  { get { return _points[0].point; } }
		public Vector2 End    { get { return _closed ? _points[0].point : _points[_points.Count-1].point; } }
		public bool    Closed { get { return _closed; } set { _closed = value; } }
	
		public SVGPath(bool aClosed = false) {
			_closed = aClosed;
		}

		public void Add(Vector2 aPt, PointType aType = PointType.Auto, Vector2 aControlPointPrev = default(Vector2), Vector2 aControlPointNext = default(Vector2)) {
			Point p = new Point();
			p.point       = aPt;
			p.type        = aType;
			p.controlNext = aControlPointNext;
			p.controlPrev = aControlPointPrev;
			_points.Add(p);
		}
		public void AddRange(List<Vector2> aPoints) {
			for (int i = 0; i < aPoints.Count; i++) {
				Add(aPoints[i]);
			}
		}
		public void Clear() {
			_points.Clear();
		}
		public List<Vector2> GetPoints(int aStepsPerUnit) {
			List<Vector2> result = new List<Vector2>();
			if (_points.Count < 2)
				return result;

			int count = _closed ? _points.Count : _points.Count-1;
			for (int i = 0; i < count; i++) {
				Vector2 p1 = _points[i].point;
				Vector2 p2 = _points[WrapI(i+1)].point;
				Vector2 c1 = GetControl(i, true);
				Vector2 c2 = GetControl(WrapI(i+1), false);
				//Gizmos.DrawLine(p1, c1);
				//Gizmos.DrawLine(p2, c2);
				float length = BezierLength(p1, c1, c2, p2);

				int steps = (int)(length * aStepsPerUnit);
				for (int s = 0; s < steps; s++) {
					result.Add(BezierPoint(p1, c1, c2, p2, s/(float)steps));
				}
			}
			result.Add(_points[WrapI(count)].point);
			return result;
		}
		public ISVGElement Reverse() {
			SVGPath result = new SVGPath(_closed);
			result._points = new List<Point>(_points);
			result._points.Reverse();
			for (int i = 0; i < result._points.Count; i++) {
				Vector2 c = result._points[i].controlNext;
				result._points[i].controlNext = result._points[i].controlPrev;
				result._points[i].controlPrev = c;
			}

			return result;
		}
		public List<ISVGElement> RenderDashed(float aStepResolution, AnimationCurve aDashStrength, Vector2 aGapSize, Vector2 aDashSize) {
			List<ISVGElement> result = new List<ISVGElement>();
			SVGLine curr = new SVGLine(false);
			float dist = 0;
			float nextSwitch = 0;
			bool  on = true;
			Vector2 prevPt = _points[0].point;

			int count = _closed ? _points.Count : _points.Count-1;
			for (int i = 0; i < count; i++) {
				Vector2 p1 = _points[i].point;
				Vector2 p2 = _points[WrapI(i+1)].point;
				Vector2 c1 = GetControl(i, true);
				Vector2 c2 = GetControl(WrapI(i+1), false);
				float length = BezierLength(p1, c1, c2, p2);
				float str1 = aDashStrength.Evaluate(i/(count-1f));
				float str2 = aDashStrength.Evaluate(WrapI(i+1)/(count-1f));

				int steps = (int)(length / aStepResolution);
				for (int s = 0; s < steps; s++) {
					float   pct = s/(float)steps;
					float   str = Mathf.Lerp(str1, str2, pct);
					Vector2 pt  = BezierPoint(p1, c1, c2, p2, s/(float)steps);
					float   d   = (pt-prevPt).magnitude;
					dist += d;

					if (dist >= nextSwitch && str < 1) {
						if (on) {
							on = false;
							nextSwitch = dist + Mathf.Lerp(aGapSize.x, aGapSize.y, 1-str);
							curr.Add(pt);
						} else {
							on = true;
							nextSwitch = dist + Mathf.Lerp(aDashSize.x, aDashSize.y, 1-str);
							if (curr.Count > 1)
								result.Add(curr);
							curr = new SVGLine();
						}
					}

					if (on)
						curr.Add(pt);

					prevPt = pt;
				}
			}
			if (curr.Count > 1)
			result.Add(curr);

			return result;
		}

		Vector2 GetControl(int aIndex, bool aNext) {
			Point p = _points[aIndex];
			if (p.type == PointType.Free)
				return p.point + (aNext ? p.controlNext : p.controlPrev);
			else if (p.type == PointType.Locked)
				return p.point + (aNext ? -p.controlPrev : p.controlPrev);
			else if (p.type == PointType.Sharp) {
				p.controlNext = (_points[WrapI(aIndex+1)].point - p.point).normalized;
				p.controlPrev = (_points[WrapI(aIndex-1)].point - p.point).normalized;

				return p.point + (aNext ? p.controlNext : p.controlPrev);
			} else {
				Vector2 n = GetPointNormal(aIndex);
				n = new Vector2(n.y, -n.x);

				float length = (GetSegLength(aIndex) + GetSegLength(aIndex-1)) / 8;
				n = n * length;
				p.controlNext = n;
				p.controlPrev = -n;

				return p.point + (aNext ? p.controlNext : p.controlPrev);
			}
		}

		int WrapI(int i) {
			if (i<0 && _closed)
				i += ((Mathf.Abs(i) / _points.Count) + 1) * _points.Count;
			return _closed ? i % _points.Count : Mathf.Clamp(i, 0, _points.Count-1);
		}
		float GetSegLength(int aIndex) {
			Vector2 p1 = _points[WrapI(aIndex  )].point;
			Vector2 p2 = _points[WrapI(aIndex+1)].point;
			return Vector2.Distance(p1, p2);
		}
		Vector2 GetPointNormal(int aIndex) {
			aIndex = WrapI(aIndex);
			Vector2 n1 = _points[WrapI(aIndex+1)].point - _points[aIndex].point;
			n1 = new Vector2(-n1.y, n1.x);
			Vector2 n2 = _points[aIndex].point - _points[WrapI(aIndex-1)].point;
			n2 = new Vector2(-n2.y, n2.x);

			n1.Normalize();
			n2.Normalize();

			Vector2 n = (n1+n2)/2;
			n.Normalize();

			return n;
		}

		protected Vector2 BezierPoint(Vector2 p1, Vector2 c1, Vector2 c2, Vector2 p2, float aPercent) {
			float t = aPercent;
			float u = 1 - t;
			float tt = t*t;
			float uu = u*u;
			float uuu = uu * u;
			float ttt = tt * t;

			return uuu*p1 + 3*uu*t*c1 + 3*u*tt*c2 + ttt*p2;
		}
		protected float BezierLength(Vector2 p1, Vector2 c1, Vector2 c2, Vector2 p2) {
			/*float lineDist = Vector2.Distance(p1, p2);
			float controlDist = Vector2.Distance(p1, c1) + Vector2.Distance(c1, c2) + Vector2.Distance(c2, p2);

			return (lineDist + controlDist ) / 2f;*/
			Vector2 p = p1;
			float   d = 0;
			for (int i = 1; i < 4; i++) {
				float t = i / (3f);
				Vector2 c = BezierPoint(p1, c1, c2, p2, t);
				d += Vector2.Distance(p, c);
				p = c;
			}
			return d;
		}
		protected float BezierLengthSlow(Vector2 p1, Vector2 c1, Vector2 c2, Vector2 p2, int count=200) {
			Vector2 p = p1;
			float   d = 0;
			for (int i = 1; i < count; i++) {
				float t = i / (count-1f);
				Vector2 c = BezierPoint(p1, c1, c2, p2, t);
				d += Vector2.Distance(p, c);
				p = c;
			}
			return d;
		}

		public string ToSVGElement() {
			Vector2 start = _points[0].point * SVG.Scale;
			int     count = _closed ? _points.Count : _points.Count-1;

			string result = string.Format("<path d='M{0},{1}", start.x, -start.y);
			for (int i = 0; i < count; i++) {
				Vector2 c1 = GetControl(i, true) * SVG.Scale;
				Vector2 c2 = GetControl(WrapI(i+1), false) * SVG.Scale;
				Vector2 p  = _points[WrapI(i+1)].point * SVG.Scale;
				result += string.Format("C{0},{1} {2},{3} {4},{5}", c1.x, -c1.y, c2.x, -c2.y, p.x, -p.y);
			}
			result += "' fill='none' stroke-width='1' stroke='#000'/>";

			return result;
		}
	}
}