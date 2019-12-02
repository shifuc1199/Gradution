using UnityEditor;
using UnityEngine;

namespace Ferr { 
	[CustomEditor(typeof(RestoreMesh)), CanEditMultipleObjects]
	public class RestoreMeshInspector : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			EditorGUILayout.HelpBox("Loads in the current version of the original mesh asset. Preserves vertex colors as best it can.", MessageType.Info);
			if (GUILayout.Button("Refresh Mesh")) {
				for (int i = 0; i < targets.Length; i++) {
					Undo.RecordObject(((RestoreMesh)targets[i]).GetComponent<MeshFilter>(), "Refresh Mesh");
					((RestoreMesh)targets[i]).Restore();
				}
			}
			EditorGUILayout.HelpBox("Loads in the current version of the original mesh asset. DISCARDS any vertex color changes that have been made.", MessageType.Warning);
			if (GUILayout.Button("Revert Mesh (DISCARD colors)")) {
				for (int i = 0; i < targets.Length; i++) {
					Undo.RecordObject(((RestoreMesh)targets[i]).GetComponent<MeshFilter>(), "Revert Mesh");
					((RestoreMesh)targets[i]).Restore(false);
				}
			}
		}
	}
}