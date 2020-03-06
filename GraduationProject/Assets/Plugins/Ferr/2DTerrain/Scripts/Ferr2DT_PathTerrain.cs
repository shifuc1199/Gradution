using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using Ferr;
using FerrPoly2Tri;
using ClipperLibFerr;
using Ferr.Extensions;

[Serializable]
public struct Ferr2D_PointData : ILerpable<Ferr2D_PointData> {
	public int       directionOverride;
	public List<int> cutOverrides;
	public float     scale;

	public Ferr2D_PointData(float aScale=1, int aDirectionOverride = (int)Ferr2DT_TerrainDirection.None) {
		directionOverride = aDirectionOverride;
		cutOverrides      = new List<int>();
		scale             = aScale;
	}

	public Ferr2D_PointData Lerp(Ferr2D_PointData aWith, float aLerp) {
		Ferr2D_PointData result = new Ferr2D_PointData(Mathf.Lerp(scale, aWith.scale, EaseInOut(aLerp)), directionOverride);
		result.cutOverrides = cutOverrides;
		return result;
	}

	float EaseInOut(float aTime) {
		return -.5f * (Mathf.Cos(Mathf.PI*aTime/1f) - 1);
	}
}
[Serializable]
public class Ferr2DPath : Path2D<Ferr2D_PointData> { }

public enum Ferr2D_SectionMode {
	Normal,
	Invert,
	None
}

public enum Ferr2D_ColliderMode {
	None      = 0,
	Polygon2D = 1,
	Edge2D    = 2,
	Mesh3D    = 3
}

public enum Ferr2D_CapColliderType {
	Rectangle,
	Circle,
	Connected,
	ConnectedSharp,
	IntersectionSharp,
	IntersectionTop,
	IntersectionEdge,
}

/// <summary>
/// Describes the way that Ferr2DT assigns vertex colors to the terrain.
/// </summary>
public enum Ferr2DT_ColorType {
    /// <summary>
    /// Assigns a single color to all verts.
    /// </summary>
	SolidColor,
    /// <summary>
    /// Assigns a color gradient across the terrain at a given angle.
    /// </summary>
	Gradient,
    /// <summary>
    /// Assigns a color gradient based on the vertex distance from the edge
    /// </summary>
	DistanceGradient,
    /// <summary>
    /// Preserves the existing colors as best as possible, for use with vertex painting and stuff like that.
    /// </summary>
	PreserveVertColor
}

[AddComponentMenu("Ferr2DT/Path Terrain"), RequireComponent (typeof (MeshFilter)), RequireComponent (typeof (MeshRenderer))]
public partial class Ferr2DT_PathTerrain : MonoBehaviour, Ferr2D_IPath, IBlendPaintable {
	const int cIntConvert = 1000;
	
	#region Public fields
    /// <summary>
    /// If fill is set to Skirt, this value represents the Y value of where the skirt will end.
    /// </summary>
	public float    fillY           = 0;
    /// <summary>
    /// In order to combat Z-Fighting, this allows you to set a Z-Offset on the fill.
    /// </summary>
	public float    fillZ           = 0.05f;
	/// <summary>
    /// When fill is inverted, how large is the outside border? Zero will auto-size the border.
    /// </summary>
	public Vector2  invertFillBorder = Vector2.zero;
    /// <summary>
    /// This will separate edges at corners, for applying different material parts to different slopes,
    /// as well as creating sharp corners on smoothed paths.
    /// </summary>
	public bool     splitCorners    = true;
    /// <summary>
    /// Roughly how many pixels we try to fit into one unit of Unity space
    /// </summary>
	[Clamp(1,768)]
	public float    pixelsPerUnit   = 64;
	/// <summary>
    /// Describes the way that Ferr2DT assigns vertex colors to the terrain.
    /// </summary>
	public Ferr2DT_ColorType vertexColorType = Ferr2DT_ColorType.SolidColor;
    /// <summary>
    /// The color for every vertex! If you use the right shader (like the Ferr2D shaders) this will influence
    /// the color of the terrain. This is faster, because you don't need additional materials for new colors!
    /// </summary>
	public Color    vertexColor     = Color.white;
    /// <summary>
    /// A gradient that starts at one end of the terrain, and ends at the other, following the angle provided
    /// by vertexGradientAngle.
    /// </summary>
	public Gradient vertexGradient  = null;
    /// <summary>
    /// Angle at which the gradient travels across the mesh (degrees).
    /// </summary>
	public float    vertexGradientAngle = 90;
	    /// <summary>
    /// Maximum distance for the gradient when using DistanceGradient vertex color type
    /// </summary>
	public float    vertexGradientDistance = 4;
    /// <summary>
    /// Tangents are important for normal mapping! Sadly, it's a tiny bit expensive, so I don't recommend doing it all the time!
    /// </summary>
    public bool     createTangents   = false;
    /// <summary>
    /// Offset from the global uv coordinates, for fine control over the fill location
    /// </summary>
    public Vector2  uvOffset;
    /// <summary>
    /// Z offset value for how slanted the edges should be. This can add a nice parallax effect to your terrain.
    /// </summary>
	[Range(-2, 2)]
	public float    slantAmount       = 0;
	/// <summary>
	/// Adds extra grid spaced verts to the fill mesh for use with vertex lighting or painting.
	/// </summary>
	public bool     fillSplit         = false;
	/// <summary>
	/// Distance between vert splits on the fill mesh.
	/// </summary>
	public float    fillSplitDistance = 4;
	/// <summary>
	/// Indicates the collider will be used by any effectors on this object
	/// </summary>
	public bool     usedByEffector    = false;
    /// <summary>
    /// Transfers over into the collider, when it gets generated
    /// </summary>
    public bool     isTrigger         = false;
    /// <summary>
    /// How wide should the collider be on the Z axis? (Unity units)
    /// </summary>
    public float    depth             = 4.0f;
	/// <summary>
    /// For terrains that have no edge, use this for physics!
    /// </summary>
    public PhysicMaterial physicsMaterial  = null;
	/// <summary>
	/// For terrains that have no edge, use this for physics!
	/// </summary>
	public PhysicsMaterial2D physicsMaterial2D = null;
	#endregion
	
	#region Private fields
	[SerializeField] Ferr2D_SectionMode  edgeMode     = Ferr2D_SectionMode.Normal;
	[SerializeField] Ferr2D_SectionMode  fillMode     = Ferr2D_SectionMode.Normal;
	[SerializeField] bool                useSkirt     = false;
	[SerializeField] Ferr2D_ColliderMode colliderMode = Ferr2D_ColliderMode.Polygon2D;
	[SerializeField] bool                fillCollider = false;

	[SerializeField] Ferr2DPath              pathData = new Ferr2DPath();
    [SerializeField] Ferr2DT_TerrainMaterial terrainMaterial;
	[SerializeField] UnityEngine.Object      terrainMaterialInterface;
	Ferr2D_DynamicMesh dMesh;
	RecolorTree        recolorTree = null;
	Vector2            unitsPerUV = Vector2.zero;
	#endregion

	#region Properties
	public Ferr2D_SectionMode  FillMode     { get { return fillMode; } set { fillMode = value; } }
	public Ferr2D_SectionMode  EdgeMode     { get { return edgeMode; } set { edgeMode = value; } }
	public Ferr2D_ColliderMode ColliderMode { get { return colliderMode; } set{ colliderMode = value; } }

	private Ferr2D_DynamicMesh DMesh { get {
        if (dMesh == null)
            dMesh = new Ferr2D_DynamicMesh();
        return dMesh;
    } }

	/// <summary>
    /// This property will call SetMaterial when set.
    /// </summary>
    public IFerr2DTMaterial TerrainMaterial { 
		get { 
	    	if (terrainMaterialInterface != null)
		    	return (IFerr2DTMaterial)terrainMaterialInterface;
		    terrainMaterialInterface = terrainMaterial;
	    	return (IFerr2DTMaterial)terrainMaterial;
	    } 
        set { SetMaterial(value); } 
    }
    /// <summary>
    /// Used by IProceduralMesh for saving. Just a call to GetComponent<MeshFilter>!
    /// </summary>
    public Mesh MeshData { get {
        return GetComponent<MeshFilter>().sharedMesh;
    } }
    /// <summary>
    /// Used by IProceduralMesh for saving. Just a call to GetComponent<MeshFilter>!
    /// </summary>
    public MeshFilter MeshFilter { get {
        return GetComponent<MeshFilter>();
    } }
	
	bool checkedLegacy = false;
	bool isLegacy      = false;
	/// <summary>
	/// Does this terrain use v1.12 path data or earlier?
	/// </summary>
	public bool IsLegacy { get {
		if (!checkedLegacy) {
			isLegacy      = GetComponent<Ferr2D_Path>() != null;
			checkedLegacy = true;
		}
		return isLegacy;
	} }
	/// <summary>
	/// Allows to see and reset whether we've checked it's a Legacy terrain. Useful for undo, or upgrade functionality.
	/// </summary>
	public bool CheckedLegacy { 
		get { return checkedLegacy; } 
		set { checkedLegacy = value; }
	}

	public Vector2 UnitsPerUV{ get { 
		if (unitsPerUV.x==0) {
			Material edgeMat = TerrainMaterial.edgeMaterial;
			if (edgeMat != null && edgeMat.mainTexture != null) {
				unitsPerUV.x = edgeMat.mainTexture.width  / pixelsPerUnit;
				unitsPerUV.y = edgeMat.mainTexture.height / pixelsPerUnit;
			} else {
				unitsPerUV.x = 32/pixelsPerUnit;
				unitsPerUV.y = 32/pixelsPerUnit;
			}
		} 
		return unitsPerUV; 
	} }
	public Ferr2DPath PathData{ get { return pathData; } }
	#endregion

	#region MonoBehaviour Methods
	void Awake() {
		if (IsLegacy) {
			LegacyAwake();
			return;
		}

        if (colliderMode != Ferr2D_ColliderMode.None && GetComponent<Collider>() == null && GetComponent<Collider2D>() == null) {
            RecreateCollider();
        }
        for (int i = 0; i < Camera.allCameras.Length; i++) {
            Camera.allCameras[i].transparencySortMode = TransparencySortMode.Orthographic;
        }
    }
    #endregion

