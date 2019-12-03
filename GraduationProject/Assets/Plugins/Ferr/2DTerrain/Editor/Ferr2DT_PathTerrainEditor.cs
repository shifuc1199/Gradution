using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Ferr;

[CustomEditor(typeof(Ferr2DT_PathTerrain)), CanEditMultipleObjects]
public partial class Ferr2DT_PathTerrainEditor : Editor {
	#region Fields
	const float cSizeLargeHandle = 0.15f;
	const float cSizeSmallHandle = 0.1f;

    bool showVisuals     = true;
    bool showTerrainType = true;
	bool showCollider    = true;
	bool showVerts       = false;
	bool usedLowQualityBuild = false;
	bool mouseDown       = false;
	bool materialChanged = false;
	int  activeSegment;
	int  activePoint;
	bool multiEdit = false;

	float currVertScale;
	float sizeSmallHandle;
	float sizeLargeHandle;

    public static List<List<Vector2>> cachedColliders = null;
	Ferr2DT_PathTerrain terrain;
	PathEditorVisuals visuals;
	#endregion

	#region Serialized Properties
	SerializedProperty edgeMode;
	SerializedProperty fillMode;
	SerializedProperty useSkirt;
	SerializedProperty fillY;
	SerializedProperty fillZ;
	SerializedProperty invertFillBorder;
	SerializedProperty splitCorners;
	SerializedProperty perfectCorners;
	SerializedProperty pixelsPerUnit;
	SerializedProperty vertexColorType;
	SerializedProperty vertexColor;
	SerializedProperty vertexGradient;
	SerializedProperty vertexGradientAngle;
	SerializedProperty vertexGradientDistance;
	SerializedProperty createTangents;
	SerializedProperty uvOffset;
	SerializedProperty slantAmount;
	SerializedProperty fillSplit;
	SerializedProperty fillSplitDistance;
	SerializedProperty fillCollider;
	
	SerializedProperty colliderMode;
	SerializedProperty usedByEffector;
	SerializedProperty isTrigger;
	SerializedProperty depth;
	SerializedProperty physicsMaterial;
	SerializedProperty physicsMaterial2D;
	
	SerializedProperty pathData;
	SerializedProperty pathDataClosed;
	
	private void LoadProperties() {
		if (terrain.IsLegacy)
			LegacyLoadProperties();

		edgeMode                 = serializedObject.FindProperty("edgeMode");
		fillMode                 = serializedObject.FindProperty("fillMode");
		useSkirt                 = serializedObject.FindProperty("useSkirt");
		fillY                    = serializedObject.FindProperty("fillY");
		fillZ                    = serializedObject.FindProperty("fillZ");
		invertFillBorder         = serializedObject.FindProperty("invertFillBorder");
		splitCorners             = serializedObject.FindProperty("splitCorners");
		
		pixelsPerUnit            = serializedObject.FindProperty("pixelsPerUnit");
		vertexColorType          = serializedObject.FindProperty("vertexColorType");
		vertexColor              = serializedObject.FindProperty("vertexColor");
		vertexGradient           = serializedObject.FindProperty("vertexGradient");
		vertexGradientAngle      = serializedObject.FindProperty("vertexGradientAngle");
		vertexGradientDistance   = serializedObject.FindProperty("vertexGradientDistance");
		createTangents           = serializedObject.FindProperty("createTangents");
		uvOffset                 = serializedObject.FindProperty("uvOffset");
		slantAmount              = serializedObject.FindProperty("slantAmount");
		fillSplit                = serializedObject.FindProperty("fillSplit");
		fillSplitDistance        = serializedObject.FindProperty("fillSplitDistance");
		
		colliderMode             = serializedObject.FindProperty("colliderMode");
		fillCollider             = serializedObject.FindProperty("fillCollider");
		usedByEffector           = serializedObject.FindProperty("usedByEffector");
		isTrigger                = serializedObject.FindProperty("isTrigger");
		depth                    = serializedObject.FindProperty("depth");
		physicsMaterial          = serializedObject.FindProperty("physicsMaterial");
		physicsMaterial2D        = serializedObject.FindProperty("physicsMaterial2D");
		
		pathData       = serializedObject.FindProperty("pathData");
		pathDataClosed = pathData.FindPropertyRelative("_closed");

		multiEdit = serializedObject.isEditingMultipleObjects;
	}
	#endregion

