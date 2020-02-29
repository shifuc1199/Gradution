using UnityEngine;

namespace Ferr {
	public class ProceduralMeshUtil {
		public const string cProcMeshPrefix = "FerrProcMesh_";
		
		public static void EnsureProceduralMesh(MeshFilter aFilter, bool aCreateRestoreComponent = true) {
			if (!IsProceduralMesh(aFilter)) {

				if (aCreateRestoreComponent) {
					RestoreMesh restore = aFilter.GetComponent<RestoreMesh>();
					if (restore == null) {
						#if UNITY_EDITOR
							restore = UnityEditor.Undo.AddComponent<RestoreMesh>(aFilter.gameObject);
						#else
							restore = aFilter.gameObject.AddComponent<RestoreMesh>();
						#endif
					}
					restore.OriginalMesh = aFilter.sharedMesh;
				}

				if (aFilter.sharedMesh == null)
					aFilter.sharedMesh = new Mesh();
				aFilter.sharedMesh = Object.Instantiate(aFilter.sharedMesh);
				aFilter.sharedMesh.name = MakeInstName(aFilter);
			} else if (!IsCorrectName(aFilter)) {
				aFilter.sharedMesh = Object.Instantiate(aFilter.sharedMesh);
				aFilter.sharedMesh.name = MakeInstName(aFilter);
			}
		}
		public static bool IsProceduralMesh(MeshFilter aFilter) {
			if (aFilter == null || aFilter.sharedMesh == null)
				return false;
			return aFilter.sharedMesh.name.StartsWith(cProcMeshPrefix);
		}
		public static string MakeInstName(MeshFilter aFilter) {
			return string.Format("{0}{1}_{2}", cProcMeshPrefix, aFilter.gameObject.name, aFilter.GetInstanceID());
		}
		public static bool IsCorrectName(MeshFilter aFilter) {
			if (aFilter == null || aFilter.sharedMesh == null)
				return false;
			return aFilter.sharedMesh.name == MakeInstName(aFilter);
		}
	}
}