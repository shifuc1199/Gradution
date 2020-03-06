using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes how the terrain path should be filled.
/// </summary>
public enum Ferr2DT_FillMode
{
    /// <summary>
    /// The interior of the path will be filled, and edges will be treated like a polygon.
    /// </summary>
    Closed,
    /// <summary>
    /// Drops some extra vertices down, and fill the interior. Edges only around the path itself.
    /// </summary>
    Skirt,
    /// <summary>
    /// Doesn't fill the interior at all. Just edges.
    /// </summary>
    None,
    /// <summary>
    /// Fills the outside of the path rather than the interior, also inverts the edges, upside-down.
    /// </summary>
    InvertedClosed,
    /// <summary>
    /// Just like Closed, but with no edges
    /// </summary>
    FillOnlyClosed,
    /// <summary>
    /// Just like Skirt, but with no edges
    /// </summary>
    FillOnlySkirt
}

public partial class Ferr2DT_PathTerrain {
	[Serializable]
	public class CutOverrides {
		public List<int> data;
	}

	#region Fields
	public Ferr2DT_FillMode fill    = Ferr2DT_FillMode.Closed;
	/// <summary>
    /// [Legacy] Makes the path curvy. It's not a perfect algorithm just yet, but it does make things curvier.
    /// </summary>
    public bool     smoothPath      = false;
	/// <summary>
    /// [Legacy] On smoothed surfaces, the distance between each split on the curve (Unity units)
    /// </summary>
    public int      splitCount      = 4;
    /// <summary>
    /// [Legacy] A modifier that allows you to specify a multiplier for many cuts go into the fill/collider relative to the initial value
    /// </summary>
	[Range(0.1f, 4)]
    public float    splitDist       = 1;
	/// <summary>
    /// [Legacy] Split the edges in half, lengthwise. This doubles tri count along edges, but can improve texture stretching along corners
    /// or turns
    /// </summary>
	public bool     splitMiddle     = true;
	/// <summary>
    /// [Legacy] Randomizes edge pieces based on its individual location, rather than by the location of the segment. This is great for
    /// terrain that might get procedurally modified in such a way that all segments get shuffled.
    /// </summary>
    public bool     randomByWorldCoordinates = false;
	/// <summary>
    /// [Legacy] Should we generate a collider on Start?
    /// </summary>
    public bool     createCollider    = true;
	/// <summary>
	/// [Legacy] Use this to force a 3D mesh collider instead of a 2D collider
	/// </summary>
	public bool     create3DCollider  = false;
	/// <summary>
    /// [Legacy] Generates colliders with sharp angles at the corners. Edges are extended by the cap size, and cut off at the intersection point.
    /// </summary>
    public bool     sharpCorners           = false;
    /// <summary>
    /// [Legacy] How far should the collider edges be extended to create a sharp corner?
    /// </summary>
    public float    sharpCornerDistance    = 2;
	/// <summary>
    /// [Legacy] An option to pass along for 3D colliders
    /// </summary>
    public bool     smoothSphereCollisions = false;
	/// <summary>
    /// [Legacy] For offseting the collider, so it can line up with stuff better visually. On fill = None terrain,
    /// this behaves significantly different than regular closed terrain.
    /// </summary>
    public float[]  surfaceOffset          = new float[] {0,0,0,0};
	/// <summary>
	/// [Legacy]Use a 2D edge collider instead of a polygon collider
	/// </summary>
	public bool     useEdgeCollider   = false;
	/// <summary> [Legacy] </summary>
	public float    colliderThickness = 0.1f;
	
	/// <summary> [Legacy] </summary>
    public bool  collidersLeft     = true;
	/// <summary> [Legacy] </summary>
    public bool  collidersRight    = true;
	/// <summary> [Legacy] </summary>
    public bool  collidersTop      = true;
	/// <summary> [Legacy] </summary>
    public bool  collidersBottom   = true;
	
	/// <summary> [Legacy] </summary>
	[SerializeField] public List<Ferr2DT_TerrainDirection> directionOverrides = null;
	/// <summary> [Legacy] </summary>
	[SerializeField] public List<CutOverrides>             cutOverrides       = null;
	/// <summary> [Legacy] </summary>
    [SerializeField] public List<float>                    vertScales         = null;

	Ferr2D_Path legacyPath;
	#endregion

	#region Properties
	/// <summary>
	/// [Legacy]
	/// </summary>
	public Ferr2D_Path Path { get { 
        if (legacyPath == null) 
            legacyPath = GetComponent<Ferr2D_Path>(); 
        return legacyPath; 
    } }
	#endregion