    #region Creation methods
    /// <summary>
    /// This method gets called automatically whenever the Ferr2DT path gets updated in the 
    /// editor. This will completely recreate the the visual mesh (only) for the terrain. If you want
    /// To recreate the collider as well, that's a separate call to RecreateCollider.
    /// </summary>
    public  void Build    (bool aFullBuild = true) {
	    //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
	    //sw.Start();

	    if (IsLegacy) {
			LegacyBuild(aFullBuild);
			return;
		}

		if (TerrainMaterial == null) {
            Debug.LogWarning("Cannot create terrain without a Terrain Material!");
            return;
        }
        if (pathData.Count < 2) {
            GetComponent<MeshFilter>().sharedMesh = null;
            return;
        }

		// ensure upUV gets recalculated
		unitsPerUV = Vector2.zero;

		MarkColorSave();
		
		DMesh.Clear ();

		if (edgeMode != Ferr2D_SectionMode.None)
			AddEdge();
		int[] submeshEdge = DMesh.GetCurrentTriangleList();

		if (fillMode != Ferr2D_SectionMode.None)
			AddFill(useSkirt, fillSplit, !aFullBuild);
		int[] submeshFill = DMesh.GetCurrentTriangleList(submeshEdge.Length);

		// compile the mesh!
        Mesh m = GetComponent<MeshFilter>().sharedMesh = GetMesh();
	    DMesh.Build(ref m, createTangents && aFullBuild);
	    
	    CreateVertColors();

        // set up submeshes and submaterials
        if (submeshEdge.Length > 0 && submeshFill.Length > 0) {
            m.subMeshCount = 2;
            m.SetTriangles(submeshEdge, 1);
            m.SetTriangles(submeshFill, 0);
        } else if (submeshEdge.Length > 0) {
            m.subMeshCount = 1;
            m.SetTriangles(submeshEdge, 0);
        } else if (submeshFill.Length > 0) {
            m.subMeshCount = 1;
            m.SetTriangles(submeshFill, 0);
        }
		AddMaterials(submeshFill.Length>0, submeshEdge.Length>0);
	    
		bool hasCollider = GetComponent<MeshCollider>() != null || GetComponent<PolygonCollider2D>() != null || GetComponent<EdgeCollider2D>() != null;
	    if (colliderMode != Ferr2D_ColliderMode.None && hasCollider) {
		    RecreateCollider();
	    }

		if (aFullBuild) ClearColorSave();

	    //sw.Stop();
	    //Debug.Log("Creating mesh took: " + sw.Elapsed.TotalMilliseconds + "ms");
	}
	#endregion

