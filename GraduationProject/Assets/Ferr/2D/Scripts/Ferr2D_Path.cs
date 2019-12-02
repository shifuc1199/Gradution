using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A generic path with lots of helper functions. Should be useful for all sorts of things!
/// </summary>
[AddComponentMenu("Ferr2D/Path")]
public class Ferr2D_Path : MonoBehaviour
{
    #region Fields and properties
    /// <summary>
    /// If the path should connect at the ends! Influences interpolation, especially for normals.
    /// </summary>
	public bool           closed    = false;
    /// <summary>
    /// If you really want access to these, you should call GetVerts
    /// </summary>
	public List<Vector2>  pathVerts = new List<Vector2>();
    /// <summary>
    /// Returns the number of vertices in the path
    /// </summary>
    public int Count { get { return pathVerts.Count; } }
    #endregion

    #region Methods
    /// <summary>
    /// Moves the object location to the center of the path verts. Also offsets the path locations to match.
    /// </summary>
    public void ReCenter        ()
    {
        Vector2 center = Vector2.zero;
        for (int i = 0; i < pathVerts.Count; i++)
        {
            center += pathVerts[i];
        }
        center = center / pathVerts.Count + new Vector2(transform.position.x, transform.position.y);
        Vector2 offset = center - new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        for (int i = 0; i < pathVerts.Count; i++)
        {
            pathVerts[i] -= offset;
        }
        gameObject.transform.position = new Vector3(center.x, center.y, gameObject.transform.position.z);

        UpdateDependants(true);
    }
    /// <summary>
    /// Updates all other component on this GameObject that implement the Ferr2DT_IPath interface.
    /// </summary>
    public void UpdateDependants(bool aFullUpdate)
    {
        Component[] coms = gameObject.GetComponents(typeof(Ferr2D_IPath));
        for (int i = 0; i < coms.Length; i++)
        {
            (coms[i] as Ferr2D_IPath).Build(aFullUpdate);
        }
    }
	
	/// <summary>
    /// Updates the colliders of all Ferr2DT_PathTerrain components attached to this object!
    /// </summary>
	public void UpdateColliders()
	{
		Component[] coms = gameObject.GetComponents(typeof(Ferr2DT_PathTerrain));
		for (int i = 0; i < coms.Length; i++)
		{
			Ferr2DT_PathTerrain path = coms[i] as Ferr2DT_PathTerrain;
			if (path.createCollider)
				path.RecreateCollider();
		}
	}
	
    /// <summary>
    /// Adds a vertex to the end of the path.
    /// </summary>
    /// <param name="aPoint">The vertex to add!</param>
    public void Add             (Vector2 aPoint)
    {
        pathVerts.Add(aPoint);
    }
    /// <summary>
    /// Gets the index of the path point that starts the closest line segment to the specified point.
    /// </summary>
    /// <param name="aPoint">The point to check from.</param>
    /// <returns>Index of the first point in the line segment, the other point would be Index+1</returns>
    public int  GetClosestSeg   (Vector2 aPoint)
    {
        if (pathVerts.Count <= 0) return -1;

        float dist  = float.MaxValue;
        int   seg   = -1;
		int   count = closed ? pathVerts.Count : pathVerts.Count-1;
        for (int i = 0; i < count; i++)
        {
            int next = i == pathVerts.Count -1 ? 0 : i + 1;
            Vector2 pt    = GetClosetPointOnLine(pathVerts[i], pathVerts[next], aPoint, true);
            float   tDist = (aPoint - pt).SqrMagnitude();
            if (tDist < dist)
            {
                dist = tDist;
                seg  = i;
            }
        }
        if (!closed)
        {
            float tDist = (aPoint - pathVerts[pathVerts.Count - 1]).SqrMagnitude();
            if (tDist <= dist)
            {
                seg = pathVerts.Count - 1;
            }
            tDist = (aPoint - pathVerts[0]).SqrMagnitude();
            if (tDist <= dist)
            {
                seg = pathVerts.Count - 1;
            }
        }
        return seg;
    }