	#region Unity Events
	void OnEnable  () {
        terrain = (Ferr2DT_PathTerrain)target;
		if (terrain == null)
			return;

        if (terrain.GetComponent<MeshFilter>().sharedMesh == null)
        	terrain.Build(true);
	    cachedColliders = null;
	    LoadProperties();
        Ferr2D_PathEditor.OnChanged = () => { cachedColliders = null; };
		Ferr2DT_Caps.LoadImages();
		UpdateVisuals();

		Undo.undoRedoPerformed += OnUndo;
    }
    void OnDisable () {
        Ferr2D_PathEditor.OnChanged = null;
		Undo.undoRedoPerformed -= OnUndo;
		
		Tools.hidden = false;
    }
	void OnUndo() {
		terrain = (Ferr2DT_PathTerrain)target;
		terrain.CheckedLegacy = false;
		terrain.PathData.SetDirty();
        terrain.Build(true);
        cachedColliders = null;
		SceneView.RepaintAll();
	}
	#endregion

	#region Scene GUI
	private void OnSceneGUI() {
		if (!Ferr2DT_PathTerrain.showGUI || multiEdit)
			return;

		terrain = (Ferr2DT_PathTerrain)target;
		if (terrain.IsLegacy) {
			LegacySceneGUI();
			return;
		}

		// make sure our visual settings are up to date
		if (currVertScale != Ferr2DT_Menu.PathScale) {
			UpdateVisuals();
		}

		// hide Unity's current tool if we're overriding it with DragSelect
		Tools.hidden = Ferr2DT_Menu.DragSelect;

		if (Event.current.type == EventType.MouseDown) mouseDown = true; 
		if (Event.current.type == EventType.MouseUp  ) mouseDown = false; 
		
	    EditorUtility.SetSelectedRenderState(terrain.gameObject.GetComponent<Renderer>(), Ferr2DT_Menu.HideMeshes ? EditorSelectedRenderState.Hidden : EditorSelectedRenderState.Highlight);
		
		// create a serialized object, the one provided by Editor isn't valid in OnSceneGUI
		SerializedObject targetObj = new SerializedObject(target); 
		targetObj.Update();
		SerializedProperty path = targetObj.FindProperty("pathData");
		
		// show the collider lines
		ShowColliders();

		// and do the actual path GUI
		ShowPathGUI(terrain.transform, terrain.PathData, path);

		// Do this last so it goes above all our handles and lines
		Ferr2DT_SceneOverlay.OnGUI();

		// build the terrain if we need to
		if (targetObj.ApplyModifiedProperties() || (usedLowQualityBuild && !mouseDown)) {
			terrain.PathData.SetDirty();
			if (mouseDown) {
				if (!Ferr2DT_Menu.BuildOnRelease)
					terrain.Build(false);
				usedLowQualityBuild = true;
			} else {
				terrain.Build(true);
				usedLowQualityBuild = false;
			}
			cachedColliders = null;
		}
    }
	private void ShowColliders() {
		if (terrain.ColliderMode == Ferr2D_ColliderMode.None || !Ferr2DT_Menu.ShowCollider)
			return;

		if (cachedColliders == null) 
			cachedColliders = terrain.GetColliderVerts();

		if (Event.current.type == EventType.Repaint && cachedColliders != null) {
			Handles.matrix = terrain.transform.localToWorldMatrix;
			for (int i = 0; i < cachedColliders.Count; i++) {
				Handles.color = Color.green;
				EditorTools.DrawPolyLine(cachedColliders[i], true);
			}
			Handles.matrix = Matrix4x4.identity;
		}
	}
	private void ShowPathGUI(Transform aTransform, Ferr2DPath aPath, SerializedProperty aPathProp) {
		EventType eType = Event.current.type;
		Matrix4x4 mat   = aTransform.localToWorldMatrix;

		// show the path and all its standard controls
		PathEditorUtil.OnSceneGUI(aTransform.localToWorldMatrix, aTransform.worldToLocalMatrix, aPathProp, aPath, true, ProcessNewPoint, ProcessRemovePoint, Path2D.Plane.XY, Ferr2DT_Menu.PathSnapMode, Ferr2DT_Menu.SmartSnap?Ferr2DT_Menu.SmartSnapDist:0, KeyCode.C, visuals);
		PathEditorUtil.Selection selection = PathEditorUtil.GetSelection(aPath);

		// find which segments and points are active
		FindActiveControls(aPath, mat, aTransform.worldToLocalMatrix);

		if (Ferr2DT_SceneOverlay.segmentLockMode) {
			ShowTexSegmentSelect(aPath, aPathProp, mat);
		} else {
			float currDistance = 0;
			for (int i = 0; i < aPath.Count; i++) {
				// show all the data at the control points
				ShowPointControls(aPath, i, selection, mat, aPathProp);
			
				// show midpoint controls
				currDistance = ShowMidpointControls(aPath, i, selection, mat, aPathProp, currDistance);
			}
		}

		// drag select has to happen last, otherwise it steals control ahead of the other handles
		ShowDragSelect(aPath, mat);
	}