	#region Asset Methods
	private Mesh   GetMesh() {
		MeshFilter filter  = GetComponent<MeshFilter>();
		string     newName = GetMeshName();
		Mesh       result  = filter.sharedMesh;
		
		if (IsPrefab()) {
#if UNITY_EDITOR
			if (filter.sharedMesh == null || filter.sharedMesh.name != newName) {
				string meshFolder = System.IO.Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(this)) + "/Meshes/";
				string path       = meshFolder + newName + ".asset";
				Mesh   assetMesh  = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;
				if (assetMesh != null) { 
					result = assetMesh;
				} else {
					path = meshFolder;
					string assetName = newName + ".asset";
					result = new Mesh();
					result.name = newName;
					
					if (!Directory.Exists(path)) {
						Directory.CreateDirectory(path);
					}
					try {
						UnityEditor.AssetDatabase.CreateAsset(result, path + assetName);
						UnityEditor.AssetDatabase.Refresh();
					} catch {
						Debug.LogError("Unable to save terrain prefab mesh! Likely, you deleted the mesh files, and the prefab is still referencing them. Restarting your Unity editor should solve this minor issue.");
					}
				}
			}
#endif
		} else {
			if (filter.sharedMesh == null || filter.sharedMesh.name != newName) {
				result = new Mesh();
			}
		}
		result.name = newName;
		return result;
	}
	private bool   IsPrefab() {
		bool isPrefab = false;
#if UNITY_EDITOR
		UnityEditor.PrefabType type = UnityEditor.PrefabUtility.GetPrefabType(gameObject);
		isPrefab = type != UnityEditor.PrefabType.None &&
				   type != UnityEditor.PrefabType.DisconnectedPrefabInstance &&
				   type != UnityEditor.PrefabType.PrefabInstance;
#endif
		return isPrefab;
	}
    public  string GetMeshName() {
        if (IsPrefab()) { 
            string    name = gameObject.name;
            Transform curr = gameObject.transform.parent;

            while (curr != null) { name = curr.name + "." + name; curr = curr.transform.parent; }
            name += "-Mesh";

            return name;
        } else {
	        return string.Format("{0}{1}-Mesh", gameObject.name, gameObject.GetInstanceID());
        }
    }
	#endregion

	#region Edges
	private void AddEdge() {
		List<EdgeSegment> edges = EdgeSegment.CreateEdgeSegments(pathData, splitCorners);
		if (edgeMode == Ferr2D_SectionMode.Invert)
			edges.ForEach( e => e.direction = Invert(e.direction) );
		edges.Sort( (a,b) => TerrainMaterial.GetDescriptor(b.direction).ZOffset.CompareTo(TerrainMaterial.GetDescriptor(a.direction).ZOffset) );
		
		for (int i=0; i<edges.Count; i++) {
			AddSegment(edges[i]);
		}

		edges.ToArray();
	}
	private void AddSegment(EdgeSegment aSegment) {
		var edgeData = TerrainMaterial.GetDescriptor(aSegment.direction);
		if (edgeData.GetBody(0).height == 0)
			return;

		if (aSegment.closed) {
			AddBody(aSegment,0,0);
			return;
		}
		
		Vector2 upUV = UnitsPerUV;
		bool leftInner  = aSegment.path.GetInteriorAngle(aSegment.end) > 180;
		bool rightInner = aSegment.path.GetInteriorAngle(aSegment.start) > 180;
		if (edgeMode == Ferr2D_SectionMode.Invert) { leftInner = !leftInner; rightInner = !rightInner; }
		Ferr2D_CapColliderType leftCapType, rightCapType;
		float leftOff, rightOff, leftColliderOffset, rightColliderOffset;
		edgeData.GetLeftCap (leftInner,  upUV, out leftCapType,  out leftColliderOffset,  out leftOff);
		edgeData.GetRightCap(rightInner, upUV, out rightCapType, out rightColliderOffset, out rightOff);

		AddBody(aSegment, -(edgeData.capOffset + rightOff), -(edgeData.capOffset + leftOff));
		AddCap (aSegment, rightInner, aSegment.startDistance                -(edgeData.capOffset+rightOff), aSegment.start, edgeData.ZOffset);
		AddCap (aSegment, leftInner,  aSegment.startDistance+aSegment.length+(edgeData.capOffset+leftOff ), aSegment.end,   edgeData.ZOffset);
	}
	private void AddBody   (EdgeSegment aSegment, float aStartOffset, float aEndOffset) {
		float start  = aSegment.startDistance + aStartOffset;
		float length = aSegment.length - (aStartOffset + aEndOffset);
		Vector2 upUV = UnitsPerUV;
		
		// figure out how many cuts are on this segment
		int   smoothCount = aSegment.path.GetFinalPath().Count;
		var   edgeData    = TerrainMaterial.GetDescriptor(aSegment.direction);
		float yOffset     = edgeData.GetYOffset(edgeMode == Ferr2D_SectionMode.Invert, upUV);
		
		float segmentStretch;
		List<int> cutBodies = CreateLineList(aSegment, out segmentStretch);

		// setup for iterating through the segment
		Ferr2D_PointData currData, nextData;
		float currSmoothPercent, nextSmoothPercent;
		int   currSmoothIndex,   nextSmoothIndex;
		float currStart = start;

		// add the first vertex slice
		Vector2 currPt, currN;
		aSegment.Sample(start, out currSmoothIndex, out currSmoothPercent, out currPt, out currN, out currData);

		for (int c = 1; c <= cutBodies.Count; c++) {
			Rect    body = TerrainMaterial.ToUV( edgeData.GetBody(cutBodies[c-1]) );
			Vector2 size = new Vector2(body.width  * upUV.x, body.height * upUV.y * .5f);
			if (edgeMode == Ferr2D_SectionMode.Invert) {
				float t = body.yMin;
				body.yMin = body.yMax;
				body.yMax = t;
				t = body.xMin;
				body.xMin = body.xMax;
				body.xMax = t;
			}
			

			float texPercent = (c / (float)(cutBodies.Count));
			float nextDist = start + texPercent * length;
			float segLength = (1f / cutBodies.Count) * length;

			Vector2 nextPt, nextN;
			aSegment.Sample(nextDist, out nextSmoothIndex, out nextSmoothPercent, out nextPt, out nextN, out nextData);

			AddSegment(aSegment, smoothCount, size, body, yOffset, edgeData.ZOffset, currStart, segLength, 
				currSmoothIndex, currPt, currN, currData, 
				nextSmoothIndex, nextPt, nextN, nextData);
			
			currPt = nextPt;
			currN  = nextN;
			currSmoothIndex = nextSmoothIndex;
			currStart = nextDist;
			currData  = nextData;
		}
	}
	private void AddCap    (EdgeSegment aSegment, bool aInner, float aDistance, int aCornerVert, float aZ) {
		float   startDist = 0;
		float   endDist   = 0;
		var     edgeData  = TerrainMaterial.GetDescriptor(aSegment.direction);
		Vector2 upUV      = UnitsPerUV;
		bool    invert    = edgeMode == Ferr2D_SectionMode.Invert;
		float   yOffset   = edgeData.GetYOffset(invert, upUV);
		Rect  UVs;
		
		// find the correct cap UVS
		if (invert ^ aCornerVert == aSegment.start) {
			UVs = aInner && edgeData.innerRightCap.width > 0 ? edgeData.innerRightCap : edgeData.rightCap;
		} else {
			UVs = aInner && edgeData.innerLeftCap.width  > 0 ? edgeData.innerLeftCap  : edgeData.leftCap;
		}
		UVs = TerrainMaterial.ToUV(UVs);
		Vector2 size = new Vector2(UVs.width*upUV.x, UVs.height*upUV.y*.5f);
		if (UVs.width <= 0) return;

		// flip it if it's inverted
		if (invert) {
			float t = UVs.yMin;
			UVs.yMin = UVs.yMax;
			UVs.yMax = t;
			t = UVs.xMin;
			UVs.xMin = UVs.xMax;
			UVs.xMax = t;
		}

		// find the distances we need to sample to get the verts
		if (aCornerVert == aSegment.start) {
			startDist = aDistance - size.x;
			endDist   = aDistance;
		} else {
			startDist = aDistance;
			endDist   = aDistance + size.x;
		}

		// sample the start and end
		int startIndex;
		float startPercent;
		Vector2 startPt, startN;
		Ferr2D_PointData startData;
		aSegment.Sample(startDist, out startIndex, out startPercent, out startPt, out startN, out startData);
		
		int endIndex;
		float endPercent;
		Vector2 endPt, endN;
		Ferr2D_PointData endData;
		aSegment.Sample(endDist, out endIndex, out endPercent, out endPt, out endN, out endData);

		AddSegment(aSegment, aSegment.path.GetFinalPath().Count, size, UVs, yOffset, aZ, startDist, size.x, 
				startIndex, startPt, startN, startData, 
				endIndex,   endPt,   endN,   endData);
	}
	private void AddSegment(EdgeSegment aSegment, int aSmoothCount, Vector2 aSegmentSize, Rect aUVs, float aYOffset, float aZ, float aStartDistance, float aSegmentLength, int aCurrSmoothIndex, Vector2 aCurrPt, Vector2 aCurrN, Ferr2D_PointData aCurrData, int aNextSmoothIndex, Vector2 aNextPt, Vector2 aNextN, Ferr2D_PointData aNextData) {
		float UVsMid = Mathf.Lerp(aUVs.yMax, aUVs.yMin, 0.5f);
		
		int i1 = DMesh.AddVertex(aCurrPt + aCurrN * ( aSegmentSize.y + aYOffset) * aCurrData.scale, aZ+slantAmount, new Vector2(aUVs.xMax, aUVs.yMax));
		int i2 = DMesh.AddVertex(aCurrPt + aCurrN * (                  aYOffset) * aCurrData.scale, aZ, new Vector2(aUVs.xMax, UVsMid   ));
		int i3 = DMesh.AddVertex(aCurrPt + aCurrN * (-aSegmentSize.y + aYOffset) * aCurrData.scale, aZ-slantAmount, new Vector2(aUVs.xMax, aUVs.yMin));
		int i4, i5, i6;
		
		// TODO: replace this with normal bending
		int crossedPointStart = (aCurrSmoothIndex + 1)%aSmoothCount; // currSmoothPercent>0.2f ? currSmoothIndex+1 : currSmoothIndex+2;
		int crossedPointEnd   = (aNextSmoothIndex + 1)%aSmoothCount; // nextSmoothPercent>0.2f ? nextSmoothIndex   : nextSmoothIndex-1;

		// Add an extra vert for each path point crossed
		float distance = 0;
		for (int s = crossedPointStart; s != crossedPointEnd; s=(s+1)%aSmoothCount) {
			distance = aSegment.path.DistanceMask.DistanceBetweenDistances(aStartDistance, aSegment.path.DistanceMask[s].distance);
			float uvX = Mathf.Lerp(aUVs.xMax, aUVs.xMin, distance / aSegmentLength);
			Vector2 pt, n;
			Ferr2D_PointData data;
			aSegment.SampleSmoothVert(s, out pt, out n, out data);

			i4 = DMesh.AddVertex(pt + n * ( aSegmentSize.y + aYOffset) * data.scale, aZ+slantAmount, new Vector3(uvX, aUVs.yMax));
			i5 = DMesh.AddVertex(pt + n * (                  aYOffset) * data.scale, aZ, new Vector3(uvX, UVsMid   ));
			i6 = DMesh.AddVertex(pt + n * (-aSegmentSize.y + aYOffset) * data.scale, aZ-slantAmount, new Vector3(uvX, aUVs.yMin));
			DMesh.AddFace(i2, i1, i4, i5);
			DMesh.AddFace(i3, i2, i5, i6);
			i1 = i4;
			i2 = i5;
			i3 = i6;
		}

		i4 = DMesh.AddVertex(aNextPt + aNextN * ( aSegmentSize.y + aYOffset) * aNextData.scale, aZ+slantAmount, new Vector3(aUVs.xMin, aUVs.yMax));
		i5 = DMesh.AddVertex(aNextPt + aNextN * (                  aYOffset) * aNextData.scale, aZ, new Vector3(aUVs.xMin, UVsMid   ));
		i6 = DMesh.AddVertex(aNextPt + aNextN * (-aSegmentSize.y + aYOffset) * aNextData.scale, aZ-slantAmount, new Vector3(aUVs.xMin, aUVs.yMin));
		DMesh.AddFace(i2, i1, i4, i5);
		DMesh.AddFace(i3, i2, i5, i6);
	}
	
	public List<int> CreateLineList(EdgeSegment aSegment,out float aScale, List<int> aIncludeSource=null) {
		UnityEngine.Random.State tSeed = UnityEngine.Random.state;
		UnityEngine.Random.InitState((int)(pathData[aSegment.end].x*200 + pathData[aSegment.end].y*500));
		var edgeDesc = TerrainMaterial.GetDescriptor(aSegment.direction);
		Vector2 upUV = UnitsPerUV;

		float length = aSegment.GetOffsetLength(edgeDesc, edgeMode == Ferr2D_SectionMode.Invert, upUV);

		List<int> result = new List<int>();
		float currLength = 0;
		float lastLength = 0;
		
		int edgeSegment = aSegment.start;
		int cutId = 0;
		var cuts = aSegment.path.GetData(edgeSegment).cutOverrides;
		while (currLength < length) {
			int id = edgeDesc.GetRandomBodyId(UnityEngine.Random.value);

			// see if there's an override id for this segment
			int currEdgeSegment = aSegment.path.GetSegmentAtDistance(currLength + aSegment.startDistance);
			if (currEdgeSegment != edgeSegment) {
				edgeSegment = currEdgeSegment;
				cutId = 0;
				cuts = aSegment.path.GetData(edgeSegment).cutOverrides;
			}
			if (cuts != null && cutId<cuts.Count && cuts[cutId] > 0 && cuts[cutId] <= edgeDesc.BodyCount) {
				id = cuts[cutId]-1;
			}

			lastLength  = currLength;
			currLength += TerrainMaterial.ToUV( edgeDesc.GetBody(id) ).width * upUV.x;
			result.Add(id);

			// track where we got this override from, if we're asked
			if (aIncludeSource != null) {
				aIncludeSource.Add(edgeSegment);
				aIncludeSource.Add(cutId);
			}

			cutId += 1;
		}

		if (result.Count > 1 && length-lastLength < currLength-length) {
			result.RemoveAt(result.Count - 1);
			if (aIncludeSource != null) {
				aIncludeSource.RemoveAt(aIncludeSource.Count-1);
				aIncludeSource.RemoveAt(aIncludeSource.Count-1);
			}
			currLength = lastLength;
		}

		UnityEngine.Random.state = tSeed;

		aScale = length / currLength;
		return result;
	}

	public  static Ferr2DT_TerrainDirection GetSegmentDirection(Ferr2DPath aPath, int aIndex) {
		Ferr2DT_TerrainDirection result = (Ferr2DT_TerrainDirection)aPath.GetData(aIndex).directionOverride;
		if (result == Ferr2DT_TerrainDirection.None) {
			Vector2 normal = aPath.GetSegmentNormal(aIndex);

			if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y)) {
				if (normal.x < 0) return Ferr2DT_TerrainDirection.Left;
				else              return Ferr2DT_TerrainDirection.Right;
			} else {
				if (normal.y < 0) return Ferr2DT_TerrainDirection.Bottom;
				else              return Ferr2DT_TerrainDirection.Top;
			}
		}
		return result;
	}
	public static Ferr2DT_TerrainDirection Invert(Ferr2DT_TerrainDirection aDirection) {
		switch (aDirection) {
			case Ferr2DT_TerrainDirection.Top:    return Ferr2DT_TerrainDirection.Bottom;
			case Ferr2DT_TerrainDirection.Bottom: return Ferr2DT_TerrainDirection.Top;
			case Ferr2DT_TerrainDirection.Left:   return Ferr2DT_TerrainDirection.Right;
			case Ferr2DT_TerrainDirection.Right:  return Ferr2DT_TerrainDirection.Left;
			default: return aDirection;
		}
	}
	public class EdgeSegment {
		// assigned fields
		public Ferr2DT_TerrainDirection direction;
		public int   start;
		public int   end;
		public bool  closed;
		public Ferr2DPath path;

		// calculated fields
		public float startDistance;
		public float length;
		public int   smoothStart;
		public int   smoothEnd;
		private List<Vector2> pathVerts;
		
		public EdgeSegment(Ferr2DPath aPath, int aStart, int aEnd, bool aClosed, Ferr2DT_TerrainDirection aDirection) {
			start     = aStart;
			end       = aEnd;
			closed    = aClosed;
			direction = aDirection;
			path      = aPath;
		}

		public void UpdateData() {
			pathVerts   = path.GetFinalPath();
			smoothStart = path.GetSmoothIndex(start);
			smoothEnd   = path.GetSmoothIndex(end);

			startDistance     = path.DistanceMask[smoothStart].distance;
			float endDistance = closed ? path.DistanceMask.GetTotalDistance() : path.DistanceMask[smoothEnd].distance;
			length = endDistance - startDistance;
			if (length < 0)
				length += path.DistanceMask.GetTotalDistance();
		}
		public void Sample(float aDistance, out Vector2 aPoint, out Vector2 aNormal, out Ferr2D_PointData aData) {
			float percent;
			int smoothIndex;
			Sample(aDistance, out smoothIndex, out percent, out aPoint, out aNormal, out aData);
		}
		public void Sample(float aDistance, out int aSmoothIndex, out float aSmoothPercent, out Vector2 aPoint, out Vector2 aNormal, out Ferr2D_PointData aData) {
			if (aDistance <= startDistance) {
				aNormal = closed ? 
					PathUtil.GetPointNormal  (smoothStart, pathVerts, true) * PathUtil.GetParallelOffset(smoothStart, pathVerts, path.Closed) :
					PathUtil.GetSegmentNormal(smoothStart, pathVerts, true);
				aPoint  = pathVerts[smoothStart] + new Vector2(aNormal.y, -aNormal.x) * (startDistance-aDistance);
				aSmoothIndex = smoothStart;
				aSmoothPercent = 1;
				aData = path.GetData(path.DistanceMask[aSmoothIndex].index, path.DistanceMask[aSmoothIndex].percent);
			} else if (aDistance >= startDistance+length) {
				aNormal = closed ?
					PathUtil.GetPointNormal  (PathUtil.WrapIndex(smoothEnd,   pathVerts.Count, true), pathVerts, true)  * PathUtil.GetParallelOffset(smoothEnd, pathVerts, path.Closed):
					PathUtil.GetSegmentNormal(PathUtil.WrapIndex(smoothEnd-1, pathVerts.Count, true), pathVerts, true);
				aPoint  = pathVerts[smoothEnd] + new Vector2(-aNormal.y, aNormal.x) * (aDistance - (startDistance+length));
				aSmoothIndex = smoothEnd;
				aSmoothPercent = 0;
				aData = path.GetData(path.DistanceMask[aSmoothIndex].index, path.DistanceMask[aSmoothIndex].percent);
				if (closed && smoothEnd == 0) {
					aSmoothIndex = pathVerts.Count-1;
					aSmoothPercent=1;
				}
			} else {
				aSmoothIndex = path.DistanceMask.GetSmoothPointIndexAtDistance(aDistance, out aSmoothPercent, path.Closed);
				aPoint       = Vector2.Lerp(pathVerts[aSmoothIndex], pathVerts[PathUtil.WrapIndex(aSmoothIndex + 1, pathVerts.Count, path.Closed)], aSmoothPercent);
				aNormal      = aSmoothPercent==0||aSmoothPercent==1 && closed ? // This monstrosity is to get closing segments correct. Sorry.
					(aSmoothIndex == pathVerts.Count - 1 ?
						PathUtil.GetPointNormal(0, pathVerts, path.Closed) * PathUtil.GetParallelOffset(0, pathVerts, path.Closed) :
						PathUtil.GetPointNormal(aSmoothIndex, pathVerts, path.Closed) * PathUtil.GetParallelOffset(aSmoothIndex, pathVerts, path.Closed)) :
					PathUtil.GetSegmentNormal(aSmoothIndex, pathVerts, path.Closed);
				
				aData = path.GetDataAtDistance(aDistance);// path.GetData(start.index, Mathf.Lerp(start.percent, end.percent, aSmoothPercent));
			}
		}
		public void SampleSmoothVert(int aSmoothIndex, out Vector2 aPoint, out Vector2 aNormal, out Ferr2D_PointData aData) {
			if (smoothEnd < smoothStart) {
				if (aSmoothIndex > smoothEnd && aSmoothIndex < smoothStart) {
					aSmoothIndex = aSmoothIndex-smoothEnd < smoothStart-aSmoothIndex ? smoothEnd : smoothStart;
				}
			} else if (closed) {
			} else {
				aSmoothIndex = Mathf.Clamp(aSmoothIndex, smoothStart, smoothEnd);
			}

			aPoint = pathVerts[aSmoothIndex];
			aData  = path.GetData(path.DistanceMask[aSmoothIndex].index, path.DistanceMask[aSmoothIndex].percent);

			if (!closed && aSmoothIndex == smoothStart) {
				aNormal = PathUtil.GetSegmentNormal(aSmoothIndex, pathVerts, true);
			} else if (!closed && aSmoothIndex == smoothEnd) {
				aNormal = PathUtil.GetSegmentNormal(PathUtil.WrapIndex(aSmoothIndex-1, pathVerts.Count, true), pathVerts, true);
			} else {
				aNormal = PathUtil.GetPointNormal(aSmoothIndex, pathVerts, path.Closed);
				if (path.DistanceMask.IsRawPoint(aSmoothIndex, path.Closed))
					aNormal *= PathUtil.GetParallelOffset(aSmoothIndex, pathVerts, path.Closed);
			}
		}
		public bool ContainsIndex(int aRawIndex) {
			if (closed)
				return true;
			if (end >= start)
				return aRawIndex >= start && aRawIndex < end;
			return aRawIndex >= start || aRawIndex < end;
		}
		public float GetOffsetLength(Ferr2DT_SegmentDescription aDesc, bool aInvert, Vector2 aUpUV) {
			float result = length;
			if (!closed) {
				bool leftInner  = path.GetInteriorAngle(end) > 180;
				bool rightInner = path.GetInteriorAngle(start) > 180;
				if (aInvert) { leftInner = !leftInner; rightInner = !rightInner; }
				float leftOff  = aDesc.GetLeftCapOffset(leftInner, aUpUV);
				float rightOff = aDesc.GetRightCapOffset(rightInner, aUpUV);

				result += (aDesc.capOffset + rightOff) + (aDesc.capOffset + leftOff);
			}
			return result;
		} 

		public static List<EdgeSegment> CreateEdgeSegments(Ferr2DPath aPath, bool aSplitCorners) {
			List<EdgeSegment> result = new List<EdgeSegment>();
			int               count  = aPath.Closed ? aPath.Count : aPath.Count-1;

			// if we don't need to split stuff, just return the whole loop!
			if (!aSplitCorners) {
				EdgeSegment fullPath;
				if (aPath.Closed)
					fullPath = new EdgeSegment(aPath, 0, 0, aPath.Closed, Ferr2DT_TerrainDirection.Top);
				else
					fullPath = new EdgeSegment(aPath, 0, aPath.GetFinalPath().Count-1, aPath.Closed, Ferr2DT_TerrainDirection.Top);
				fullPath.UpdateData();
				result.Add(fullPath);
				return result;
			}

			// otherwise, parse through all the segments, and figure out which go where
			EdgeSegment curr = new EdgeSegment(aPath, 0, 1, false, GetSegmentDirection(aPath, 0));
			for (int i = 1; i < count; i++) {
				var currDirection = GetSegmentDirection(aPath, i);
				int next = PathUtil.WrapIndex(i+1, aPath.Count, aPath.Closed);

				if (curr.direction != currDirection) {
					result.Add(curr);
					curr = new EdgeSegment(aPath, i, next, false, currDirection);
				}
				curr.end = next;
			}
			result.Add(curr);

			// check the closing segment for any sort of merging
			if (aPath.Closed) {
				if (result[0].direction == result[result.Count-1].direction) {
					if (result.Count == 1) {
						result[0].closed = true;
					} else {
						result[0].start = result[result.Count-1].start;
						result.RemoveAt(result.Count-1);
					}
				}
			}

			// ensure distance data is correct!
			for (int i = 0; i < result.Count; i++) {
				result[i].UpdateData();
			}

			return result;
		}
	}
	#endregion

	#region Fill
	private void AddFill(bool aAddSkirt, bool aSteinerPoints, bool aSimplify) {
		List<Vector2>    fillVerts  = pathData.GetFinalPath();
		Rect             bounds     = PathUtil.GetBounds(fillVerts);
		if (aSimplify)
			fillVerts = PathUtil.SimplifyPath(fillVerts, 0.1f);

		// get the list in a Clipper format
		List<IntPoint> pts = fillVerts.ConvertAll(v => new IntPoint(v.x * cIntConvert, v.y * cIntConvert));
		List<List<IntPoint>> sourcePaths = new List<List<IntPoint>>();

		if (fillMode == Ferr2D_SectionMode.Invert) {
			float width  = invertFillBorder.x == 0 ? bounds.width  : invertFillBorder.x;
			float height = invertFillBorder.y == 0 ? bounds.height : invertFillBorder.y;

			IntPoint pt;
			List<IntPoint> exterior = new List<IntPoint>();
			
			pt = new IntPoint((bounds.xMin - width) * cIntConvert, (bounds.yMin - height) * cIntConvert); exterior.Add(pt);
			pt = new IntPoint((bounds.xMax + width) * cIntConvert, (bounds.yMin - height) * cIntConvert); exterior.Add(pt);
			pt = new IntPoint((bounds.xMax + width) * cIntConvert, (bounds.yMax + height) * cIntConvert); exterior.Add(pt);
			pt = new IntPoint((bounds.xMin - width) * cIntConvert, (bounds.yMax + height) * cIntConvert); exterior.Add(pt);
			
			bounds.xMin -= width;
			bounds.xMax += width;
			bounds.yMin -= height;
			bounds.yMax += height;

			sourcePaths.Add(exterior);
		}

		// add path points for the skirt, if we need one
		if (aAddSkirt) {
            Vector2 start = fillVerts[0];
            Vector2 end   = fillVerts[fillVerts.Count - 1];

            pts.Add(new IntPoint(end.x * cIntConvert, fillY * cIntConvert));
            pts.Add(new IntPoint(Mathf.Lerp(end.x, start.x, 0.33f) * cIntConvert, fillY * cIntConvert));
            pts.Add(new IntPoint(Mathf.Lerp(end.x, start.x, 0.66f) * cIntConvert, fillY * cIntConvert));
            pts.Add(new IntPoint(start.x * cIntConvert, fillY * cIntConvert));
        }
		sourcePaths.Add(pts);
		
		// simplify so it doesn't freak out
		List<List<IntPoint>> simplified = Clipper.SimplifyPolygons(sourcePaths, PolyFillType.pftEvenOdd);
		if (simplified.Count <= 0)
			return;

		List<List<IntPoint>> exteriors = new List<List<IntPoint>>();
		List<List<IntPoint>> holes     = new List<List<IntPoint>>();
		for (int i = 0; i < simplified.Count; i++) {
			if (Clipper.Orientation(simplified[i])) {
				exteriors.Add(simplified[i]);
			} else {
				holes    .Add(simplified[i]);
			}
		}

		// triangulate the polygons we've found
		List<Polygon> holePolys = CreateHoles(holes);
		for (int i = 0; i < exteriors.Count; i++) {
			TriangulatePoints(exteriors[i], holePolys, bounds, aSteinerPoints);
		}
	}
	List<Polygon> CreateHoles(List<List<IntPoint>> aHoles) {
		List<Polygon> result = new List<Polygon>();

		int startId = DMesh.VertCount;
		// find the UV modifiers
		IFerr2DTMaterial material = TerrainMaterial;
		Vector2 scale   = Vector2.one;
		Vector2 uvOff   = uvOffset + new Vector2(transform.position.x, transform.position.y);
		Texture fillTex = material.fillMaterial == null ? null : material.fillMaterial.mainTexture;
		if (fillTex != null) {
            scale = new Vector2( pixelsPerUnit/fillTex.width, pixelsPerUnit/fillTex.height );
        }

		// create a poly for each hole
		for (int h = 0; h < aHoles.Count; h++) {
			List<IntPoint>     hole    = aHoles[h];
			List<PolygonPoint> holePts = new List<PolygonPoint>(hole.Count);
			for (int v = 0; v < hole.Count; v++) {
				float x = hole[v].X / (float)cIntConvert;
				float y = hole[v].Y / (float)cIntConvert;
				holePts.Add(new PolygonPoint(x, y, startId + v));
				DMesh.AddVertex(
					new Vector3(x, y, fillZ),
					Vector2.Scale( uvOff + new Vector2(x,y), scale ));
			}
			result.Add(new Polygon(holePts));
		}

		return result;
	}
	private void TriangulatePoints(List<IntPoint> aPolygon, List<Polygon> aHoles, Rect aBounds, bool aSteinerPoints) {
		int startId = DMesh.VertCount;
		
		// find the UV modifiers
		IFerr2DTMaterial material = TerrainMaterial;
		Vector2 scale   = Vector2.one;
		Vector2 uvOff   = uvOffset + new Vector2(transform.position.x, transform.position.y);
		Texture fillTex = material.fillMaterial == null ? null : material.fillMaterial.mainTexture;
		if (fillTex != null) {
            scale = new Vector2( pixelsPerUnit/fillTex.width, pixelsPerUnit/fillTex.height );
        }
		
		// add the exterior polygon
		List<PolygonPoint> polyPts = new List<PolygonPoint>(aPolygon.Count);
		for (int v = 0; v < aPolygon.Count; v++) {
			float x = aPolygon[v].X / (float)cIntConvert;
			float y = aPolygon[v].Y / (float)cIntConvert;
			polyPts.Add(new PolygonPoint(x, y, startId + v));
			DMesh.AddVertex(
				new Vector3(x, y, fillZ),
				Vector2.Scale( uvOff + new Vector2(x,y), scale ));
		}
		Polygon poly = new Polygon(polyPts);
		
		for (int i = 0; i < aHoles.Count; i++) {
			// Poly2Tri does not seem to behave when holes share a vert, so prevent more than one hole
			if (i==0)
				poly.AddHole(aHoles[i]);
		}
		
		// add steiner points for the split fill
		if (aSteinerPoints) {
			float distance = fillSplitDistance;
			if (distance < pathData.SmoothSplitDistance)
				distance = pathData.SmoothSplitDistance;
			if (distance < 0.2f)
				distance = 0.2f;

			float left   = Mathf.Ceil((float)aBounds.xMin / distance) * distance + 0.01f;
			float right  = (float)aBounds.xMax - 0.01f;
			float top    = Mathf.Ceil((float)aBounds.yMin / distance) * distance - 0.01f;
			float bottom = (float)aBounds.yMax + 0.01f;
			
			for (float x = left; x < right; x+=distance) {
				for (float y = top; y < bottom; y+=distance) {
					TriangulationPoint pt = new TriangulationPoint(x, y, DMesh.VertCount);
					bool inside = poly.IsPointInside(pt);
						
					if (inside) {
						poly.AddSteinerPoint(pt);
						DMesh.AddVertex(
							new Vector3(x, y, fillZ),
							Vector2.Scale( uvOff + new Vector2(x,y), scale ));
					}
				}
			}
		}

		// Triangulate! It'll sometimes throw errors due to bad point data
		try {
			P2T.Triangulate(poly);
		} catch (System.Exception e) {
			e.ToString();
		}
		
		// get the indices from the triangulation
		var tris = poly.Triangles;
		for (int t = 0; t < tris.Count; t++) {
            var tri = tris[t];
            if (tri.Points._0.Id == -1 || tri.Points._1.Id == -1 || tri.Points._2.Id == -1)
                continue;

			DMesh.AddFace(tri.Points._2.Id, tri.Points._1.Id, tri.Points._0.Id);
		}
	}
	#endregion

	#region Vertex Colors
	public void MarkColorSave   () {
		if (vertexColorType == Ferr2DT_ColorType.PreserveVertColor && recolorTree == null) {
			recolorTree = new RecolorTree(MeshFilter.sharedMesh);
		}
	}
	public  void ClearColorSave  () {
		recolorTree = null;
	}
	private void CreateVertColors() {
		Mesh mesh = MeshFilter.sharedMesh;
		
		switch(vertexColorType) {
			case Ferr2DT_ColorType.SolidColor:        ColorVertsSolid           (mesh, vertexColor);
				break;
			case Ferr2DT_ColorType.Gradient:          ColorVertsGradient        (mesh, vertexGradient, vertexGradientAngle);
				break;
			case Ferr2DT_ColorType.DistanceGradient:  ColorVertsDistanceGradient(mesh, pathData.GetFinalPath(), pathData.Closed, vertexGradient, vertexGradientDistance);
				break;
			case Ferr2DT_ColorType.PreserveVertColor: ColorVertsRecolor         (mesh, recolorTree);
				break;
		}
	}

	private static void ColorVertsSolid           (Mesh aMesh, Color aColor) {
		Color[] colors = new Color[aMesh.vertexCount];
		for (int i = 0; i < colors.Length; i += 1) {
			colors[i] = aColor;
		}
		aMesh.colors = colors;
	}
	private static void ColorVertsGradient        (Mesh aMesh, Gradient aGradient, float aGradientAngle) {
		if (aGradient == null) {
			Debug.LogError("Gradient hasn't been created! Can't use a vertexColorType of gradient unless vertexGradient has been initialized!");
			return;
		}
			
		Color  [] colors = new Color[aMesh.vertexCount];
		Vector3[] verts  = aMesh.vertices;
		Vector2 center = new Vector2(aMesh.bounds.center.x, aMesh.bounds.center.y);
		float   radius = aMesh.bounds.extents.magnitude;
		float   angle  = aGradientAngle * Mathf.Deg2Rad;
		Vector2 bottom = center + new Vector2( Mathf.Cos(angle+Mathf.PI), Mathf.Sin(angle+Mathf.PI)) * radius;
		Vector2 top    = center + new Vector2( Mathf.Cos(angle         ), Mathf.Sin(angle)         ) * radius;
			
		for (int i=0; i<verts.Length; i+=1) {
				
			Vector2 pt   = Ferr2D_Path.GetClosetPointOnLine(bottom, top, verts[i], false);
			float   dist = Vector2.Distance( pt, bottom );
			colors[i] = aGradient.Evaluate(dist/(radius*2));
		}
		aMesh.colors = colors;
	}
	private static void ColorVertsDistanceGradient(Mesh aMesh, List<Vector2> aPath, bool aClosed, Gradient aGradient, float aGradientMaxDistance) {
		if (aGradient == null) {
			Debug.LogError("Gradient hasn't been created!");
			return;
		}
			
		Color  [] colors = new Color[aMesh.vertexCount];
		Vector3[] verts  = aMesh.vertices;
		for (int i=0; i<verts.Length; i+=1) {
			float dist = PathUtil.GetDistanceFromPath(aPath, verts[i], aClosed);
				
			colors[i] = aGradient.Evaluate(Mathf.Clamp01(dist/aGradientMaxDistance));
		}
		aMesh.colors = colors;
	}
	private static void ColorVertsRecolor         (Mesh aMesh, RecolorTree aRecolorTree) {
		if (aRecolorTree == null) {
			Debug.LogError("Color save point wasn't marked! Can't restore previous colors!");
			return;
		}
		aMesh.colors = aRecolorTree.Recolor(aMesh.vertices);
	}
	#endregion

	#region Colliders
	/// <summary>
	/// Creates a mesh or poly and adds it to the collider object. This is automatically calld on Start,
	/// if createCollider is set to true. This will automatically add a collider if none is 
	/// attached already.
	/// </summary>
	public void RecreateCollider() {
		if (IsLegacy) {
			LegacyRecreateCollider();
			return;
		}
		
		if (colliderMode == Ferr2D_ColliderMode.Mesh3D) {
			CreateCollider3D();
		} else if (colliderMode == Ferr2D_ColliderMode.Polygon2D) {
			CreatePolyCollider2D();
		} else if (colliderMode == Ferr2D_ColliderMode.Edge2D) {
			CreateEdgeCollider2D();
		}
	}

	class ComponentPool<T, U> where T:Component {
		List<T> _pool;
		List<U> _mapItems;

		public List<T> Pool { get { return _pool; } }
		public List<U> Map  { get { return _mapItems; } }

		public ComponentPool(GameObject aFrom, List<U> aMapTo, bool aConsolidate) {
			T[] components = aFrom.GetComponents<T>();
			_pool = new List<T>(components);

			if (aConsolidate) {
				_mapItems = new List<U>();
				for (int i = 0; i < aMapTo.Count; i++) {
					if (_mapItems.IndexOf(aMapTo[i]) == -1) {
						_mapItems.Add(aMapTo[i]);
					}
				}
			} else {
				_mapItems = aMapTo;
			}

			int extra = components.Length - _mapItems.Count;
			if (extra > 0) {
				// we have too many, remove a few
				for (int i=0; i<extra; i+=1) {
					if (Application.isPlaying)
						Destroy(_pool[0]);
					else
						DestroyImmediate(_pool[0]);
					_pool.RemoveAt(0);
				}
			} else {
				// we have too few, add in a few
				for (int i=0; i<Mathf.Abs(extra); i+=1) {
					_pool.Add(aFrom.AddComponent<T>());
				}
			}
		}
		public T Get(U aFromData) {
			return _pool[_mapItems.IndexOf(aFromData)];
		}
	}

	void CreateEdgeCollider2D() {
		List<PhysicsMaterial2D> materials = new List<PhysicsMaterial2D>();
		List<List<Vector2>>     segs      = GetColliderVerts(materials);

		ComponentPool<EdgeCollider2D, PhysicsMaterial2D> pool = new ComponentPool<EdgeCollider2D, PhysicsMaterial2D>(gameObject, materials, false);
		
		for (int i = 0; i < segs.Count; i++) {
			EdgeCollider2D edge = pool.Pool[i];
			segs[i].Add(segs[i][0]);
			edge.points = segs[i].ToArray();
			segs[i].RemoveAt(segs[i].Count-1);
			edge.sharedMaterial = pool.Map[i];
		}
		Set2DColliderSettings(pool.Pool);
	}
	void CreatePolyCollider2D() {
		List<PhysicsMaterial2D> materials = new List<PhysicsMaterial2D>();
		List<List<Vector2>>     segs      = GetColliderVerts(materials);

		ComponentPool<PolygonCollider2D, PhysicsMaterial2D> pool = new ComponentPool<PolygonCollider2D, PhysicsMaterial2D>(gameObject, materials, true);
		for (int i = 0; i < pool.Pool.Count; i++) {
			pool.Pool[i].pathCount = 0;
			pool.Pool[i].sharedMaterial = pool.Map[i];
		}

		for (int i = 0; i < segs.Count; i++) {
			PolygonCollider2D poly = pool.Get(materials[i]);
			poly.pathCount += 1;
			poly.SetPath(poly.pathCount-1, segs[i].ToArray());
		}
		Set2DColliderSettings(pool.Pool);
	}
	void Set2DColliderSettings<T>(List<T> aColliders) where T:Collider2D {
		for (int i=0; i<aColliders.Count; i+=1) {
			aColliders[i].isTrigger      = isTrigger;
			#if UNITY_5_5_OR_NEWER
			aColliders[i].usedByEffector = usedByEffector;
			#endif
		}
	}
	void CreateCollider3D() {
		
		List<PhysicMaterial> materials = new List<PhysicMaterial>();
        List<List<Vector2>>  segs      = GetColliderVerts(null, materials);

		ComponentPool<MeshCollider, PhysicMaterial> pool = new ComponentPool<MeshCollider, PhysicMaterial>(gameObject, materials, false);
		
		for (int s = 0; s < segs.Count; s++) {
			Ferr2D_DynamicMesh colMesh = new Ferr2D_DynamicMesh();
			MeshCollider collider = pool.Pool[s];
			
			if (fillMode == Ferr2D_SectionMode.Invert) {
				for (int i = 0; i < segs[s].Count; i+=1) {
					colMesh.AddVertex(segs[s][i]);
				}
			} else {
				for (int i = segs[s].Count-1; i >= 0; i-=1) {
					colMesh.AddVertex(segs[s][i]);
				}
			}
			colMesh.ExtrudeZ(depth, false);

			collider.sharedMaterial = pool.Map[s];
			collider.isTrigger = isTrigger;

			// compile the mesh!
			Mesh   m    = collider.sharedMesh;
			string name = string.Format("Ferr2DT_PathCollider_{0}", gameObject.GetInstanceID());
			if (m == null || m.name != name) {
				collider.sharedMesh = m = new Mesh();
				m.name = name;
			}
			collider.sharedMesh = null;
			colMesh.Build(ref m, false);
			collider.sharedMesh = m;
		}
	}
	
	public List<List<Vector2>>  GetColliderVerts  (List<PhysicsMaterial2D> aPhysicsMaterials2D=null, List<PhysicMaterial> aPhysicsMaterials3D=null) {
		if (IsLegacy)
			return LegacyGetColliderVerts();
		if (TerrainMaterial == null)
			return new List<List<Vector2>>();

		// ensure upUV gets recalculated
		unitsPerUV = Vector2.zero;
		
		List<List<Vector2>> result = new List<List<Vector2>>();
		Vector2 upUV = UnitsPerUV;
		bool invert = edgeMode == Ferr2D_SectionMode.Invert;

		if (edgeMode == Ferr2D_SectionMode.None || fillCollider) {
			result.Add(CreateFillCollider());
			if (aPhysicsMaterials2D != null) aPhysicsMaterials2D.Add(physicsMaterial2D);
			if (aPhysicsMaterials3D != null) aPhysicsMaterials3D.Add(physicsMaterial);

			if (edgeMode == Ferr2D_SectionMode.None)
				return result;
		}

		int smoothCount = pathData.GetFinalPath().Count;
		var segments    = EdgeSegment.CreateEdgeSegments(pathData, splitCorners);
		for (int i = 0; i < segments.Count; i++) {
			var seg       = segments[i];
			var direction = invert ? Invert( seg.direction ) : seg.direction;
			var edgeData  = TerrainMaterial.GetDescriptor(direction);
			if (edgeData.ColliderThickness <= 0)
				continue;

			// find size information needed to calculate the collider offsets
			float thickness = edgeData.ColliderThickness * edgeData.GetBodyMaxHeight() * upUV.y;
			float yOffset   = edgeData.GetYOffsetCollider(invert, upUV);
			
			// figure out which caps we're using
			Ferr2D_CapColliderType leftCapType, rightCapType;
			float                  leftCapSize, rightCapSize, leftOffset, rightOffset;
			bool leftInner  = seg.path.GetInteriorAngle(seg.end) > 180;
			bool rightInner = seg.path.GetInteriorAngle(seg.start) > 180;
			if (invert) { leftInner = !leftInner; rightInner = !rightInner; }
			Rect leftCap    = edgeData.GetLeftCap ( leftInner,  upUV, out leftCapType,  out leftCapSize,  out leftOffset  );
			Rect rightCap   = edgeData.GetRightCap( rightInner, upUV, out rightCapType, out rightCapSize, out rightOffset );
			leftCap         = TerrainMaterial.ToUV(leftCap );
			rightCap        = TerrainMaterial.ToUV(rightCap);
			float rightCapOffset = rightCapType == Ferr2D_CapColliderType.Circle ? rightOffset : rightOffset+rightCapSize;
			float leftCapOffset  = leftCapType  == Ferr2D_CapColliderType.Circle ? leftOffset  : leftOffset +leftCapSize;
			if (seg.closed) {
				rightCapOffset = 0;
				leftCapOffset  = 0;
				leftCapType    = Ferr2D_CapColliderType.Rectangle;
				rightCapType   = Ferr2D_CapColliderType.Rectangle;
			}
			// if there's no caps, simplify them to rectangles so they don't add verts!
			if (rightCap.width <= 0 && rightCapType == Ferr2D_CapColliderType.Circle) rightCapType = Ferr2D_CapColliderType.Rectangle;
			if (leftCap .width <= 0 && leftCapType  == Ferr2D_CapColliderType.Circle) leftCapType  = Ferr2D_CapColliderType.Rectangle;
			
			List<Vector2> points  = new List<Vector2>();
			List<Vector2> normals = new List<Vector2>();
			
			// find the start and the end points
			Vector2 startPt, startN, endPt, endN;
			int   smoothStart, smoothEnd;
			float smoothPercent;
			Ferr2D_PointData startData, endData;
				
			seg.Sample(seg.startDistance-rightCapOffset, out smoothStart, out smoothPercent, out startPt, out startN, out startData);
			points .Add(startPt);
			normals.Add(startN*startData.scale);

			seg.Sample((seg.startDistance+seg.length)+leftCapOffset, out smoothEnd, out smoothPercent, out endPt, out endN, out endData);
			int crossedPointStart = (rightCapOffset > 0 ? smoothStart : smoothStart + 1)%smoothCount;
			int crossedPointEnd   = PathUtil.WrapIndex(smoothEnd+1, smoothCount, seg.path.Closed);

			// find all the points in between
			for (int s = crossedPointStart; s != crossedPointEnd; s=(s+1)%smoothCount) {
				Vector2 spt, sn;
				Ferr2D_PointData data;
				seg.SampleSmoothVert(s, out spt, out sn, out data);
				points.Add(spt);
				normals.Add(sn*data.scale);
			}

			points .Add(endPt);
			normals.Add(endN*endData.scale);
			
			// turn those points into an edge matching the correct size
			int bodyVertsCount = points.Count-1;
			// add the body collider verts
			for (int p = bodyVertsCount; p >= 0; p-=1) {
				points.Add(new Vector2( points[p].x + normals[p].x*(yOffset - thickness), points[p].y + normals[p].y*(yOffset - thickness) ));
				points[p]= new Vector2( points[p].x + normals[p].x*(yOffset            ), points[p].y + normals[p].y*(yOffset            ) );
			}
			// add left collider caps
			if (!seg.closed && leftCapType != Ferr2D_CapColliderType.Rectangle) {
				if (leftCapType == Ferr2D_CapColliderType.Circle) {
					AddCircleColliderCap(false, ref points, normals[bodyVertsCount], endPt, leftCapSize, yOffset, thickness);
				} else {
					EdgeSegment nextSeg = segments[(i+1)%segments.Count];
					if (nextSeg != seg && nextSeg.start == seg.end) {
						if (leftCapType == Ferr2D_CapColliderType.Connected || leftCapType == Ferr2D_CapColliderType.ConnectedSharp) {
							bool sharp = leftCapType == Ferr2D_CapColliderType.ConnectedSharp;
							AddConnectedColliderCap(false, invert?!leftInner:leftInner, sharp, nextSeg, ref points, normals);
						} else if (leftCapType == Ferr2D_CapColliderType.IntersectionSharp || leftCapType == Ferr2D_CapColliderType.IntersectionTop) {
							bool top = leftCapType == Ferr2D_CapColliderType.IntersectionTop;
							AddIntersectionColliderCap(false, invert?!leftInner:leftInner, top, nextSeg, ref points, normals );
						} else if (leftCapType == Ferr2D_CapColliderType.IntersectionEdge) {
							AddIntersectionEdgeColliderCap(false, invert?!leftInner:leftInner, nextSeg, ref points, normals );
						}
					}
				}
			}
			// and add the right collider caps
			if (!seg.closed  && rightCapType != Ferr2D_CapColliderType.Rectangle) {
				if (rightCapType == Ferr2D_CapColliderType.Circle) {
					AddCircleColliderCap(true, ref points, normals[0], startPt, rightCapSize, yOffset, thickness);
				} else {
					EdgeSegment nextSeg = segments[PathUtil.WrapIndex((i-1),segments.Count, true)];
					if (nextSeg != seg && nextSeg.end == seg.start) {
						if (rightCapType == Ferr2D_CapColliderType.Connected || rightCapType == Ferr2D_CapColliderType.ConnectedSharp) {
							bool sharp = rightCapType == Ferr2D_CapColliderType.ConnectedSharp;
							AddConnectedColliderCap(true, invert?!rightInner:rightInner, sharp, nextSeg, ref points, normals);
						} else if (rightCapType == Ferr2D_CapColliderType.IntersectionSharp || rightCapType == Ferr2D_CapColliderType.IntersectionTop) {
							bool top = rightCapType == Ferr2D_CapColliderType.IntersectionTop;
							AddIntersectionColliderCap(true, invert?!rightInner:rightInner, top, nextSeg, ref points, normals);
						} else if (rightCapType == Ferr2D_CapColliderType.IntersectionEdge) {
							AddIntersectionEdgeColliderCap(true, invert?!rightInner:rightInner, nextSeg, ref points, normals );
						}
					}
				}
			}
			
			// track data for results
			if (aPhysicsMaterials2D != null)
				aPhysicsMaterials2D.Add(edgeData.PhysicsMaterial2D);
			if (aPhysicsMaterials3D != null)
				aPhysicsMaterials3D.Add(edgeData.PhysicsMaterial3D);
			result.Add(points);
		}
		
		return result;
	}
	List<Vector2> CreateFillCollider() {
		List<Vector2> fill = pathData.GetFinalPathCopy();
		Rect bounds = PathUtil.GetBounds(fill);
		if (useSkirt) {
			Vector2 start = fill[0];
			Vector2 end   = fill[fill.Count - 1];

			fill.Add(new Vector2(end.x, fillY));
			fill.Add(new Vector2(Mathf.Lerp(end.x, start.x, 0.33f), fillY));
			fill.Add(new Vector2(Mathf.Lerp(end.x, start.x, 0.66f), fillY));
			fill.Add(new Vector2(start.x, fillY));
		}

		if (fillMode == Ferr2D_SectionMode.Invert) {
			int   top     = 0;
			float highest = float.MinValue;
			for (int i = 0; i < fill.Count; i++) {
				if (fill[i].y > highest) {
					highest = fill[i].y;
					top = i;
				}
			}
			
			float width  = invertFillBorder.x == 0 ? bounds.width  : invertFillBorder.x;
			float height = invertFillBorder.y == 0 ? bounds.height : invertFillBorder.y;

			fill.InsertRange(top, new Vector2[] {
				fill[top],
				new Vector2((bounds.xMax + width), (bounds.yMax + height)),
				new Vector2((bounds.xMax + width), (bounds.yMin - height)),
				new Vector2((bounds.xMin - width), (bounds.yMin - height)),
				new Vector2((bounds.xMin - width), (bounds.yMax + height)),
				new Vector2((bounds.xMax + width), (bounds.yMax + height)),
				
			});
		}
		return fill;
	}
	#endregion

	#region Collider Caps
	int  Trim(bool aRightSide, ref List<Vector2> aPoints, Vector2[] aPts, Vector2[] aNormals) {
		int midpoint = aPoints.Count/2;
		if (aPts == null)
			return midpoint;

		if (aRightSide) {
			_TrimArray(ref aPoints, 0,               aPts, aNormals);
			_TrimArray(ref aPoints, aPoints.Count-1, aPts, aNormals);
		} else {
			midpoint = _TrimArray(ref aPoints, midpoint, aPts, aNormals);
		}
		return midpoint;
	}
	int  _TrimArray (ref List<Vector2> aPoints, int aStart, Vector2[] aPts, Vector2[] aNormals) {
		if (aStart < 0 || aStart > aPoints.Count)
			return aStart;

		int  curr    = aStart;
		bool removed = false;
		while (curr<aPoints.Count && _TrimCheckPlanes(aPoints[curr], aPts, aNormals)) {
			aPoints.RemoveAt(curr);
			removed = true;
		}
		curr-=1;
		while (curr>0 && curr<aPoints.Count && _TrimCheckPlanes(aPoints[curr], aPts, aNormals)) {
			aPoints.RemoveAt(curr);
			curr-=1;
			removed = true;
		}

		if (removed)
			return curr;
		return aStart-1;
	}
	bool _TrimCheckPlanes(Vector2 aPoint, Vector2[] aPts, Vector2[] aNormals) {
		for (int i = 0; i < aPts.Length; i++) {
			if (Vector2.Dot(aPoint - aPts[i], aNormals[i]) < 0)
				return false;
		}
		return true;
	}

	void CalcCapData(bool aRightSide, bool aInner, EdgeSegment aAdjSeg, List<Vector2> aPoints, List<Vector2> aNormals, 
			out Vector2 adjTop,  out Vector2 adjBot,  out Vector2 adjDir, 
			out Vector2 currTop, out Vector2 currBot, out Vector2 currDir ) {
		Vector2          capPt, capN;
		Ferr2D_PointData capData;
		bool             invert = edgeMode == Ferr2D_SectionMode.Invert;
		Vector2          upUV = UnitsPerUV;

		Ferr2DT_SegmentDescription nextData = TerrainMaterial.GetDescriptor(invert ? Invert( aAdjSeg.direction ) : aAdjSeg.direction);
		aInner = invert ? !aInner : aInner;
		
		float capOffset = aRightSide ? nextData.GetLeftCapOffset(aInner, upUV) + nextData.GetLeftCapSize(aInner, upUV): nextData.GetRightCapOffset(aInner, upUV) + nextData.GetRightCapSize(aInner, upUV);
		float capYOff   = nextData.GetYOffsetCollider(invert, upUV);
		float sampleAt  = aRightSide ? aAdjSeg.startDistance + aAdjSeg.length+capOffset : aAdjSeg.startDistance-capOffset;
		aAdjSeg.Sample(sampleAt, out capPt, out capN, out capData);
		
		// find the data we need from the adjacent edge
		Vector2 scaledNormal = capN*capData.scale;
		adjTop = capPt + scaledNormal * capYOff;
		adjBot = capPt + scaledNormal * (capYOff -  nextData.ColliderThickness * nextData.GetBodyMaxHeight() * upUV.y);
		adjDir = aRightSide ? capN.Rot90CCW() : capN.Rot90CW();

		// find the data we need from the current edge
		if (aRightSide) {
			currDir = aNormals[0].Rot90CW();
			currTop = aPoints[0];
			currBot = aPoints[aPoints.Count-1];
		} else {
			int midpoint = aPoints.Count/2-1;
			currDir = aNormals[midpoint].Rot90CCW();
			currTop = aPoints[midpoint];
			currBot = aPoints[midpoint+1];
		}
	}
	void AddConnectedColliderCap       (bool aRightSide, bool aInner, bool aSharp, EdgeSegment aToSeg, ref List<Vector2> aPoints, List<Vector2> aNormals) {
		Ferr2DT_SegmentDescription nextData = TerrainMaterial.GetDescriptor(edgeMode == Ferr2D_SectionMode.Invert ? Invert( aToSeg.direction ) : aToSeg.direction);
		if (nextData.ColliderThickness <= 0)
			return;

		// get data for the caps
		Vector2 adjTop, adjBottom, adjDir, dir, top, bot;
		CalcCapData(aRightSide, aInner, aToSeg, aPoints, aNormals,
			out adjTop, out adjBottom, out adjDir, out top, out bot, out dir);
		
		// If it's at extreme angles, we'll need to treat things differently
		float dot = Vector2.Dot(adjDir, -dir);

		// Snip verts that are in the way of our collider cap
		Vector2[] planePts = null, planeDirs = null;
		if        (dot < -0.95f) { // near parallel facing the same direction
		} else if (dot >  0.75f) { // near parallel facing towards eachother
			planePts  = new Vector2[] {  adjTop };
			planeDirs = new Vector2[] { -adjDir };
		} else                   { // normal range
			planePts  = new Vector2[] { aInner?adjTop           : adjBottom        };
			planeDirs = new Vector2[] { aInner?adjBottom-adjTop : adjTop-adjBottom };
		}
		int midpoint = Trim(aRightSide, ref aPoints, planePts, planeDirs);

		// and add the new cap points
		if        (dot < -0.95f) { // near parallel facing the same direction
		} else if (dot >  0.75f) { // near parallel facing towards eachother
			midpoint += 1;
			aPoints.Insert(aRightSide?aPoints.Count:midpoint, adjBottom);
			aPoints.Insert(aRightSide?aPoints.Count:midpoint, adjTop);
		} else                  { // normal range
			midpoint += 1;
			aPoints.Insert(aRightSide?aPoints.Count:midpoint, aSharp || !aInner ? PathUtil.LineIntersectionPoint(bot, bot+dir, adjBottom, adjBottom+adjDir) : bot);
			aPoints.Insert(aRightSide?aPoints.Count:midpoint, adjBottom);
			aPoints.Insert(aRightSide?aPoints.Count:midpoint, adjTop);
			aPoints.Insert(aRightSide?aPoints.Count:midpoint, aSharp ||  aInner ? PathUtil.LineIntersectionPoint(top, top+dir, adjTop, adjTop+adjDir) : top);
		}
	}
	void AddIntersectionColliderCap    (bool aRightSide, bool aInner, bool aOnlyBottom, EdgeSegment aToSeg, ref List<Vector2> aPoints, List<Vector2> aNormals) {
		Ferr2DT_SegmentDescription nextData = TerrainMaterial.GetDescriptor(edgeMode == Ferr2D_SectionMode.Invert ? Invert( aToSeg.direction ) : aToSeg.direction);
		if (nextData.ColliderThickness <= 0)
			return;

		// get data for the caps
		Vector2 adjTop, adjBottom, adjDir, dir, top, bot;
		CalcCapData(aRightSide, aInner, aToSeg, aPoints, aNormals,
			out adjTop, out adjBottom, out adjDir, out top, out bot, out dir);
		
		Vector2 intersectTop    = aOnlyBottom && !aInner ? top : PathUtil.LineIntersectionPoint(top, top+dir, adjTop,    adjTop   +adjDir);
		Vector2 intersectBottom = aOnlyBottom &&  aInner ? bot : PathUtil.LineIntersectionPoint(bot, bot+dir, adjBottom, adjBottom+adjDir);
		Vector2 intersectNormal = (intersectTop-intersectBottom);
		intersectNormal = aRightSide ? intersectNormal.Rot90CW() : intersectNormal.Rot90CCW();

		// If it's at extreme angles, we'll need to treat things differently
		float dot = Vector2.Dot(adjDir, -dir);
		
		if (dot <= 0.75f && (dot >= -0.95f)) {
			Vector2[] planePts  = new Vector2[] { intersectTop    };
			Vector2[] planeDirs = new Vector2[] { intersectNormal };
			int midpoint = Trim(aRightSide, ref aPoints, planePts, planeDirs) + 1;
		
			aPoints.Insert(aRightSide?aPoints.Count:midpoint, intersectBottom);
			aPoints.Insert(aRightSide?aPoints.Count:midpoint, intersectTop);
		}
	}
	void AddIntersectionEdgeColliderCap(bool aRightSide, bool aInner, EdgeSegment aToSeg, ref List<Vector2> aPoints, List<Vector2> aNormals) {
		Ferr2DT_SegmentDescription nextData = TerrainMaterial.GetDescriptor(edgeMode == Ferr2D_SectionMode.Invert ? Invert( aToSeg.direction ) : aToSeg.direction);
		if (nextData.ColliderThickness == 0)
			return;

		// get data for the caps
		Vector2 adjTop, adjBottom, adjDir, dir, top, bot;
		CalcCapData(aRightSide, aInner, aToSeg, aPoints, aNormals,
			out adjTop, out adjBottom, out adjDir, out top, out bot, out dir);
		Vector2 adjUp = aRightSide?adjDir.Rot90CW():adjDir.Rot90CCW();

		Vector2 intersectCap,intersectBottom,intersectInner;
		if (aInner) {
			intersectCap    = PathUtil.LineIntersectionPoint(adjTop, adjBottom,     bot, bot+dir);
			intersectBottom = PathUtil.LineIntersectionPoint(adjTop, adjTop+adjDir, bot, bot+dir);
			intersectInner  = PathUtil.LineIntersectionPoint(top, top+dir, adjTop, adjTop+adjDir);
		} else {
			intersectCap    = PathUtil.LineIntersectionPoint(adjTop,    adjBottom,        top, top+dir);
			intersectBottom = PathUtil.LineIntersectionPoint(adjBottom, adjBottom+adjDir, top, top+dir);
			intersectInner  = PathUtil.LineIntersectionPoint(bot, bot+dir, adjBottom, adjBottom+adjDir);
		}

		bool capIsBelow = Vector2.Dot(intersectCap-adjBottom, -adjUp) >= 0;
		bool capIsAbove = Vector2.Dot(intersectCap-adjTop,     adjUp) >= 0;
		Vector2 v1 = capIsBelow || capIsAbove ? intersectBottom : intersectCap;

		// If it's at extreme angles, we'll need to treat things differently
		float dot = Vector2.Dot(adjDir, -dir);
		
		if (dot <= 0.75f && dot >= -0.95f) {
			Vector2 cullDir = aInner ? intersectInner-v1 : v1-intersectInner;
			cullDir = aRightSide ? cullDir.Rot90CW():cullDir.Rot90CCW();
			Vector2[] planePts  = new Vector2[] { v1 };
			Vector2[] planeDirs = new Vector2[] { cullDir };
			int midpoint = Trim(aRightSide, ref aPoints, planePts, planeDirs) + 1;
			
			if (aInner) {
				aPoints.Insert(aRightSide?aPoints.Count:midpoint, v1);
				aPoints.Insert(aRightSide?aPoints.Count:midpoint, adjBottom);
				aPoints.Insert(aRightSide?aPoints.Count:midpoint, intersectInner);
			} else {
				aPoints.Insert(aRightSide?aPoints.Count:midpoint, intersectInner);
				aPoints.Insert(aRightSide?aPoints.Count:midpoint, adjTop);
				aPoints.Insert(aRightSide?aPoints.Count:midpoint, v1);
			}
		}
	}
	void AddCircleColliderCap      (bool aRightSide, ref List<Vector2> aPoints, Vector2 aNormal, Vector2 aPt, float aWidth, float aYOffset, float aThickness) {
		Vector2 dir = new Vector2(aNormal.y, -aNormal.x) * aWidth;
		int start = aPoints.Count/2;

		int steps = 8;
		for (int a = 1; a < steps; a++) {
			float addAngle = (a * (180f/steps) + 90);
			if (aRightSide) addAngle = addAngle+180;
			addAngle *= Mathf.Deg2Rad;

			Vector2 c = Mathf.Cos(addAngle)*dir + Mathf.Sin(addAngle)*aNormal*aThickness*.5f;
			c += aNormal * (aYOffset-aThickness/2);
			if (!aRightSide) {
				aPoints.Insert(start,aPt+c);
				start += 1;
			}  else
				aPoints.Add(aPt + c);
		}
	}
	#endregion
	
	#region Mesh manipulation methods
	private void AddMaterials(bool aHasFill, bool aHasEdge) {
		Material[] newMaterials = null;

		if (aHasFill && aHasEdge) {
			newMaterials = new Material[] {
                TerrainMaterial.fillMaterial,
                TerrainMaterial.edgeMaterial
            };
		} else if (aHasEdge) {
			newMaterials = new Material[] {
                TerrainMaterial.edgeMaterial
            };
		} else if (aHasFill) {
			newMaterials = new Material[] {
                TerrainMaterial.fillMaterial
            };
		} else {
			newMaterials = new Material[0];
		}
		GetComponent<Renderer>().sharedMaterials = newMaterials;
	}
	#endregion
	
	#region Supporting methods
    /// <summary>
    /// Sets the material of the mesh.
    /// </summary>
    /// <param name="aMaterial">The terrain material! Usually from a terrain material asset.</param>
    public  void                        SetMaterial     (IFerr2DTMaterial aMaterial) {
		if (IsLegacy) {
			ForceMaterial(aMaterial, false);
			return;
		}

		terrainMaterialInterface = (UnityEngine.Object)aMaterial;
    }
    /// <summary>
    /// Adds a terrain vertex at the specified index, or at the end if the index is -1. Returns the index of the added vert. Does not rebuild meshes.
    /// </summary>
    /// <param name="aPt">The terrain point to add, z is always 0</param>
    /// <param name="aAtIndex">The index to put the point at, or -1 to put at the end</param>
    /// <returns>Index of the point</returns>
    public  int                         AddPoint        (Vector2 aPt, int aAtIndex = -1, PointType aType = PointType.Sharp) {
	    if (IsLegacy)
			return LegacyAddPoint(aPt, aAtIndex);

		if (aAtIndex == -1) {
			return pathData.Add(aPt, new Ferr2D_PointData(1), aType);
		} else {
			pathData.Insert(aAtIndex, aPt, new Ferr2D_PointData(1), aType);
			return aAtIndex;
		}
    }
	/// <summary>
    /// Inserts a point into the path, automatically determining insert index using Ferr2DT_Path.GetClosestSeg. Does not rebuild meshes.
	/// </summary>
	/// <returns>The index of the point that was just added.</returns>
	/// <param name="aPt">A 2D point to add to the path.</param>
	public int                          AddAutoPoint    (Vector2 aPt, PointType aType = PointType.Sharp) {
		if (IsLegacy)
			return LegacyAddAutoPoint(aPt);

		int at = PathUtil.GetClosestSegment(pathData.GetPathRaw(), aPt, pathData.Closed);
		return AddPoint(aPt, at+1 == pathData.Count ? -1 : at+1, aType );
	}
    /// <summary>
    /// Removes the indicated point as well as corresponding data. Does not rebuild meshes.
    /// </summary>
    /// <param name="aPtIndex">Index of the point</param>
    public void                         RemovePoint     (int     aPtIndex) {
       if (IsLegacy) {
			LegacyRemovePoint(aPtIndex);
			return;
		}
       pathData.RemoveAt(aPtIndex);
    }
    /// <summary>
    /// Removes all points from the terrain properly. Does not rebuild meshes.
    /// </summary>
    public void                         ClearPoints     () {
		if (IsLegacy) {
			LegacyClearPoints();
			return;
		}
        pathData.Clear();
    }
	#endregion

	#region IBlendPaintable
	#if UNITY_EDITOR
	public static bool showGUI = true;
	#endif
	public void OnPainterSelected(IBlendPaintType aType) {
		if (GetComponent<Collider>() == null) {
			BoxCollider box = gameObject.AddComponent<BoxCollider>();
			box.size = new Vector3(100000, 100000, 0);
			box.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
		}
		this.vertexColorType = Ferr2DT_ColorType.PreserveVertColor;

		#if UNITY_EDITOR
		showGUI = false;
		#endif
	}
	public void OnPainterUnselected(IBlendPaintType aType) {
		BoxCollider box = GetComponent<BoxCollider>();
		if (box != null)
			DestroyImmediate(box);
		#if UNITY_EDITOR
		showGUI = true;
		#endif
	}
	#endregion
}