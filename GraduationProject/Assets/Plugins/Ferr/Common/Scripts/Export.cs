using UnityEngine;
using System.IO;

namespace Ferr {
    public static class Export {
    #if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Ferr/Utility/Export .obj")]
        public static void MenuSaveMeshAsOBJ() {
            if (UnityEditor.Selection.gameObjects.Length <= 0) {
                return;
            }
            GameObject selected = UnityEditor.Selection.gameObjects[0];
            SaveOBJ(selected.GetComponent<MeshFilter>().sharedMesh, /*UnityEditor.AssetDatabase.GetAssetPath(selected)*/ "Assets\\" + selected.name+".obj");
        }
	    [UnityEditor.MenuItem("Tools/Ferr/Utility/Export .ply")]
	    public static void MenuSaveMeshAsPLY() {
		    if (UnityEditor.Selection.gameObjects.Length <= 0) {
			    return;
		    }
		    GameObject selected = UnityEditor.Selection.gameObjects[0];
		    SavePLY(selected.GetComponent<MeshFilter>().sharedMesh, /*UnityEditor.AssetDatabase.GetAssetPath(selected)*/ "Assets\\" + selected.name + ".ply");
	    }
    #endif

	    public static void SaveOBJ(Mesh aMesh, string aFileName) {
            Vector3[] verts = aMesh.vertices;
		    Vector2[] uvs   = aMesh.uv;
		    int    [] inds  = aMesh.triangles;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("o {0}\n", aMesh.name);
        
            for (int i = 0; i < verts.Length; i++) {
                sb.AppendFormat("v {0} {1} {2}\n", verts[i].x, verts[i].y, verts[i].z);
                sb.AppendFormat("vt {0} {1}\n", uvs[i].x, uvs[i].y);
            }
            for (int i = 0; i < inds.Length; i+=3) {
                sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}\n", inds[i]+1, inds[i+1]+1, inds[i+2]+1);
            }
            StreamWriter writer = new StreamWriter(aFileName);
            writer.Write(sb.ToString());
            writer.Close();
            Debug.Log(aFileName);
        }

	    public static void SavePLY(Mesh aMesh, string aFileName) {
		    Vector3[] verts = aMesh.vertices;
            Vector2[] uvs   = aMesh.uv;
            Color  [] cols  = aMesh.colors;
		    int    [] inds  = aMesh.triangles;

		    System.Text.StringBuilder sb = new System.Text.StringBuilder();
		    sb.AppendFormat(
@"ply
format ascii 1.0
element vertex {0}
property float x
property float y
property float z
property float s
property float t
property float red
property float green
property float blue
property float alpha
element face {1}
property list uchar int vertex_index
end_header
", 
                verts.Length, inds.Length/3);
		
		    for (int i = 0; i < verts.Length; i++) {
			    Vector3 v = verts[i];
			    Color   c = cols[i];
                Vector2 u = uvs[i];
                sb.AppendFormat("{0} {1} {2} {3} {4} {5} {6} {7} {8}\n", v.x, v.z, v.y, u.x, u.y, c.r, c.g, c.b, c.a);
		    }
		    for (int i = 0; i < inds.Length; i += 3) {
			    sb.AppendFormat("3 {2} {1} {0}\n", inds[i], inds[i+1], inds[i+2]);
		    }
		    StreamWriter writer = new StreamWriter(aFileName);
		    writer.Write(sb.ToString());
		    writer.Close();
		    Debug.Log(aFileName);
	    }
    }
}