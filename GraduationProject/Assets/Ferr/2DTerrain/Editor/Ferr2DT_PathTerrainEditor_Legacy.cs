using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class Ferr2DT_PathTerrainEditor {
	SerializedProperty fill;
	SerializedProperty smoothPath;
	SerializedProperty splitCount;
	SerializedProperty splitDist;
	SerializedProperty splitMiddle;
	SerializedProperty randomByWorldCoordinates;
	
	SerializedProperty sharpCorners;
	SerializedProperty sharpCornerDistance;
	SerializedProperty smoothSphereCollisions;
	
	SerializedProperty createCollider;
	SerializedProperty create3DCollider;
	SerializedProperty useEdgeCollider;
	SerializedProperty surfaceOffset;
	SerializedProperty colliderThickness;
	SerializedProperty collidersLeft;
	SerializedProperty collidersRight;
	SerializedProperty collidersTop;
	SerializedProperty collidersBottom;

	private void LegacyLoadProperties() {
		fill                     = serializedObject.FindProperty("fill");
		smoothPath               = serializedObject.FindProperty("smoothPath");
		splitCount               = serializedObject.FindProperty("splitCount");
		splitDist                = serializedObject.FindProperty("splitDist");
		splitMiddle              = serializedObject.FindProperty("splitMiddle");
		randomByWorldCoordinates = serializedObject.FindProperty("randomByWorldCoordinates");

		surfaceOffset            = serializedObject.FindProperty("surfaceOffset");
		smoothSphereCollisions   = serializedObject.FindProperty("smoothSphereCollisions");
		sharpCorners             = serializedObject.FindProperty("sharpCorners");
		sharpCornerDistance      = serializedObject.FindProperty("sharpCornerDistance");
		colliderThickness        = serializedObject.FindProperty("colliderThickness");
		createCollider           = serializedObject.FindProperty("createCollider");
		create3DCollider         = serializedObject.FindProperty("create3DCollider");
		useEdgeCollider          = serializedObject.FindProperty("useEdgeCollider");
		collidersLeft            = serializedObject.FindProperty("collidersLeft");
		collidersRight           = serializedObject.FindProperty("collidersRight");
		collidersTop             = serializedObject.FindProperty("collidersTop");
		collidersBottom          = serializedObject.FindProperty("collidersBottom");
	}

	private void LegacyInspectorGUI() {
		Undo.RecordObject(target, "Modified Path Terrain");
		
		EditorGUILayout.HelpBox("This terrain is using legacy data! Legacy support will be removed in future versions. Upgrading will likely result in visual differences.", MessageType.Warning);
		if (GUILayout.Button("Upgrade")) {
			for (int i = 0; i < targets.Length; i++) {
				Ferr2DT_PathTerrain t = (Ferr2DT_PathTerrain)targets[i];
				if (t.IsLegacy)
					t.LegacyUpgrade();
			}
			GUIUtility.ExitGUI();
			return;
		}

        // render the material selector!
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
                });
            }

            UnityEngine.Object obj = EditorGUILayout.ObjectField((UnityEngine.Object)terrain.TerrainMaterial, typeof(Ferr2DT_Material), false, GUILayout.Height(64f));
            if (obj != (UnityEngine.Object)terrain.TerrainMaterial) {
                SelectMaterial(obj);
            }
            EditorGUILayout.EndHorizontal();
		});
		
		showTerrainType = EditorGUILayout.Foldout(showTerrainType, "TERRAIN DATA");
        if (showTerrainType) {
	        EditorGUI.indentLevel = 2;
	        Ferr.EditorTools.Box(4, ()=>{
		        EditorGUILayout.PropertyField(fill, new GUIContent("Fill Type"));
		        if (fill.enumValueIndex == (int)Ferr2DT_FillMode.Closed || fill.enumValueIndex == (int)Ferr2DT_FillMode.InvertedClosed || fill.enumValueIndex == (int)Ferr2DT_FillMode.FillOnlyClosed && terrain.GetComponent<Ferr2D_Path>() != null) terrain.GetComponent<Ferr2D_Path>().closed = true;
		        if (fill.enumValueIndex != (int)Ferr2DT_FillMode.None && (terrain.TerrainMaterial != null && terrain.TerrainMaterial.fillMaterial == null)) fill.enumValueIndex = (int)Ferr2DT_FillMode.None;
		        if (fill.enumValueIndex != (int)Ferr2DT_FillMode.None ) EditorGUILayout.PropertyField(fillZ, new GUIContent("Fill Z Offset"));
		        if (fill.enumValueIndex == (int)Ferr2DT_FillMode.Skirt) EditorGUILayout.PropertyField(fillY, new GUIContent("Skirt Y Value"));
		        if (fill.enumValueIndex == (int)Ferr2DT_FillMode.InvertedClosed) EditorGUILayout.PropertyField(invertFillBorder);
		        
		        EditorGUILayout.PropertyField(splitCorners  );
		        EditorGUILayout.PropertyField(smoothPath    );
		        EditorGUI.indentLevel = 3;
		        if (smoothPath.boolValue) {
			        EditorGUILayout.PropertyField(splitCount, new GUIContent("Edge Splits"));
			        EditorGUILayout.PropertyField(splitDist,  new GUIContent("Fill Split" ));
			        if (splitCount.intValue < 1) splitCount.intValue = 2;
		        } else {
			        splitCount.intValue   = 0;
			        splitDist .floatValue = 1;
		        }
		        EditorGUI.indentLevel = 2;
		        
		        EditorGUILayout.PropertyField(fillSplit, new GUIContent("Split fill mesh"));
		        if (fillSplit.boolValue) {
			        EditorGUI.indentLevel = 3;
			        EditorGUILayout.PropertyField(fillSplitDistance, new GUIContent("Split Distance"));
			        EditorGUI.indentLevel = 2;
		        }
	        });
        }
        EditorGUI.indentLevel = 0;

        showVisuals = EditorGUILayout.Foldout(showVisuals, "VISUALS");
		
        if (showVisuals) {
	        EditorGUI.indentLevel = 2;
	        Ferr.EditorTools.Box(4, ()=>{
	            // other visual data
		        EditorGUILayout.PropertyField(vertexColorType);
		        EditorGUI.indentLevel = 3;
		        if (!vertexColorType.hasMultipleDifferentValues && vertexColorType.enumValueIndex == (int)Ferr2DT_ColorType.SolidColor) {
			        EditorGUILayout.PropertyField(vertexColor        );
		        } else if (!vertexColorType.hasMultipleDifferentValues && vertexColorType.enumValueIndex == (int)Ferr2DT_ColorType.Gradient) {
			        EditorGUILayout.PropertyField(vertexGradientAngle);
			        EditorGUILayout.PropertyField(vertexGradient     );
		        } else if (!vertexColorType.hasMultipleDifferentValues && vertexColorType.enumValueIndex == (int)Ferr2DT_ColorType.DistanceGradient) {
			        EditorGUILayout.PropertyField(vertexGradientDistance);
			        EditorGUILayout.PropertyField(vertexGradient        );
		        }
		        EditorGUI.indentLevel = 2;
		        
		        EditorGUILayout.PropertyField(pixelsPerUnit );
		        EditorGUILayout.PropertyField(slantAmount   );
		        EditorGUILayout.PropertyField(splitMiddle   );
		        EditorGUILayout.PropertyField(createTangents);
		        EditorGUILayout.PropertyField(randomByWorldCoordinates, new GUIContent("Randomize Edge by World Coordinates"));
		        EditorGUILayout.PropertyField(uvOffset,                 new GUIContent("Fill UV Offset"));
		        
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
					        EditorGUILayout.HelpBox("Layer properties won't work properly unless your shaders are all 'transparent'!", MessageType.Warning);
				        }
			        }
		        }
	        });
        }
		EditorGUI.indentLevel = 0;
		
        showCollider = EditorGUILayout.Foldout(showCollider, "COLLIDER");
        // render collider options
        if (showCollider) {
	        EditorGUI.indentLevel = 2;
	        Ferr.EditorTools.Box(4, ()=>{
		        EditorGUILayout.PropertyField(createCollider);
		        if (createCollider.boolValue) {
			        EditorGUILayout.PropertyField(sharpCorners);
			        if (sharpCorners.boolValue) {
				        EditorGUI.indentLevel = 3;
				        EditorGUILayout.PropertyField(sharpCornerDistance, new GUIContent("Corner Distance"));
				        EditorGUI.indentLevel = 2;
			        }
			        
			        EditorGUILayout.PropertyField(create3DCollider, new GUIContent("Use 3D Collider"));
			        if (terrain.create3DCollider) {
				        EditorGUI.indentLevel = 3;
				        EditorGUILayout.PropertyField(depth, new GUIContent("Collider Width"));
				        EditorGUILayout.PropertyField(smoothSphereCollisions);
				        EditorGUI.indentLevel = 2;
				        EditorGUILayout.PropertyField(isTrigger);
				        EditorGUILayout.PropertyField(physicsMaterial);
			        } else {
				        EditorGUILayout.PropertyField(useEdgeCollider);
				        EditorGUILayout.PropertyField(usedByEffector);
				        EditorGUILayout.PropertyField(isTrigger);
				        EditorGUILayout.PropertyField(physicsMaterial2D);
			        }
			        
			        if (terrain.fill == Ferr2DT_FillMode.None) {
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Top   ), new GUIContent("Thickness Top"));
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Bottom), new GUIContent("Thickness Bottom"));
			        }
			        else {
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Top   ), new GUIContent("Offset Top"));
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Left  ), new GUIContent("Offset Left"));
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Right ), new GUIContent("Offset Right"));
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Bottom), new GUIContent("Offset Bottom"));
			        }
			        
                    //EditorGUI.indentLevel = 0;
			        EditorGUILayout.LabelField("Generate colliders along:");
			        EditorGUILayout.PropertyField(collidersTop,    new GUIContent("Top"   ));
			        EditorGUILayout.PropertyField(collidersLeft,   new GUIContent("Left"  ));
			        EditorGUILayout.PropertyField(collidersRight,  new GUIContent("Right" ));
			        EditorGUILayout.PropertyField(collidersBottom, new GUIContent("Bottom"));
			        
			        if (!collidersBottom.boolValue || !collidersLeft.boolValue || !collidersRight.boolValue || !collidersTop.boolValue) {
				        EditorGUI.indentLevel = 2;
				        EditorGUILayout.PropertyField(colliderThickness);
				        EditorGUI.indentLevel = 0;
			        }

					if (GUILayout.Button("Prebuild collider")) {
						for (int i = 0; i < targets.Length; i++) {
							Ferr2DT_PathTerrain t = targets[i] as Ferr2DT_PathTerrain;
							if (t != null) t.RecreateCollider();
						}
					}
				}
	        });
        }
		EditorGUI.indentLevel = 0;
		
		if(serializedObject.ApplyModifiedProperties() || GUI.changed) {
			for (int i = 0; i < targets.Length; i++) {
				EditorUtility.SetDirty(targets[i]);
				((Ferr2DT_PathTerrain)targets[i]).Build(true);
			}
			
			cachedColliders = terrain.GetColliderVerts();
		}
        if (Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    terrain.ForceMaterial(terrain.TerrainMaterial, true);
                    terrain.Build(true);
                    cachedColliders = terrain.GetColliderVerts();
                    break;
            }
        }
	}
	void LegacySceneGUI() {
		if (!Ferr2DT_PathTerrain.showGUI)
			return;

	    #if UNITY_5_5_OR_NEWER
	    EditorUtility.SetSelectedRenderState(terrain.gameObject.GetComponent<Renderer>(), Ferr2DT_Menu.HideMeshes ? EditorSelectedRenderState.Hidden : EditorSelectedRenderState.Highlight);
		#else
	    EditorUtility.SetSelectedWireframeHidden(terrain.gameObject.GetComponent<Renderer>(), Ferr2DT_Menu.HideMeshes);
		#endif
	    
        Ferr2DT_SceneOverlay.OnGUI();
    }
	public static void DrawColliderEdge (Ferr2DT_PathTerrain aTerrain) {
		if ((aTerrain.enabled == false || aTerrain.Path == null || aTerrain.Path.Count <= 1 || !aTerrain.createCollider) || !Ferr2DT_Menu.ShowCollider)
			return;
		Handles.color = Ferr2D_Visual.ColliderColor;

		if (cachedColliders == null) cachedColliders = aTerrain.GetColliderVerts();
		List<List<Vector2>> verts     = cachedColliders;
        Matrix4x4           mat       = aTerrain.transform.localToWorldMatrix;

		Handles.matrix = mat;
        for (int t = 0; t < verts.Count; t++) {
            for (int i = 0; i < verts[t].Count - 1; i++) {
				Handles.DrawLine(verts[t][i], verts[t][i+1]);
            }
        }
        if (verts.Count > 0 && verts[verts.Count - 1].Count > 0) {
            Handles.color = Color.yellow;
			Handles.DrawLine(verts[0][0], verts[verts.Count - 1][verts[verts.Count - 1].Count - 1]);
			Handles.color = Ferr2D_Visual.ColliderColor;
        }
		Handles.matrix = Matrix4x4.identity;
	}
}
