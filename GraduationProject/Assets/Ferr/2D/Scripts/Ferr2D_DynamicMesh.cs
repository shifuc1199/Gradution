using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I recommend agains using this for your own mesh creation, it's a little quirky, and pretty specific
/// to the path stuff in Ferr2DTerrain. But it's a utility to make mesh creation easier for the path stuff.
/// </summary>
public class Ferr2D_DynamicMesh
{
    #region Fields and Properties
    List<Vector3> mVerts;
	List<int    > mIndices;
	List<Vector2> mUVs;
    List<Vector2> mUV2s;
    List<Vector4> mTans;
    List<Vector3> mNorms;

    /// <summary>
    /// The number of vertices currently in the mesh.
    /// </summary>
    public int VertCount
    {
        get { return mVerts.Count; }
    }
    #endregion

    #region Constructor
    public Ferr2D_DynamicMesh() {
		mVerts   = new List<Vector3>();
		mUVs     = new List<Vector2>();
		mIndices = new List<int>();
        mTans    = new List<Vector4>();
        mNorms   = new List<Vector3>();
	}
    #endregion

    #region General Methods
    /// <summary>
    /// Clears all verices, indices, uvs, and colors from this mesh, resets color to white.
    /// </summary>
    public void  Clear                 ()
    {
        mVerts  .Clear();
        mIndices.Clear();
        mUVs    .Clear();
        mTans   .Clear();
        mNorms  .Clear();
        if (Application.isEditor) {
        	mUV2s = null;
        } else if (mUV2s != null) {
        	mUV2s.Clear();
        }
    }
    /// <summary>
    /// Clears out the mesh, fills in the data, and recalculates normals and bounds.
    /// </summary>
    /// <param name="aMesh">An already existing mesh to fill out.</param>
    public void  Build                 (ref Mesh aMesh, bool aCalculateTangents)
    {
    	// round off a few decimal points to try and get better pixel-perfect results
    	/*for(int i=0;i<mVerts.Count;i+=1) mVerts[i] = new Vector3(
    		(float)System.Math.Round(mVerts[i].x, 3),
    		(float)System.Math.Round(mVerts[i].y, 3),
    		(float)System.Math.Round(mVerts[i].z, 3));*/
			
        aMesh.Clear();
        aMesh.SetVertices (mVerts);
        aMesh.SetUVs      (0, mUVs);
        aMesh.SetTriangles(mIndices, 0);
        aMesh.SetNormals  (mNorms);
        if (mUV2s != null) {
			aMesh.SetUVs(1, mUV2s);
        }
        if (mNorms.Count == 0)
			aMesh.RecalculateNormals();
        if (aCalculateTangents && mTans.Count == 0) {
            aMesh.RecalculateTangents();
        } else {
            aMesh.tangents = null;
        }
        aMesh.RecalculateBounds ();
    }
    /// <summary>
    /// This extrude is pretty specific to the Ferr2DT path stuff, but it extrudes a 2D mesh out a certain
    /// distance, for use with collision meshes.
    /// </summary>
    /// <param name="aDist">How far on the Z axis to extrude.</param>
    /// <param name="aInverted">If this is the mesh of an inverted terrain, it should behave differently.</param>
    public void  ExtrudeZ              (float    aDist, bool aInverted)
    {
        int count    = mVerts.Count;
        int indCount = mIndices.Count;

        mUVs.AddRange(mUVs.ToArray());

        Vector3 off = new Vector3(0,0, aDist);
        for (int i = 0; i < count; i++)
        {
            mVerts[i] -= off/2;
        }
        for (int i = 0; i < count; i++)
        {
            mVerts.Add(mVerts[i] + off);
        }
        
        for (int i = 0; i < indCount; i += 3)
        {
            mIndices.Add(mIndices[i+2] + count);
            mIndices.Add(mIndices[i+1] + count);
            mIndices.Add(mIndices[i  ] + count);
        }

        int edges = count - 1;
        for (int i = 0; i < edges; i++)
        {
            if (aInverted) AddFace(i, i + count,i + count + 1, i+1);
            else           AddFace(i + 1, i + count + 1, i + count, i);
        }

        if (aInverted) AddFace(count - 1, (count - 1) + count, count, 0);
        else           AddFace(0, count, (count - 1) + count, count - 1);
    }
    /// <summary>
    /// Removes any faces that match the given normal, within the tolerance specified. No verts are deleted, just faces.
    /// </summary>
    /// <param name="aFacing">Normalized direction to delete with</param>
    /// <param name="aDegreesTolerance">Angle of tolerance, in degrees</param>
    public void RemoveFaces(Vector3 aFacing, float aDegreesTolerance) 
    {
        for (int i = 0; i < mIndices.Count; i+=3)
        {
            Vector3 norm = Vector3.Cross(mVerts[mIndices[i+1]] - mVerts[mIndices[i]], mVerts[mIndices[i+2]] - mVerts[mIndices[i]]);
            norm.Normalize();
            if (Vector3.Angle(norm, aFacing) < aDegreesTolerance) {
                mIndices.RemoveRange(i,3);
                i-=3;
            }
        }
    }
    /// <summary>
    /// Gets the current list of triangles.
    /// </summary>
    /// <param name="aStart">An offset to start from.</param>
    /// <returns></returns>
	public int[] GetCurrentTriangleList(int      aStart = 0) {
		int[] result = new int[mIndices.Count - aStart];
		int   curr   = 0;
		for (int i = aStart; i < mIndices.Count; i++) {
			result[curr] = mIndices[i];
			curr+=1;
		}
		return result;
	}

