using UnityEditor;
using UnityEngine;

public partial class TerrainTracker : AssetPostprocessor {
	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
		for (int i = 0; i < importedAssets.Length; i++) {
			if (importedAssets[i].EndsWith(".prefab")) {
				GameObject o = AssetDatabase.LoadAssetAtPath(importedAssets[i], typeof(UnityEngine.Object)) as GameObject;
				if (o == null) continue;

				Ferr2DT_PathTerrain[] terrains = o.GetComponentsInChildren<Ferr2DT_PathTerrain>();
				for (int t = 0; t < terrains.Length; t++) {
					MeshFilter filter = terrains[t].gameObject.GetComponent<MeshFilter>();
					if (filter.sharedMesh == null){
						terrains[t].CheckedLegacy = false;
						terrains[t].PathData.SetDirty();
						terrains[t].Build(true);
					}
				}

				if (terrains.Length > 0) {
					Ferr2DT_PathTerrain[] sceneTerrains = null;
					if (PrefabUtility.GetPrefabParent(Selection.activeGameObject) == o)
						sceneTerrains = Selection.activeGameObject.GetComponentsInChildren<Ferr2DT_PathTerrain>();
					
					if (sceneTerrains != null) {
						for (int t = 0; t < sceneTerrains.Length; t++) {
							PrefabUtility.ResetToPrefabState(sceneTerrains[t].GetComponent<MeshFilter>());
						}
					}
				}
			}
		}
	}
}