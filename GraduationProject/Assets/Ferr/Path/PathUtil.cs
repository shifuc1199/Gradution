using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ferr {
	public static class PathUtil {
		#region Helping Structs and Flags
		public enum ConvertOptions {
            XY,
            XZ
        }
		public enum PathSide {
			Inside,
			Outside,
			Intersect
		}
		[System.Flags] enum OutCode { 
			Inside = 0, 
			Left = 1, 
			Right = 2, 
			Bottom = 4, 
			Top = 8 
		}
		public struct Intersection { 
			public int     segmentIndex;
			public Vector2 point;
		}
		#endregion

		#region Path Info
		/// <summary>
		/// Checks if a closed polygon's points are in clockwise order.
		/// </summary>
		/// <param name="aPoly">A closed polygon.</param>
		/// <returns>True if clockwise, false if counter-clockwise.</returns>
		public static bool    IsClockwise(List<Vector2> aPoly) {
			float sum = 0;
			for (int i = 0; i < aPoly.Count; ++i) {
				int next = (i + 1)%aPoly.Count;
				sum += (aPoly[next].x - aPoly[i].x) * (aPoly[next].y + aPoly[i].y);
			}
			return sum > 0;
		}
		/// <summary>
		/// Sorts a cloud of points into a clockwise order. The center of the cloud is determined by averaging the points.
		/// </summary>
		/// <param name="aPoints">A list of points that will be modified into a clockwise order around the averaged center.</param>
		public static void    SortPointsClockwise(ref List<Vector2> aPoints) {
			SortPointsClockwise(ref aPoints, Average(aPoints));
		}
		/// <summary>
		/// Sorts a cloud of points into a clockwise order around the provided center.
		/// </summary>
		/// <param name="aCenter">The center point of the circle, with which to sort the points around.</param>
		/// <param name="aPoints">A list of points that will be modified into a clockwise order around the provided center.</param>
		public static void    SortPointsClockwise(ref List<Vector2> aPoints, Vector2 aCenter) {
			// TODO: Slow, can be improved
			aPoints.Sort((a, b) => {
				return Mathf.Atan2(b.y-aCenter.y, b.x-aCenter.x).CompareTo(Mathf.Atan2(a.y-aCenter.y, a.x-aCenter.x));
			});
		}
		/// <summary>
		/// Finds the distance along the path from the start to the end, including the closing segment if marked as closed.
		/// </summary>
		/// <param name="aPath">The path to find the length of.</param>
		/// <param name="aClosed">Is the path closed? Should the closing segment also be considered?</param>
		/// <returns>Total length of the given path.</returns>
		public static float   GetLength(List<Vector3> aPath, bool aClosed) {
			int   count  = aClosed ? aPath.Count : aPath.Count - 1;
			float length = 0;

			for (int i = 0; i < count; ++i) {
				int next = i % aPath.Count;
				length += Vector3.Distance(aPath[i], aPath[next]);
			}

			return length;
		}
		/// <summary>
		/// Finds the distance along the path from the start to the end, including the closing segment if marked as closed.
		/// </summary>
		/// <param name="aPath">The path to find the length of.</param>
		/// <param name="aClosed">Is the path closed? Should the closing segment also be considered?</param>
		/// <returns>Total length of the given path.</returns>
		public static float   GetLength(List<Vector2> aPath, bool aClosed) {
			int   count  = aClosed?aPath.Count:aPath.Count-1;
			float length = 0;
			
			for (int i = 0; i < count; ++i) {
				int next = i % aPath.Count;
				length += Vector2.Distance(aPath[i], aPath[next]);
			}
			
			return length;
		}
		/// <summary>
		/// Finds a 2D rectangle that encloses the entire list of points.
		/// </summary>
		/// <param name="aPoints">List of points to enclose.</param>
		/// <param name="aPadding">This padding is added to the final rectangle. Positive values will inflate size, negative values will shrink size. xMin-=x, yMax+=y, xMax+=z, yMin-=w</param>
		/// <returns>A rectangle enclosing the list of points, modified by the padding.</returns>
		public static Rect    GetBounds(List<Vector2> aPoints, Vector4 aPadding = default(Vector4)) {
			if (aPoints == null || aPoints.Count <= 0) return new Rect(0, 0, 0, 0);

			float xMin = float.MaxValue;
			float xMax = float.MinValue;
			float yMax = float.MinValue;
			float yMin = float.MaxValue;
			
			for (int i = 0; i < aPoints.Count; i++) {
				if (aPoints[i].x > xMax) xMax = aPoints[i].x;
				if (aPoints[i].x < xMin) xMin = aPoints[i].x;
				if (aPoints[i].y < yMin) yMin = aPoints[i].y;
				if (aPoints[i].y > yMax) yMax = aPoints[i].y;
			}

			xMin-=aPadding.x;
			xMax+=aPadding.z;
			yMax+=aPadding.y;
			yMin-=aPadding.w;
			return new Rect(xMin, yMin, xMax-xMin, yMax-yMin);
		}
		/// <summary>
		/// Finds the average of the points.
		/// </summary>
		/// <param name="aPoints">A list of points.</param>
		/// <returns>Average of all the points.</returns>
		public static Vector2 Average  (List<Vector2> aPoints) {
			Vector2 total = Vector2.zero;
			for (int i = 0; i < aPoints.Count; ++i) {
				total += aPoints[i] / aPoints.Count;
			}
			return total;
		}
		#endregion

		#region Normals and Tangents
		/// <summary>
		/// Finds the interior angle (degrees) of the corner specified by 'i'. Endpoints on unclosed lines are 180. Path should be counterclockwise.
		/// </summary>
		/// <param name="i">Index of the corner to calculate the angle of</param>
		/// <param name="aPath">The path of points, sorted counterclockwise.</param>
		/// <param name="aClosed">Is the path closed?</param>
		/// <returns>The interior angle, in degrees.</returns>
		public static float GetInteriorAngle(int i, List<Vector2> aPath, bool aClosed) {
            if (aPath.Count < 2) return 180;

			if (!aClosed && (i == 0 || i == aPath.Count-1))
				return 180;
			
            Vector2 curr = aPath[WrapIndex(i,   aPath.Count, aClosed)];
            Vector2 prev = aPath[WrapIndex(i-1, aPath.Count, aClosed)] - curr;
            Vector2 next = aPath[WrapIndex(i+1, aPath.Count, aClosed)] - curr;

            prev.Normalize();
            next.Normalize();

            return ClockwiseAngle(prev, next);
        }
		/// <summary>
		/// Gets the normal at the specified path index. Doesn't account for segment lengths.
		/// </summary>
		/// <param name="aPath">The list of vertices used to calculate the normal.</param>
		/// <param name="i">Index of the vertex to get the normal of.</param>
		/// <param name="aClosed">Should we interpolate at the edges of the path as though it was closed?</param>
		/// <returns>A normalized normal!</returns>
		public static Vector2 GetPointNormal(int i, List<Vector2> aPath, bool aClosed) {
            if (aPath.Count < 2) return Vector2.up;
			if (aPath.Count == 2 || (!aClosed && (i == 0 || i == aPath.Count-1)))
				return GetSegmentNormal(i, aPath, aClosed);
			
            Vector2 curr = aPath[WrapIndex(i,   aPath.Count, aClosed)];
            Vector2 prev = aPath[WrapIndex(i-1, aPath.Count, aClosed)] - curr;
            Vector2 next = aPath[WrapIndex(i+1, aPath.Count, aClosed)] - curr;

            prev.Normalize();
            next.Normalize();

	        prev = new Vector2(-prev.y,  prev.x);
	        next = new Vector2( next.y, -next.x);

            Vector2 norm = (prev + next) / 2;
            norm.Normalize();

            return norm;
        }
		/// <summary>
		/// Gets the normal at the specified path index, taking into account weighting based on segment length.
		/// </summary>
		/// <param name="aPath">The list of vertices used to calculate the normal.</param>
		/// <param name="i">Index of the vertex to get the normal of.</param>
		/// <param name="aClosed">Should we interpolate at the edges of the path as though it was closed?</param>
		/// <returns>A normalized normal!</returns>
		public static Vector2 GetPointNormalWeighted(int i, List<Vector2> aPath, bool aClosed) {
			if (aPath.Count < 2) return Vector2.up;

			if (!aClosed && (i == 0 || i == aPath.Count - 1))
				return GetSegmentNormal(i, aPath, aClosed);

			Vector2 curr = aPath[WrapIndex(i, aPath.Count, aClosed)];
			Vector2 prev = aPath[WrapIndex(i - 1, aPath.Count, aClosed)] - curr;
			Vector2 next = aPath[WrapIndex(i + 1, aPath.Count, aClosed)] - curr;

			float prevLength = prev.magnitude;
			float nextLength = next.magnitude;
			float prevWeight = prevLength / (prevLength + nextLength);
			float nextWeight = nextLength / (prevLength + nextLength);

			// normalize
			prev = prev / prevLength;
			next = next / nextLength;

			prev = new Vector2(-prev.y, prev.x);
			next = new Vector2(next.y, -next.x);

			Vector2 norm = prev*prevWeight + next*nextWeight;
			norm.Normalize();

			return norm;
		}
		/// <summary>
		/// Gets the normal at the specified path index.
		/// </summary>
		/// <param name="aPath">The list of vertices used to calculate the normal.</param>
		/// <param name="i">Index of the vertex to get the normal of.</param>
		/// <param name="aClosed">Should we interpolate at the edges of the path as though it was closed?</param>
		/// <returns>A normalized normal!</returns>
		public static Vector3 GetPointNormal3D(int i, List<Vector3> aPath, bool aClosed) {
            if (aPath.Count < 2) return Vector3.up;

			//if (!aClosed && (i == 0 || i == aPath.Count-1))
			//	return GetSegmentNormal3D(i, aPath, aClosed);
			
            Vector3 curr = aPath[WrapIndex(i,   aPath.Count, aClosed)];
            Vector3 prev = curr - aPath[WrapIndex(i-1, aPath.Count, aClosed)];
            Vector3 next = aPath[WrapIndex(i+1, aPath.Count, aClosed)] - curr;

            prev.Normalize();
            next.Normalize();

			Vector3 tan = prev + next;
			tan.Normalize();
			
			Vector3 cross = Vector3.Cross(tan, prev);
			Gizmos.DrawLine(curr, curr + tan);
			Gizmos.DrawLine(curr, curr + prev);
			Vector3 norm  = Vector3.Cross(cross, tan);
			norm.Normalize();

            return norm;
        }
		/// <summary>
        /// Gets the tangent at the specified path index.
        /// </summary>
        /// <param name="aPath">The list of vertices used to calculate the tanget.</param>
        /// <param name="i">Index of the vertex to get the tangent of.</param>
        /// <param name="aClosed">Should we interpolate at the edges of the path as though it was closed?</param>
        /// <returns>A normalized tangent!</returns>
		public static Vector2 GetPointTangent(int i, List<Vector2> aPath, bool aClosed) {
			Vector2 norm = GetPointNormal(i, aPath, aClosed);
			return new Vector2(norm.y, -norm.x);
		}
		/// <summary>
        /// Gets the tangent at the specified segment index.
        /// </summary>
        /// <param name="aPath">The list of vertices used to calculate the tanget.</param>
        /// <param name="i">Index of the segment to get the tangent of.</param>
        /// <param name="aClosed">Should we check the closing edge, or clamp?</param>
        /// <returns>A normalized tangent!</returns>
		public static Vector2 GetSegmentTangent(int i, List<Vector2> aPath, bool aClosed) {
			Vector2 norm = GetSegmentNormal(i, aPath, aClosed);
			return new Vector2(norm.y, -norm.x);
		}
        /// <summary>
        /// Gets the normal at the specified path segment.
        /// </summary>
        /// <param name="aPath">The list of vertices used to calculate the normal.</param>
        /// <param name="i">Index of the first vert of the segment.</param>
        /// <param name="aClosed">Should we wrap around as though it was closed?</param>
        /// <returns>A normalized normal!</returns>
        public static Vector2 GetSegmentNormal(int i, List<Vector2> aPath, bool aClosed) {
			if (aPath.Count < 2) return Vector2.up;
			Vector2 dir;
			if (!aClosed && i == aPath.Count - 1) {
				dir = aPath[WrapIndex(i, aPath.Count, aClosed)] - aPath[WrapIndex(i - 1, aPath.Count, aClosed)];
			} else {
				dir = aPath[WrapIndex(i + 1, aPath.Count, aClosed)] - aPath[WrapIndex(i, aPath.Count, aClosed)];
			}
			return new Vector2( dir.y, -dir.x).normalized;
		}
		public static List<Vector2> GetNormals        (List<Vector2> aPath, bool aClosed) {
            List<Vector2> result = new List<Vector2>(aPath.Count);

            for (int i = 0; i < aPath.Count; ++i) {
                result.Add(GetPointNormal(i, aPath, aClosed));
            }

            return result;
        }
		public static List<Vector2> GetSegmentNormals (List<Vector2> aPath, bool aClosed) {
            List<Vector2> result = new List<Vector2>(aPath.Count);

            for (int i = 0; i < aPath.Count; ++i) {
                result.Add(GetSegmentNormal(i, aPath, aClosed));
            }

            return result;
        }
		public static List<Vector2> GetNormalsWeighted(List<Vector2> aPath, bool aClosed) {
			List<Vector2> result = new List<Vector2>(aPath.Count);

			for (int i = 0; i < aPath.Count; ++i) {
				result.Add(GetPointNormalWeighted(i, aPath, aClosed));
			}

			return result;
		}
		public static List<Vector3> GetNormals3D      (List<Vector3> aPath, bool aClosed) {
			List<Vector3> result = new List<Vector3>(aPath.Count);

			for (int i = 0; i < aPath.Count; ++i) {
				result.Add(GetPointNormal3D(i, aPath, aClosed));
			}

			return result;
		}
		public static List<Vector2> NormalsToTangents (List<Vector2> aNormals) {
            List<Vector2> result = new List<Vector2>(aNormals.Count);
            for (int i = 0; i < aNormals.Count; i++) {
                result.Add(new Vector2(aNormals[i].y, -aNormals[i].x));
            }
            return result;
        }
        public static List<Vector2> TangentsToNormals (List<Vector2> aTangents) {
            List<Vector2> result = new List<Vector2>(aTangents.Count);
            for (int i = 0; i < aTangents.Count; i++) {
                result.Add(new Vector2(-aTangents[i].y, aTangents[i].x));
            }
            return result;
        }
		#endregion

		#region Distance and Intersections
		/// <summary>
		/// Gets the length of the specified path segment.
		/// </summary>
		/// <param name="aPath">The list of vertices used to calculate the length.</param>
		/// <param name="i">Index of the first vert of the segment.</param>
		/// <param name="aClosed">Should we wrap around as though it was closed?</param>
		/// <returns>Distance between the indicated vertex and the next one on the path</returns>
		public static float GetSegmentLength(int i, List<Vector2> aPath, bool aClosed) {
			if (aPath.Count < 2) return 0;
			Vector2 delta = aPath[WrapIndex(i+1,aPath.Count, aClosed)] - aPath[WrapIndex(i,aPath.Count,aClosed)];
			return delta.magnitude;
		}
		/// <summary>
		/// Gets the index of the path point that starts the closest line segment to the specified point.
		/// </summary>
		/// <param name="aPoint">The point to check from.</param>
		/// <returns>Index of the first point in the line segment, the other point would be Index+1</returns>
		public static int GetClosestSegment(List<Vector2> aPath, Vector2 aPoint, bool aClosed) {
            if (aPath.Count == 1) return 0;
            if (aPath.Count <= 1) return -1;
			
			float dist  = float.MaxValue;
			int   seg   = -1;
			int   count = aClosed ? aPath.Count : aPath.Count-1;
			for (int i = 0; i < count; i++)
			{
				int     next  = (i+1) % aPath.Count;
				Vector2 pt    = GetClosetPointOnLine(aPath[i], aPath[next], aPoint, true);
				float   tDist = (aPoint - pt).SqrMagnitude();
				if (tDist < dist)
				{
					dist = tDist;
					seg  = i;
				}
			}
			return seg;
		}
		/// <summary>
		/// Gets the index of the point closest to the specified point, while ignoring a particular index.
		/// </summary>
		/// <param name="aPoint">The point to check from.</param>
		/// <param name="aIgnore">The point to ignore, or -1 for don't ignore any of them</param>
		/// <returns>Index of the point.</returns>
		public static int GetClosestPoint(List<Vector2> aPoints, Vector2 aPoint, int aIgnore=-1) {
			if (aPoints.Count <= 0 || (aPoints.Count<=1 && aIgnore==0)) return -1;

			float dist = float.MaxValue;
			int   id   = -1;
			for (int i = 0; i < aPoints.Count; i++) {
				float tDist = (aPoint - aPoints[i]).SqrMagnitude();
				if (tDist < dist) {
					dist = tDist;
					id   = i;
				}
			}
			return id;
		}
		/// <summary>
		/// Gets the smallest distance to the provided path.
		/// </summary>
		/// <param name="aPath">The path you're checking distance against</param>
		/// <param name="aClosed">Is the path provided closed? Should we check the closing segment too?</param>
		/// <param name="aPoint">The point to check from.</param>
		/// <returns>Smallest distance! float.MaxValue if none found</returns>
		public static float GetDistanceFromPath(List<Vector2> aPath, Vector2 aPoint, bool aClosed) {
			return Mathf.Sqrt(GetDistanceFromPathSq(aPath, aPoint, aClosed));
		}
		/// <summary>
		/// Gets the smallest distance^2 to the provided path.
		/// </summary>
		/// <param name="aPath">The path you're checking distance against</param>
		/// <param name="aClosed">Is the path provided closed? Should we check the closing segment too?</param>
		/// <param name="aPoint">The point to check from.</param>
		/// <returns>Smallest distance^2! float.MaxValue if none found</returns>
		public static float GetDistanceFromPathSq(List<Vector2> aPath, Vector2 aPoint, bool aClosed) {
			if (aPath.Count <= 1) return float.MaxValue;
			
			float dist  = float.MaxValue;
			int   count = aClosed ? aPath.Count : aPath.Count-1;
			for (int i = 0; i < count; i++)
			{
				int     next  = (i+1) % aPath.Count;
				Vector2 pt    = GetClosetPointOnLine(aPath[i], aPath[next], aPoint, true);
				float   tDist = (aPoint - pt).SqrMagnitude();
				if (tDist < dist)
					dist = tDist;
			}
			return dist;
		}
		/// <summary>
	    /// A utility function! Gets the closest point on a line, clamped to the line segment provided.
	    /// </summary>
	    /// <param name="aStart">Start of the line segment.</param>
	    /// <param name="aEnd">End of the line segment.</param>
	    /// <param name="aPoint">The point to compare distance to.</param>
	    /// <param name="aClamp">Should we clamp at the ends of the segment, or treat it as an infinite line?</param>
	    /// <returns>The closest point =D</returns>
		public static Vector2 GetClosetPointOnLine(Vector2 aStart, Vector2 aEnd, Vector2 aPoint, bool aClamp)
		{
			Vector2 AP = aPoint - aStart;
			Vector2 AB = aEnd - aStart;
			float ab2 = AB.x*AB.x + AB.y*AB.y;
			float ap_ab = AP.x*AB.x + AP.y*AB.y;
			float t = ap_ab / ab2;
			if (aClamp)
			{
				if (t < 0.0f) t = 0.0f;
				else if (t > 1.0f) t = 1.0f;
			}
			Vector2 Closest = aStart + AB * t;
			return Closest;
		}
		/// <summary>
	    /// A utility function! Gets the closest point on a line, clamped to the line segment provided.
	    /// </summary>
	    /// <param name="aStart">Start of the line segment.</param>
	    /// <param name="aEnd">End of the line segment.</param>
	    /// <param name="aPoint">The point to compare distance to.</param>
	    /// <param name="aClamp">Should we clamp at the ends of the segment, or treat it as an infinite line?</param>
	    /// <returns>The closest point =D</returns>
		public static Vector3 GetClosetPointOnLine(Vector3 aStart, Vector3 aEnd, Vector3 aPoint, bool aClamp)
		{
			Vector3 AP = aPoint - aStart;
			Vector3 AB = aEnd - aStart;
			float ab2 = AB.x*AB.x + AB.y*AB.y + AB.z*AB.z;
			float ap_ab = AP.x*AB.x + AP.y*AB.y + AP.z*AB.z;
			float t = ap_ab / ab2;
			if (aClamp)
			{
				if (t < 0.0f) t = 0.0f;
				else if (t > 1.0f) t = 1.0f;
			}
			Vector3 Closest = aStart + AB * t;
			return Closest;
		}
		/// <summary>
		/// A utility function! Gets the distance from the point to a line, clamped to the line segment provided.
		/// </summary>
		/// <param name="aStart">Start of the line segment.</param>
		/// <param name="aEnd">End of the line segment.</param>
		/// <param name="aPoint">The point to compare distance to.</param>
		/// <param name="aClamp">Should we clamp at the ends of the segment, or treat it as an infinite line?</param>
		/// <returns>Distance from the closest point</returns>
		public static float GetDistanceFromLine(Vector2 aStart, Vector2 aEnd, Vector2 aPoint, bool aClamp) {
			Vector2 pt = GetClosetPointOnLine(aStart, aEnd, aPoint, aClamp);
			return Vector2.Distance(aPoint, pt);
		}
		/// <summary>
		/// A utility function! Gets the squared distance from the point to a line, clamped to the line segment provided.
		/// </summary>
		/// <param name="aStart">Start of the line segment.</param>
		/// <param name="aEnd">End of the line segment.</param>
		/// <param name="aPoint">The point to compare distance to.</param>
		/// <param name="aClamp">Should we clamp at the ends of the segment, or treat it as an infinite line?</param>
		/// <returns>Distance squared from the closest point</returns>
		public static float GetSqDistanceFromLine(Vector2 aStart, Vector2 aEnd, Vector2 aPoint, bool aClamp) {
			Vector2 pt = GetClosetPointOnLine(aStart, aEnd, aPoint, aClamp);
			return (aPoint - pt).sqrMagnitude;
		}
		/// <summary>
		/// Gets the intersection point of two unbounded lines, returns (0,0) if lines are parallel.
		/// </summary>
		/// <param name="aLine1Pt1">A point along the first line.</param>
		/// <param name="aLine1Pt2">Second point along the first line.</param>
		/// <param name="aLine2Pt1">A point along the second line.</param>
		/// <param name="aLine2Pt2">Second point along the second line.</param>
		/// <returns>Intersection point, or (0,0) if parallel.</returns>
		public static Vector2 LineIntersectionPoint(Vector2 aLine1Pt1, Vector2 aLine1Pt2, Vector2 aLine2Pt1, Vector2 aLine2Pt2) {
        	// Get A,B,C of first line
			float A1 = aLine1Pt2.y - aLine1Pt1.y;
			float B1 = aLine1Pt1.x - aLine1Pt2.x;
			float C1 = A1 * aLine1Pt1.x + B1 * aLine1Pt1.y;
			
        	// Get A,B,C of second line
			float A2 = aLine2Pt2.y - aLine2Pt1.y;
			float B2 = aLine2Pt1.x - aLine2Pt2.x;
			float C2 = A2 * aLine2Pt1.x + B2 * aLine2Pt1.y;
			
        	// Get delta and check if the lines are parallel
			float delta = A1 * B2 - A2 * B1;
			if (delta == 0)
				return Vector2.zero;
			
        	// now return the intersection point
			return new Vector2(
				(B2 * C1 - B1 * C2) / delta,
				(A1 * C2 - A2 * C1) / delta
			);
		}
		/// <summary>
		/// Checks if the two bounded line segments intersect.
		/// </summary>
		/// <returns>Do they intersect?</returns>
		public static bool LineSegmentIntersection(Vector2 aLine1Start, Vector2 aLine1End, Vector2 aLine2Start, Vector2 aLine2End) {
			Vector2 a = aLine1End - aLine1Start;
			Vector2 b = aLine2Start - aLine2End;
			Vector2 c = aLine1Start - aLine2Start;
			
			float alphaNumerator   = b.y * c.x - b.x * c.y;
			float alphaDenominator = a.y * b.x - a.x * b.y;
			float betaNumerator    = a.x * c.y - a.y * c.x;
			float betaDenominator  = alphaDenominator;
			
			bool doIntersect = true;
			
			if (alphaDenominator == 0 || betaDenominator == 0) {
				doIntersect = false;
			} else {
				
				if (alphaDenominator > 0) {
					if (alphaNumerator < 0 || alphaNumerator > alphaDenominator) {
						doIntersect = false;
					}
				} else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator) {
					doIntersect = false;
				}
				
				if (doIntersect && betaDenominator > 0) {
					if (betaNumerator < 0 || betaNumerator > betaDenominator) {
						doIntersect = false;
					}
				} else if (betaNumerator > 0 || betaNumerator < betaDenominator) {
					doIntersect = false;
				}
			}
			
			return doIntersect;
		}
		/// <summary>
		/// Finds all the intersections of a closed line segment with a path.
		/// </summary>
		/// <param name="aLineStart">Start of the intersection line.</param>
		/// <param name="aLineEnd">End of the intersection line.</param>
		/// <param name="aPath">The path to test for intersections on.</param>
		/// <param name="aClosed">Is the path closed, should we test the closing segment?</param>
		/// <param name="aGetPoints">Do we need to calculate the exact intersection points, or is just the segment info enough?</param>
		/// <returns>A list of intersections. Intersection.segmentIndex is always filled in, Intersection.point is only filled if aGetPoints is true.</returns>
		public static List<Intersection> GetIntersections(Vector2 aLineStart, Vector2 aLineEnd, List<Vector2> aPath, bool aClosed, bool aGetPoints) {
			List<Intersection> result = new List<Intersection>();
			int count = aClosed ? aPath.Count : aPath.Count-1;

			for (int i = 0; i < count; i++) {
				Vector2 pathSegmentPt1 = aPath[i];
				Vector2 pathSegmentPt2 = aPath[WrapIndex(i+1,aPath.Count, aClosed)];

				if (LineSegmentIntersection(aLineStart,aLineEnd,pathSegmentPt1,pathSegmentPt2)) {
					Intersection intersection = new Intersection();
					intersection.segmentIndex = i;
					if (aGetPoints)
						intersection.point = LineIntersectionPoint(aLineStart, aLineEnd, pathSegmentPt1, pathSegmentPt2);
					result.Add(intersection);
				}
			}

			return result;
		}
		/// <summary>
	    /// Gets a list of line segments that are under the given point. Two indices per segment.
	    /// </summary>
	    /// <param name="aPath">A list of path vertices.</param>
	    /// <param name="aPoint">The point.</param>
	    /// <returns></returns>
		public  static List<int> GetSegmentsUnder     (List<Vector2> aPath, Vector2 aPoint) {
			List<int> result = new List<int>();
			for (int i=0;i<aPath.Count;i+=1) {
				int next = (i+1) % aPath.Count;
				int min  = aPath[i].x < aPath[next].x ? i : next;
				int max  = aPath[i].x > aPath[next].x ? i : next;
				
				if (aPath[min].x <= aPoint.x && aPath[max].x > aPoint.y) {
					float height = Mathf.Lerp(aPath[min].y, aPath[max].y, (aPoint.x - aPath[min].x) / (aPath[max].x - aPath[min].x));
					if (aPoint.y > height) {
						result.Add(min);
						result.Add(max);
					}
				}
			}
			
			return result;
		}
		/// <summary>
	    /// Gets the count of line segments that are under the given point.
	    /// </summary>
	    /// <param name="aPath">A list of path vertices.</param>
	    /// <param name="aPoint">The point.</param>
	    /// <returns></returns>
		public  static int CountSegmentsUnder     (List<Vector2> aPath, Vector2 aPoint) {
			int result = 0;
			
			for (int i=0;i<aPath.Count;i+=1) {
				int next = (i+1) % aPath.Count;
				int min  = aPath[i].x < aPath[next].x ? i : next;
				int max  = aPath[i].x > aPath[next].x ? i : next;
				
				if (aPath[min].x <= aPoint.x && aPath[max].x >= aPoint.x) {
					float height = Mathf.Lerp(aPath[min].y, aPath[max].y, (aPoint.x - aPath[min].x) / (aPath[max].x - aPath[min].x));
					if (aPoint.y >= height) {
						result += 1;
					}
				}
			}
			
			return result;
		}
		/// <summary>
		/// Finds if a point is inside or on a polygon.
		/// </summary>
		/// <param name="aPoly">A list of vertices representing a closed polygon.</param>
		/// <param name="aPoint">The point.</param>
		/// <returns>True if inside the poly, false if outside.</returns>
		public static bool     IsInPoly(List<Vector2> aPoly,   Vector2       aPoint) {
            bool result = false;

            for (int i = 0; i < aPoly.Count; i += 1) {
                int next = (i + 1) % aPoly.Count;
                int min  = aPoly[i].x < aPoly[next].x ? i : next;
                int max  = aPoly[i].x > aPoly[next].x ? i : next;

                if (aPoly[min].x <= aPoint.x && aPoly[max].x >= aPoint.x) {
                    float height = Mathf.Lerp(aPoly[min].y, aPoly[max].y, (aPoint.x - aPoly[min].x) / (aPoly[max].x - aPoly[min].x));
                    if (aPoint.y == height) {
                        return true;
                    }
                    if (aPoint.y >= height) {
                        result = !result;
                    }
                }
            }

            return result;
        }
		/// <summary>
		/// Checks if one closed polygon is inside, outside, or intersecting another closed polygon. No overlap is fast, but overlap invokes an O(n^2) intersection algorithm.
		/// </summary>
		/// <param name="aIsPoly">Answer is with respect to this closed polygon.</param>
		/// <param name="aInPoly">Closed polygon to check against.</param>
		/// <returns>Is aIsPoly inside, outside, or intersectiong aInPoly?</returns>
		public static PathSide IsInPoly(List<Vector2> aIsPoly, List<Vector2> aInPoly) {
			// fast bounds check
			Rect inRect = GetBounds(aInPoly);
			Rect isRect = GetBounds(aIsPoly);
			if (!inRect.Overlaps(isRect))
				return PathSide.Outside;

            // check each line segment intersection, naive approach O(n^2) (ew, try this later: https://en.wikipedia.org/wiki/Bentley%E2%80%93Ottmann_algorithm)
            for (int i = 0; i < aIsPoly.Count; i++) {
                Vector2 isP1 = aIsPoly[i];
                Vector2 isP2 = aIsPoly[(i + 1) % aIsPoly.Count];
                for (int t = 0; t < aInPoly.Count; t++) {
                    Vector2 inP1 = aInPoly[t];
                    Vector2 inP2 = aInPoly[(t + 1) % aInPoly.Count];

                    if (LineSegmentIntersection(isP1, isP2, inP1, inP2))
                        return PathSide.Intersect;
                }
            }
			// no intersections, check a single point for inside/outside, convex polys may still be weird
			return IsInPoly(aInPoly, aIsPoly[0]) ? PathSide.Inside : PathSide.Outside;
		}
		#endregion
		
		#region Bezier Functions
		public static Vector2 GetBezierPoint  (int i, float aPercent, List<Vector2> aPath, List<PointControl> aControls, bool aClosed) {
			i            = WrapIndex(i,   aPath.Count, aClosed);
			int     next = WrapIndex(i+1, aPath.Count, aClosed);
			Vector2 p1   = aPath[i];
			Vector2 p2   = aPath[next];
			return BezierPoint(p1, p1+aControls[i].controlNext, p2+aControls[next].controlPrev, p2, aPercent);
		}
		public static Vector2 GetBezierNormal (int i, float aPercent, List<Vector2> aPath, List<PointControl> aControls, bool aClosed) {
			i            = WrapIndex(i,   aPath.Count, aClosed);
			int     next = WrapIndex(i+1, aPath.Count, aClosed);
			Vector2 p1   = aPath[i];
			Vector2 p2   = aPath[next];
			return BezierNormal(p1, p1+aControls[i].controlNext, p2+aControls[next].controlPrev, p2, aPercent);
		}
		public static Vector2 GetBezierTangent(int i, float aPercent, List<Vector2> aPath, List<PointControl> aControls, bool aClosed) {
			i            = WrapIndex(i,   aPath.Count, aClosed);
			int     next = WrapIndex(i+1, aPath.Count, aClosed);
			Vector2 p1   = aPath[i];
			Vector2 p2   = aPath[next];
			return BezierTangent(p1, p1+aControls[i].controlNext, p2+aControls[next].controlPrev, p2, aPercent);
		}
		
		public static List<Vector2> SmoothBezier        (List<Vector2> aPath, List<PointControl> aControls, float aSplitDistance, bool aClosed) {
			List<Vector2> result = new List<Vector2>();
			int count = aClosed ? aPath.Count : aPath.Count -1;
			
			for (int i = 0; i < count; ++i) {
				int next = (i+1) % aPath.Count;
				if (aControls[i].type == PointType.Sharp && aControls[next].type == PointType.Sharp) {
					result.Add(aPath[i]);
					continue;
				}

				Vector2 p1 = aPath  [i];
				Vector2 c1 = aControls[i].controlNext + p1;
				Vector2 p2 = aPath  [next];
				Vector2 c2 = aControls[next].controlPrev + p2;
				float dist   = BezierLength(p1, c1, c2, p2);
				int   slices = Mathf.Max(1,(int)(dist / aSplitDistance));

				for (int s = 0; s < slices; ++s) {
					result.Add(BezierPoint(p1, c1, c2, p2, s/(float)slices));
				}
			}
			if (!aClosed)
				result.Add(aPath[aPath.Count-1]);

			return result;
		}
		public static List<Vector2> SmoothBezierNormals (List<Vector2> aPath, List<PointControl> aControls, float aSplitDistance, bool aClosed) {
			List<Vector2> result = new List<Vector2>();
			int count = aClosed ? aPath.Count : aPath.Count -1;
			
			for (int i = 0; i < count; ++i) {
				int next = (i+1) % aPath.Count;
				Vector2 p1 = aPath  [i];
				Vector2 c1 = aControls[i].controlNext + p1;
				Vector2 p2 = aPath  [next];
				Vector2 c2 = aControls[next].controlPrev + p2;

				if (aControls[i].type == PointType.Sharp && aControls[next].type == PointType.Sharp) {
					result.Add(BezierNormal(p1, c1, c2, p2, 0));
					continue;
				}

				float dist   = BezierLength(p1, c1, c2, p2);
				int   slices = Mathf.Max(1,(int)(dist / aSplitDistance));

				for (int s = 0; s < slices; ++s) {
					result.Add(BezierNormal(p1, c1, c2, p2, s/(float)slices));
				}
			}
			if (!aClosed) {
				Vector2 p1 = aPath[aPath.Count-2];
				Vector2 c1 = aControls[aPath.Count-2].controlNext + p1;
				Vector2 p2 = aPath[aPath.Count-1];
				Vector2 c2 = aControls[aPath.Count-1].controlPrev + p2;
				result.Add(BezierNormal(p1, c1, c2, p2, 1));
			}

			return result;
		}
		public static List<Vector2> SmoothBezierTangents(List<Vector2> aPath, List<PointControl> aControls, float aSplitDistance, bool aClosed) {
			List<Vector2> result = new List<Vector2>();
			int count = aClosed ? aPath.Count : aPath.Count -1;

			for (int i = 0; i < count; ++i) {
				int next = (i+1) % aPath.Count;
				Vector2 p1 = aPath[i];
				Vector2 c1 = aControls[i].controlNext + p1;
				Vector2 p2 = aPath[next];
				Vector2 c2 = aControls[next].controlPrev + p2;

				if (aControls[i].type == PointType.Sharp && aControls[next].type == PointType.Sharp) {
					result.Add(BezierTangent(p1, c1, c2, p2, 0));
					continue;
				}

				float dist = BezierLength(p1, c1, c2, p2);
				int slices = Mathf.Max(1,(int)(dist / aSplitDistance));

				for (int s = 0; s < slices; ++s) {
					result.Add(BezierTangent(p1, c1, c2, p2, s/(float)slices));
				}
			}
			if (!aClosed)
				result.Add(aPath[aPath.Count-1]);

			return result;
		}
		
		public static Vector2 BezierPoint  (Vector2 p1, Vector2 c1, Vector2 c2, Vector2 p2, float aPercent) {
			float t = aPercent;
			float u = 1 - t;
			float tt = t*t;
			float uu = u*u;
			float uuu = uu * u;
			float ttt = tt * t;

			return uuu*p1 + 3*uu*t*c1 + 3*u*tt*c2 + ttt*p2;
		}
		public static Vector2 BezierTangent(Vector2 p1, Vector2 c1, Vector2 c2, Vector2 p2, float aPercent) {
			float t   = aPercent;
			float tt  = t*t;
			float t6  = t*6;
			float tt3 = 3*tt;
			float tt9 = 9*tt;

			Vector2 tan = (tt3*p2 + (t6-tt9)*c2 + (tt9-12*t+3)*c1 + (t6-tt3-3)*p1);
			return tan.normalized;
		}
		public static Vector2 BezierNormal (Vector2 p1, Vector2 c1, Vector2 c2, Vector2 p2, float aPercent) {
			Vector2 tan = BezierTangent(p1, c1, c2, p2, aPercent);
			return new Vector2(tan.y, -tan.x);
		}

		public static float BezierLength    (Vector2 p1, Vector2 c1, Vector2 c2, Vector2 p2) {
			return BezierLengthSlow(p1, c1, c2, p2, 4);
		}
		public static float BezierLengthSlow(Vector2 p1, Vector2 c1, Vector2 c2, Vector2 p2, int count = 200) {
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
		#endregion
		
		#region Path Transforms and Manipulations
		/// <summary>
		/// Converts a list of path indices to a list of path verts created from the given list.
		/// </summary>
		/// <param name="aPath">Path to pull vert data from</param>
		/// <param name="aIndices">List of inidces to pull from the path.</param>
		/// <returns>A list of path verts as indicated by the index list</returns>
		public static List<Vector2> IndicesToPath(List<Vector2> aPath, List<int> aIndices) {
			List<Vector2> result = new List<Vector2>(aIndices.Count);
			for (int i = 0; i < aIndices.Count; i++) {
				result.Add(aPath[aIndices[i]]);
			}
			return result;
		}
		public static List<Vector2> SimplifyPath (List<Vector2> aPath, float aTolerance) {
            if (aPath.Count <= 2) return new List<Vector2>(aPath);
            int end = aPath.Count - 1;
			while (aPath[0] == aPath[end] && end > 1) end-=1;

			float   maxDistance = 0;
			int     id  = -1;
			Vector2 p1  = aPath[0];
			Vector2 p2  = aPath[end];
			for (int i = 1; i < end; ++i) {
				Vector2 pt   = aPath[i];
				Vector2 diff = GetClosetPointOnLine(p1, p2, pt, true);
				float d = (diff - pt).sqrMagnitude;
				if (d > maxDistance) {
					id = i;
					maxDistance = d;
				}
			}

			List<Vector2> result = new List<Vector2>();
			if (id != -1 && maxDistance > aTolerance * aTolerance) {
				List<Vector2> results1 = SimplifyPath(aPath.GetRange(0, id + 1), aTolerance);
				List<Vector2> results2 = SimplifyPath(aPath.GetRange(id, aPath.Count - id), aTolerance);

				results2.RemoveAt(0);

				result.AddRange(results1);
				result.AddRange(results2);
			} else {
				result.Add(aPath[0]);
				result.Add(aPath[end]);
			}

			return result;
        }
		public static List<Vector3> To3D(List<Vector2> aPath, ConvertOptions aOptions = ConvertOptions.XY, float aAxisValue=0) {
            List<Vector3> result = new List<Vector3>();
            for (int i = 0; i < aPath.Count; i++) {
                if (aOptions == ConvertOptions.XY) {
                    result.Add(new Vector3(aPath[i].x, aPath[i].y, aAxisValue));
                } else if (aOptions == ConvertOptions.XZ) {
                    result.Add(new Vector3(aPath[i].x, aAxisValue, aPath[i].y));
                }
            }
            return result;
        }
		public static List<Vector3> To3D(List<Vector2> aPath, Matrix4x4 aMat, ConvertOptions aOptions = ConvertOptions.XY, float aAxisValue=0) {
			List<Vector3> result = new List<Vector3>();
			for (int i = 0; i < aPath.Count; i++) {
				if (aOptions == ConvertOptions.XY) {
					result.Add(aMat.MultiplyPoint3x4(new Vector3(aPath[i].x, aPath[i].y, aAxisValue)));
				} else if (aOptions == ConvertOptions.XZ) {
					result.Add(aMat.MultiplyPoint3x4(new Vector3(aPath[i].x, aAxisValue, aPath[i].y)));
				}
			}
			return result;
		}
		public static List<Vector2> Reverse(List<Vector2> aPath) {
			List<Vector2> result = new List<Vector2> (aPath.Count);
			for (int i = aPath.Count-1; i >= 0; i--) {
				result.Add(aPath[i]);
			}
			return result;
		}
		public static void          Reverse<T>(ref List<T> aPath) {
			for (int i = 0; i < aPath.Count / 2; ++i) {
				T       t      = aPath[i];
				int     mirror = (aPath.Count-1)-i;
				aPath[i]       = aPath[mirror];
				aPath[mirror]  = t;
			}
		}
		public static List<Vector2> Subdivide(List<Vector2> aPath, int aCuts, bool aClosed) {
			List<Vector2> result = new List<Vector2>(aPath.Count + (aPath.Count-1)*aCuts);
			int           count  = aClosed ? aPath.Count : aPath.Count-1;
			aCuts += 1;

			for (int i = 0; i < count; i+=1) {
				int next = (i + 1)%aPath.Count;
				
				Vector2 p1 = aPath[i];
				Vector2 p2 = aPath[next];
				for (int c = 0; c < aCuts; c++) {
					result.Add(Vector2.Lerp(p1, p2, c/(float)aCuts));
				}
			}

			if (!aClosed)
				result.Add(aPath[aPath.Count-1]);

			return result;
		}
		public static List<Vector2> SubdivideDistance(List<Vector2> aPath, float aCutDistance, bool aClosed) {
			List<Vector2> result = new List<Vector2>(aPath.Count);
			int           count  = aClosed ? aPath.Count : aPath.Count-1;

			for (int i = 0; i < count; i+=1) {
				int next = (i + 1)%aPath.Count;

				Vector2 p1   = aPath[i];
				Vector2 p2   = aPath[next];
				float   dist = Vector2.Distance(p1, p2);
				int     cuts = (int)Mathf.Max(1, aCutDistance / dist );
				for (int c = 0; c < cuts; c++) {
					result.Add(Vector2.Lerp(p1, p2, c/(float)cuts));
				}
			}

			if (!aClosed)
				result.Add(aPath[aPath.Count-1]);

			return result;
		}
		public static List<Vector2> SubdivideEnsureDistance(List<Vector2> aPath, float aMaxDistance, bool aClosed) {
			List<Vector2> result = new List<Vector2>(aPath);
			SubdivideEnsureDistance(ref result, aMaxDistance, aClosed);
			return result;
		}
		public static void          SubdivideEnsureDistance(ref List<Vector2> aPath, float aMaxDistance, bool aClosed) {
			int           count  = aClosed ? aPath.Count : aPath.Count-1;
			float         sqMax  = aMaxDistance * aMaxDistance;

			for (int i = 0; i < count; i+=1) {
				int next = (i + 1)%aPath.Count;
				Vector2 p1   = aPath[i];
				Vector2 p2   = aPath[next];
				float   distSq = (p1 - p2).sqrMagnitude;
				
				if (distSq > sqMax) {
					float dist = Mathf.Sqrt(distSq);
					int subdivisions = Mathf.CeilToInt(dist / aMaxDistance);

					for (int s = 1; s < subdivisions; s++) {
						aPath.Insert(i+1, Vector2.LerpUnclamped(p1, p2, (float)s/subdivisions));
						count += 1;
						i     += 1;
					}
				}
			}
		}
		public static List<Vector2> ScalePathByNormals(float aScale, List<Vector2> aPath, bool aClosed, float aClampCornerCompensation=3, bool aMergeIntersections=false) {
			List<Vector2> source = aPath;

			// when scaling down, verts that are too close together can experience undesirable overlap
			if (aMergeIntersections) {
				source = new List<Vector2>(aPath);
				int   ptCount = -1;
				float s       = aScale * aScale;
				while (source.Count != ptCount && source.Count > 3) {
					ptCount = source.Count;
					int count = aClosed ? source.Count : source.Count -1;
					for (int i = 0; i < count; i++) {
						int next = (i+1)%source.Count;
						Vector2 p1 = source[i];
						Vector2 p2 = source[next];
						Vector2 n1 = GetPointNormal(i, source, aClosed);
						Vector2 n2 = GetPointNormal(next, source, aClosed);
						
						Vector2 pt = LineIntersectionPoint(p1, p1+n1, p2, p2+n2);
						if (Vector2.SqrMagnitude(pt-p1) < s) {
							source[next] = (p1+p2)/2;
							source.RemoveAt(i);
							i--;
							count--;

							if (source.Count <= 3)
								break;
						}
					}
				}
			}

			List<Vector2> result = new List<Vector2>(source.Count);

			for (int i = 0; i < source.Count; ++i) {
				float   off     = aScale;
				Vector2 ptNorm  = GetPointNormal  (i, source, aClosed);
				Vector2 segNorm = GetSegmentNormal(i, source, aClosed);
				if (aClampCornerCompensation > 0)
					off *= Mathf.Min(aClampCornerCompensation, (1 / Mathf.Cos((Vector2.Angle(segNorm, ptNorm)) * Mathf.Deg2Rad)));
				else 
					off *= (1 / Mathf.Cos((Vector2.Angle(segNorm, ptNorm)) * Mathf.Deg2Rad));
				result.Add(source[i] - ptNorm * off);
			}

			return result;
		}
		public static List<Vector2> ScalePathToPoint(Vector2 aPt, float aScale, List<Vector2> aPoints, bool aClosed) {
			List<Vector2> result = new List<Vector2>(aPoints.Count);

			for (int i = 0; i < aPoints.Count; ++i) {
				result.Add((aPoints[i]-aPt) * aScale + aPt);
			}

			return result;
		}
		public static void          OffsetPathSegments(List<Vector2> aPath, bool aClosed, Vector2 aOffset, Func<int, bool> aSegmentMask) {
			float mag = aOffset.magnitude;
			Vector2 nOffset = aOffset.normalized;
			
			Vector2 pNorm  = GetSegmentNormal(WrapIndex(-1, aPath.Count, aClosed), aPath, aClosed);
			bool    pShift = aSegmentMask(WrapIndex(-1, aPath.Count, aClosed));
			bool    currShift = aSegmentMask(0);
			Vector2 currNorm  = GetSegmentNormal(0, aPath, aClosed);
			for (int i = 0; i < aPath.Count; i++) {
				int next = WrapIndex(i+1, aPath.Count, aClosed);
				bool    nShift = aSegmentMask(next);
				Vector2 nNorm  = GetSegmentNormal(next, aPath, aClosed);
				
				Vector2 offsetDir = Vector2.zero;
				bool skip = false;

				if (currShift && !pShift) {
					offsetDir = new Vector2(-pNorm.y, pNorm.x);
				} else if (pShift && !currShift) {
					offsetDir = new Vector2(currNorm.y, -currNorm.x);
				} else if (pShift && currShift) {
					offsetDir = (currNorm + pNorm).normalized;
				} else {
					skip = true;
				}

				if (!skip) {
					aPath[i] += offsetDir * (mag/Vector2.Dot(offsetDir,nOffset));
				}
				
				pNorm  = currNorm;
				pShift = currShift;
				currShift = nShift;
				currNorm  = nNorm;
			}
		}
		public static List<Vector2> RoundCorners(List<Vector2> aPath, List<PointControl> aControls, bool aClosed, List<float> aRadii, float aSegmentSize = 1) {
			List<Vector2> result = new List<Vector2>(aPath.Count);

			for (int i = 0; i < aPath.Count; i++) {
				if (aRadii[i] <= 0 || ( !aClosed && (i == 0 || i == aPath.Count-1) )) {
					result.Add(aPath[i]);
					continue;
				}
				AddRoundCorners(i, aPath, aControls, aClosed, ref result, aRadii[i], aSegmentSize);
			}
			return result;
		}
		public static void          AddRoundCorners(int aCornerId, List<Vector2> aPath, List<PointControl> aControls, bool aClosed, ref List<Vector2> aTo, float aRadius, float aSegmentSize = 1) {
			if (aRadius <= 0 || ( !aClosed && (aCornerId == 0 || aCornerId == aPath.Count-1) )) {
				aTo.Add(aPath[aCornerId]);
				return;
			}
			int prevIndex = WrapIndex(aCornerId-1, aPath.Count, aClosed);
			int nextIndex = WrapIndex(aCornerId+1, aPath.Count, aClosed);
				
			float   radius    = aRadius;
			Vector2 point     = aPath[aCornerId];
			Vector2 prevPoint = aPath[prevIndex];
			Vector2 nextPoint = aPath[nextIndex];
			if (aControls != null) {
				prevPoint += aControls[prevIndex].controlNext;
				nextPoint += aControls[nextIndex].controlPrev;
			}
			Vector2 prevDir = (prevPoint-point);
			Vector2 nextDir = (nextPoint-point);
			float prevMag = prevDir.magnitude;
			float nextMag = nextDir.magnitude;
			prevDir = prevDir / prevMag;
			nextDir = nextDir / nextMag;
			float halfMinMag = Mathf.Min(prevMag, nextMag)/2.1f;

			float cornerAngle = Vector2.Angle(prevDir, nextDir)*Mathf.Deg2Rad;
			if (Math.Abs(cornerAngle-Mathf.PI) < 0.01f || Math.Abs(cornerAngle) < 0.01f) {
				// this is for angles at or near 180, which will create lots of infinity type errors
				aTo.Add(point);
				return;
			}
			bool  acute       = Vector2.Dot(new Vector2(prevDir.y, -prevDir.x), nextDir) > 0;
			float maxRadius = (Mathf.Tan(cornerAngle/2) * halfMinMag);
			if (radius > maxRadius)
				radius = maxRadius;

			float   tangentDistance = radius / Mathf.Tan(cornerAngle/2);
			float   centerDistance  = radius / Mathf.Sin(cornerAngle/2);
			float   arcAngle        = Mathf.Acos(radius/centerDistance)*2;
			float   arcLength       = Mathf.PI * radius * (arcAngle/Mathf.PI);
			int     count      = (int)(arcLength / aSegmentSize + 0.5f);
			if (count%2==1) count += 1;// ensure an even count so we always get a center vert.
			Vector2 arcCenter  = point + Vector2.Lerp(prevDir, nextDir, 0.5f).normalized * centerDistance;
			float   startAngle = ClockwiseAngle((point+tangentDistance*prevDir) - arcCenter, Vector2.right) * Mathf.Deg2Rad;
			float   endAngle   = ClockwiseAngle((point+tangentDistance*nextDir) - arcCenter, Vector2.right) * Mathf.Deg2Rad;
				
			if (!acute) {
				while(endAngle > startAngle)
					endAngle -= Mathf.PI*2;
			} else {
				while(endAngle < startAngle)
					endAngle += Mathf.PI*2;
			}

			for (int s = 0; s <= count; s++) {
				float pct = (float)s/count;
				if (count == 0)
					pct = .5f;
				float angle = Mathf.Lerp(startAngle, endAngle, pct);
				aTo.Add(arcCenter + new Vector2(
					Mathf.Cos(angle) * radius,
					Mathf.Sin(angle) * radius));
			}
		}
		public static Vector2       GetRoundedCornerEnd(int aCornerId, List<Vector2> aPath, List<PointControl> aControls, bool aClosed, float aRadius, bool aGetBeginning) {
			if (aRadius <= 0 || ( !aClosed && (aCornerId == 0 || aCornerId == aPath.Count-1) )) {
				return aPath[aCornerId];
			}
			int prevIndex = WrapIndex(aCornerId-1, aPath.Count, aClosed);
			int nextIndex = WrapIndex(aCornerId+1, aPath.Count, aClosed);

			float radius = aRadius;
			Vector2 point     = aPath[aCornerId];
			Vector2 prevPoint = aPath[prevIndex];
			Vector2 nextPoint = aPath[nextIndex];
			if (aControls != null) {
				prevPoint += aControls[prevIndex].controlNext;
				nextPoint += aControls[nextIndex].controlPrev;
			}
			Vector2 prevDir = (prevPoint-point);
			Vector2 nextDir = (nextPoint-point);
			float prevMag = prevDir.magnitude;
			float nextMag = nextDir.magnitude;
			prevDir = prevDir / prevMag;
			nextDir = nextDir / nextMag;
			float halfMinMag = Mathf.Min(prevMag, nextMag)/2.1f;

			float cornerAngle = Vector2.Angle(prevDir, nextDir)*Mathf.Deg2Rad;
			if (Math.Abs(cornerAngle-Mathf.PI) < 0.01f || Math.Abs(cornerAngle) < 0.01f) {
				// this is for angles at or near 180, which will create lots of infinity type errors
				return point;
			}
			float maxRadius = (Mathf.Tan(cornerAngle/2) * halfMinMag);
			if (radius > maxRadius)
				radius = maxRadius;
			float tangentDistance = radius / Mathf.Tan(cornerAngle/2);

			if (aGetBeginning)
				return point + tangentDistance * prevDir;
			else
				return point + tangentDistance * nextDir;
		}
		#endregion

		#region Utility and Helper Functions
		public static void GizmoDraw(List<Vector2> aPath, float aVertSize, bool aClosed, bool aDrawLine = true, float aArrowSize = 0.1f) {
			int count = aClosed ? aPath.Count : aPath.Count-1;
			if (aDrawLine) {
				for (int i = 0; i < count; ++i) {
					Gizmos.DrawLine(aPath[i], aPath[(i+1)%aPath.Count]);
				}
			}

			if (aArrowSize > 0) {
				for (int i = 0; i < count; i++) {
					Vector2 diff = aPath[(i+1) % aPath.Count] - aPath[i];
					Vector2 center = aPath[i] + diff / 2;
					diff.Normalize();
					diff *= aArrowSize;
					Vector2 norm = new Vector2(diff.y, -diff.x);
					Gizmos.DrawLine(center, center + norm - diff);
					Gizmos.DrawLine(center, center - norm - diff);
				}
			}
			
			if (aVertSize > 0) {
				for (int i = 0; i < aPath.Count; ++i) {
					Gizmos.DrawCube(aPath[i], Vector3.one * aVertSize);
				}
			}
		}

		public  static bool    ClipLine(Rect aClipRect, Vector2 aStart, Vector2 aEnd, out Vector2 aStartResult, out Vector2 aEndResult) {
			OutCode outcode0 = ComputeOutCode(aClipRect, aStart);
			OutCode outcode1 = ComputeOutCode(aClipRect, aEnd);

			aStartResult = aStart;
			aEndResult   = aEnd;
			
			while (true) {
				if ((outcode0 | outcode1) == OutCode.Inside) {
					return true;
				} else if ((outcode0 & outcode1) > 0) {
					return false;
				} else {
					Vector2 pt         = Vector2.zero;
					OutCode outcodeOut = outcode0 > 0 ? outcode0 : outcode1;
					
					if ((outcodeOut & OutCode.Top) > 0) {
						pt.x = aStartResult.x + (aEndResult.x - aStartResult.x) * (aClipRect.yMax - aStartResult.y) / (aEndResult.y - aStartResult.y);
						pt.y = aClipRect.yMax;
					} else if ((outcodeOut & OutCode.Bottom) > 0) {
						pt.x = aStartResult.x + (aEndResult.x - aStartResult.x) * (aClipRect.yMin - aStartResult.y) / (aEndResult.y - aStartResult.y);
						pt.y = aClipRect.yMin;
					} else if ((outcodeOut & OutCode.Right) > 0) {
						pt.y = aStartResult.y + (aEndResult.y - aStartResult.y) * (aClipRect.xMax - aStartResult.x) / (aEndResult.x - aStartResult.x);
						pt.x = aClipRect.xMax;
					} else if ((outcodeOut & OutCode.Left) > 0) {
						pt.y = aStartResult.y + (aEndResult.y - aStartResult.y) * (aClipRect.xMin - aStartResult.x) / (aEndResult.x - aStartResult.x);
						pt.x = aClipRect.xMin;
					}
					
					if (outcodeOut == outcode0) {
						aStartResult = pt;
						outcode0 = ComputeOutCode(aClipRect, aStartResult);
					} else {
						aEndResult = pt;
						outcode1 = ComputeOutCode(aClipRect, aEndResult);
					}
				}
			}
		}
		private static OutCode ComputeOutCode(Rect aBounds, Vector2 aPos) {
			OutCode result = OutCode.Inside;
			if      (aPos.x < aBounds.xMin) result |= OutCode.Left;
			else if (aPos.x > aBounds.xMax) result |= OutCode.Right;
			if      (aPos.y < aBounds.yMin) result |= OutCode.Bottom;
			else if (aPos.y > aBounds.yMax) result |= OutCode.Top;
			return result;
		}

		public static float GetParallelOffset(int i, List<Vector2> aPath, bool aClosed) {
			Vector2 segNorm = GetSegmentNormal(i, aPath, aClosed);
			Vector2 ptNorm  = GetPointNormal  (i, aPath, aClosed);
			return Mathf.Min(3,1 / Mathf.Cos((Vector2.Angle(segNorm, ptNorm)) * Mathf.Deg2Rad));
		}
		public static bool  IsInnerCorner(List<Vector2> aFirst, List<Vector2> aSecond) {
			Vector2 center = aFirst[aFirst.Count-1];
			
			Vector2 first  = aFirst[aFirst.Count-2] - center;
			Vector2 second  = aSecond[1] - center;
			
			first = new Vector2(first.y, -first.x);
			
			return Vector2.Dot(first, second) > 0;
		}

        public static float SignedAngle   (Vector2 v1, Vector2 v2) {
            float sign = Mathf.Sign(v1.x * v2.y - v1.y * v2.x);
            return Vector2.Angle(v1, v2) * sign;
        }
        public static float ClockwiseAngle(Vector2 v1, Vector2 v2) {
            float ang = SignedAngle(v1, v2);
            if (ang > 0) ang = -180 - (180 - ang);
            return -ang;
        }
		public static float CounterclockwiseAngle(Vector2 v1, Vector2 v2) {
            return ClockwiseAngle(v2,v1);
        }

		public static int WrapIndex(int aIndex, int aCount, bool aClosed) {
			if (aCount == 0) return 0;

			if (aClosed) return ((aIndex % aCount) + aCount) % aCount;
			else return aIndex<0 ? 0 : (aIndex>=aCount ? aCount-1 : aIndex);
		}
		public static int IndexDistance(int aIndexStart, int aIndexEnd, int aCount, bool aClosed) {
			if (aCount == 0) return 0;

			if (aIndexEnd < aIndexStart) {
				return (aIndexEnd + aCount) - aIndexStart;
			} 
			else
				return aIndexEnd - aIndexStart;
		}
		#endregion
	}
}