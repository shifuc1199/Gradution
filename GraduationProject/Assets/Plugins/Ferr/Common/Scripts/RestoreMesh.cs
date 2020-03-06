using UnityEngine;

namespace Ferr { 
	public class RestoreMesh : MonoBehaviour {
		[SerializeField] Mesh _originalMesh;

		public Mesh OriginalMesh { get { return _originalMesh; } set { _originalMesh = value; } }

		public void Restore(bool aMaintainColors = true) {
			MeshFilter filter = GetComponent<MeshFilter>();
			if (filter == null) {
				Debug.LogError("No mesh filter to restore to!", gameObject);
				return;
			}
		
			RecolorTree recolor = null;
			if (aMaintainColors) {
				recolor = new RecolorTree(filter.sharedMesh);
			}

			filter.sharedMesh = _originalMesh;

			if (aMaintainColors) {
				ProceduralMeshUtil.EnsureProceduralMesh(filter);
				Mesh m = filter.sharedMesh;
				recolor.Recolor(ref m);
			}
		}
	}
}