	#region Upgrade!
	public void LegacyUpgrade() {
		if (!IsLegacy)
			return;

		#if UNITY_EDITOR
		UnityEditor.Undo.RecordObject(gameObject, "Upgrade Ferr2D Terrain");
		#endif

		Ferr2D_Path oldPath = GetComponent<Ferr2D_Path>();
		MatchOverrides();

		// upgrade the path
		pathData = new Ferr2DPath();
		pathData.Closed = oldPath.closed;
		for (int i = 0; i < oldPath.pathVerts.Count; i++) {
			int next = Ferr.PathUtil.WrapIndex(i-1, Path.Count, Path.closed);
			Ferr.PointType pointType = Ferr.PointType.Sharp;
			if (smoothPath) {
				Ferr2DT_TerrainDirection prevSegmentDirection = Ferr2D_Path.GetDirection(Path.pathVerts, next, fill == Ferr2DT_FillMode.InvertedClosed, Path.closed, directionOverrides);
				Ferr2DT_TerrainDirection nextSegmentDirection = Ferr2D_Path.GetDirection(Path.pathVerts, i,    fill == Ferr2DT_FillMode.InvertedClosed, Path.closed, directionOverrides);
				if (prevSegmentDirection == nextSegmentDirection)
					pointType = Ferr.PointType.Auto;
			}

			Ferr2D_PointData data = new Ferr2D_PointData();
			data.scale             = vertScales[next];
			data.directionOverride = (int)directionOverrides[next];
			data.cutOverrides      = cutOverrides[next].data;
			pathData.Add(oldPath.pathVerts[i], data, pointType);
		}
		pathData.ReverseSelf();
		pathData.SetDirty();
		
		// remove old path values
		directionOverrides = null;
		cutOverrides       = null;
		vertScales         = null;

		// upgrade collider settings
		if (createCollider) {
			if (useEdgeCollider) colliderMode = Ferr2D_ColliderMode.Edge2D;
			else if (create3DCollider) colliderMode = Ferr2D_ColliderMode.Mesh3D;
			else colliderMode = Ferr2D_ColliderMode.Polygon2D;
		} else {
			colliderMode = Ferr2D_ColliderMode.None;
		}

		// upgrade the fill settings
		switch (fill) {
			case Ferr2DT_FillMode.None:
				edgeMode = Ferr2D_SectionMode.Normal;
				fillMode = Ferr2D_SectionMode.None; break;
			case Ferr2DT_FillMode.Closed:
				edgeMode = Ferr2D_SectionMode.Normal;
				fillMode = Ferr2D_SectionMode.Normal; break;
			case Ferr2DT_FillMode.InvertedClosed:
				edgeMode = Ferr2D_SectionMode.Invert;
				fillMode = Ferr2D_SectionMode.Invert; break;
			case Ferr2DT_FillMode.FillOnlyClosed:
				edgeMode = Ferr2D_SectionMode.None;
				fillMode = Ferr2D_SectionMode.Normal; break;
			case Ferr2DT_FillMode.Skirt:
				edgeMode = Ferr2D_SectionMode.Normal;
				fillMode = Ferr2D_SectionMode.Normal;
				useSkirt = true; break;
			case Ferr2DT_FillMode.FillOnlySkirt:
				edgeMode = Ferr2D_SectionMode.None;
				fillMode = Ferr2D_SectionMode.Normal;
				useSkirt = true; break;
		}

		isLegacy = false;

		#if UNITY_EDITOR
		UnityEditor.Undo.DestroyObjectImmediate(oldPath);
		#else
		Destroy(oldPath);
		#endif

		Build(true);

		#if UNITY_EDITOR
		UnityEditor.SceneView.RepaintAll();
		#endif
	}
	#endregion

	#region Private Legacy Methods
	private void LegacyAwake() {
		if (createCollider && GetComponent<Collider>() == null && GetComponent<Collider2D>() == null) {
            RecreateCollider();
        }
        for (int i = 0; i < Camera.allCameras.Length; i++) {
            Camera.allCameras[i].transparencySortMode = TransparencySortMode.Orthographic;
        }
	}
	private void LegacyBuild(bool aFullBuild) {
		if (TerrainMaterial == null) {
            Debug.LogWarning("Cannot create terrain without a Terrain Material!");
            return;
        }
        if (Path.Count < 2) {
            GetComponent<MeshFilter>().sharedMesh = null;
            return;
        }
	    
	    MarkColorSave();
		MatchOverrides();
	    ForceMaterial (TerrainMaterial, true, false);

		DMesh.Clear ();
		
        if (fill != Ferr2DT_FillMode.FillOnlyClosed && fill != Ferr2DT_FillMode.FillOnlySkirt) {
            LegacyAddEdge();
        }
		int[] submesh1 = DMesh.GetCurrentTriangleList();
		
		// add a fill if the user desires
	    if        ((fill == Ferr2DT_FillMode.Skirt  || fill == Ferr2DT_FillMode.FillOnlySkirt) && TerrainMaterial.fillMaterial != null) {
	        LegacyAddFill(true,  aFullBuild);
	    } else if ((fill == Ferr2DT_FillMode.Closed || fill == Ferr2DT_FillMode.InvertedClosed || fill == Ferr2DT_FillMode.FillOnlyClosed) && TerrainMaterial.fillMaterial != null) {
            LegacyAddFill(false, aFullBuild);
        }
		int[] submesh2 = DMesh.GetCurrentTriangleList(submesh1.Length);

        // compile the mesh!
        Mesh m = GetComponent<MeshFilter>().sharedMesh = GetMesh();
	    DMesh.Build(ref m, createTangents && aFullBuild);
	    
	    LegacyCreateVertColors();

        // set up submeshes and submaterials
        if (submesh1.Length > 0 && submesh2.Length > 0) {
            m.subMeshCount = 2;
            m.SetTriangles(submesh1, 1);
            m.SetTriangles(submesh2, 0);
        } else if (submesh1.Length > 0) {
            m.subMeshCount = 1;
            m.SetTriangles(submesh1, 0);
        } else if (submesh2.Length > 0) {
            m.subMeshCount = 1;
            m.SetTriangles(submesh2, 0);
        }
	    
	    bool hasCollider = GetComponent<MeshCollider>() != null || GetComponent<PolygonCollider2D>() != null || GetComponent<EdgeCollider2D>() != null;
	    if (createCollider && hasCollider) {
		    RecreateCollider();
	    }
#if UNITY_EDITOR
	    if (aFullBuild && gameObject.isStatic) {
            UnityEditor.Unwrapping.GenerateSecondaryUVSet(m);
        }
#endif
	    
	    if (aFullBuild) ClearColorSave();
	}
	private void LegacyCreateVertColors() {
		Mesh mesh = MeshFilter.sharedMesh;
		
		switch(vertexColorType) {
			case Ferr2DT_ColorType.SolidColor:        ColorVertsSolid           (mesh, vertexColor);
				break;
			case Ferr2DT_ColorType.Gradient:          ColorVertsGradient        (mesh, vertexGradient, vertexGradientAngle);
				break;
			case Ferr2DT_ColorType.DistanceGradient:  ColorVertsDistanceGradient(mesh, Path.pathVerts, Path.closed, vertexGradient, vertexGradientDistance);
				break;
			case Ferr2DT_ColorType.PreserveVertColor: ColorVertsRecolor         (mesh, recolorTree);
				break;
		}
	}
	