	private void  FindActiveControls  (Ferr2DPath aPath, Matrix4x4 aTransform, Matrix4x4 aInvTransform) {
		if (Event.current.type == EventType.MouseMove) {
			Ray   r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Plane p = new Plane(aTransform.MultiplyVector(Vector3.forward), aTransform.MultiplyPoint(Vector3.zero));
			float dist = 0;
			
			if (p.Raycast(r, out dist)) {
				Vector2 mousePoint = aInvTransform.MultiplyPoint(r.GetPoint(dist));
				int newSegment = aPath.GetClosestSegment(mousePoint);
				if (newSegment != activeSegment) {
					activeSegment = newSegment;
					SceneView.RepaintAll();
				}
				int newPoint = aPath.GetClosestControlPoint(mousePoint);
				if (newPoint != activePoint) {
					activePoint = newPoint;
					SceneView.RepaintAll();
				}
			}
		}
	}
	private void  ShowPointControls   (Ferr2DPath aPath, int i, PathEditorUtil.Selection aSelection, Matrix4x4 aTransform, SerializedProperty aPathProp) {
		EventType eType = Event.current.type;

		Ferr2D_PointData pointData = aPath.GetData(i);
		Vector2 point            = aPath[i];
		Vector2 pointNormal      = aPath.GetNormal(i);
		Vector3 pointWorld       = aTransform.MultiplyPoint(point);
		Vector3 pointWorldNormal = aTransform.MultiplyVector(pointNormal);
		float   pointSize        = eType==EventType.Layout||eType==EventType.Repaint ? HandleUtility.GetHandleSize(pointWorld) : 0;

		// point scale button
		if (terrain.EdgeMode != Ferr2D_SectionMode.None && (i == activePoint || aSelection.IsSelected(i) || pointData.scale != 1)) {
			Vector3 scaleStart = pointWorld+pointWorldNormal*(visuals.sizeVertex+sizeSmallHandle)*pointSize;
			Vector3 scalePos   = scaleStart + ((pointData.scale-0.5f) * 3f)*pointWorldNormal;
			if (Event.current.alt) {
				// reset scale
				if (pointData.scale != 1 && Handles.Button(scalePos, Quaternion.identity, pointSize*sizeSmallHandle, pointSize*sizeSmallHandle, Ferr2DT_Caps.CapDotReset)) {
					aSelection.Each(i, id => {
						SerializedProperty scaleProp = aPathProp.FindPropertyRelative("_data").GetArrayElementAtIndex(id).FindPropertyRelative("scale");
						scaleProp.floatValue = 1;
					});
				}
			} else {
				Vector2 screenNormal = HandleUtility.WorldToGUIPoint(pointWorld + pointWorldNormal) - HandleUtility.WorldToGUIPoint(pointWorld);
				screenNormal.Normalize();

				Vector3 move = Handles.FreeMoveHandle(scalePos, Quaternion.identity, pointSize*sizeSmallHandle, Vector3.zero, Ferr2DT_Caps.GetScaleCap(screenNormal));
				if (move != scalePos) {
					move = PathUtil.GetClosetPointOnLine(scaleStart, scaleStart+pointWorldNormal*3, move, true);
					float finalScale = 0.5f+(scaleStart - move).magnitude/3f;
					float delta      = finalScale - pointData.scale;

					aSelection.Each(i, id => {
						SerializedProperty scaleProp = aPathProp.FindPropertyRelative("_data").GetArrayElementAtIndex(id).FindPropertyRelative("scale");
						scaleProp.floatValue += delta;
					});
				}
			}
		}

		// show index numbers
		if (Ferr2DT_SceneOverlay.showIndices) {
			Vector2 pt = HandleUtility.WorldToGUIPoint(pointWorld - pointWorldNormal * pointSize * sizeLargeHandle*2);
			Rect r = new Rect(pt.x-15, pt.y-EditorGUIUtility.singleLineHeight*0.5f, 30, EditorGUIUtility.singleLineHeight);
			Handles.BeginGUI();
			GUI.Label(r, i.ToString(), PathEditorUtil.CenteredShadowStyle);
			GUI.Label(r, i.ToString(), PathEditorUtil.CenteredLabelStyle);
			Handles.EndGUI();
		}
	}
	private float ShowMidpointControls(Ferr2DPath aPath, int i, PathEditorUtil.Selection aSelection, Matrix4x4 aTransform, SerializedProperty aPathProp, float aCurrDistance) {
		EventType eType = Event.current.type;
		
		// calculate distance data for finding the midpoint 
		float segmentDistance = aPath.GetDistanceBetween(i,PathUtil.WrapIndex(i+1, aPath.Count, aPath.Closed));
		float nextDistance    = aCurrDistance +  segmentDistance;
		float midDist         = aCurrDistance + (segmentDistance)/2;

		// if we have no segment on the control vert, skip this midpoint
		if (aPath.Count <= 1 || (i==aPath.Count-1 && !aPath.Closed)) return nextDistance;
		
		// find the midpoint of this line
		Vector2 midPoint  = aPath.GetPointAtDistance (midDist);
		Vector2 midNormal = aPath.GetNormalAtDistance(midDist);
		Vector3 midWorldPoint  = aTransform.MultiplyPoint(midPoint);
		Vector3 midWorldNormal = aTransform.MultiplyVector(midNormal);
		float   midSize        = eType==EventType.Layout||eType==EventType.Repaint ? HandleUtility.GetHandleSize(midWorldPoint) : 0;
			
		// edge override button
		int direction = aPath.GetData(i).directionOverride;
		if (terrain.EdgeMode != Ferr2D_SectionMode.None && (i == activeSegment || aSelection.IsSegmentSelected(i, aPath.Count, aPath.Closed) || direction != (int)Ferr2DT_TerrainDirection.None)) {
			if (Event.current.alt) {
				if (direction != (int)Ferr2DT_TerrainDirection.None && Handles.Button(midWorldPoint + midWorldNormal * midSize*sizeLargeHandle*2, Quaternion.identity, midSize*sizeSmallHandle, midSize*sizeSmallHandle, Ferr2DT_Caps.CapDotReset)) {
					aSelection.EachSegment(i, aPath.Count, aPath.Closed, id=>{
						SerializedProperty overrideProp = aPathProp.FindPropertyRelative("_data").GetArrayElementAtIndex(id).FindPropertyRelative("directionOverride");
						overrideProp.intValue = (int)Ferr2DT_TerrainDirection.None;
					});
				}
			} else {
				if (Handles.Button(midWorldPoint + midWorldNormal * midSize*sizeLargeHandle*2, Quaternion.identity, midSize*sizeSmallHandle, midSize*sizeSmallHandle, Ferr2DT_Caps.GetEdgeCap(direction))) {
					CycleEdgeOverride(aPathProp, i);
					SerializedProperty cycledOverride = aPathProp.FindPropertyRelative("_data").GetArrayElementAtIndex(i).FindPropertyRelative("directionOverride");
					aSelection.EachSegment(i, aPath.Count, aPath.Closed, id=>{
						SerializedProperty overrideProp = aPathProp.FindPropertyRelative("_data").GetArrayElementAtIndex(id).FindPropertyRelative("directionOverride");
						overrideProp.intValue = cycledOverride.intValue;
					});
				}
			}
		}

		// new point button
		if (!Event.current.alt && i == activeSegment && Handles.Button(midWorldPoint, Quaternion.identity, midSize*sizeLargeHandle, midSize*sizeLargeHandle, Ferr2DT_Caps.CapDotPlus)) {
			PathEditorUtil.AddPoint(aPathProp, midPoint, i+1);
			ProcessNewPoint(aPathProp, i+1);
		}

		return nextDistance;
	}
	private void  ShowDragSelect      (Ferr2DPath aPath, Matrix4x4 aTransform) {
		if (!Ferr2DT_Menu.DragSelect)
			return;

		// manually make a move widget, since we have to override Unity's widget
		Vector3 center = Vector3.zero;
		if (Tools.pivotMode == PivotMode.Center)
			center = PathUtil.Average(aPath.GetPathRaw());
		Vector3 newPos = Handles.DoPositionHandle(terrain.transform.position+center, Quaternion.identity)-center;
		if (newPos != terrain.transform.position) {
			Undo.RecordObject(terrain.transform, "Move Terrain Object"); 
			terrain.transform.position = newPos;
		}
		// execute the drag-select code
		PathEditorUtil.DoDragSelect(aTransform, aPath, new Rect(0, EditorGUIUtility.singleLineHeight, Screen.width,Screen.height - EditorGUIUtility.singleLineHeight), visuals);
	}
	private void  ShowTexSegmentSelect(Ferr2DPath aPath,  SerializedProperty aPathProp, Matrix4x4 aTransform) {
		// calculate segment information for displaying the handles correctly
		List<Ferr2DT_PathTerrain.EdgeSegment> segments = Ferr2DT_PathTerrain.EdgeSegment.CreateEdgeSegments(aPath, terrain.splitCorners);
		bool    invert = terrain.EdgeMode == Ferr2D_SectionMode.Invert;
		Vector2 upUV   = terrain.UnitsPerUV;

		for (int i = 0; i < segments.Count; i++) {
			// get our segment data, and see if we have any data to override with
			var edgeSegment = segments[i];
			edgeSegment.direction = invert ? Ferr2DT_PathTerrain.Invert( edgeSegment.direction ) : edgeSegment.direction;
			var edgeData    = terrain.TerrainMaterial.GetDescriptor( edgeSegment.direction );
			if (edgeData.BodyCount < 2)
				continue;
			
			bool  rightInner = edgeSegment.path.GetInteriorAngle(edgeSegment.start) > 180;
			float rightOff   = edgeData.GetRightCapOffset(invert? !rightInner:rightInner, upUV);
			
			// calculate all the texture segments present
			List<int> texSources  = new List<int>();
			float     scale       = 0;
			List<int> texSegments = terrain.CreateLineList(edgeSegment, out scale, texSources);

			// now go and add handles for all the texture segments
			for (int t = 0; t < texSegments.Count; t++) {
				// find handle location
				float   segDist = TexDistAtSegment(edgeSegment, edgeData, texSegments, t)*scale;
				Vector3 pt      = aTransform.MultiplyPoint(aPath.GetPointAtDistance(edgeSegment.startDistance-rightOff + segDist));
				float   size    = HandleUtility.GetHandleSize(pt);

				// figure out what our data says about this segment, if anything
				int cutPointId = texSources[t*2];
				int cutId      = texSources[t*2+1];
				int cutValue   = 0;
				var cutOverrides = aPath.GetData(cutPointId).cutOverrides;
				if (cutOverrides != null && cutOverrides.Count>cutId)
					cutValue = cutOverrides[cutId];
				
				// and execute the handles
				if (Event.current.alt) {
					if (cutValue != 0 && Handles.Button(pt, Quaternion.identity, size * sizeSmallHandle, size * sizeSmallHandle, Ferr2DT_Caps.CapDotReset)) {
						SerializedProperty cutProp = aPathProp.FindPropertyRelative("_data").GetArrayElementAtIndex(cutPointId).FindPropertyRelative("cutOverrides");
						SerializedProperty segCutProperty = cutProp.GetArrayElementAtIndex(cutId);
						segCutProperty.intValue = 0;
					}
				} else {
					if (Handles.Button(pt, Quaternion.identity, size * sizeSmallHandle, size * sizeSmallHandle, Ferr2DT_Caps.GetNumberCap(cutValue))) {
						SerializedProperty cutProp = aPathProp.FindPropertyRelative("_data").GetArrayElementAtIndex(cutPointId).FindPropertyRelative("cutOverrides");
						while (cutProp.arraySize <= cutId) {
							cutProp.arraySize += 1;
							cutProp.GetArrayElementAtIndex(cutProp.arraySize-1).intValue = 0;
						}
						while (cutProp.arraySize > texSegments.Count)
							cutProp.arraySize--;
						SerializedProperty segCutProperty = cutProp.GetArrayElementAtIndex(cutId);
						segCutProperty.intValue = (segCutProperty.intValue + 1) % (edgeData.BodyCount+1);
					}
				}
			}
		}
	}
	private float TexDistAtSegment(Ferr2DT_PathTerrain.EdgeSegment aSegment, Ferr2DT_SegmentDescription edgeData, List<int> aTexSegments, int aSegmentId) {
		float currLength = 0;
		for (int i = 0; i < aTexSegments.Count && i <= aSegmentId; i++) {
			float segLength = terrain.TerrainMaterial.ToUV( edgeData.GetBody(aTexSegments[i]) ).width * terrain.UnitsPerUV.x;

			if (i == aSegmentId)
				return currLength + segLength/2;
			currLength += terrain.TerrainMaterial.ToUV( edgeData.GetBody(aTexSegments[i]) ).width * terrain.UnitsPerUV.x;
		}
		return -1;
	}