    /// <summary>
    /// Don't care about smoothing? If aSmoothed is false, GetVerts calls this method.
    /// </summary>
    /// <returns>Just a plain old copy of pathVerts.</returns>
    public List<Vector2> GetVertsRaw     ()
    {
        List<Vector2> result = new List<Vector2>(pathVerts);
        return result;
    }
    /// <summary>
    /// Gets a copy of the vertices that's smoothed.
    /// </summary>
    /// <param name="aSplitDistance">If they're smoothed, how far apart should each smooth split be?</param>
    /// <param name="aSplitCorners">Should we make corners sharp? Sharp corners don't get smoothed.</param>
    /// <returns>A copy of the smoothed data.</returns>
    public List<Vector2> GetVertsSmoothed(float aSplitDistance, bool aSplitCorners, bool aInverted)
    {
        List<Vector2> result = new List<Vector2>();
        if (aSplitCorners)
        {
            List<Ferr2DT_TerrainDirection> dirs;
            List<List<int>> segments = GetSegments(pathVerts, out dirs);
            if (closed) CloseEnds(pathVerts, ref segments, ref dirs, aSplitCorners, aInverted);
            if (segments.Count > 1) {
                for (int i = 0; i < segments.Count; i++) {
                    List<Vector2> smoothed = SmoothSegment(IndicesToList<Vector2>(pathVerts, segments[i]), aSplitDistance, false);
                    if (i != 0 && smoothed.Count > 0) smoothed.RemoveAt(0);
                    result.AddRange(smoothed);
                }
            } else {
                result = SmoothSegment(pathVerts, aSplitDistance, closed);
                if (closed) result.Add(pathVerts[0]);
            }
        }
        else {
            result = SmoothSegment(pathVerts, aSplitDistance, closed);
            if (closed) result.Add(pathVerts[0]);
        }
        return result;
    }

    /// <summary>
    /// Finds a rectangle bounding the entire path, based on the raw vertex data.
    /// </summary>
    /// <returns>Floating point rectangle that goes up exactly to the edges of the raw path.</returns>
    public Rect          GetBounds       () {
        return GetBounds(pathVerts);
    }
    #endregion