	private int  LegacyAddPoint(Vector2 aPt, int aAtIndex = -1) {
		MatchOverrides();
	    
        if (aAtIndex == -1) {
            Path              .Add(aPt                          );
            directionOverrides.Add(Ferr2DT_TerrainDirection.None);
			cutOverrides      .Add(new CutOverrides()         );
            vertScales        .Add(1                            );
            return Path.pathVerts.Count;
        } else {
            Path.pathVerts    .Insert(aAtIndex, aPt                          );
            directionOverrides.Insert(aAtIndex, Ferr2DT_TerrainDirection.None);
			cutOverrides      .Insert(aAtIndex, new CutOverrides()         );
			vertScales        .Insert(aAtIndex, 1                            );
            return aAtIndex;
        }
	}
	private int  LegacyAddAutoPoint(Vector2 aPt) {
		int at = Path.GetClosestSeg(aPt);
		return LegacyAddPoint(aPt, at+1 == Path.pathVerts.Count ? -1 : at+1 );
	}
	private void LegacyRemovePoint(int aPtIndex) {
		if (aPtIndex < 0 || aPtIndex >= Path.pathVerts.Count) throw new ArgumentOutOfRangeException();
		Path.pathVerts    .RemoveAt(aPtIndex);
        directionOverrides.RemoveAt(aPtIndex);
		cutOverrides      .RemoveAt(aPtIndex);
		vertScales        .RemoveAt(aPtIndex);
	}
	private void LegacyClearPoints() {
		Path.pathVerts    .Clear();
        directionOverrides.Clear();
		cutOverrides      .Clear();
        vertScales        .Clear();
	}
	#endregion

