using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

#if UNITY_5_6_OR_NEWER
using CapFunction = UnityEditor.Handles.CapFunction;
#else
using CapFunction = UnityEditor.Handles.DrawCapFunction;
#endif

[CustomEditor(typeof(Ferr2D_Path))]
public class Ferr2D_PathEditor : Editor { 
	static Texture2D texMinus;
    static Texture2D texMinusSelected;
    static Texture2D texDot;
    static Texture2D texDotSnap;
    static Texture2D texDotPlus;
    static Texture2D texDotSelected;
    static Texture2D texDotSelectedSnap;

	static Texture2D texDot1;
	static Texture2D texDot2;
	static Texture2D texDot3;
	static Texture2D texDot4;
	static Texture2D texDot5;
	static Texture2D texDotN;

	static Texture2D texLeft;
    static Texture2D texRight;
    static Texture2D texTop;
    static Texture2D texBottom;
	static Texture2D texAuto;
	static Texture2D texReset;
	static Texture2D texScale;

#if UNITY_5_6_OR_NEWER
    private void CapDotMinus        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texMinus,           aType);}
    private void CapDotMinusSelected(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texMinusSelected,   aType);}
    private void CapDot             (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot,             aType);}
    private void CapDotSnap         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSnap,         aType);}
    private void CapDotPlus         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotPlus,         aType);}
    private void CapDotSelected     (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSelected,     aType);}
    private void CapDotSelectedSnap (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSelectedSnap, aType);}
    private void CapDotLeft         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texLeft,            aType);}
    private void CapDotRight        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texRight,           aType);}
    private void CapDotTop          (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texTop,             aType);}
    private void CapDotBottom       (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texBottom,          aType);}
    private void CapDotAuto         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texAuto,            aType);}
    private void CapDotScale        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texScale,           aType);}
    private void CapDotReset        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texReset,           aType);}
	private void CapDot1            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot1,            aType);}
	private void CapDot2            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot2,            aType);}
	private void CapDot3            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot3,            aType);}
	private void CapDot4            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot4,            aType);}
	private void CapDot5            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot5,            aType);}
	private void CapDotN            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotN,            aType);}
#else
    private void CapDotMinus        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texMinus,           Event.current.type);}
    private void CapDotMinusSelected(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texMinusSelected,   Event.current.type);}
    private void CapDot             (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot,             Event.current.type);}
    private void CapDotSnap         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSnap,         Event.current.type);}
    private void CapDotPlus         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotPlus,         Event.current.type);}
    private void CapDotSelected     (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSelected,     Event.current.type);}
    private void CapDotSelectedSnap (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSelectedSnap, Event.current.type);}
    private void CapDotLeft         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texLeft,            Event.current.type);}
    private void CapDotRight        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texRight,           Event.current.type);}
    private void CapDotTop          (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texTop,             Event.current.type);}
    private void CapDotBottom       (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texBottom,          Event.current.type);}
    private void CapDotAuto         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texAuto,            Event.current.type);}
    private void CapDotScale        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texScale,           Event.current.type);}
    private void CapDotReset        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texReset,           Event.current.type);}
	private void CapDot1            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot1,            Event.current.type);}
	private void CapDot2            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot2,            Event.current.type);}
	private void CapDot3            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot3,            Event.current.type);}
	private void CapDot4            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot4,            Event.current.type);}
	private void CapDot5            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot5,            Event.current.type);}
	private void CapDotN            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotN,            Event.current.type);}
