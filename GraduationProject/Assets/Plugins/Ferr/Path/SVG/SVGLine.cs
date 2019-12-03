using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr {
	[Serializable]
	public class SVGLine : ISVGElement {
		[SerializeField] List<Vector2> _points = new List<Vector2>();
		[SerializeField] bool _closed = false;

		public Rect    Bounds {
			get {
				Rect bounds = new Rect(_points[0], Vector2.zero);
				for (int i = 0; i < _points.Count; i++) {
					Vector2 p = _points[i];
					if (p.x < bounds.xMin) bounds.xMin = p.x;
					if (p.x > bounds.xMax) bounds.xMax = p.x;
					if (p.y < bounds.yMin) bounds.yMin = p.y;
					if (p.y > bounds.yMax) bounds.yMax = p.y;
				}
				return bounds;
			}
		}
		public Vector2 Start  { get { return _points[0]; } }
		public Vector2 End    { get { return _closed ? _points[0] : _points[_points.Count-1]; } }
		public bool    Closed { get { return _closed; } set { _closed = value; } }
		public int     Count  { get { return _points.Count; } }

		public SVGLine(bool aClosed = false) {
			_closed = aClosed;
		}
		public SVGLine(List<Vector2> aPoints, bool aClosed = false) {
			_closed = aClosed;
			_points.AddRange(aPoints);
		}

		public void Add(Vector2 aPt) {
			_points.Add(aPt);
		}
		public void Clear() {
			_points.Clear();
		}
		public List<Vector2> GetPoints(int aResolution) {
			return _points;
		}
		public ISVGElement Reverse() {
			SVGLine result = new SVGLine(_closed);
			result._points = new List<Vector2>(_points);
			result._points.Reverse();

			return result;
		}

		int WrapI(int i) {
			if (i<0)
				i += ((Mathf.Abs(i) / _points.Count) + 1) * _points.Count;
			return _closed ? i % _points.Count : Mathf.Clamp(i, 0, _points.Count-1);
		}

		public string ToSVGElement() {
			Vector2 start = _points[0] * SVG.Scale;
			int     count = _points.Count-1;

			string result = string.Format("<path d='M{0},{1}", start.x, -start.y);
			for (int i = 0; i < count; i++) {
				Vector2 p = _points[WrapI(i+1)] * SVG.Scale;
				result += string.Format("L{0},{1}", p.x, -p.y);
			}
			if (_closed)
				result += 'z';
			result += "' fill='none' stroke-width='1' stroke='#000'/>";

			return result;
		}
	}
}