	#region Edge Building
	private void LegacyAddEdge() {
        // split the path into segments based on the split angle
        List<List<int>               > segments = new List<List<int>               >();
        List<Ferr2DT_TerrainDirection> dirs     = new List<Ferr2DT_TerrainDirection>();
        List<int                     > order    = new List<int>();
        
        segments = GetSegments(Path.GetVertsRaw(), out dirs);
        if (dirs.Count < segments.Count) 
            dirs.Add(directionOverrides[directionOverrides.Count - 1]);
        for (int i = 0; i < segments.Count; i++) order.Add(i);
        
        order.Sort(
            new Ferr.LambdaComparer<int>(
                (x, y) => {
                    Ferr2DT_TerrainDirection dirx = dirs[x] == Ferr2DT_TerrainDirection.None ? Ferr2D_Path.GetDirection(Path.pathVerts, segments[x], 0, fill == Ferr2DT_FillMode.InvertedClosed) : dirs[x];
                    Ferr2DT_TerrainDirection diry = dirs[y] == Ferr2DT_TerrainDirection.None ? Ferr2D_Path.GetDirection(Path.pathVerts, segments[y], 0, fill == Ferr2DT_FillMode.InvertedClosed) : dirs[y];
	                return TerrainMaterial.GetDescriptor(diry).zOffset.CompareTo(TerrainMaterial.GetDescriptor(dirx).zOffset);
                }
            ));

        // process the segments into meshes
	    for (int i = 0; i < order.Count; i++) {
		    List<int> currSeg = segments[order[i]];
		    List<int> prevSeg = order[i]-1 < 0 ?  segments[segments.Count-1] : segments[order[i]-1];
		    List<int> nextSeg = segments[(order[i]+1) % segments.Count];
		    
		    int curr = currSeg[0];
		    int prev = prevSeg[prevSeg.Count-2];
		    int next = currSeg[1];
		    
		    Vector2 p1 = Path.pathVerts[prev] - Path.pathVerts[curr];
		    Vector2 p2 = Path.pathVerts[next] - Path.pathVerts[curr];
		    bool leftInner = Mathf.Atan2(p1.x*p2.y - p1.y*p2.x, Vector2.Dot(p1, p2)) < 0;
		    
		    curr = currSeg[currSeg.Count-1];
		    prev = currSeg[currSeg.Count-2];
		    next = nextSeg[1];
		    
		    p1 = Path.pathVerts[prev] - Path.pathVerts[curr];
		    p2 = Path.pathVerts[next] - Path.pathVerts[curr];
		    bool rightInner = Mathf.Atan2(p1.x*p2.y - p1.y*p2.x, Vector2.Dot(p1, p2)) < 0;
		    
		    LegacyAddSegment(Ferr2D_Path.IndicesToList<Vector2>(Path.pathVerts, segments[order[i]]), leftInner, rightInner, Ferr2D_Path.IndicesToList<float>(vertScales, segments[order[i]]), Ferr2D_Path.IndicesToList<CutOverrides>(cutOverrides, segments[order[i]]), order.Count <= 1 && Path.closed, smoothPath, dirs[order[i]]);
        }
    }
	private void LegacyAddSegment       (List<Vector2> aSegment, bool aLeftInner, bool aRightInner, List<float> aScale, List<CutOverrides> aCutOverrides, bool aClosed, bool aSmooth, Ferr2DT_TerrainDirection aDir = Ferr2DT_TerrainDirection.None) {
		Ferr2DT_SegmentDescription desc;
		if (aDir != Ferr2DT_TerrainDirection.None) { desc = TerrainMaterial.GetDescriptor(aDir); }
		else                                       { desc = GetDescription(aSegment);            }

        #if UNITY_5_4_OR_NEWER
		UnityEngine.Random.State tSeed = UnityEngine.Random.state;
        #else
        int tSeed = UnityEngine.Random.seed;
        #endif
		Rect    body      = TerrainMaterial.ToUV( desc.body[0] );
		float   bodyWidth = body.width * unitsPerUV.x;
		
		#if UNITY_5_4_OR_NEWER
		UnityEngine.Random.InitState((int)(aSegment[0].x * 100000 + aSegment[0].y * 10000));
		#else
		UnityEngine.Random.seed = (int)(aSegment[0].x * 100000 + aSegment[0].y * 10000);
		#endif
		
        Vector2 capLeftSlideDir  = (aSegment[1                 ] - aSegment[0                 ]);
        Vector2 capRightSlideDir = (aSegment[aSegment.Count - 2] - aSegment[aSegment.Count - 1]);
        capLeftSlideDir .Normalize();
        capRightSlideDir.Normalize();
        aSegment[0                 ] -= capLeftSlideDir  * desc.capOffset;
        aSegment[aSegment.Count - 1] -= capRightSlideDir * desc.capOffset;
		
		LegacyCreateBody(desc, aSegment, aScale, aCutOverrides, bodyWidth, Mathf.Max(2, splitCount+2), aClosed);
        
		if (!aClosed) {
			LegacyAddCap(aSegment, desc, aLeftInner, -1, aScale[0], aSmooth);
			LegacyAddCap(aSegment, desc, aRightInner, 1, aScale[aScale.Count-1], aSmooth);
		}
		#if UNITY_5_4_OR_NEWER
		UnityEngine.Random.state = tSeed;
		#else
		UnityEngine.Random.seed = tSeed;
		#endif
	}
	private void LegacyCreateBody       (Ferr2DT_SegmentDescription aDesc, List<Vector2> aSegment, List<float> aSegmentScale, List<CutOverrides> aCutOverrides, float aBodyWidth, int aTextureSlices, bool aClosed) {
		float distance    = Ferr2D_Path.GetSegmentLength(aSegment);
		int   textureCuts = Mathf.Max(1, Mathf.FloorToInt(distance / aBodyWidth + 0.5f));

		Ferr2D_DynamicMesh mesh = DMesh;

		Rect  body = LegacyPickBody(aDesc, aCutOverrides, aSegment[0], 0, 0);
		float d    = (body.height / 2) * unitsPerUV.y;
		float yOff = fill == Ferr2DT_FillMode.InvertedClosed ? -aDesc.yOffset : aDesc.yOffset;
		
		int p1 = 0, p2 = 0, p3 = 0;
		int pIndex = 0;
		int cutIndex = 0;
		// loop for each instance of the texture
		for (int t = 0; t < textureCuts; t++) {
			float texPercent     = (t/(float)(textureCuts));
			float texPercentStep = (1f/(float)(textureCuts));
			
			// slice each texture chunk a number of times
			for (int i = 0; i < aTextureSlices; i++) {
				float slicePercent = (i/(float)(aTextureSlices-1));
				float totalPercent = texPercent + slicePercent*texPercentStep;

				int   ptLocal  = 0;
				float pctLocal = 0;
				Ferr2D_Path.PathGlobalPercentToLocal(aSegment, totalPercent, out ptLocal, out pctLocal, distance, aClosed);

				// if we skip over path points, we need to add slices at each path point to prevent the mesh from bypassing it.
				for (int extra = 0; extra < ptLocal-pIndex; extra++) {
					float traveledDist = Ferr2D_Path.GetSegmentLengthToIndex(aSegment, pIndex + extra + 1);
					float v = (traveledDist / distance) * textureCuts;
					v = v-(int)v;
					LegacyAddVertexColumn(aDesc, aSegment, aSegmentScale, aClosed, mesh, body, d, yOff, i!=0, v, pIndex + extra + 1, 0, ref p1, ref p2, ref p3);
					cutIndex = -1;
				}
				pIndex = ptLocal;
				
				LegacyAddVertexColumn(aDesc, aSegment, aSegmentScale, aClosed, mesh, body, d, yOff, i!=0, slicePercent, ptLocal, pctLocal, ref p1, ref p2, ref p3);
			}

			cutIndex += 1;
			body = LegacyPickBody(aDesc, aCutOverrides, mesh.GetVert(p2), pIndex, cutIndex);
		}
	}
	private Rect LegacyPickBody         (Ferr2DT_SegmentDescription aDesc, List<CutOverrides> aCutOverrides, Vector2 aStartPos, int aCurrIndex, int aCurrCut) {
		int  cutOverride = -1;
		Rect result = default(Rect);
		
		if (aCutOverrides[aCurrIndex].data != null && aCurrCut < aCutOverrides[aCurrIndex].data.Count)
			cutOverride = aCutOverrides[aCurrIndex].data[aCurrCut]-1;

		if (cutOverride == -1 || cutOverride >= aDesc.body.Length) {
			result = LegacyGetRandomBodyRect(aStartPos, aDesc);
		} else {
			// trigger this so a random number is consumed
			LegacyGetRandomBodyRect(aStartPos, aDesc);
			result = TerrainMaterial.ToUV(aDesc.body[cutOverride]);
		}
		return result;
	}
	private Rect LegacyGetRandomBodyRect(Vector2 aInitialPos, Ferr2DT_SegmentDescription aDesc) {
#if UNITY_5_4_OR_NEWER
		UnityEngine.Random.State tSeed = default(UnityEngine.Random.State);
#else
        int tSeed = 0;
#endif
		if (randomByWorldCoordinates) {
#if UNITY_5_4_OR_NEWER
			tSeed = UnityEngine.Random.state;
			UnityEngine.Random.InitState((int)(aInitialPos.x + aInitialPos.y));
#else
            tSeed = UnityEngine.Random.seed;
            UnityEngine.Random.seed = (int)(aInitialPos.x + aInitialPos.y);
#endif
		}

		Rect body = TerrainMaterial.ToUV(aDesc.body[UnityEngine.Random.Range(0, aDesc.body.Length)]);
		if (randomByWorldCoordinates) {
#if UNITY_5_4_OR_NEWER
			UnityEngine.Random.state = tSeed;
#else
			UnityEngine.Random.seed = tSeed;
#endif
		}
		return body;
	}
	private void LegacyAddVertexColumn  (Ferr2DT_SegmentDescription aDesc, List<Vector2> aSegment, List<float> aSegmentScale, bool aClosed, Ferr2D_DynamicMesh mesh, Rect body, float d, float yOff, bool aConnectFace, float slicePercent, int ptLocal, float pctLocal, ref int p1, ref int p2, ref int p3) {
		
		Vector2 pos1 = smoothPath ? Ferr2D_Path.HermiteGetPt    (aSegment, ptLocal, pctLocal, aClosed) : Ferr2D_Path.LinearGetPt    (aSegment, ptLocal, pctLocal, aClosed);
		Vector2 n1   = smoothPath ? Ferr2D_Path.HermiteGetNormal(aSegment, ptLocal, pctLocal, aClosed) : Ferr2D_Path.LinearGetNormal(aSegment, ptLocal, pctLocal, aClosed);
		float   s    = aClosed    ? Mathf.Lerp(aSegmentScale[ptLocal], aSegmentScale[(ptLocal+1)%aSegmentScale.Count], pctLocal) : Mathf.Lerp(aSegmentScale[ptLocal], aSegmentScale[Mathf.Min(ptLocal+1, aSegmentScale.Count-1)], pctLocal);
		
		// this compensates for scale distortion when corners are very sharp, but the normals are not long enough to keep the edge the appropriate width
		// not actually a problem for smooth paths
		if (!smoothPath) {
			n1.Normalize();
			float rootScale = 1f/Mathf.Abs(Mathf.Cos(Vector2.Angle(Ferr2D_Path.GetSegmentNormal(ptLocal, aSegment, aClosed), n1)*Mathf.Deg2Rad));
			s = s * rootScale;
		}

		int v1 = mesh.AddVertex(pos1.x + n1.x * (d*s + yOff), pos1.y + n1.y * (d*s + yOff), -slantAmount + aDesc.zOffset, Mathf.Lerp(body.x, body.xMax, slicePercent), fill == Ferr2DT_FillMode.InvertedClosed ? body.yMax : body.y);
		int v2 = mesh.AddVertex(pos1.x - n1.x * (d*s - yOff), pos1.y - n1.y * (d*s - yOff),  slantAmount + aDesc.zOffset, Mathf.Lerp(body.x, body.xMax, slicePercent), fill == Ferr2DT_FillMode.InvertedClosed ? body.y : body.yMax);
		int v3 = splitMiddle ? mesh.AddVertex(pos1.x + n1.x * yOff, pos1.y + n1.y * yOff, aDesc.zOffset, Mathf.Lerp(body.x, body.xMax, slicePercent), Mathf.Lerp(body.y, body.yMax, 0.5f)) : -1;
		if (aConnectFace) {
			if (!splitMiddle) {
				mesh.AddFace(v2, p2, p1, v1);
			} else {
				mesh.AddFace(v2, p2, p3, v3);
				mesh.AddFace(v3, p3, p1, v1);
			}
		}

		p1 = v1;
		p2 = v2;
		p3 = v3;
	}
	private void LegacyAddCap           (List<Vector2> aSegment, Ferr2DT_SegmentDescription aDesc, bool aInner, float aDir, float aScale, bool aSmooth) {
		IFerr2DTMaterial   mat   = TerrainMaterial;
		Ferr2D_DynamicMesh mesh  = DMesh;
		int                index = 0;
		Vector2            dir   = Vector2.zero;
		if (aDir < 0) {
			index = 0;
			dir   = aSegment[0] - aSegment[1];
		} else {
			index = aSegment.Count-1;
			dir   = aSegment[aSegment.Count-1] - aSegment[aSegment.Count-2];
		}
		dir.Normalize();
        Vector2 norm = aSmooth ? Ferr2D_Path.HermiteGetNormal(aSegment, index, 0, false): Ferr2D_Path.GetNormal(aSegment, index, false);
		Vector2 pos  = aSegment[index];
        float   yOff = fill == Ferr2DT_FillMode.InvertedClosed ? -aDesc.yOffset : aDesc.yOffset;
        Rect cap;
		if (aDir < 0) {
			if (fill == Ferr2DT_FillMode.InvertedClosed) cap = (!aInner && aDesc.innerRightCap.width > 0) ? mat.ToUV(aDesc.innerRightCap) : mat.ToUV(aDesc.rightCap);
			else                                         cap = ( aInner && aDesc.innerLeftCap .width > 0) ? mat.ToUV(aDesc.innerLeftCap ) : mat.ToUV(aDesc.leftCap );
		} else        {
			if (fill == Ferr2DT_FillMode.InvertedClosed) cap = (!aInner && aDesc.innerLeftCap .width > 0) ? mat.ToUV(aDesc.innerLeftCap ) : mat.ToUV(aDesc.leftCap );
			else                                         cap = ( aInner && aDesc.innerRightCap.width > 0) ? mat.ToUV(aDesc.innerRightCap) : mat.ToUV(aDesc.rightCap);
		}
        
		float width =  cap.width     * unitsPerUV.x;
		float scale = (cap.height/2) * unitsPerUV.y * aScale;

        float minU = fill == Ferr2DT_FillMode.InvertedClosed ? cap.xMax : cap.x;
        float maxU = fill == Ferr2DT_FillMode.InvertedClosed ? cap.x    : cap.xMax;
        float minV = fill == Ferr2DT_FillMode.InvertedClosed ? cap.yMax : cap.y;
        float maxV = fill == Ferr2DT_FillMode.InvertedClosed ? cap.y    : cap.yMax;

        if (aDir >= 0) {
            float t = minU;
            minU = maxU;
            maxU = t;
        }

        int v1  =           mesh.AddVertex(pos + dir * width + norm * (scale + yOff), -slantAmount + aDesc.zOffset, new Vector2(minU, minV));
        int v2  =           mesh.AddVertex(pos +               norm * (scale + yOff), -slantAmount + aDesc.zOffset, new Vector2(maxU, minV));

        int v15 = splitMiddle ? mesh.AddVertex(pos + dir * width +        (norm  * yOff),  aDesc.zOffset,           new Vector2(minU, cap.y + (cap.height / 2))) : -1;
        int v25 = splitMiddle ? mesh.AddVertex(pos               +        (norm  * yOff),  aDesc.zOffset,           new Vector2(maxU, cap.y + (cap.height / 2))) : -1;

        int v3  =           mesh.AddVertex(pos -               norm * (scale - yOff),  slantAmount + aDesc.zOffset, new Vector2(maxU, maxV));
        int v4  =           mesh.AddVertex(pos + dir * width - norm * (scale - yOff),  slantAmount + aDesc.zOffset, new Vector2(minU, maxV));

        if (splitMiddle && aDir < 0) {
            mesh.AddFace(v1,  v2,  v25, v15);
            mesh.AddFace(v15, v25, v3,  v4 );
        } else if (splitMiddle && aDir >= 0) {
            mesh.AddFace(v2,  v1,  v15, v25);
            mesh.AddFace(v25, v15, v4,  v3 );
        } else if (aDir < 0){
            mesh.AddFace(v1, v2, v3, v4);
        } else {
            mesh.AddFace(v2, v1, v4, v3);
        }
	}
	#endregion