	/// <summary>
	/// Returns the vert at the indicated index. Index isn't checked for validity.
	/// </summary>
	/// <param name="aIndex">Value between 0 and number of verts in the mesh.</param>
	/// <returns>Vertex from the indicated index</returns>
	public Vector3 GetVert(int aIndex) {
		return mVerts[aIndex];
	}

    #endregion 

    #region Vertex and Face Methods
    public int AddVertex(float aX, float aY, float aZ, float aU, float aV) {
	    return AddVertex(new Vector3(aX, aY, aZ), new Vector2(aU,aV));
    }
	public int AddVertex(Vector3 aVert) {
		return AddVertex(aVert, new Vector2(0,0));
	}
	public int AddVertex(Vector2 aVert, float aZ, Vector2 aUV) {
		return AddVertex(new Vector3(aVert.x, aVert.y, aZ), aUV);
	}
	public int AddVertex(Vector3 aPos, Vector3 aUV) {
		mVerts.Add(aPos);
		mUVs  .Add(aUV);
		
		if (mUV2s        != null) mUV2s .Add(new Vector3(0, 0, -1));
		if (mNorms.Count != 0   ) mNorms.Add(new Vector3(0, 0, 1));
		if (mTans .Count != 0   ) mTans .Add(new Vector4(1, 0, 0, 1));
		
		return mVerts.Count-1;
	}
	
    public void CheckAllVerts() {
        for (int i = 0; i < mVerts.Count; i++) {
            CheckVert(i);
        }
    }
    void CheckVert(int i) {
        BadVertCheck(mVerts[i]);
        BadVertCheck(mUVs[i]);
        if (mUV2s  != null) BadVertCheck(mUV2s[i]);
        if (mNorms.Count != 0) BadVertCheck(mNorms[i]);
        if (mTans.Count  != 0) BadVertCheck(mTans[i]);
    }
    static void BadVertCheck(Vector3 aVert) {
        if (float.IsInfinity(aVert.x) || 
            float.IsInfinity(aVert.y) || 
            float.IsInfinity(aVert.z)) {
            Debug.Log("Infinity vert");
        }

        if (float.IsNaN(aVert.x) || 
            float.IsNaN(aVert.y) || 
            float.IsNaN(aVert.z)) {
            Debug.Log("NaN vert");
        }
    }
	
	public void AddFace(int aV1, int aV2, int aV3) {
		mIndices.Add (aV1);
		mIndices.Add (aV2);
		mIndices.Add (aV3);
	}
	public void AddFace(int aV1, int aV2, int aV3, int aV4) {
		mIndices.Add (aV3);
		mIndices.Add (aV2);
		mIndices.Add (aV1);
		
		mIndices.Add (aV4);
		mIndices.Add (aV3);
		mIndices.Add (aV1);
	}
    private void SetupNormals() {
        if (mUV2s == null) mUV2s = new List<Vector2>();
        for (int i = 0; i < mVerts.Count; i++) {
            mUV2s.Add(new Vector3(0, 0));
        }
    }
    #endregion
}