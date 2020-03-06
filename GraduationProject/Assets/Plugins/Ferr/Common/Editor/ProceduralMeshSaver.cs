using UnityEngine;
using UnityEditor;
using System.IO;

namespace Ferr {
	class ProceduralMeshSaver : AssetPostprocessor {
        #region Fields
        
        #endregion

        #region Unity Events
        static void OnPostprocessAllAssets ( string[] aImportedAssets, string[] aDeletedAssets, string[] aMovedAssets, string[] aMovedFromAssetPaths) {
            // Check any items that were added or updated
			for (int i=0; i<aImportedAssets.Length; i+=1) {
				if (aImportedAssets[i].EndsWith(".prefab")) {
					GameObject go = AssetDatabase.LoadAssetAtPath(aImportedAssets[i], typeof(GameObject)) as GameObject;
					if (go == null) continue;

					Component[] coms = go.GetComponentsInChildren(typeof(IProceduralMesh), true);
					for (int c=0; c<coms.Length; c+=1) {
						SaveMesh(coms[c] as IProceduralMesh);
					}
				}
			}
            for (int i=0; i<aMovedFromAssetPaths.Length; i+=1) {
				if (aMovedFromAssetPaths[i].EndsWith(".prefab")) {
                }
            }
		}
        #endregion

        #region Helper methods
        static void SaveMesh     (IProceduralMesh aMesh) {
			aMesh.Build(true);
			Mesh m = aMesh.MeshData;
			
			string name = "";
			string file = "";
			GetUniquePath((aMesh as Component).gameObject, m.name, out name, out file);
			
			try {
				UnityEditor.AssetDatabase.CreateAsset(m, file);
				UnityEditor.AssetDatabase.Refresh    (       );
			} catch {
				Debug.LogError("Unable to save prefab procedural mesh! Likely, you deleted the mesh files, and the prefab is still referencing them. Restarting your Unity editor should solve this minor issue.");
			}
		}
		static void GetUniquePath(GameObject aObj, string aMeshName, out string aName, out string aFileName) {
			string    path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(aObj)) + "/Meshes";
			string    name = aMeshName;
			Transform curr = aObj.transform.parent;
			
			// make sure the path folder exists
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			
			// check for other files with the same name
			int    count = 0;
			string file  = path + "/" + name + ".asset";
			while (File.Exists(file)) {
				count += 1;
				file   = path + "/" + name + count + ".asset";
			}
			
			// and return results
			aName     = name + (count == 0 ? "" : ""+count);
			aFileName = file;
		}
        #endregion
    }
}