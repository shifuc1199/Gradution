using UnityEngine;
using System.Collections;
using System.IO;

namespace Ferr {
	public class SOUtil {
		#if UNITY_EDITOR
		public static ScriptableObject CreateAsset(System.Type aType, string aBaseName, bool aFocus = true) {
			
			string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeInstanceID);
			if (Path.GetExtension(path) != "") path = Path.GetDirectoryName(path);
			if (path == "") path = "Assets";
			
			return CreateAsset(aType, path, aBaseName, aFocus);
		}
		
		public static ScriptableObject CreateAsset(System.Type aType, string aFolder, string aBaseName, bool aFocus = true) {
			ScriptableObject style = ScriptableObject.CreateInstance(aType);
			
			if (!Directory.Exists(aFolder))
				Directory.CreateDirectory(aFolder);
			
			string name = aFolder+ "/"+aBaseName+".asset";
			int id = 0;
			while (UnityEditor.AssetDatabase.LoadAssetAtPath(name, aType) != null) {
				id += 1;
				name = aFolder + "/" + aBaseName + id + ".asset";
			}
			
			UnityEditor.AssetDatabase.CreateAsset(style, name);
			UnityEditor.AssetDatabase.SaveAssets();
			
			if (aFocus) {
				UnityEditor.EditorUtility.FocusProjectWindow();
				UnityEditor.Selection.activeObject = style;
			}
			
			return style;
		}
		#endif
	}
}