	#region Fill
	private void LegacyAddFill(bool aSkirt, bool aFullBuild) {
		IFerr2DTMaterial mat       = TerrainMaterial;
		float            texWidth  = mat.edgeMaterial ? 256 : mat.edgeMaterial.mainTexture.width;
		float            fillDist  = (mat.ToUV( mat.GetBody((Ferr2DT_TerrainDirection)0,0) ).width * (texWidth  / pixelsPerUnit)) / (Mathf.Max(1, splitCount)) * splitDist;
		List<Vector2>    fillVerts = GetSegmentsCombined(fillDist);
        Vector2          scale     = Vector2.one;

        // scale is different for the fill texture
		if (mat.fillMaterial != null && mat.fillMaterial.mainTexture != null) {
            scale = new Vector2(
	        mat.fillMaterial.mainTexture.width  / pixelsPerUnit,
	        mat.fillMaterial.mainTexture.height / pixelsPerUnit);
        }

        if (aSkirt) {
            Vector2 start = fillVerts[0];
            Vector2 end   = fillVerts[fillVerts.Count - 1];

            fillVerts.Add(new Vector2(end.x, fillY));
            fillVerts.Add(new Vector2(Mathf.Lerp(end.x, start.x, 0.33f), fillY));
            fillVerts.Add(new Vector2(Mathf.Lerp(end.x, start.x, 0.66f), fillY));
            fillVerts.Add(new Vector2(start.x, fillY));
        }

        int       offset  = DMesh.VertCount;
		List<int> indices = Ferr2D_Triangulator.GetIndices(ref fillVerts, true, fill == Ferr2DT_FillMode.InvertedClosed, invertFillBorder, fillSplit && aFullBuild ? fillSplitDistance : 0);
        for (int i = 0; i < fillVerts.Count; i++) {
            DMesh.AddVertex(fillVerts[i].x, fillVerts[i].y, fillZ, (fillVerts[i].x + uvOffset.x + transform.position.x) / scale.x, (fillVerts[i].y + uvOffset.y + transform.position.y ) / scale.y);
        }
        for (int i = 0; i < indices.Count; i+=3) {
            try {
                DMesh.AddFace(indices[i    ] + offset,
                              indices[i + 1] + offset,
                              indices[i + 2] + offset);
            } catch {

            }
        }
	}
	#endregion

