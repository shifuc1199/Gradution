using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr {
	public static class SVG {
		public const float Scale = 100;

		public static string CreateFile(List<ISVGElement> aPaths, bool aSort, Vector2 aPadding = default(Vector2)) {
			List<ISVGElement> elements = aPaths;
			if (aSort)
				elements = Sort(aPaths);

			Rect r = GetBounds(elements, aPadding);

			string paths = "";
			for (int i = 0; i < elements.Count; i++) {
				paths += "\t"+elements[i].ToSVGElement()+"\n";
			}
			string file = string.Format("<svg xmlns='http://www.w3.org/2000/svg' viewBox='{0} {1} {2} {3}'>\n{4}</svg>",
				r.xMin*Scale, -r.yMax*Scale, r.width*Scale, r.height*Scale, paths);
		
			return file;
		}

		public static List<ISVGElement> Sort(List<ISVGElement> aElements) {
			if (aElements.Count == 0)
				return new List<ISVGElement>();

			List<ISVGElement> result = new List<ISVGElement>(aElements);
			Vector2           end    = Vector2.zero;

			for (int i = 0; i < result.Count; i++) {
				float minDist = float.MaxValue;
				bool  reverse = false;
				int   index = i;

				// find path with an endpoint that is closes to the last path
				for (int e = i; e < result.Count; e++) {
					float d = (end-result[e].Start).sqrMagnitude;
					if (d < minDist) {
						minDist = d;
						reverse = false;
						index   = e;
					}

					d = (end-result[e].End).sqrMagnitude;
					if (d < minDist) {
						minDist = d;
						reverse = true;
						index   = e;
					}
				}
			
				ISVGElement t = result[i];
				result[i] = result[index];
				result[index] = t;

				if (reverse) {
					result[i] = result[i].Reverse();
				}
				end = result[i].End;
			}

			return result;
		}

		public static Rect GetBounds(List<ISVGElement> aPaths, Vector2 aPadding) {
			if (aPaths.Count == 0)
				return new Rect();

			Rect result = aPaths[0].Bounds;
			for (int i = 1; i < aPaths.Count; i++) {
				Rect b = aPaths[i].Bounds;
				if (b.xMin < result.xMin) result.xMin = b.xMin;
				if (b.xMax > result.xMax) result.xMax = b.xMax;
				if (b.yMin < result.yMin) result.yMin = b.yMin;
				if (b.yMax > result.yMax) result.yMax = b.yMax;
			}

			result.xMin -= aPadding.x;
			result.xMax += aPadding.x;

			result.yMin -= aPadding.y;
			result.yMax += aPadding.y;

			return result;
		}

		public static void GizmoDraw(List<ISVGElement> aElements, int aResolution) {
			if (aElements == null)
				return;
			for (int i = 0; i < aElements.Count; i++) {
				List<Vector2> pts = aElements[i].GetPoints(aResolution);
				Ferr.PathUtil.GizmoDraw(pts, 0, aElements[i].Closed);
			}
		}

		public static ISVGElement CreateRect(Rect r) {
			SVGLine line = new SVGLine(true);
			line.Add(new Vector2(r.xMin, r.yMin));
			line.Add(new Vector2(r.xMax, r.yMin));
			line.Add(new Vector2(r.xMax, r.yMax));
			line.Add(new Vector2(r.xMin, r.yMax));
			return line;
		}
		public static ISVGElement CreateCircle(Vector2 aAt, float aRadius) {
			float d = Mathf.Sqrt(aRadius * aRadius) * 1.2f;
			SVGPath path = new SVGPath(true);
			path.Add(aAt + Vector2.left  * aRadius, SVGPath.PointType.Free, new Vector2(0, -d), new Vector2(0,  d));
			path.Add(aAt + Vector2.right * aRadius, SVGPath.PointType.Free, new Vector2(0,  d), new Vector2(0, -d));
			return path;
		}
	}
}