#endif


	public static Action OnChanged = null;
	static int updateCount    = 0;
	bool       prevChanged    = false;
	List<int>  selectedPoints = new List<int>();
	bool       deleteSelected = false;
	Vector2    dragStart;
	bool       drag           = false;
	Vector3    snap           = Vector3.one;

	static int nudgeIndex = 0;

	SerializedProperty closed;
	SerializedProperty pathVerts;

	void LoadTextures() {
        if (texMinus != null) return;

        texMinus           = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-minus.png"         );
        texMinusSelected   = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-minus-selected.png");
        texDot             = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot.png"               );
        texDotSnap         = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-snap.png"          );
        texDotPlus         = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-plus.png"          );
        texDotSelected     = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-selected.png"      );
        texDotSelectedSnap = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-selected-snap.png" );

		texDot1 = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-1.png");
		texDot2 = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-2.png");
		texDot3 = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-3.png");
		texDot4 = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-4.png");
		texDot5 = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-5.png");
		texDotN = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-n.png");

		texLeft   = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-left.png" );
        texRight  = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-right.png");
        texTop    = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-top.png"  );
        texBottom = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-down.png" );
	    texAuto   = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-auto.png" );
	    texReset  = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-reset.png");
	    texScale  = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-scale.png");
    }
	private         void OnEnable      () {
		closed    = serializedObject.FindProperty("closed");
		pathVerts = serializedObject.FindProperty("pathVerts");
		
		selectedPoints.Clear();
        LoadTextures();
		
		if (nudgeIndex == 0)
			nudgeIndex = UnityEngine.Random.value > 0.5f ? 1 : 2;
	}
	private         void OnSceneGUI    () {
		if (!Ferr2DT_PathTerrain.showGUI)
			return;

		Ferr2D_Path         path      = (Ferr2D_Path)target;
		Ferr2DT_PathTerrain terrain   = path.GetComponent<Ferr2DT_PathTerrain>();
		GUIStyle            iconStyle = new GUIStyle();
		iconStyle.alignment    = TextAnchor.MiddleCenter;
		snap                   = new Vector3(EditorPrefs.GetFloat("MoveSnapX", 1), EditorPrefs.GetFloat("MoveSnapY", 1), EditorPrefs.GetFloat("MoveSnapZ", 1));
		
		// setup undoing things
		Undo.RecordObject(target, "Modified Path");

		if (Event.current.type == EventType.Repaint && terrain != null)
			Ferr2DT_PathTerrainEditor.DrawColliderEdge(terrain);

		// draw the path line
		if (Event.current.type == EventType.Repaint)
			DoPath(path);
		
		// Check for drag-selecting multiple points
		DragSelect(path);
		
        // do adding verts in when the shift key is down!
		if (Event.current.shift && !Event.current.control) {
			DoShiftAdd(path, iconStyle);
		}
		
        // draw and interact with all the path handles
		DoHandles(path, iconStyle);

		// if this was an undo, refresh stuff too
		if (Event.current.type == EventType.ValidateCommand) {
			switch (Event.current.commandName) {
				case "UndoRedoPerformed":
					// Only rebuild this from an undo if the inspector is not visible.
					UnityEngine.Object[] objs = Resources.FindObjectsOfTypeAll(Type.GetType("UnityEditor.InspectorWindow, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", true));
					if (objs == null || objs.Length == 0) {
						path.UpdateDependants(true);
						if (OnChanged != null) OnChanged();
					}
					break;
			}
		}

		// update everything that relies on this path, if the GUI changed
		if (GUI.changed) {
			if (PrefabUtility.GetPrefabParent(target) != null) {
				NudgeArray(pathVerts);
				serializedObject.ApplyModifiedProperties();
			}

			UpdateDependentsSmart(path, false, false);
			EditorUtility.SetDirty (target);
			prevChanged = true;
		} else if (Event.current.type == EventType.Used) {
			if (prevChanged == true) {
				UpdateDependentsSmart(path, false, true);
			}
			prevChanged = false;
		}
	}
	public override void OnInspectorGUI() {
		Ferr2D_Path path = (Ferr2D_Path)target;
		bool updateMesh = false;

		// if this was an undo, refresh stuff too
		if (Event.current.type == EventType.ValidateCommand) {
			switch (Event.current.commandName) {
			case "UndoRedoPerformed":
				
				path.UpdateDependants(true);
				if (OnChanged != null) OnChanged();
				return;
			}
		}

		EditorGUILayout.PropertyField(closed);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(pathVerts, true);
		if (EditorGUI.EndChangeCheck() && PrefabUtility.GetPrefabParent(target) != null) {
			NudgeArray(pathVerts);
		}

		// button for updating the origin of the object
		if (GUILayout.Button("Center Position")) {
			Undo.RecordObject(target, "Modified Path");
			path.ReCenter();
			updateMesh = true;
		}
		
		Ferr2DT_PathTerrain terrain = path.GetComponent<Ferr2DT_PathTerrain>();
		if (!path.closed && (terrain.fill == Ferr2DT_FillMode.Closed || terrain.fill == Ferr2DT_FillMode.InvertedClosed || terrain.fill == Ferr2DT_FillMode.FillOnlyClosed)) {
			Undo.RecordObject(target, "Modified Path");
			path.closed = true;
			updateMesh  = true;
		}
		if (path.closed && (terrain.fill == Ferr2DT_FillMode.FillOnlySkirt || terrain.fill == Ferr2DT_FillMode.Skirt)) {
			Undo.RecordObject(target, "Modified Path");
			path.closed = false;
			updateMesh  = true;
		}
		
        // update dependants when it changes
		if (updateMesh || serializedObject.ApplyModifiedProperties()) {
			if (OnChanged != null) OnChanged();
			path.UpdateDependants(true);
			EditorUtility.SetDirty(target);
		}
	}

	static private void NudgeArray(SerializedProperty aArray) {
		for (int i = 0; i < aArray.arraySize; i++) {
			SerializedProperty item = aArray.GetArrayElementAtIndex(i);
			if (!item.prefabOverride) {
				float nudge = nudgeIndex % 2 == 1 ? 0.00001f : -0.00001f;
				item.vector2Value = new Vector2(item.vector2Value.x + nudge, item.vector2Value.y + nudge);
			}
		}
		nudgeIndex += 1;
	}

	private void    UpdateDependentsSmart(Ferr2D_Path aPath, bool aForce, bool aFullUpdate) {
		if (aForce || Ferr2DT_Menu.UpdateTerrainSkipFrames == 0 || updateCount % Ferr2DT_Menu.UpdateTerrainSkipFrames == 0) {
			aPath.UpdateDependants(aFullUpdate);
			if (Application.isPlaying) aPath.UpdateColliders();
			if (OnChanged != null) OnChanged();
		}
		updateCount += 1;
	}
	
	private void    DragSelect           (Ferr2D_Path path) {
		
		if (Event.current.type == EventType.Repaint) {
			if (drag) {
				Vector3 pt1 = HandleUtility.GUIPointToWorldRay(dragStart).GetPoint(0.2f);
				Vector3 pt2 = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).GetPoint(0.2f);
				Vector3 pt3 = HandleUtility.GUIPointToWorldRay(new Vector2(dragStart.x, Event.current.mousePosition.y)).GetPoint(0.2f);
				Vector3 pt4 = HandleUtility.GUIPointToWorldRay(new Vector2(Event.current.mousePosition.x, dragStart.y)).GetPoint(0.2f);
				Handles.DrawSolidRectangleWithOutline(new Vector3[] { pt1, pt3, pt2, pt4 }, Ferr2D_Visual.DragBoxInnerColor, Ferr2D_Visual.DragBoxOuterColor);
			}
		}
		
		if (Event.current.shift && Event.current.control) {
			switch(Event.current.type) {
			case EventType.MouseDrag:
				SceneView.RepaintAll();
				break;
			case EventType.MouseMove:
				SceneView.RepaintAll();
				break;
			case EventType.MouseDown:
				if (Event.current.button != 0) break;
				
				dragStart = Event.current.mousePosition;
				drag      = true;
				
				break;
			case EventType.MouseUp:
				if (Event.current.button != 0) break;
				
				Vector2 dragEnd = Event.current.mousePosition;
				
				selectedPoints.Clear();
				for	(int i=0;i<path.pathVerts.Count;i+=1) {
					float left   = Mathf.Min(dragStart.x, dragStart.x + (dragEnd.x - dragStart.x));
					float right  = Mathf.Max(dragStart.x, dragStart.x + (dragEnd.x - dragStart.x));
					float top    = Mathf.Min(dragStart.y, dragStart.y + (dragEnd.y - dragStart.y));
					float bottom = Mathf.Max(dragStart.y, dragStart.y + (dragEnd.y - dragStart.y));
					
					Rect r = new Rect(left, top, right-left, bottom-top);
					if (r.Contains(HandleUtility.WorldToGUIPoint(path.transform.TransformPoint( path.pathVerts[i]) ) )) {
						selectedPoints.Add(i);
					}
				}
				
				HandleUtility.AddDefaultControl(0);
				drag = false;
				SceneView.RepaintAll();
				break;
			case EventType.Layout :
				HandleUtility.AddDefaultControl(GetHashCode());
				break;
			}
		} else if (drag == true) {
			drag = false;
			Repaint();
		}
	}
	private void    DoHandles            (Ferr2D_Path path, GUIStyle iconStyle)
	{
        Transform           transform    = path.transform;
        Matrix4x4           mat          = transform.localToWorldMatrix;
        Matrix4x4           invMat       = transform.worldToLocalMatrix;
        Transform           camTransform = SceneView.lastActiveSceneView.camera.transform;
		Ferr2DT_PathTerrain terrain      = path.GetComponent<Ferr2DT_PathTerrain>();
		
		terrain.MatchOverrides();
		
		Handles.color = Ferr2D_Visual.HandleColor;
		for (int i = 0; i < path.pathVerts.Count; i++)
		{
            // check if we want to remove points
			if (Event.current.alt) {
				DoResetModeHandles (path, terrain, i, mat, invMat, camTransform);
			} else {
				DoNormalModeHandles(path, terrain, i, mat, invMat, camTransform);
				
			}
		}
		if (Ferr2DT_SceneOverlay.segmentLockMode) {
			DoCutOverrideModeHandles(path, terrain, mat, camTransform);
		}


		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete && selectedPoints.Count > 0) {
			deleteSelected = true;
			GUI.changed = true;
			Event.current.Use();
		}
		
		if (deleteSelected) {
			DeleteSelected(path, terrain);
			deleteSelected = false;
		}
	}
	
	private void DeleteSelected(Ferr2D_Path path, Ferr2DT_PathTerrain terrain) {
		for (int i = 0; i < selectedPoints.Count; i++) {
			terrain.RemovePoint(selectedPoints[i]);
			
			for (int u = 0; u < selectedPoints.Count; u++) {
				if (selectedPoints[u] > selectedPoints[i]) selectedPoints[u] -= 1;
			}
		}
		selectedPoints.Clear();
	}
	
	private void DoResetModeHandles(Ferr2D_Path path, Ferr2DT_PathTerrain terrain, int i, Matrix4x4 mat, Matrix4x4 invMat, Transform camTransform) {
		int     nextId     = i==path.Count-1?(path.closed?i%path.Count:i-1):i+1;
		Vector3 pos        = mat.MultiplyPoint3x4(path.pathVerts[i]);
		Vector3 posNext    = mat.MultiplyPoint3x4(path.pathVerts[nextId]);
		Vector3 normal     = -(Vector3)Ferr2D_Path.GetNormal(path.pathVerts, i, path.closed);
		Vector3 posStart   = pos;
		bool    isSelected = false;
		if (selectedPoints!= null) isSelected = selectedPoints.Contains(i);
		
		float       handleScale = HandleScale(posStart) * Ferr2D_Visual.HandleSize;
		CapFunction cap         = (isSelected || selectedPoints.Count <= 0) ? (CapFunction)CapDotMinusSelected : (CapFunction)CapDotMinus;
		if (Handles.Button(posStart, camTransform.rotation, handleScale, handleScale, cap))
		{
			EnsureVertSelected(i, ref isSelected);
			deleteSelected = true;
			GUI.changed = true;
		} else if (!Ferr2DT_SceneOverlay.segmentLockMode) {
			handleScale = handleScale * Ferr2D_Visual.SmallHandleSize;
			
			// do scaling
			Vector3 displayPos = pos + normal * terrain.vertScales[i] * 2 * Ferr2DT_Menu.PathScale;
			if (IsVisible(displayPos) && Handles.Button(displayPos, camTransform.rotation, handleScale, handleScale, CapDotReset)) {
				EnsureVertSelected(i, ref isSelected);
				
				Undo.RecordObject(terrain, "Scale Path Vert");
				
				for (int s = 0; s < selectedPoints.Count; s++) {
					terrain.vertScales[selectedPoints[s]] = 1;
				}
				EditorUtility.SetDirty(terrain);
				GUI.changed = true;
			}
			
			// do edge overrides
			displayPos = GetOverridePos(i, path, mat, pos, posNext);
			if (IsVisible(displayPos) && Handles.Button(displayPos, camTransform.rotation, handleScale, handleScale, CapDotReset)) {
				EnsureVertSelected(i, ref isSelected);
				
				Undo.RecordObject(terrain, "Override Vert Direction");
				
				for (int s = 0; s < selectedPoints.Count; s++) {
					terrain.directionOverrides[selectedPoints[s]] = Ferr2DT_TerrainDirection.None;
				}
				EditorUtility.SetDirty(terrain);
				GUI.changed = true;
			}
		}
	}
	private void DoNormalModeHandles(Ferr2D_Path path, Ferr2DT_PathTerrain terrain, int i, Matrix4x4 mat, Matrix4x4 invMat, Transform camTransform) {
		int     nextId     = i==path.Count-1?(path.closed?0:i-1):i+1;
		Vector3 pos        = mat.MultiplyPoint3x4(path.pathVerts[i]);
		Vector3 posNext    = mat.MultiplyPoint3x4(path.pathVerts[nextId]);
		Vector3 normal     = -(Vector3)Ferr2D_Path.GetNormal(path.pathVerts, i, path.closed);
		bool    isSelected = false;
		if (selectedPoints != null) 
			isSelected = selectedPoints.Contains(i);
		
		// check for moving the point
		CapFunction cap = CapDot;
		if (Event.current.control) cap = isSelected ? (CapFunction)CapDotSelectedSnap : (CapFunction)CapDotSnap;
		else                       cap = isSelected ? (CapFunction)CapDotSelected     : (CapFunction)CapDot;
		
		Vector3 result = Handles.FreeMoveHandle(pos, camTransform.rotation, HandleScale(pos) * Ferr2D_Visual.HandleSize, snap, cap);
		
		if (result != pos) {
			EnsureVertSelected(i, ref isSelected);
			
			Vector2 relative = GetRelativeMovementWithSnap(result, invMat, i, path);
			
			for (int s = 0; s < selectedPoints.Count; s++) {
				path.pathVerts[selectedPoints[s]] += relative;
			}
		}
		
		if (Ferr2DT_SceneOverlay.showIndices) {
			Vector3 labelPos = pos + normal;
			Handles.color    = Ferr2D_Visual.IndicesColor;
			Handles.Label(labelPos, "" + i);
			Handles.color    = Ferr2D_Visual.HandleColor;
		}

		if (!Ferr2DT_SceneOverlay.segmentLockMode) {
			float   scale      = HandleScale (pos) * Ferr2D_Visual.SmallHandleSize;
			Vector3 displayPos = pos;
		
			if (path.closed || i+1 < path.pathVerts.Count) {
				displayPos = GetOverridePos(i, path, mat, pos, posNext);
			
				if (IsVisible(displayPos) && terrain.directionOverrides != null) {
					cap = Event.current.alt ? (CapFunction)CapDotReset : GetDirIcon(terrain.directionOverrides[i]);
					if (Handles.Button(displayPos, camTransform.rotation, scale, scale, cap)) {
						EnsureVertSelected(i, ref isSelected);
					
						Undo.RecordObject(terrain, "Override Vert Direction");
					
						Ferr2DT_TerrainDirection dir = NextDir(terrain.directionOverrides[i]);
						for (int s = 0; s < selectedPoints.Count; s++) {
							terrain.directionOverrides[selectedPoints[s]] = dir;
						}
						EditorUtility.SetDirty(terrain);
						GUI.changed = true;
					}
				}
			
			}
		
			displayPos = pos + normal * terrain.vertScales[i]*2*Ferr2DT_Menu.PathScale;
			if (IsVisible(displayPos)) {
				cap = Event.current.alt ? (CapFunction)CapDotReset : (CapFunction)CapDotScale;
			
				Vector3 scaleMove = Handles.FreeMoveHandle(displayPos, camTransform.rotation, scale, Vector3.zero, cap);
				float   scaleAmt  = Vector3.Distance(displayPos, scaleMove);
				if (Mathf.Abs(scaleAmt) > 0.01f ) {
					EnsureVertSelected(i, ref isSelected);
				
					Undo.RecordObject(terrain, "Scale Path Vert");
				
					float vertScale = Vector3.Distance(scaleMove, pos) / 2 / Ferr2DT_Menu.PathScale;
					vertScale = Mathf.Clamp(vertScale, 0.2f, 3f);
					for (int s = 0; s < selectedPoints.Count; s++) {
						terrain.vertScales[selectedPoints[s]] = vertScale;
					}
					EditorUtility.SetDirty(terrain);
					GUI.changed = true;
				}
			}
		
			// make sure we can add new point at the midpoints!
			if (i + 1 < path.pathVerts.Count || path.closed == true) {
				Vector3 mid         = (pos + posNext) / 2;
				float   handleScale = HandleScale(mid) * Ferr2D_Visual.HandleSize;
			
				if (Handles.Button(mid, camTransform.rotation, handleScale, handleScale, CapDotPlus)) {
					Vector2 pt = invMat.MultiplyPoint3x4(mid);
				
					terrain.AddPoint(pt, nextId);
					EditorUtility.SetDirty(terrain);
					GUI.changed = true;
				}
			}
		}
	}
	private void DoCutOverrideModeHandles(Ferr2D_Path path, Ferr2DT_PathTerrain terrain, Matrix4x4 mat, Transform camTransform) {
		List<List<int>               > segments = new List<List<int>               >();
        List<Ferr2DT_TerrainDirection> dirs     = new List<Ferr2DT_TerrainDirection>();
		List<Vector2>                  rawVerts = path.GetVertsRaw();

		// cut the terrain into segments, we need segment info to draw these points
        segments = terrain.GetSegments(rawVerts, out dirs);

		for (int s = 0; s < segments.Count; s++) {
			List<int>                              currSeg   = segments[s];
			List<Vector2>                          currVerts = Ferr2D_Path.IndicesToList(rawVerts, currSeg);
			List<Ferr2DT_PathTerrain.CutOverrides> overrides = Ferr2D_Path.IndicesToList(terrain.cutOverrides, currSeg);
			
			// find information about this segment
			Ferr2DT_TerrainDirection   currDir  = dirs[s];
			Ferr2DT_SegmentDescription desc     = default(Ferr2DT_SegmentDescription);

			if (currDir != Ferr2DT_TerrainDirection.None) {
				desc = terrain.TerrainMaterial.GetDescriptor(currDir);
			} else {
				desc = terrain.GetDescription(currSeg);
			}

			// if there's no body segment choices, don't bother with the rest of this
			if (desc.body.Length < 2)
				continue;
			
			Vector2 capLeftSlideDir = (currVerts[1] - currVerts[0]);
			Vector2 capRightSlideDir = (currVerts[currVerts.Count - 2] - currVerts[currVerts.Count - 1]);
			capLeftSlideDir.Normalize();
			capRightSlideDir.Normalize();
			currVerts[0] -= capLeftSlideDir  * desc.capOffset;
			currVerts[currVerts.Count - 1] -= capRightSlideDir * desc.capOffset;

			float distance = Ferr2D_Path.GetSegmentLength(currVerts);

			// how many texture cuts are there on the segment
			float bodyWidth   = desc.body[0].width * (terrain.TerrainMaterial.edgeMaterial.mainTexture.width  / terrain.pixelsPerUnit);
			int   textureCuts = Mathf.Max(1, Mathf.FloorToInt(distance / bodyWidth + 0.5f));

			// data is attached to the points still, check if we've switched to a new point
			int activePt = -1;
			int activeLocalCut = -1;
			for (int c = 0; c < textureCuts; c++) {
				float pctGlobal = c / (float)textureCuts;

				int   ptLocal  = 0;
				float pctLocal = 0;
				Ferr2D_Path.PathGlobalPercentToLocal(currVerts, pctGlobal, out ptLocal, out pctLocal, distance, false);

				if (ptLocal != activePt) {
					// if they size down, we need to shorten the data too
					if (activePt != -1) CapListSize<int>(ref overrides[activePt].data, activeLocalCut + 3);
					activePt = ptLocal;
					activeLocalCut = 0;

					if (overrides[activePt].data == null)
						overrides[activePt].data = new List<int>();
				}

				while (activeLocalCut >= overrides[activePt].data.Count)
					overrides[activePt].data.Add(0);

				CapFunction cap = CapDotAuto;
				int activeOverride = overrides[activePt].data[activeLocalCut];
				if (activeOverride != 0) {
					if      (activeOverride == 1) cap = CapDot1;
					else if (activeOverride == 2) cap = CapDot2;
					else if (activeOverride == 3) cap = CapDot3;
					else if (activeOverride == 4) cap = CapDot4;
					else if (activeOverride == 5) cap = CapDot5;
					else if (activeOverride >= 6) cap = CapDotN;
				}
				if (Event.current.alt) {
					cap = CapDotReset;
				}

				int   ptShow  = 0;
				float pctShow = 0;
				Ferr2D_Path.PathGlobalPercentToLocal(currVerts, pctGlobal + (1f/textureCuts)*0.5f, out ptShow, out pctShow, distance, false);

				Vector2 pt  = Ferr2D_Path.LinearGetPt(currVerts, ptShow, pctShow, false);
				Vector3 pos = mat.MultiplyPoint3x4(pt);
				float   sc  = HandleScale(pos) * Ferr2D_Visual.SmallHandleSize;
				if (Handles.Button(pos, camTransform.rotation, sc, sc, cap)) {
					Undo.RecordObject(terrain, "Lock Texture Segment");

					overrides[activePt].data[activeLocalCut] = Event.current.alt ? 0 : (activeOverride + 1) % (desc.body.Length+1);
					EditorUtility.SetDirty(terrain);
					GUI.changed = true;
				}

				activeLocalCut += 1;
			}
			if (activePt != -1) CapListSize<int>(ref overrides[activePt].data, activeLocalCut + 3);
		}
	}
	private void CapListSize<T>(ref List<T> aList, int aMax) {
		if (aList.Count > aMax) {
			int over = aList.Count - aMax;
			aList.RemoveRange(aList.Count - over, over);
		}
	}
	
	private void EnsureVertSelected(int aIndex, ref bool aIsSelected) {
		if (selectedPoints.Count < 2 || aIsSelected == false) {
			selectedPoints.Clear();
			selectedPoints.Add(aIndex);
			aIsSelected = true;
		}
	}
	private Vector3 GetRelativeMovementWithSnap(Vector3 aHandlePos, Matrix4x4 aInvMat, int i, Ferr2D_Path aPath) {
		if (!(Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapRelative))
			aHandlePos = GetRealPoint(aHandlePos, aPath.transform);
		
		Vector3 global = aHandlePos;
		if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapGlobal) global = SnapVector(global, snap);
		Vector3 local  = aInvMat.MultiplyPoint3x4(global);
		if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapLocal ) local  = SnapVector(local, snap);
		if (!Event.current.control && Ferr2DT_Menu.SmartSnap) {
			local = SmartSnap(local, aPath.pathVerts, selectedPoints, Ferr2DT_Menu.SmartSnapDist);
		}
		
		return new Vector2( local.x, local.y) - aPath.pathVerts[i];
	}
	private Vector3 GetOverridePos(int i, Ferr2D_Path aPath, Matrix4x4 aObjTransform, Vector3 currPos, Vector3 nextPos) {
		Vector3 mid    = (currPos + nextPos) / 2;
		Vector3 offset = Ferr2D_Path.GetSegmentNormal(i, aPath.pathVerts, aPath.closed) * 0.5f * Ferr2DT_Menu.PathScale;
		return mid + aObjTransform.MultiplyVector(offset);
	}
	
	private Vector3 GetTickerOffset      (Ferr2D_Path path, Vector3  aRootPos, int aIndex) {
		float   scale  = HandleScale(aRootPos) * 0.5f;
		Vector3 result = Vector3.zero;
		
		int     index  = (aIndex + 1) % path.pathVerts.Count;
		Vector3 delta  = Vector3.Normalize(path.pathVerts[index] - path.pathVerts[aIndex]);
		Vector3 norm   = new Vector3(-delta.y, delta.x, 0);
		result = delta * scale * 3 + new Vector3(norm.x, norm.y, 0) * scale * 2;

		return result;
	}
	private void    DoShiftAdd           (Ferr2D_Path path, GUIStyle iconStyle)
	{
        Vector3             snap      = Event.current.control ? new Vector3(EditorPrefs.GetFloat("MoveSnapX"), EditorPrefs.GetFloat("MoveSnapY"), EditorPrefs.GetFloat("MoveSnapZ")) : Vector3.zero;
		Ferr2DT_PathTerrain terrain   = path.gameObject.GetComponent<Ferr2DT_PathTerrain>();
        Transform           transform = path.transform;
        Transform           camTransform = SceneView.lastActiveSceneView.camera.transform;
		Vector3             pos       = transform.InverseTransformPoint( GetMousePos(Event.current.mousePosition, transform) );
		bool                hasDummy  = path.pathVerts.Count <= 0;
		
		if (hasDummy) path.pathVerts.Add(Vector2.zero);
		
		int   closestID  = path.GetClosestSeg(pos);
		int   secondID   = closestID + 1 >= path.Count ? 0 : closestID + 1;
		
		float firstDist  = Vector2.Distance(pos, path.pathVerts[closestID]);
		float secondDist = Vector2.Distance(pos, path.pathVerts[secondID]);
		
		Vector3 local  = pos;
		if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapLocal ) local  = SnapVector(local,  snap);
		Vector3 global = transform.TransformPoint(pos);
		if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapGlobal) global = SnapVector(global, snap);
		Vector3 handlePos = transform.TransformPoint(pos);
		float scale = HandleScale(handlePos);

		Handles.color = Ferr2D_Visual.PathColor;
		if (!(secondID == 0 && !path.closed && firstDist > secondDist))
		{
			Handles.DrawDottedLine(transform.TransformPoint(path.pathVerts[closestID]), global, 4);
		}
		if (!(secondID == 0 && !path.closed && firstDist < secondDist))
		{
			Handles.DrawDottedLine(transform.TransformPoint(path.pathVerts[secondID]), global, 4);
		}
		Handles.color = Ferr2D_Visual.HandleColor;

		scale = scale * Ferr2D_Visual.HandleSize;
		if (Handles.Button(handlePos, camTransform.rotation, scale, scale, CapDotPlus))
		{
			Vector3 finalPos = transform.InverseTransformPoint(global);
			if (secondID == 0) {
				if (firstDist < secondDist) {
					terrain.AddPoint(finalPos);
				} else {
					terrain.AddPoint(finalPos, 0);
				}
			} else {
				terrain.AddPoint(finalPos, Mathf.Max(closestID, secondID));
			}
			selectedPoints.Clear();
			EditorUtility.SetDirty(terrain);
			GUI.changed = true;
		}
		
		if (hasDummy) path.pathVerts.RemoveAt(0);
	}
	private void    DoPath               (Ferr2D_Path path)
	{
		Handles.color = Ferr2D_Visual.PathColor;
		List<Vector2> verts     = path.GetVertsRaw();
        Matrix4x4     mat       = path.transform.localToWorldMatrix;

		Handles.matrix = mat;
		for (int i = 0; i < verts.Count - 1; i++)
		{
			Handles.DrawLine(verts[i], verts[i+1]);
		}
		if (path.closed)
		{
			Handles.DrawLine(verts[0], verts[verts.Count - 1]);
		}
		Handles.matrix = Matrix4x4.identity;
	}
	
	private CapFunction  GetDirIcon(Ferr2DT_TerrainDirection aDir) {
		if      (aDir == Ferr2DT_TerrainDirection.Top   ) return CapDotTop;
		else if (aDir == Ferr2DT_TerrainDirection.Right ) return CapDotRight;
		else if (aDir == Ferr2DT_TerrainDirection.Left  ) return CapDotLeft;
		else if (aDir == Ferr2DT_TerrainDirection.Bottom) return CapDotBottom;
		return CapDotAuto;
	}
	private Ferr2DT_TerrainDirection NextDir   (Ferr2DT_TerrainDirection aDir) {
		if      (aDir == Ferr2DT_TerrainDirection.Top   ) return Ferr2DT_TerrainDirection.Right;
		else if (aDir == Ferr2DT_TerrainDirection.Right ) return Ferr2DT_TerrainDirection.Bottom;
		else if (aDir == Ferr2DT_TerrainDirection.Left  ) return Ferr2DT_TerrainDirection.Top;
		else if (aDir == Ferr2DT_TerrainDirection.Bottom) return Ferr2DT_TerrainDirection.None;
		return Ferr2DT_TerrainDirection.Left;
	}
	
	public static Vector3 GetMousePos  (Vector2 aMousePos, Transform aTransform) {
		Ray   ray   = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(aMousePos.x, aMousePos.y, 0));
		Plane plane = new Plane(aTransform.TransformDirection(new Vector3(0,0,-1)), aTransform.position);
		float dist  = 0;
		Vector3 result = new Vector3(0,0,0);
		
		ray = HandleUtility.GUIPointToWorldRay(aMousePos);
		if (plane.Raycast(ray, out dist)) {
			result = ray.GetPoint(dist);
		}
		return result;
	}
	public static float   GetCameraDist(Vector3 aPt) {
		return Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, aPt);
	}
	public static bool    IsVisible    (Vector3 aPos) {
		Transform t = SceneView.lastActiveSceneView.camera.transform;
		if (Vector3.Dot(t.forward, aPos - t.position) > 0)
			return true;
		return false;
	}
	public static float   HandleScale  (Vector3 aPos) {
		float dist = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.45f : GetCameraDist(aPos);
		return Mathf.Min(0.4f * Ferr2DT_Menu.PathScale, (dist / 5.0f) * 0.4f * Ferr2DT_Menu.PathScale);
	}
	
	private static Vector3 SnapVector  (Vector3 aVector, Vector3 aSnap) {
		return new Vector3(
			((int)(aVector.x / aSnap.x + (aVector.x > 0 ? 0.5f : -0.5f))) * aSnap.x,
			((int)(aVector.y / aSnap.y + (aVector.y > 0 ? 0.5f : -0.5f))) * aSnap.y,
			((int)(aVector.z / aSnap.z + (aVector.z > 0 ? 0.5f : -0.5f))) * aSnap.z);
	}
	private static Vector2 SnapVector  (Vector2 aVector, Vector2 aSnap) {
		return new Vector2(
			((int)(aVector.x / aSnap.x + (aVector.x > 0 ? 0.5f : -0.5f))) * aSnap.x,
			((int)(aVector.y / aSnap.y + (aVector.y > 0 ? 0.5f : -0.5f))) * aSnap.y);
	}
	private static Vector3 GetRealPoint(Vector3 aPoint, Transform aTransform) {
		Plane p = new Plane( aTransform.TransformDirection(new Vector3(0, 0, -1)), aTransform.position);
		Ray   r = new Ray  (SceneView.lastActiveSceneView.camera.transform.position, aPoint - SceneView.lastActiveSceneView.camera.transform.position);
		float d = 0;
		
		if (p.Raycast(r, out d)) {
			return r.GetPoint(d);;
		}
		return aPoint;
	}
	private Vector3 SmartSnap(Vector3 aPoint, List<Vector2> aPath, List<int> aIgnore, float aSnapDist) {
		float   minXDist = aSnapDist;
		float   minYDist = aSnapDist;
		Vector3 result   = aPoint;
		
		for (int i = 0; i < aPath.Count; ++i) {
			if (aIgnore.Contains(i)) continue;
			
			float xDist = Mathf.Abs(aPoint.x - aPath[i].x);
			float yDist = Mathf.Abs(aPoint.y - aPath[i].y);
			
			if (xDist < minXDist) {
				minXDist = xDist;
				result.x = aPath[i].x;
			}
			
			if (yDist < minYDist) {
				minYDist = yDist;
				result.y = aPath[i].y;
			}
		}
		return result;
	}
}