	#region Colliders
	public  void                LegacyRecreateCollider() {
		if (!createCollider) return;
		
		if (create3DCollider) {
			LegacyRecreateCollider3D();
		} else {
			LegacyRecreateCollider2D();
		}
	}
	private void                LegacyRecreateCollider3D()
    {
		Ferr2D_DynamicMesh colMesh = new Ferr2D_DynamicMesh();
        List<List<Vector2>> verts = GetColliderVerts();

        // create the solid mesh for it
        for (int t = 0; t < verts.Count; t++) {
            for (int i = 0; i < verts[t].Count; i++) {
                if (Path.closed && i == verts.Count - 1) colMesh.AddVertex(verts[t][0]);
                else colMesh.AddVertex(verts[t][i]);
            }
        }
        colMesh.ExtrudeZ(depth, fill == Ferr2DT_FillMode.InvertedClosed);

        // remove any faces the user may not want
        if (!collidersTop   ) colMesh.RemoveFaces(new Vector3( 0, 1,0), 45);
        if (!collidersLeft  ) colMesh.RemoveFaces(new Vector3(-1, 0,0), 45);
        if (!collidersRight ) colMesh.RemoveFaces(new Vector3( 1, 0,0), 45);
        if (!collidersBottom) colMesh.RemoveFaces(new Vector3( 0,-1,0), 45);

        // make sure there's a MeshCollider component on this object
		MeshCollider collider = GetComponent<MeshCollider>();
        if (collider == null) {
            collider = gameObject.AddComponent<MeshCollider>();
        }
        if (physicsMaterial != null) collider.sharedMaterial = physicsMaterial;
		#if !UNITY_5_3_OR_NEWER
        collider.smoothSphereCollisions = smoothSphereCollisions; 
		#endif
        collider.isTrigger              = isTrigger;

        // compile the mesh!
        Mesh   m    = collider.sharedMesh;
	    string name = string.Format("Ferr2DT_PathCollider_{0}", gameObject.GetInstanceID());
        if (m == null || m.name != name) {
            collider.sharedMesh = m = new Mesh();
            m.name = name;
        }
        collider.sharedMesh = null;
        colMesh.Build(ref m, createTangents);
        collider.sharedMesh = m;
    }
	private void                LegacyRecreateCollider2D() {
		List<Collider2D>    colliders = new List<Collider2D>(1);
		List<List<Vector2>> segs      = GetColliderVerts();
		bool                closed    = collidersBottom && collidersLeft && collidersRight && collidersTop;
		
		if (useEdgeCollider) {
			EdgeCollider2D[]     edges    = GetComponents<EdgeCollider2D>();
			List<EdgeCollider2D> edgePool = new List<EdgeCollider2D>(edges);
			int                  extra    = edges.Length - segs.Count;
			
			if (extra > 0) {
				// we have too many, remove a few
				for (int i=0; i<extra; i+=1) {
					Destroy(edgePool[0]);
					edgePool.RemoveAt(0);
				}
			} else {
				// we have too few, add in a few
				for (int i=0; i<Mathf.Abs(extra); i+=1) {
					edgePool.Add(gameObject.AddComponent<EdgeCollider2D>());
				}
			}
			
			for (int i = 0; i < segs.Count; i++) {
				EdgeCollider2D edge = edgePool[i];
				edge.points = segs[i].ToArray();
				colliders.Add(edge);
			}
		} else {
			// make sure there's a collider component on this object
			PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
			if (poly == null) {
				poly = gameObject.AddComponent<PolygonCollider2D>();
			}
			colliders.Add(poly);
			
			poly.pathCount = segs.Count;
			if (segs.Count > 1 || !closed) {
				for (int i = 0; i < segs.Count; i++) {
					poly.SetPath (i, LegacyExpandColliderPath(segs[i], colliderThickness).ToArray());
				}
			} else {
				if (fill == Ferr2DT_FillMode.InvertedClosed) {
					Rect bounds = Ferr2D_Path.GetBounds(segs[0]);
					poly.pathCount = 2;
					poly.SetPath (0, segs[0].ToArray());
					if (invertFillBorder != Vector2.zero) {
						poly.SetPath(1, new Vector2[]{
							new Vector2(bounds.xMin-invertFillBorder.x, bounds.yMax-invertFillBorder.y),
							new Vector2(bounds.xMax+invertFillBorder.x, bounds.yMax-invertFillBorder.y),
							new Vector2(bounds.xMax+invertFillBorder.x, bounds.yMin+invertFillBorder.y),
							new Vector2(bounds.xMin-invertFillBorder.x, bounds.yMin+invertFillBorder.y)
						});
					} else {
						poly.SetPath(1, new Vector2[]{
							new Vector2(bounds.xMin-bounds.width, bounds.yMax+bounds.height),
							new Vector2(bounds.xMax+bounds.width, bounds.yMax+bounds.height),
							new Vector2(bounds.xMax+bounds.width, bounds.yMin-bounds.height),
							new Vector2(bounds.xMin-bounds.width, bounds.yMin-bounds.height)
						});
					}
				} else {
	                if (segs.Count > 0 && segs[0].Count > 0) {
	                    poly.SetPath(0, segs[0].ToArray());
	                }
				}
			}
		}
		
		
		for (int i=0; i<colliders.Count; i+=1) {

			colliders[i].isTrigger      = isTrigger;
			colliders[i].sharedMaterial = physicsMaterial2D;
			#if UNITY_5_5_OR_NEWER
			colliders[i].usedByEffector = usedByEffector;
			#endif
		}
	}
	private List<Vector2>       LegacyExpandColliderPath(List<Vector2> aList, float aAmount) {
		int count = aList.Count;
		for (int i = count - 1; i >= 0; i--) {
			Vector2 norm = Ferr2D_Path.GetNormal(aList, i, false);
			aList.Add (aList [i] + new Vector2 (norm.x * aAmount, norm.y * aAmount));
		}
		return aList;
	}
    /// <summary>
    /// Retrieves a list of line segments that directly represent the collision volume of the terrain. This includes offsets and removed edges.
    /// </summary>
    /// <returns>A list of line segments.</returns>
	public List<List<Vector2>>  LegacyGetColliderVerts  () {
		if (TerrainMaterial == null)
			return new List<List<Vector2>>();

        List<Vector2> tVerts = Path.GetVertsRaw();

        // drop a skirt on skirt-based terrain
        if ((fill == Ferr2DT_FillMode.Skirt || fill == Ferr2DT_FillMode.FillOnlySkirt) && tVerts.Count > 0) {
            Vector2 start = tVerts[0];
            Vector2 end   = tVerts[tVerts.Count - 1];
            tVerts.Add(new Vector2(end.x, fillY));
            tVerts.Add(new Vector2(start.x, fillY));
            tVerts.Add(new Vector2(start.x, start.y));
        }

		float                           fillDist = (TerrainMaterial.ToUV(TerrainMaterial.GetBody((Ferr2DT_TerrainDirection)0, 0)).width * (TerrainMaterial.edgeMaterial.mainTexture.width / pixelsPerUnit)) / (Mathf.Max(1, splitCount)) * splitDist;
        List<Ferr2DT_TerrainDirection>  dirs     = new List<Ferr2DT_TerrainDirection>();
        List<List<Vector2>>             result   = new List<List<Vector2>           >();
        List<List<int>>                 list     = GetSegments(tVerts, out dirs);
        List<Vector2>                   curr     = new List<Vector2                 >();

        // remove segments that aren't on the terrain
        for (int i = 0; i < list.Count; i++) {
            if ( (dirs[i] == Ferr2DT_TerrainDirection.Bottom && !collidersBottom) ||
                 (dirs[i] == Ferr2DT_TerrainDirection.Left   && !collidersLeft  ) ||
                 (dirs[i] == Ferr2DT_TerrainDirection.Top    && !collidersTop   ) ||
                 (dirs[i] == Ferr2DT_TerrainDirection.Right  && !collidersRight )) {
                if (curr.Count > 0) { 
                    result.Add  (new List<Vector2>(curr));
                    curr  .Clear(                       );
                }
            } else {
                // create a list of verts and scales for this edge
                List<float  > tScales = null;
                List<Vector2> tList   = null;
                Ferr2D_Path.IndicesToPath(tVerts, vertScales, list[i], out tList, out tScales);

                // smooth it!
                if (smoothPath && tList.Count > 2) {
                    Ferr2D_Path.SmoothSegment(tList, tScales, fillDist, false, out tList, out tScales);
                }

                // offset the verts based on scale and terrain edge info
                tList = LegacyOffsetColliderVerts(tList, tScales, dirs[i]);

                // sharpen corners properly!
                if (curr.Count > 0 && sharpCorners) {
                    LegacyMergeCorner(ref curr, ref tList);
                }

                curr.AddRange(tList);
            }
        }
        if (sharpCorners) {
            LegacyMergeCorner(ref curr, ref curr);
        }
        if (curr.Count > 0) result.Add(curr);

        return result;
	}