	private void CycleEdgeOverride(SerializedProperty aPathProp, int i) {
		SerializedProperty overrideProp = aPathProp.FindPropertyRelative("_data").GetArrayElementAtIndex(i).FindPropertyRelative("directionOverride");
		if (overrideProp.intValue == (int)Ferr2DT_TerrainDirection.None)
			overrideProp.intValue = (int)Ferr2DT_TerrainDirection.Top;
		else if (overrideProp.intValue == (int)Ferr2DT_TerrainDirection.Top)
			overrideProp.intValue = (int)Ferr2DT_TerrainDirection.Right;
		else if (overrideProp.intValue == (int)Ferr2DT_TerrainDirection.Right)
			overrideProp.intValue = (int)Ferr2DT_TerrainDirection.Bottom;
		else if (overrideProp.intValue == (int)Ferr2DT_TerrainDirection.Bottom)
			overrideProp.intValue = (int)Ferr2DT_TerrainDirection.Left;
		else if (overrideProp.intValue == (int)Ferr2DT_TerrainDirection.Left)
			overrideProp.intValue = 4;
		else
			overrideProp.intValue += 1;
		if (overrideProp.intValue >= terrain.TerrainMaterial.descriptorCount)
			overrideProp.intValue = (int)Ferr2DT_TerrainDirection.None;
	}
	#endregion

