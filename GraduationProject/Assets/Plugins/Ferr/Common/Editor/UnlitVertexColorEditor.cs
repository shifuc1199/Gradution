using UnityEditor;
using UnityEngine;

namespace Ferr {
	public class UnlitVertexColorEditor : ShaderGUI {
		public override void OnGUI (MaterialEditor aMaterialEditor, MaterialProperty[] aProperties) {
			base.OnGUI (aMaterialEditor, aProperties);
			
			Material targetMat = aMaterialEditor.target as Material;
			string[] keyWords  = targetMat.shaderKeywords;
			
			bool useTex = System.Array.IndexOf(keyWords, "NO_TEX") == -1;
			EditorGUI.BeginChangeCheck();
			useTex = EditorGUILayout.Toggle ("Use texture", useTex);
			if (EditorGUI.EndChangeCheck()) {
				string[] keywords = new string[] { useTex ? "USE_TEX" : "NO_TEX" };
				targetMat.shaderKeywords = keywords;
				EditorUtility.SetDirty (targetMat);
			}
		}
	}
}