    #region Static Methods
    /// <summary>
    /// Gets the normal at the specified path index.
    /// </summary>
    /// <param name="aSegment">The list of vertices used to calculate the normal.</param>
    /// <param name="i">Index of the vertex to get the normal of.</param>
    /// <param name="aClosed">Should we interpolate at the edges of the path as though it was closed?</param>
    /// <returns>A normalized normal!</returns>
    public  static Vector2   GetNormal          (List<Vector2> aSegment, int i, bool  aClosed) {
		if (aSegment.Count < 2) return Vector2.up;
		Vector2 curr = aClosed && i == aSegment.Count - 1 ? aSegment[0] : aSegment[i];

        // get the vertex before the current vertex
		Vector2 prev = Vector2.zero;
		if (i-1 < 0) {
			if (aClosed) {
				prev = aSegment[aSegment.Count-2];
			} else {
				prev = curr - (aSegment[i+1]-curr);
			}
		} else {
			prev = aSegment[i-1];
		}
		
        // get the vertex after the current vertex
		Vector2 next = Vector2.zero;
		if (i+1 > aSegment.Count-1) {
			if (aClosed) {
				next = aSegment[1];
			} else {
				next = curr - (aSegment[i-1]-curr);
			}
		} else {
			next = aSegment[i+1];
		}

		prev = prev - curr;
		next = next - curr;
		
		prev.Normalize ();
		next.Normalize ();
		
		prev = new Vector2(-prev.y, prev.x);
		next = new Vector2(next.y, -next.x);
		
		Vector2 norm = (prev + next) / 2;
		norm.Normalize();

		return norm;
	}
    /// <summary>
    /// Gets the normal at the specified path index using cubic interpolation for smoothing.
    /// </summary>
    /// <param name="aSegment">The list of vertices used to calculate the normal.</param>
    /// <param name="i">Index of the vertex to start from.</param>
    /// <param name="aPercentage">How far between this vertex and the next should we look?</param>
    /// <param name="aClosed">Should we interpolate at the edges of the path as though it was closed?</param>
    /// <returns>A normalized cubic interpolated normal!</returns>
    public  static Vector2   CubicGetNormal     (List<Vector2> aSegment, int i, float aPercentage, bool aClosed)
    {
        Vector2 p1 = CubicGetPt(aSegment, i, aPercentage, aClosed);
        Vector2 p2 = CubicGetPt(aSegment, i, aPercentage+ 0.01f, aClosed);
        Vector2 dir = p2 - p1;
        dir.Normalize();
        return new Vector2(dir.y, -dir.x);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aSegment">The list of vertices used to calculate the point.</param>
    /// <param name="i">Index of the vertex to start from.</param>
    /// <param name="aPercentage">How far between this vertex and the next should we look?</param>
    /// <param name="aClosed">Should we interpolate at the edges of the path as though it was closed?</param>
    /// <returns>A cubic interpolated point.</returns>
    public  static Vector2   CubicGetPt         (List<Vector2> aSegment, int i, float aPercentage, bool aClosed) 
    {
        int a1 = aClosed ? i - 1 < 0 ? aSegment.Count-1 : i - 1 : Mathf.Clamp(i - 1, 0, aSegment.Count - 1);
        int a2 = i;
        int a3 = aClosed ? (i + 1) % (aSegment.Count - 1) : Mathf.Clamp(i + 1, 0, aSegment.Count - 1);
        int a4 = aClosed ? (i + 2) % (aSegment.Count - 1) : Mathf.Clamp(i + 2, 0, aSegment.Count - 1);

        return new Vector2(
            Cubic(aSegment[a1].x, aSegment[a2].x, aSegment[a3].x, aSegment[a4].x, aPercentage),
            Cubic(aSegment[a1].y, aSegment[a2].y, aSegment[a3].y, aSegment[a4].y, aPercentage));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="aSegment">The list of vertices used to calculate the normal.</param>
    /// <param name="i">Index of the vertex to start from.</param>
    /// <param name="aPercentage">How far between this vertex and the next should we look?</param>
    /// <param name="aClosed">Should we interpolate at the edges of the path as though it was closed?</param>
    /// <returns>A normalized Hermite interpolated normal!</returns>
    public static Vector2    HermiteGetNormal   (List<Vector2> aSegment, int i, float aPercentage, bool aClosed, float aTension = 0, float aBias = 0) {
        Vector2 tangent = HermiteGetPtTangent(aSegment, i, Mathf.Clamp01(aPercentage), aClosed, aTension, aBias);
        return new Vector2(tangent.y, -tangent.x).normalized;
    }
    /// <param name="aSegment">The list of vertices used to calculate the point.</param>
    /// <param name="i">Index of the vertex to start from.</param>
    /// <param name="aPercentage">How far between this vertex and the next should we look?</param>
    /// <param name="aClosed">Should we interpolate at the edges of the path as though it was closed?</param>
    /// <returns>A Hermite interpolated point.</returns>
    public  static Vector2   HermiteGetPt       (List<Vector2> aSegment, int i, float aPercentage, bool aClosed, float aTension = 0, float aBias = 0)
    {
        int a1 = aClosed ?  i - 1 < 0 ? aSegment.Count - 2 : i - 1 : Mathf.Clamp(i - 1, 0, aSegment.Count - 1);
        int a2 = i;
        int a3 = aClosed ? (i + 1) % (aSegment.Count) : Mathf.Clamp(i + 1, 0, aSegment.Count - 1);
        int a4 = aClosed ? (i + 2) % (aSegment.Count) : Mathf.Clamp(i + 2, 0, aSegment.Count - 1);

        return new Vector2(
            Hermite(aSegment[a1].x, aSegment[a2].x, aSegment[a3].x, aSegment[a4].x, aPercentage, aTension, aBias),
            Hermite(aSegment[a1].y, aSegment[a2].y, aSegment[a3].y, aSegment[a4].y, aPercentage, aTension, aBias));
    }
    /// <param name="aSegment">The list of vertices used to calculate the point.</param>
    /// <param name="i">Index of the vertex to start from.</param>
    /// <param name="aPercentage">How far between this vertex and the next should we look?</param>
    /// <param name="aClosed">Should we interpolate at the edges of the path as though it was closed?</param>
    /// <returns>The tangent at a Hermite interpolated point. (not normalized)</returns>
    public  static Vector2   HermiteGetPtTangent(List<Vector2> aSegment, int i, float aPercentage, bool aClosed, float aTension = 0, float aBias = 0) {
        int a1 = aClosed ? i - 1 < 0 ? aSegment.Count - 2 : i - 1 : Mathf.Clamp(i - 1, 0, aSegment.Count - 1);
        int a2 = i;
        int a3 = aClosed ? (i + 1) % (aSegment.Count) : Mathf.Clamp(i + 1, 0, aSegment.Count - 1);
        int a4 = aClosed ? (i + 2) % (aSegment.Count) : Mathf.Clamp(i + 2, 0, aSegment.Count - 1);

        return new Vector2(
            HermiteSlope(aSegment[a1].x, aSegment[a2].x, aSegment[a3].x, aSegment[a4].x, aPercentage, aTension, aBias),
            HermiteSlope(aSegment[a1].y, aSegment[a2].y, aSegment[a3].y, aSegment[a4].y, aPercentage, aTension, aBias));
    }
    /// <param name="aSegment">The list of floats used to calculate the result.</param>
    /// <param name="i">Index of the float to start from.</param>
    /// <param name="aPercentage">How far between this float and the next should we look?</param>
    /// <param name="aClosed">Should we interpolate at the edges of the path as though it was closed?</param>
    /// <returns>The tangent at a Hermite interpolated point. (not normalized)</returns>
    public  static float     HermiteGetFloat    (List<float  > aSegment, int i, float aPercentage, bool aClosed, float aTension = 0, float aBias = 0) {
        int a1 = aClosed ? i - 1 < 0 ? aSegment.Count - 2 : i - 1 : Mathf.Clamp(i - 1, 0, aSegment.Count - 1);
        int a2 = i;
        int a3 = aClosed ? (i + 1) % (aSegment.Count) : Mathf.Clamp(i + 1, 0, aSegment.Count - 1);
        int a4 = aClosed ? (i + 2) % (aSegment.Count) : Mathf.Clamp(i + 2, 0, aSegment.Count - 1);

        return Hermite(aSegment[a1], aSegment[a2], aSegment[a3], aSegment[a4], aPercentage, aTension, aBias);
    }

    private static float Cubic       (float v1, float v2, float v3, float v4, float aPercentage)
    {
        float percentageSquared = aPercentage * aPercentage;
        float a1 = v4 - v3 - v1 + v2;
        float a2 = v1 - v2 - a1;
        float a3 = v3 - v1;
        float a4 = v2;

        return (a1 * aPercentage * percentageSquared + a2 * percentageSquared + a3 * aPercentage + a4);
    }
    private static float Linear      (float v1, float v2,                     float aPercentage)
    {
        return v1 + (v2 - v1) * aPercentage;
    }
    private static float Hermite     (float v1, float v2, float v3, float v4, float aPercentage, float aTension, float aBias)
    {
        float mu2 = aPercentage * aPercentage;
        float mu3 = mu2 * aPercentage;
        float m0 = (v2 - v1) * (1 + aBias) * (1 - aTension) / 2 + (v3 - v2) * (1 - aBias) * (1 - aTension) / 2;
        float m1 = (v3 - v2) * (1 + aBias) * (1 - aTension) / 2 + (v4 - v3) * (1 - aBias) * (1 - aTension) / 2;
        float a0 = 2 * mu3 - 3 * mu2 + 1;
        float a1 = mu3 - 2 * mu2 + aPercentage;
        float a2 = mu3 - mu2;
        float a3 = -2 * mu3 + 3 * mu2;

        return (a0 * v2 + a1 * m0 + a2 * m1 + a3 * v3);
    }
    private static float HermiteSlope(float v1, float v2, float v3, float v4, float aPercentage, float aTension, float aBias) {
        float mu2 = aPercentage * aPercentage;
        float m0 = ((1 - aTension) * (aBias + 1) * (v2 - v1) + (1 - aBias) * (1 - aTension) * (v3 - v2)) / 2;
        float m1 = ((1 - aTension) * (aBias + 1) * (v3 - v2) + (1 - aBias) * (1 - aTension) * (v4 - v3)) / 2;
        float a0 = (3 * mu2 - 4 * aPercentage + 1);
        float a1 = (3 * mu2 - 2 * aPercentage);
        float a2 = v2 * (6 * mu2 - 6 * aPercentage);
        float a3 = v3 * (6 * aPercentage - 6 * mu2);

        return (a0 * m0 + a1 * m1 + a2 + a3);
    }

    /// <summary>
    /// Gets the direction enum of a line segment.
    /// </summary>
    /// <param name="aOne">First vertex in the line segment.</param>
    /// <param name="aTwo">Second vertex in the line segment.</param>
    /// <returns>Direction enum!</returns>
    public static Ferr2DT_TerrainDirection GetDirection (Vector2 aOne, Vector2 aTwo)
    {
        Vector2 dir = aOne - aTwo;
        dir = new Vector2(-dir.y, dir.x);
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x < 0) return Ferr2DT_TerrainDirection.Left;
            else return Ferr2DT_TerrainDirection.Right;
        }
        else
        {
            if (dir.y < 0) return Ferr2DT_TerrainDirection.Bottom;
            else return Ferr2DT_TerrainDirection.Top;
        }
    }
    /// <summary>
    /// Gets the direction enum of a line segment, invertable!
    /// </summary>
    /// <param name="aSegment">list of vertices to pick from</param>
    /// <param name="i">First vertex to use as the ine segment, next is i+1 (or i-1 if i+1 is outside the array)</param>
    /// <param name="aInvert">Flip the direction around?</param>
    /// <returns>Direction enum!</returns>
    public static Ferr2DT_TerrainDirection GetDirection (List<Vector2> aSegment, int i, bool aInvert, bool aClosed = false, List<Ferr2DT_TerrainDirection> aOverrides = null)
    {
        if (aSegment.Count <= 0) return Ferr2DT_TerrainDirection.Top;

        int next = i+1;
        if (i < 0) {
            if (aClosed) {
                i    = aSegment.Count-2;
                next = 0;
            } else {
                i=0;
                next = 1;
            }
        }

        if (aOverrides != null && aOverrides.Count >= aSegment.Count && aOverrides[i] != Ferr2DT_TerrainDirection.None) return aOverrides[i];

        Vector2 dir = aSegment[next > aSegment.Count-1? (aClosed? aSegment.Count-1 : i-1) : next] - aSegment[i];
        dir         = new Vector2(-dir.y, dir.x);
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x < 0) return aInvert ? Ferr2DT_TerrainDirection.Right : Ferr2DT_TerrainDirection.Left;
            else           return aInvert ? Ferr2DT_TerrainDirection.Left  : Ferr2DT_TerrainDirection.Right;
        }
        else
        {
            if (dir.y < 0) return aInvert ? Ferr2DT_TerrainDirection.Top    : Ferr2DT_TerrainDirection.Bottom;
            else           return aInvert ? Ferr2DT_TerrainDirection.Bottom : Ferr2DT_TerrainDirection.Top;
        }
    }
    // <summary>
    /// Gets the direction enum of a line segment, invertable!
    /// </summary>
    /// <param name="aSegment">list of vertices to pick from</param>
    /// <param name="i">First vertex to use as the ine segment, next is i+1 (or i-1 if i+1 is outside the array)</param>
    /// <param name="aInvert">Flip the direction around?</param>
    /// <returns>Direction enum!</returns>
    public static Ferr2DT_TerrainDirection GetDirection (List<Vector2> aPath, List<int> aSegment, int i, bool aInvert, bool aClosed = false, List<Ferr2DT_TerrainDirection> aOverrides = null) {
        if (aSegment.Count <= 0) return Ferr2DT_TerrainDirection.Top;

        int next = i + 1;
        if (i < 0) {
            if (aClosed) {
                i = aSegment.Count - 2;
                next = 0;
            } else {
                i = 0;
                next = 1;
            }
        }

        if (aOverrides != null && aOverrides.Count >= aSegment.Count && aOverrides[i] != Ferr2DT_TerrainDirection.None) return aOverrides[i];

        Vector2 dir = aPath[aSegment[next > aSegment.Count - 1 ? (aClosed ? aSegment.Count - 1 : i - 1) : next]] - aPath[aSegment[i]];
        dir = new Vector2(-dir.y, dir.x);
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) {
            if (dir.x < 0) return aInvert ? Ferr2DT_TerrainDirection.Right : Ferr2DT_TerrainDirection.Left;
            else return aInvert ? Ferr2DT_TerrainDirection.Left : Ferr2DT_TerrainDirection.Right;
        } else {
            if (dir.y < 0) return aInvert ? Ferr2DT_TerrainDirection.Top : Ferr2DT_TerrainDirection.Bottom;
            else return aInvert ? Ferr2DT_TerrainDirection.Bottom : Ferr2DT_TerrainDirection.Top;
        }
    }
    /// <summary>
    /// Checks to see if a vertex is at a corner. Ends are not considered corners.
    /// </summary>
    /// <param name="aSegment">A list of vertices to pick from.</param>
    /// <param name="i">Index of the vertex we're checking.</param>
    /// <returns>Is it a corner? Ends are not corners.</returns>
    public static bool                     IsSplit      (List<Vector2> aSegment, int i, List<Ferr2DT_TerrainDirection> aOverrides = null)
    {
        if (i == 0 || i == aSegment.Count - 1) return false;
        if (aOverrides!= null && aOverrides.Count < aSegment.Count) aOverrides = null;

        Ferr2DT_TerrainDirection dir1 = aOverrides == null ? 
            GetDirection(aSegment[i], aSegment[i - 1]) :
            aOverrides[i-1] == Ferr2DT_TerrainDirection.None ? 
                GetDirection(aSegment[i], aSegment[i - 1]) : 
                aOverrides[i-1];

        Ferr2DT_TerrainDirection dir2 = aOverrides == null ? 
            GetDirection(aSegment[i + 1], aSegment[i]) :
            aOverrides[i] == Ferr2DT_TerrainDirection.None ? 
                GetDirection(aSegment[i + 1], aSegment[i]) : 
                aOverrides[i];

        return dir1 != dir2;
    }
    /// <summary>
    /// Splits a path up based on corners. Corner verts are included in both segments when split.
    /// </summary>
    /// <param name="aPath">The list of path points to split.</param>
    /// <returns>An array of path segments.</returns>
    public static List<List<int>>          GetSegments  (List<Vector2> aPath, out List<Ferr2DT_TerrainDirection> aSegDirections, List<Ferr2DT_TerrainDirection> aOverrides = null, bool aInvert = false, bool aClosed = false)
    {
        List<List<int>> segments    = new List<List<int>>();
        List<int      > currSegment = new List<int      >();
        aSegDirections = new List<Ferr2DT_TerrainDirection>();
        int startIndex = 0;

        for (int i = 0; i < aPath.Count; i++)
        {
            currSegment.Add(i);
            if (IsSplit(aPath, i, aOverrides))
            {
                segments.Add(currSegment);
                aSegDirections.Add(GetDirection(aPath, startIndex, aInvert, aClosed, aOverrides));

                currSegment = new List<int>();
                currSegment.Add(i);
                startIndex = i;
            }
        }
        segments.Add(currSegment);
        aSegDirections.Add(GetDirection(aPath, startIndex, aInvert, aClosed, aOverrides));
        return segments;
    }
    /// <summary>
    /// Smooths a segment of path points.
    /// </summary>
    /// <param name="aSegment">The collection of points to smooth out.</param>
    /// <param name="aSplitDistance">How far should each smooth split be?</param>
    /// <param name="aClosed">Should we close the segment?</param>
    /// <returns>A new list of vertices.</returns>
    public static List<Vector2>            SmoothSegment(List<Vector2> aSegment, float aSplitDistance, bool aClosed)
    {
        List<Vector2> result = new List<Vector2>(aSegment);
        int           curr   = 0;
        int           count  = aClosed ? aSegment.Count : aSegment.Count - 1;
        for (int i = 0; i < count; i++)
        {
            int next   = i == count - 1 ? aClosed ? 0 : aSegment.Count-1 : i+1;
            int splits = (int)(Vector2.Distance(aSegment[i], aSegment[next]) / aSplitDistance);
            for (int t = 0; t < splits; t++)
            {
                float percentage = (float)(t + 1) / (splits + 1);
                result.Insert(curr + 1, HermiteGetPt(aSegment, i, percentage, aClosed));
                curr += 1;
            }
            curr += 1;
        }
        return result;
    }
    /// <summary>
    /// Smooths a segment of path points and scales.
    /// </summary>
    /// <param name="aSegment">The collection of points to smooth out.</param>
    /// <param name="aSplitDistance">How far should each smooth split be?</param>
    /// <param name="aClosed">Should we close the segment?</param>
    public static void                     SmoothSegment(List<Vector2> aSegment, List<float> aScales, float aSplitDistance, bool aClosed, out List<Vector2> aNewSegment, out List<float> aNewScales) {
        List<Vector2> newSegment = new List<Vector2>(aSegment);
        List<float  > newScales  = new List<float  >(aScales );

        int curr  = 0;
        int count = aClosed ? aSegment.Count : aSegment.Count - 1;
        for (int i = 0; i < count; i++) {
            int next   = i == count - 1 ? aClosed ? 0 : aSegment.Count - 1 : i + 1;
            int splits = (int)(Vector2.Distance(aSegment[i], aSegment[next]) / aSplitDistance);
            for (int t = 0; t < splits; t++) {
                float   percentage = (float)(t + 1) / (splits + 1);
                Vector2 pt         = HermiteGetPt   (aSegment, i, percentage, aClosed);
                float   scale      = HermiteGetFloat(aScales,  i, percentage, aClosed);
                newSegment.Insert(curr + 1, new Vector2(pt.x, pt.y));
                newScales .Insert(curr + 1, scale);
                curr += 1;
            }
            curr += 1;
        }

        aNewSegment = newSegment;
        aNewScales  = newScales;
    }
    /// <summary>
    /// This method will close a list of split segments, merging and adding points to the end chunks.
    /// </summary>
    /// <param name="aSegmentList">List of split segments that make up the path.</param>
    /// <param name="aCorners">If there are corners or not.</param>
    /// <returns>A closed loop of segments.</returns>
    public static bool                     CloseEnds    (List<Vector2> aPath, ref List<List<int>> aSegmentList, ref List<Ferr2DT_TerrainDirection> aSegmentDirections, bool aCorners, bool aInverted)
    {
        int     startID   = aSegmentList[0][0];
        Vector2 start     = aPath[startID];
        Vector2 startNext = aPath[aSegmentList[0][1]];

        int     endID   = aSegmentList[aSegmentList.Count - 1][aSegmentList[aSegmentList.Count - 1].Count - 1];
        Vector2 end     = aPath[endID];
        Vector2 endPrev = aPath[aSegmentList[aSegmentList.Count - 1][aSegmentList[aSegmentList.Count - 1].Count - 2]];

        if (aCorners == false) {
            aSegmentList[0].Add(startID);
            return true;
        }

        bool endCorner   = Ferr2D_Path.GetDirection(endPrev, end  ) != Ferr2D_Path.GetDirection(end,   start    );
        bool startCorner = Ferr2D_Path.GetDirection(end,     start) != Ferr2D_Path.GetDirection(start, startNext);

        if (endCorner && startCorner) {
            List<int> lastSeg = new List<int>();
            lastSeg.Add(endID  );
            lastSeg.Add(startID);

            aSegmentList.Add(lastSeg);

            Ferr2DT_TerrainDirection dir = GetDirection(start, end);
            if (aInverted && dir == Ferr2DT_TerrainDirection.Top   ) dir = Ferr2DT_TerrainDirection.Bottom;
            if (aInverted && dir == Ferr2DT_TerrainDirection.Bottom) dir = Ferr2DT_TerrainDirection.Top;
            if (aInverted && dir == Ferr2DT_TerrainDirection.Right ) dir = Ferr2DT_TerrainDirection.Left;
            if (aInverted && dir == Ferr2DT_TerrainDirection.Left  ) dir = Ferr2DT_TerrainDirection.Right;
            
            aSegmentDirections.Add(dir);
        }
        else if (endCorner && !startCorner) {
            aSegmentList[0].Insert(0, endID);
        }
        else if (!endCorner && startCorner) {
            aSegmentList[aSegmentList.Count - 1].Add(startID);
        }
        else {
            aSegmentList[0].InsertRange(0, aSegmentList[aSegmentList.Count - 1]);
            aSegmentList      .RemoveAt(aSegmentList      .Count - 1);
            aSegmentDirections.RemoveAt(aSegmentDirections.Count - 1);
        }
        return true;
    }
    /// <summary>
    /// Converts a list of indices to a list of data created from the given list.
    /// </summary>
    /// <param name="aPath">Path to pull data from</param>
    /// <param name="aIndices">List of indices to pull from the data.</param>
    /// <returns>A list of data as indicated by the index list</returns>
    public static List<T>                 IndicesToList<T>(List<T> aData, List<int> aIndices) {
        List<T> result = new List<T>(aIndices.Count);
        for (int i = 0; i < aIndices.Count; i++) {
            result.Add(aData[aIndices[i]]);
        }
        return result;
    }
    /// <summary>
    /// Converts a list of path indices to a list of path verts and scales created from the given list.
    /// </summary>
    /// <param name="aPath">Path to pull vert data from</param>
    /// <param name="aScales">List to pull scales data from</param>
    /// <param name="aIndices">List of inidces to pull from the path.</param>
    public static void                     IndicesToPath(List<Vector2> aPath, List<float> aScales, List<int> aIndices, out List<Vector2> aNewPath, out List<float> aNewScales) {
        aNewPath   = new List<Vector2>(aIndices.Count);
        aNewScales = new List<float  >();
        for (int i = 0; i < aIndices.Count; i++) {
            aNewPath  .Add(aPath  [aIndices[i]]);
            if (aIndices[i] >= aScales.Count) aNewScales.Add(1);
            else aNewScales.Add(aScales[aIndices[i]]);
        }
    }

    /// <summary>
    /// A utility function! Gets the closest point on a line, clamped to the line segment provided.
    /// </summary>
    /// <param name="aStart">Start of the line segment.</param>
    /// <param name="aEnd">End of the line segment.</param>
    /// <param name="aPoint">The point to compare distance to.</param>
    /// <param name="aClamp">Should we clamp at the ends of the segment, or treat it as an infinite line?</param>
    /// <returns>The closest point =D</returns>
    public static Vector2 GetClosetPointOnLine   (Vector2 aStart, Vector2 aEnd, Vector2 aPoint, bool aClamp)
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
	public static Rect    GetBounds              (List<Vector2> aFrom) {
        if (aFrom == null || aFrom.Count <= 0) return new Rect(0, 0, 0, 0);

		float l = float.MaxValue;
		float r = float.MinValue;
		float t = float.MinValue;
		float b = float.MaxValue;

		for (int i = 0; i < aFrom.Count; i++) {
			if (aFrom[i].x > r) r = aFrom[i].x;
			if (aFrom[i].x < l) l = aFrom[i].x;
			if (aFrom[i].y < b) b = aFrom[i].y;
			if (aFrom[i].y > t) t = aFrom[i].y;
		}

		return new Rect(l, t, r-l, b-t);
	}
    public static Vector2 LineIntersectionPoint  (Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2) {
        // Get A,B,C of first line - points : ps1 to pe1
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        float C1 = A1 * ps1.x + B1 * ps1.y;

        // Get A,B,C of second line - points : ps2 to pe2
        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;
        float C2 = A2 * ps2.x + B2 * ps2.y;

        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
            return Vector3.zero;

        // now return the Vector2 intersection point
        return new Vector2(
            (B2 * C1 - B1 * C2) / delta,
            (A1 * C2 - A2 * C1) / delta
        );
    }
    public static bool    LineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {

        Vector2 a = p2 - p1;
        Vector2 b = p3 - p4;
        Vector2 c = p1 - p3;

        float alphaNumerator   = b.y * c.x - b.x * c.y;
        float alphaDenominator = a.y * b.x - a.x * b.y;
        float betaNumerator    = a.x * c.y - a.y * c.x;
        float betaDenominator  = alphaDenominator; /*2013/07/05, fix by Deniz*/

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
    public static bool    LineSegmentIntersectionNoOverlap(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {

        Vector2 a = p2 - p1;
        Vector2 b = p3 - p4;
        Vector2 c = p1 - p3;

        float alphaNumerator   = b.y * c.x - b.x * c.y;
        float alphaDenominator = a.y * b.x - a.x * b.y;
        float betaNumerator    = a.x * c.y - a.y * c.x;
        float betaDenominator  = alphaDenominator; /*2013/07/05, fix by Deniz*/

        bool doIntersect = true;

        if (alphaDenominator == 0 || betaDenominator == 0) {
            doIntersect = false;
        } else {

            if (alphaDenominator >= 0) {
                if (alphaNumerator <= 0 || alphaNumerator >= alphaDenominator) {
                    doIntersect = false;
                }
            } else if (alphaNumerator >= 0 || alphaNumerator <= alphaDenominator) {
                doIntersect = false;
            }

            if (doIntersect && betaDenominator >= 0) {
                if (betaNumerator <= 0 || betaNumerator >= betaDenominator) {
                    doIntersect = false;
                }
            } else if (betaNumerator >= 0 || betaNumerator <= betaDenominator) {
                doIntersect = false;
            }
        }

        return doIntersect;
    }
	
	/// <summary>
    /// Gets the smallest distance to the provided path
    /// </summary>
    /// <param name="aPoint">The point to check from.</param>
    /// <returns>Smallest distance! float.MaxValue if none found</returns>
	public static float GetDistanceFromPath(List<Vector2> aPath, Vector2 aPoint, bool aClosed) {
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
		return Mathf.Sqrt(dist);
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
		Vector2 dir = aPath[aClosed ? (i+1) % aPath.Count : Mathf.Min(i+1, aPath.Count-1)] - aPath[!aClosed&&i==aPath.Count-1?aPath.Count-2:i];
		return new Vector2(-dir.y, dir.x).normalized;
	}

	/// <summary>
	/// Calculates the length of a path by summing the distances of its segments.
	/// </summary>
	/// <param name="aPath">A list of consecutive points.</param>
	/// <param name="aClosed">Should we include the segment between the first and the last point?</param>
	/// <returns>Length of the path, 0 if null or only one point.</returns>
	public static float GetSegmentLength(List<Vector2> aPath, bool aClosed = false) {
		if (aPath == null || aPath.Count <= 1)
			return 0;

		float result = 0;
		for (int i = 0; i < aPath.Count-1; i++) {
			result += Vector2.Distance(aPath[i], aPath[i+1]);
		}
		if (aClosed)
			result += Vector2.Distance(aPath[0], aPath[aPath.Count-1]);

		return result;
	}

	public static float GetSegmentLengthToIndex(List<Vector2> aPath, int aIndex) {
		if (aPath == null || aPath.Count <= 1)
			return 0;

		float result = 0;
		for (int i = 0; i < aPath.Count-1 && i < aIndex; i++) {
			result += Vector2.Distance(aPath[i], aPath[i+1]);
		}

		return result;
	}

	public static Vector2 LinearGetPt(List<Vector2> aPath, int aIndex, float aPercent, bool aClosed) {
		if (aPath == null)
			return Vector2.zero;
		if (aPath.Count <= 1)
			return aPath[0];

		int curr = Mathf.Clamp(aIndex, 0, aPath.Count-1);
		int next = aClosed ? (curr + 1) % aPath.Count : Mathf.Min(aPath.Count-1, curr+1);

		return Vector2.LerpUnclamped(aPath[curr], aPath[next], aPercent);
	}

	public static Vector2 LinearGetNormal(List<Vector2> aPath, int aIndex, float aPercent, bool aClosed) {
		if (aPath == null)
			return Vector2.zero;
		if (aPath.Count <= 1)
			return aPath[0];

		int curr = Mathf.Clamp(aIndex, 0, aPath.Count-1);
		int next = aClosed ? (curr + 1) % aPath.Count : Mathf.Min(aPath.Count-1, curr+1);

		return Vector2.Lerp(GetNormal(aPath, curr, aClosed), GetNormal(aPath, next, aClosed), aPercent);
	}

	public static void PathGlobalPercentToLocal(List<Vector2> aPath, float aPercent, out int aLocalPoint, out float aLocalPercent, float aPathLength = 0, bool aClosed = false) {
		if (aPercent>=1) {
			if (aClosed)
				aPercent = aPercent - (int)aPercent;
			else {
				aLocalPoint = Mathf.Max(0,aPath.Count-2);
				aLocalPercent = 1;
				return;
			}
		}
		aLocalPoint         = 0;
		aLocalPercent       = 0;
		float pathLength    = aPathLength == 0 ? GetSegmentLength(aPath) : aPathLength;
		float percentLength = pathLength * aPercent;
		
		float currLength = 0;
		int   count      = aClosed ? aPath.Count : aPath.Count-1;
		for (int i = 0; i < count; i++) {
			int   next      = (i+1) % aPath.Count;
			float segLength = Vector2.Distance(aPath[i], aPath[next]);
			
			if (currLength+segLength >= percentLength) {
				aLocalPoint = i;
				aLocalPercent = (percentLength - currLength) / segLength;
				break;
			}
			currLength += segLength;
		}
	}
	#endregion
}