	private List<Vector2>       LegacyOffsetColliderVerts(List<Vector2> aSegment, List<float> aSegmentScales, Ferr2DT_TerrainDirection aDir) {
        List<Vector2> result = new List<Vector2>(aSegment);
        int           count  = aSegment.Count;
        
        for (int v = count - 1; v >= 0; v--) {
            Vector2 norm  = smoothPath ? Ferr2D_Path.HermiteGetNormal(aSegment, v, 0, false) : Ferr2D_Path.GetNormal(aSegment, v, false);
			Vector2 segNormal = Ferr2D_Path.GetSegmentNormal(v, aSegment, false);
			float   scale = v >= aSegmentScales.Count ? 1 : aSegmentScales[v];
			float   rootScale = smoothPath ? 1 : 1f/Mathf.Abs(Mathf.Cos(Vector2.Angle(-segNormal, norm)*Mathf.Deg2Rad));
			scale = scale * rootScale;

			if (fill == Ferr2DT_FillMode.None) {
                result.Add(aSegment[v] +  new Vector2(norm.x *  surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ], norm.y *  surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ]) * scale);
                result[v]              += new Vector2(norm.x * -surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom], norm.y * -surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom]) * scale;
            } else {
                float   dist   = surfaceOffset[(int)aDir];
                Vector2 offset = new Vector2(dist, dist);
                result[v]     += new Vector2(norm.x * -offset.x, norm.y * -offset.y) * scale;
            }
        }
        return result;
    }
    private void                LegacyMergeCorner(ref List<Vector2> aPrevList, ref List<Vector2> aNextList) {
        if (aNextList.Count < 2 || aPrevList.Count < 2) return;

        float maxD = sharpCornerDistance;

        Vector2 pt = Ferr2D_Path.LineIntersectionPoint(aPrevList[aPrevList.Count - 2], aPrevList[aPrevList.Count - 1], aNextList[0], aNextList[1]);
        float   d1 = (pt - aPrevList[aPrevList.Count - 1]).sqrMagnitude;
        float   d2 = (pt - aNextList[0]).sqrMagnitude;

        if (d1 <= maxD * maxD) {
            aPrevList[aPrevList.Count - 1] = pt;
        } else {
            Vector2 tP = (pt - aPrevList[aPrevList.Count - 1]);
            tP.Normalize();
            aPrevList[aPrevList.Count - 1] += tP * maxD;
        }
        if (d2 <= maxD * maxD) {
            aNextList[0] = pt;
        } else {
            Vector2 tP = (pt - aNextList[0]);
            tP.Normalize();
            aNextList[0] += tP * maxD;
        }
    }
	#endregion

	#region Public Legacy Methods
	/// <summary>
	/// [Legacy] This method ensures that path overrides are properly present. Adds them if there aren't enough, and removes them if there are too many.
	/// </summary>
	public void                         MatchOverrides  () {
        if (directionOverrides == null) directionOverrides = new List<Ferr2DT_TerrainDirection>();
	    if (vertScales         == null) vertScales         = new List<float>();
		if (cutOverrides       == null) cutOverrides       = new List<CutOverrides>();
		Ferr2D_Path path = Path;

        for (int i = directionOverrides.Count; i < path.pathVerts.Count; i++) {
            directionOverrides.Add(Ferr2DT_TerrainDirection.None);
        }
		for (int i = cutOverrides.Count; i < path.pathVerts.Count; i++) {
			cutOverrides      .Add(new CutOverrides());
		}
		for (int i = vertScales.Count; i < path.pathVerts.Count; i++) {
            vertScales        .Add(1);
        }
        if (directionOverrides.Count > path.pathVerts.Count && path.pathVerts.Count > 0) {
            int diff = directionOverrides.Count - path.pathVerts.Count;
            directionOverrides.RemoveRange(directionOverrides.Count - diff - 1, diff);
            vertScales        .RemoveRange(vertScales        .Count - diff - 1, diff);
			cutOverrides      .RemoveRange(cutOverrides      .Count - diff - 1, diff);
		}
    }
	/// <summary>
    /// [Legacy] This will allow you to set the terrain material regardless of whether it's marked as the current material already or not. Also calls RecreatePath when finished.
    /// </summary>
    /// <param name="aMaterial">The terrain material! Usually from a terrain material prefab.</param>
    /// <param name="aForceUpdate">Force it to set the material, even if it's already the set material, or no?</param>
    /// <param name="aRecreate">Should we recreate the mesh? Usually, this is what you want (only happens if the material changes, or is forced to change)</param>
    public  void                        ForceMaterial   (IFerr2DTMaterial aMaterial, bool aForceUpdate, bool aRecreate = true)
    {
	    if (terrainMaterialInterface != (UnityEngine.Object)aMaterial || aForceUpdate)
        {
		    terrainMaterialInterface = (UnityEngine.Object)aMaterial;

            // copy the materials into the renderer
            Material[] newMaterials = null;
            if (fill == Ferr2DT_FillMode.Closed || fill == Ferr2DT_FillMode.InvertedClosed || fill == Ferr2DT_FillMode.Skirt) {
                newMaterials = new Material[] {
                    aMaterial.fillMaterial,
                    aMaterial.edgeMaterial
                };
            } else if (fill == Ferr2DT_FillMode.None) {
                newMaterials = new Material[] {
                    aMaterial.edgeMaterial
                };
            } else if (fill == Ferr2DT_FillMode.FillOnlyClosed || fill == Ferr2DT_FillMode.FillOnlySkirt) {
                newMaterials = new Material[] {
                    aMaterial.fillMaterial
                };
            }
            GetComponent<Renderer>().sharedMaterials = newMaterials;
		    
            // make sure we update the units per UV
		    Material edgeMat = TerrainMaterial.edgeMaterial;
		    if (edgeMat != null && edgeMat.mainTexture != null) {
			    unitsPerUV.x = edgeMat.mainTexture.width  / pixelsPerUnit;
			    unitsPerUV.y = edgeMat.mainTexture.height / pixelsPerUnit;
            }

            if (aRecreate) {
                Build(true);
            }
        }
    }

	public  Ferr2DT_SegmentDescription	GetDescription	    (List<Vector2> aSegment  ) {
        Ferr2DT_TerrainDirection dir = Ferr2D_Path.GetDirection(aSegment, 0, fill == Ferr2DT_FillMode.InvertedClosed);
	    return TerrainMaterial.GetDescriptor(dir);
	}
    public  Ferr2DT_SegmentDescription  GetDescription      (List<int>     aSegment  ) {
        Ferr2DT_TerrainDirection dir = Ferr2D_Path.GetDirection(Path.pathVerts, aSegment, 0, fill == Ferr2DT_FillMode.InvertedClosed);
	    return TerrainMaterial.GetDescriptor(dir);
    }
    public  List<List<int>>             GetSegments         (List<Vector2> aPath, out List<Ferr2DT_TerrainDirection> aSegDirections)
    {
        List<List<int>> segments = new List<List<int>>();
        if (splitCorners) {
            segments = Ferr2D_Path.GetSegments(aPath, out aSegDirections, directionOverrides,
                fill == Ferr2DT_FillMode.InvertedClosed,
                GetComponent<Ferr2D_Path>().closed);
        } else {
            aSegDirections = new List<Ferr2DT_TerrainDirection>();
            aSegDirections.Add(Ferr2DT_TerrainDirection.Top);
            List<int> seg = new List<int>();
            for (int i = 0; i < aPath.Count; i++) {
                seg.Add(i);
            }
            segments.Add(seg);
        }
        if (Path.closed ) {
            Ferr2D_Path.CloseEnds(aPath, ref segments, ref aSegDirections, splitCorners, fill == Ferr2DT_FillMode.InvertedClosed);
			Ferr2DT_TerrainDirection dir = directionOverrides[segments[segments.Count-1][0]];
			if (dir != Ferr2DT_TerrainDirection.None)
				aSegDirections[aSegDirections.Count-1] = dir;
        }
        return segments;
    }
	private List<Vector2>               GetSegmentsCombined (float         aSplitDist) {
		Ferr2D_Path                    path   = Path;
		List<Ferr2DT_TerrainDirection> dirs   = new List<Ferr2DT_TerrainDirection>();
		List<Vector2                 > result = new List<Vector2>();
		List<List<int>               > list   = GetSegments(path.GetVertsRaw(), out dirs);

		for (int i = 0; i < list.Count; i++) {
			if (smoothPath && list[i].Count > 2) {
				result.AddRange(Ferr2D_Path.SmoothSegment( Ferr2D_Path.IndicesToList<Vector2>(path.pathVerts, list[i]), aSplitDist, false));
			} else {
                result.AddRange(Ferr2D_Path.IndicesToList<Vector2>(path.pathVerts, list[i]));
			}
		}
		return result;
	}

	#endregion
}