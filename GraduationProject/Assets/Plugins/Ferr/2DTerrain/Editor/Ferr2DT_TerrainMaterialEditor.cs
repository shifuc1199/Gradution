using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

using Object = UnityEngine.Object;

[CustomEditor(typeof(Ferr2DT_TerrainMaterial))]
public class Ferr2DT_TerrainMaterialEditor : Editor {

	public override void OnInspectorGUI() {
		Undo.RecordObject(target, "Modified Terrain Material");
        
		IFerr2DTMaterial mat = target as IFerr2DTMaterial;
		Material         newMat;
		
		newMat = mat.edgeMaterial = (Material)EditorGUILayout.ObjectField("Edge Material", mat.edgeMaterial, typeof(Material), true);
		if (mat.edgeMaterial != newMat) {
			mat.edgeMaterial  = newMat;
			Ferr2DT_TerrainMaterialUtility.CheckMaterialMode(mat.edgeMaterial, TextureWrapMode.Clamp);
		}
		
		newMat = (Material)EditorGUILayout.ObjectField("Fill Material", mat.fillMaterial, typeof(Material), true);
		if (mat.fillMaterial != newMat) {
			mat.fillMaterial  = newMat;
			Ferr2DT_TerrainMaterialUtility.CheckMaterialMode(mat.fillMaterial, TextureWrapMode.Repeat);
		}
		
        if (mat.edgeMaterial == null) EditorGUILayout.HelpBox("Please add an edge material to enable the material editor!", MessageType.Warning);
        else {
            if (GUILayout.Button("Open Material Editor")) Ferr2DT_TerrainMaterialWindow.Show(mat);
        }

        DrawUpdateUI();

        if (GUI.changed) {
			EditorUtility.SetDirty(target);

            Ferr2DT_PathTerrain[] terrain = GameObject.FindObjectsOfType(typeof(Ferr2DT_PathTerrain)) as Ferr2DT_PathTerrain[];
            for (int i = 0; i < terrain.Length; i++)
            {
                if(terrain[i].TerrainMaterial == mat)
                    terrain[i].Build(true);
            }
		}
	}

    void DrawUpdateUI() {
        if (target is Ferr2DT_TerrainMaterial && GUILayout.Button("Create Updated Material Object")) {
            string path = AssetDatabase.GetAssetPath(target);
            path = System.IO.Path.ChangeExtension(path, ".asset");

            ScriptableObject material = ((Ferr2DT_TerrainMaterial)target).CreateNewFormatMaterial();

            if (AssetDatabase.LoadAssetAtPath(path, typeof(Object)) != null) {
                Debug.LogWarning("Already exists!");
            } else {
                
                AssetDatabase.CreateAsset(material, path);
                AssetDatabase.SaveAssets();

                EditorGUIUtility.PingObject(material);
            }
        }
    }
}