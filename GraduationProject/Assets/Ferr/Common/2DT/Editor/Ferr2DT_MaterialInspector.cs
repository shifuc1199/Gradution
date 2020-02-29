using UnityEngine;
using UnityEditor;

using System;
using System.Reflection;
using System.Collections;

[CustomEditor(typeof(Ferr2DT_Material))]
public class Ferr2DT_MaterialInspector : Editor {
	
	public override void OnInspectorGUI() {
		Undo.RecordObject(target, "Modified Terrain Material");
		
		IFerr2DTMaterial mat = target as IFerr2DTMaterial;
		Material         newMat;
		
		newMat = mat.edgeMaterial = (Material)EditorGUILayout.ObjectField("Edge Material", mat.edgeMaterial, typeof(Material), true);
		if (mat.edgeMaterial != newMat) {
			mat.edgeMaterial  = newMat;
			CheckMaterialMode(mat.edgeMaterial, TextureWrapMode.Clamp);
		}
		
		newMat = (Material)EditorGUILayout.ObjectField("Fill Material", mat.fillMaterial, typeof(Material), true);
		if (mat.fillMaterial != newMat) {
			mat.fillMaterial  = newMat;
			CheckMaterialMode(mat.fillMaterial, TextureWrapMode.Repeat);
		}
		
		Type window = Type.GetType("Ferr2DT_TerrainMaterialWindow");
		if (window == null) {
			EditorGUILayout.HelpBox("Ferr2D was not detected in the project! You must have Ferr2D installed to edit or use this material.", MessageType.Error);
			if (GUILayout.Button("Get Ferr2D")) {
				Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/11653");
			}
		} else {
			if (mat.edgeMaterial == null) EditorGUILayout.HelpBox("Please add an edge material to enable the material editor!", MessageType.Warning);
			else if (window != null){
				MethodInfo show = window.GetMethod("Show", BindingFlags.Static | BindingFlags.Public);
				if (show == null) {
					EditorGUILayout.HelpBox("No window show method found!", MessageType.Error);
				} else if (GUILayout.Button("Open Material Editor")) {
					show.Invoke(null, new object[] {mat});
				}
			}
		}
	}
	
	static void CheckMaterialMode(Material aMat, TextureWrapMode aDesiredMode) {
		if (aMat != null && aMat.mainTexture != null && aMat.mainTexture.wrapMode != aDesiredMode) {
			if (EditorUtility.DisplayDialog("Ferr2D Terrain", "The Material's texture 'Wrap Mode' generally works best when set to "+aDesiredMode+"! Would you like this texture to be updated?", "Yes", "No")) {
				string          path = AssetDatabase.GetAssetPath(aMat.mainTexture);
				TextureImporter imp  = AssetImporter.GetAtPath   (path) as TextureImporter;
				if (imp != null) {
					imp.wrapMode = aDesiredMode;
					AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
				}
			}
		}
	}
}