	#region Inspector GUI
	public override void OnInspectorGUI() {
		terrain = (Ferr2DT_PathTerrain)target;
		if (terrain.IsLegacy) {
			LegacyInspectorGUI();
			return;
		}

		ShowMaterialSelector  ();
		ShowTerrainProperties ();
		ShowVisualProperties  ();
		ShowColliderProperties();
		ShowVerts             ();

		if(serializedObject.ApplyModifiedProperties() || materialChanged) {
			materialChanged = false;
			cachedColliders = null;
			for (int i = 0; i < targets.Length; i++) {
				EditorUtility.SetDirty(targets[i]);
				((Ferr2DT_PathTerrain)targets[i]).PathData.SetDirty();
				((Ferr2DT_PathTerrain)targets[i]).Build(true);
			}
		}
	}
	private void ShowMaterialSelector  () {
        EditorGUILayout.LabelField("TERRAIN MATERIAL");

        Ferr.EditorTools.Box(4, ()=>{
            EditorGUILayout.BeginHorizontal();
            IFerr2DTMaterial material = terrain.TerrainMaterial;
            GUIContent button = material != null && material.edgeMaterial != null && material.edgeMaterial.mainTexture != null ? new GUIContent(material.edgeMaterial.mainTexture) : new GUIContent("Pick");
            if (GUILayout.Button(button, GUILayout.Width(64f), GUILayout.Height(64f))) {
                Ferr2DT_MaterialSelector.Show((mat) => {
                    if (mat != terrain.TerrainMaterial) {
                        SelectMaterial((UnityEngine.Object)mat);
                    }
					materialChanged = true;
                });
            }

            UnityEngine.Object obj = EditorGUILayout.ObjectField((UnityEngine.Object)terrain.TerrainMaterial, typeof(Ferr2DT_Material), false, GUILayout.Height(64f));
            if (obj != (UnityEngine.Object)terrain.TerrainMaterial) {
                SelectMaterial(obj);
				materialChanged = true;
            }
            EditorGUILayout.EndHorizontal();
		});
	}
	private void ShowTerrainProperties () {
		showTerrainType = EditorGUILayout.Foldout(showTerrainType, "TERRAIN DATA");
		if (!showTerrainType)
			return;
        
	    EditorGUI.indentLevel = 1;
	    Ferr.EditorTools.Box(4, ()=>{
			EditorGUILayout.PropertyField(pathData);

			// center the object position for the terrain
			if (targets.Length == 1 && GUILayout.Button("Center Position")) {
				Vector2 center = PathUtil.Average(terrain.PathData.GetPathRaw());
				var points = pathData.FindPropertyRelative("_points");
				for (int i = 0; i < points.arraySize; i++) {
					points.GetArrayElementAtIndex(i).vector2Value -= center;
				}
				Undo.RecordObject(terrain.transform, "Center Position");
				terrain.transform.position += (Vector3)center;
			}
			EditorGUILayout.Space();

		    EditorGUILayout.PropertyField(fillMode);
			EditorGUI.indentLevel = 2;
			if (fillMode.enumValueIndex != (int)Ferr2D_SectionMode.None  ) {
				if (useSkirt.boolValue != true && pathDataClosed.boolValue != true) pathDataClosed.boolValue = true;
				EditorGUILayout.PropertyField(fillZ, new GUIContent("Z Offset"));
				EditorGUILayout.PropertyField(uvOffset, new GUIContent("UV Offset"));
				if (fillMode.enumValueIndex == (int)Ferr2D_SectionMode.Invert) EditorGUILayout.PropertyField(invertFillBorder);
				EditorGUILayout.PropertyField(fillSplit, new GUIContent("Interior Grid Verts"));
				if (fillSplit.boolValue) {
					EditorGUI.indentLevel = 3;
					EditorGUILayout.PropertyField(fillSplitDistance, new GUIContent("Grid Spacing"));
					EditorGUI.indentLevel = 2;
				}
			}
			EditorGUI.indentLevel = 1;

			EditorGUILayout.PropertyField(edgeMode);
			if (edgeMode.enumValueIndex != (int)Ferr2D_SectionMode.None) {
				EditorGUI.indentLevel = 2;
				EditorGUILayout.PropertyField(slantAmount);
				splitCorners.boolValue = !EditorGUILayout.Toggle(new GUIContent("Use Only Top Edge"), !splitCorners.boolValue);
				EditorGUI.indentLevel = 1;
			}

			EditorGUILayout.PropertyField(useSkirt);
			if (useSkirt.boolValue) {
				if (pathDataClosed.boolValue)
					pathDataClosed.boolValue = false;
				EditorGUI.indentLevel = 2;
				EditorGUILayout.PropertyField(fillY, new GUIContent("Skirt Y Value"));
				EditorGUI.indentLevel = 1;
			}
	    });
        EditorGUI.indentLevel = 0;
	}
	private void ShowVisualProperties  () {
		showVisuals = EditorGUILayout.Foldout(showVisuals, "VISUALS");
		if (!showVisuals)
			return;

	    EditorGUI.indentLevel = 1;
	    Ferr.EditorTools.Box(4, ()=>{
	        // other visual data
		    EditorGUILayout.PropertyField(vertexColorType);
		    EditorGUI.indentLevel = 2;
		    if (!vertexColorType.hasMultipleDifferentValues && vertexColorType.enumValueIndex == (int)Ferr2DT_ColorType.SolidColor) {
			    EditorGUILayout.PropertyField(vertexColor        );
		    } else if (!vertexColorType.hasMultipleDifferentValues && vertexColorType.enumValueIndex == (int)Ferr2DT_ColorType.Gradient) {
			    EditorGUILayout.PropertyField(vertexGradientAngle);
			    EditorGUILayout.PropertyField(vertexGradient     );
		    } else if (!vertexColorType.hasMultipleDifferentValues && vertexColorType.enumValueIndex == (int)Ferr2DT_ColorType.DistanceGradient) {
			    EditorGUILayout.PropertyField(vertexGradientDistance);
			    EditorGUILayout.PropertyField(vertexGradient        );
		    }
		    EditorGUI.indentLevel = 1;
		        
		    EditorGUILayout.PropertyField(pixelsPerUnit );
		    EditorGUILayout.PropertyField(createTangents);
		        
		    if (!serializedObject.isEditingMultipleObjects) {
			    Renderer renderCom     = terrain.GetComponent<Renderer>();
			    string[] sortingLayers = Ferr.LayerUtil.GetSortingLayerNames();
			    if (sortingLayers != null) {
				    string currName = renderCom.sortingLayerName == "" ? "Default" : renderCom.sortingLayerName;
				    int    nameID   = EditorGUILayout.Popup("Sorting Layer", Array.IndexOf(sortingLayers, currName), sortingLayers);
				        
				    renderCom.sortingLayerName = sortingLayers[nameID];
			    } else {
					renderCom.sortingLayerID = EditorGUILayout.IntField("Sorting Layer", renderCom.sortingLayerID);
			    }
			    renderCom.sortingOrder = EditorGUILayout.IntField  ("Order in Layer", renderCom.sortingOrder);
			        
                // warn if the shader's aren't likely to work with the settings provided!
			    if (renderCom.sortingOrder != 0 || (renderCom.sortingLayerName != "Default" && renderCom.sortingLayerName != "")) {
				    bool opaque = false;
				    for (int i = 0; i < renderCom.sharedMaterials.Length; ++i) {
					    Material mat = renderCom.sharedMaterials[i];
					    if (mat != null && mat.GetTag("RenderType", false, "") == "Opaque") {
						    opaque = true;
					    }
				    }
				    if (opaque) {
					    EditorGUILayout.HelpBox("Layer properties may not work properly if one of your shaders is not transparent!", MessageType.Info);
				    }
			    }
		    }
	    });
		EditorGUI.indentLevel = 0;
	}
	private void ShowColliderProperties() {
		showCollider = EditorGUILayout.Foldout(showCollider, "COLLIDER");
		if (!showCollider)
			return;
		
	    EditorGUI.indentLevel = 1;
	    Ferr.EditorTools.Box(4, ()=>{
		    EditorGUILayout.PropertyField(colliderMode);
		    if (colliderMode.enumValueIndex != (int)Ferr2D_ColliderMode.None) {
			    EditorGUI.indentLevel = 2;
			    if (colliderMode.enumValueIndex == (int)Ferr2D_ColliderMode.Mesh3D) {
				    EditorGUILayout.PropertyField(depth, new GUIContent("Collider Depth"));
					if (edgeMode.enumValueIndex == (int)Ferr2D_SectionMode.None)
						EditorGUILayout.PropertyField(physicsMaterial, new GUIContent("Physics Material"));
			    } else {
				    EditorGUILayout.PropertyField(usedByEffector);
					EditorGUILayout.PropertyField(fillCollider);
					if (edgeMode.enumValueIndex == (int)Ferr2D_SectionMode.None || fillCollider.boolValue)
						EditorGUILayout.PropertyField(physicsMaterial2D, new GUIContent("Physics Material"));
			    }
				EditorGUI.indentLevel = 1;
				EditorGUILayout.PropertyField(isTrigger);
			    
				if (GUILayout.Button("Prebuild collider")) {
					for (int i = 0; i < targets.Length; i++) {
						Ferr2DT_PathTerrain t = targets[i] as Ferr2DT_PathTerrain;
						if (t != null) t.RecreateCollider();
					}
				}
			}
	    });
		EditorGUI.indentLevel = 0;
	}
	private void ShowVerts() {
		if (multiEdit) return;

		showVerts = EditorGUILayout.Foldout(showVerts, "CONTROL POINTS");
		if (!showVerts)
			return;

	    EditorGUI.indentLevel = 1;
	    Ferr.EditorTools.Box(4, ()=>{
			SerializedProperty points = pathData.FindPropertyRelative("_points");
			SerializedProperty pointControls = pathData.FindPropertyRelative("_pointControls");

			for (int i = 0; i < points.arraySize; i++) {
				EditorGUILayout.PropertyField(points.GetArrayElementAtIndex(i), new GUIContent("#"+i.ToString()));
				EditorGUILayout.PropertyField(pointControls.GetArrayElementAtIndex(i));
			}
	    });
		EditorGUI.indentLevel = 0;
	}
	#endregion

