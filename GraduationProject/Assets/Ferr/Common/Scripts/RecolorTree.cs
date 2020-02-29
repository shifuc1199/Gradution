using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ferr {
	public class RecolorTree {
		class SortX : IComparer<TreePoint> { public int Compare(TreePoint a, TreePoint b) { return a.point.x.CompareTo(b.point.x); } }
		class SortY : IComparer<TreePoint> { public int Compare(TreePoint a, TreePoint b) { return a.point.y.CompareTo(b.point.y); } }
		class SortZ : IComparer<TreePoint> { public int Compare(TreePoint a, TreePoint b) { return a.point.z.CompareTo(b.point.z); } }
		static SortX sortX = new SortX();
		static SortY sortY = new SortY();
		static SortZ sortZ = new SortZ();
		
		public class TreeSettings {
			int[] axes;
			
			public TreeSettings(bool aUseX, bool aUseY, bool aUseZ) {
				List<int> tAxes = new List<int>(3);
				if (aUseX) tAxes.Add(0);
				if (aUseY) tAxes.Add(1);
				if (aUseZ) tAxes.Add(2);
				axes = tAxes.ToArray();
			}
			public int GetAxis(int aDepth) {
				int axis = aDepth % axes.Length;
				return axes[axis];
			}
			
			public float AxisDist(int aAxis, Vector3 a, Vector3 b) {
				if (aAxis == 0) return Mathf.Abs(a.x - b.x);
				if (aAxis == 1) return Mathf.Abs(a.y - b.y);
				if (aAxis == 2) return Mathf.Abs(a.z - b.z);
				return 0;
			}
		}
		public class TreePoint {
			public Vector3 point;
			public Color   data;
			
			public TreePoint(Vector3 aPoint, Color aData) {
				point = aPoint;
				data  = aData;
			}
		}
		class TreeNode {
			TreePoint point;
			
			TreeNode left;
			TreeNode right;
			
			public bool IsLeaf {get {return left == null && right == null;}}
			
			public TreeNode(TreeSettings aSettings, List<TreePoint> aPoints, int aDepth) {
				int axis = aSettings.GetAxis(aDepth);
				if      (axis == 0) aPoints.Sort(sortX);
				else if (axis == 1) aPoints.Sort(sortY);
				else if (axis == 2) aPoints.Sort(sortZ);
				
				int median = aPoints.Count/2;
				point = aPoints[median];
				
				List<TreePoint> leftList  = aPoints.GetRange(0, median);
				List<TreePoint> rightList = aPoints.GetRange(median+1, aPoints.Count - (median+1));
				
				if (leftList .Count > 0) left  = new TreeNode(aSettings, leftList,  aDepth + 1);
				if (rightList.Count > 0) right = new TreeNode(aSettings, rightList, aDepth + 1);
			}
			
			public void GetNearest(TreeSettings aSettings, int aDepth, Vector3 aPt, ref TreePoint aClosest, ref float aClosestDist) {
				if (IsLeaf) { 
					float dist = (point.point-aPt).sqrMagnitude;
					if (aClosest == null || dist < aClosestDist) {
						aClosest     = point;
						aClosestDist = dist;
					}
					return;
				}
				
				int  axis   = aSettings.GetAxis(aDepth);
				bool goLeft = false;
				if      (axis == 0) goLeft = aPt.x <= point.point.x ? true : false;
				else if (axis == 1) goLeft = aPt.y <= point.point.y ? true : false;
				else if (axis == 2) goLeft = aPt.z <= point.point.z ? true : false;
				
				TreeNode first = goLeft ? left : right;
				TreeNode other = goLeft ? right: left;
				if (first == null) {
					first = other;
					other = null;
				}
				
				first.GetNearest(aSettings, aDepth + 1, aPt, ref aClosest, ref aClosestDist);
				
				float thisDist = (point.point-aPt).sqrMagnitude;
				if (thisDist < aClosestDist) {
					aClosest     = point;
					aClosestDist = thisDist;
				}
				
				if (other != null) {
					float axisDist = aSettings.AxisDist(axis, point.point, aPt);
					if (axisDist*axisDist <= aClosestDist) other.GetNearest(aSettings, aDepth+1, aPt, ref aClosest, ref aClosestDist);
				}
			}
			
			public void Draw(TreeSettings aSettings, int aDepth, Vector3 aPt) {
				int axis = aSettings.GetAxis(aDepth);
				
				if (axis == 0) {
					Gizmos.color = Color.red;
					Gizmos.DrawLine(point.point + Vector3.left, point.point + Vector3.right);
				} else if ( axis == 1) {
					Gizmos.color = Color.green;
					Gizmos.DrawLine(point.point + Vector3.up, point.point + Vector3.down);
				} else if ( axis == 2) {
					Gizmos.color = Color.blue;
					Gizmos.DrawLine(point.point + Vector3.forward, point.point + Vector3.back);
				}
				
				if (left  != null) {
					left .Draw(aSettings, aDepth+1, point.point);
					Gizmos.color = point.data;
					Gizmos.DrawLine(point.point, left.point.point);
				}
				if (right != null) {
					right.Draw(aSettings, aDepth+1, point.point);
					Gizmos.color = point.data;
					Gizmos.DrawLine(point.point, right.point.point);
				}
			}
		}
		
		TreeNode     root;
		TreeSettings settings;
		
		public RecolorTree(Mesh aMesh, Matrix4x4? aTransform = null, bool aX = true, bool aY = true, bool aZ = true) {
			if (aMesh == null) {
				Create(new Vector3[] { Vector3.zero }, new Color[] { Color.white }, aTransform, aX, aY, aZ);
				return;
			}

			Vector3[] points = aMesh.vertices;
			Color  [] cols   = aMesh.colors;
			if (cols == null || cols.Length == 0) {
				cols = new Color[points.Length];
				for (int i = 0; i < cols.Length; ++i) cols[i] = Color.white;
			}
			
			Create(points, cols, aTransform, aX, aY, aZ);
		}
		public RecolorTree(Vector3[] aPoints, Color[] aColors, Matrix4x4? aTransform = null, bool aX = true, bool aY = true, bool aZ = true) {
			Create(aPoints, aColors, aTransform, aX, aY, aZ);
		}
		public RecolorTree(List<Vector3> aPoints, List<Color> aColors, Matrix4x4? aTransform = null, bool aX = true, bool aY = true, bool aZ = true) {
			Create(aPoints, aColors, aTransform, aX, aY, aZ);
		}
		public TreePoint Get(Vector3 aAt) {
			TreePoint pt   = null;
			float     dist = 0;
			root.GetNearest(settings, 0, aAt, ref pt, ref dist);
			return pt;
		}
		public void Recolor(ref Mesh aMesh, Matrix4x4? aTransform = null) {
			if (aMesh == null)
				return;

			Vector3[] points = aMesh.vertices;
			aMesh.colors = Recolor(points, aTransform);
		}
		public void Recolor(Vector3[] aPoints, ref Color[] aColors, Matrix4x4? aTransform = null) {
			if (aPoints.Length != aColors.Length) Debug.LogError("Arguments must be the same length!");
			
			if (aTransform.HasValue) for (int i = 0; i < aPoints.Length; ++i) aColors[i] = Get(aTransform.Value.MultiplyPoint(aPoints[i])).data;
			else                     for (int i = 0; i < aPoints.Length; ++i) aColors[i] = Get(aPoints[i]).data;
		}
		public Color[] Recolor(Vector3[] aAt, Matrix4x4? aTransform = null) {
			Color[] cols = new Color[aAt.Length];

			if (aTransform.HasValue) for (int i = 0; i < aAt.Length; ++i) cols[i] = Get(aTransform.Value.MultiplyPoint(aAt[i])).data;
			else                     for (int i = 0; i < aAt.Length; ++i) cols[i] = Get(aAt[i]).data;
			
			return cols;
		}
		public List<Color> Recolor(List<Vector3> aAt, Matrix4x4? aTransform = null) {
			List<Color> cols = new List<Color>(aAt.Count);

			if (aTransform.HasValue) for (int i = 0; i < aAt.Count; ++i) cols[i] = Get(aTransform.Value.MultiplyPoint(aAt[i])).data;
			else                     for (int i = 0; i < aAt.Count; ++i) cols[i] = Get(aAt[i]).data;
			
			return cols;
		}
		public void Recolor(List<Vector3> aPoints, ref List<Color> aColors, Matrix4x4? aTransform = null) {
			if (aTransform.HasValue) for (int i = 0; i < aPoints.Count; ++i) aColors.Add( Get(aTransform.Value.MultiplyPoint(aPoints[i])).data );
			else                     for (int i = 0; i < aPoints.Count; ++i) aColors.Add( Get(aPoints[i]).data );
		}

		public void DrawTree () {
			TreeSettings settings = new TreeSettings(true, true, true);
			root.Draw(settings, 0, Vector3.zero);
		}
		
		void Create(Vector3[] aPoints, Color[] aColors, Matrix4x4? aTransform, bool aX, bool aY, bool aZ) {
			if (aPoints.Length != aColors.Length) Debug.LogError("Arguments must be the same length!");
			List<TreePoint> points = new List<TreePoint>(aPoints.Length);
			
			for (int i = 0; i < aPoints.Length; ++i) {
				Vector3 pt = aPoints[i];
				if (aTransform.HasValue)
					pt = aTransform.Value.MultiplyPoint(pt);
				points.Add(new TreePoint(pt, aColors[i]));
			}
			settings = new TreeSettings(aX, aY, aZ);
			root     = new TreeNode    (settings, points, 0);
		}
		void Create(List<Vector3> aPoints, List<Color> aColors, Matrix4x4? aTransform, bool aX, bool aY, bool aZ) {
			if (aPoints.Count != aColors.Count) Debug.LogError("Arguments must be the same length!");
			List<TreePoint> points = new List<TreePoint>(aPoints.Count);
			
			for (int i = 0; i < aPoints.Count; ++i) {
				Vector3 pt = aPoints[i];
				if (aTransform.HasValue)
					pt = aTransform.Value.MultiplyPoint(pt);
				points.Add(new TreePoint(pt, aColors[i]));
			}
			settings = new TreeSettings(aX, aY, aZ);
			root     = new TreeNode    (settings, points, 0);
		}
	}
}