	#region Misc Methods
	void UpdateVisuals() {
		sizeLargeHandle = Ferr2DT_Menu.PathScale * cSizeLargeHandle;
		sizeSmallHandle = Ferr2DT_Menu.PathScale * cSizeSmallHandle;
		visuals = new PathEditorVisuals();
		visuals.colorLine         = Color.white;
		visuals.capVertex         = Ferr2DT_Caps.CapDot;
		visuals.capVertexMode     = Ferr2DT_Caps.CapDot;
		visuals.capVertexDelete   = Ferr2DT_Caps.CapDotMinusSelected;
		visuals.capVertexAdd      = Ferr2DT_Caps.CapDotPlus;
		visuals.capControlHandle  = Ferr2DT_Caps.CapControl;
		visuals.colorHandle       = Color.white;
		visuals.colorHandleDelete = Color.white;
		visuals.colorSelectionTint = new Color(.5f,1,.5f,1);
		visuals.sizeVertexDelete  = sizeLargeHandle;
		visuals.sizeVertex        = sizeLargeHandle;
		visuals.sizeVertexAdd     = sizeLargeHandle;
		visuals.sizeVertexMode    = sizeLargeHandle * 1.15f;
		visuals.sizeControlHandle = sizeSmallHandle * .5f;
		visuals.capVertexTypes = new Handles.CapFunction[] {
			Ferr2DT_Caps.CapDotBezier,
			Ferr2DT_Caps.CapDotBezier,
			Ferr2DT_Caps.CapDotAutoBezier,
			Ferr2DT_Caps.CapDotBezier,
			Ferr2DT_Caps.CapDot,
			Ferr2DT_Caps.CapDotArc,
		};

		currVertScale = Ferr2DT_Menu.PathScale;
	}
	void ProcessNewPoint(SerializedProperty aPath, int aIndex) {
		SerializedProperty dataList = aPath.FindPropertyRelative("_data");
		dataList.InsertArrayElementAtIndex(aIndex);
		SerializedProperty data = dataList.GetArrayElementAtIndex(aIndex);
		data.FindPropertyRelative("scale").floatValue = 1;
		//data.FindPropertyRelative("directionOverride").intValue = (int)Ferr2DT_TerrainDirection.None;
	}
	void ProcessRemovePoint(SerializedProperty aPath, int aIndex) {
		SerializedProperty dataList = aPath.FindPropertyRelative("_data");
		dataList.DeleteArrayElementAtIndex(aIndex);
	}
    void SelectMaterial(UnityEngine.Object aMaterial) {
        for (int i = 0; i<targets.Length; i+=1) {
            Ferr2DT_PathTerrain curr = (Ferr2DT_PathTerrain)targets[i];
            Undo.RecordObject(curr, "Changed Terrain Material");
            curr.SetMaterial((IFerr2DTMaterial)aMaterial);
            EditorUtility.SetDirty(curr);
        }
    }
	#endregion
}