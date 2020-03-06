using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Ferr {
	public class EditorTools {
		public enum MultiType {
			None,
			All,
			Some
		}
		
        #region Fields
        static int    handleID       = 0;
		static int    selectedHandle = -1;

		static Material _CapMaterial2D = null;
		static Material _CapMaterial3D = null;
		
		internal static Matrix4x4 capDir = Matrix4x4.identity;
		#endregion

		#region Properties
		static Material CapMaterial2D {
			get {
				if (_CapMaterial2D == null) { _CapMaterial2D = new Material(Shader.Find("Hidden/Ferr Gizmo Shader 2D")); }
				return _CapMaterial2D;
			}
		}
		static Material CapMaterial3D {
			get {
				if (_CapMaterial3D == null) { _CapMaterial3D = new Material(Shader.Find("Hidden/Ferr Gizmo Shader 3D")); }
				return _CapMaterial3D;
			}
		}
		#endregion

		#region Menus
		[MenuItem("Tools/Ferr/Utility/Clear PlayerPrefs")]
		static void ClearPlayerPrefs() {
			PlayerPrefs.DeleteAll();
		}
		#endregion

		#region General utilities
		public static bool IsVisibleInSceneView(Vector3 aWorldPoint) {
			Vector3 pt = SceneView.lastActiveSceneView.camera.WorldToViewportPoint(aWorldPoint);
			return pt.x>=0 && pt.x<=1 && pt.y>=0 && pt.y<=1 && pt.z>0;
		}
		public static Vector3   GetUnitySnap() {
            return new Vector3(EditorPrefs.GetFloat("MoveSnapX", 1), EditorPrefs.GetFloat("MoveSnapY", 1), EditorPrefs.GetFloat("MoveSnapZ", 1));
        }
		public static float     GetUnityScaleSnap() {
            return EditorPrefs.GetFloat("ScaleSnap", .1f);
        }
		public static float     GetUnityRotationSnap() {
            return EditorPrefs.GetFloat("RotationSnap", 15);
        }
		public static MultiType IsStatic(UnityEngine.Object[] aItems) {
			MultiType result = MultiType.None;
			for (int i = 0; i < aItems.Length; ++i) {
				if ((aItems[i] as Component).gameObject.isStatic) {
					if (result == MultiType.None) result = MultiType.All;
				} else {
					if (result == MultiType.All ) result = MultiType.Some;
				}
			}
			return result;
		}
		public static object GetTargetObjectOfProperty(SerializedProperty prop) {
			var path = prop.propertyPath.Replace(".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split('.');
			foreach (var element in elements) {
				if (element.Contains("[")) {
					var elementName = element.Substring(0, element.IndexOf("["));
					var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
					obj = GetValue_Imp(obj, elementName, index);
				} else {
					obj = GetValue_Imp(obj, element);
				}
			}
			return obj;
		}
		private static object GetValue_Imp(object source, string name) {
			if (source == null)
				return null;
			var type = source.GetType();

			while (type != null) {
				var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				if (f != null)
					return f.GetValue(source);

				var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (p != null)
					return p.GetValue(source, null);

				type = type.BaseType;
			}
			return null;
		}
		private static object GetValue_Imp(object source, string name, int index) {
			var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
			if (enumerable == null) return null;
			var enm = enumerable.GetEnumerator();

			for (int i = 0; i <= index; i++) {
				if (!enm.MoveNext()) return null;
			}
			return enm.Current;
		}
		public static Vector3 ProjectPoint(Vector3 aPoint, Plane aPlane) {
			Ray r = new Ray(SceneView.lastActiveSceneView.camera.transform.position, aPoint - SceneView.lastActiveSceneView.camera.transform.position);
			float d = 0;

			if (aPlane.Raycast(r, out d)) {
				return r.GetPoint(d); ;
			}
			return aPoint;
		}
		public static T    GetEditorVar<T>(string aName, ref T? aVar, T aDefault) where T : struct  {
			Type typ = typeof(T);
			if (!aVar.HasValue) {
				if (typ == typeof(bool))
					aVar = (T)(object)EditorPrefs.GetBool(aName, (bool)(object)aDefault);
				else if (typ == typeof(string))
					aVar = (T)(object)EditorPrefs.GetString(aName, (string)(object)aDefault);
				else if (typ == typeof(float))
					aVar = (T)(object)EditorPrefs.GetFloat(aName, (float)(object)aDefault);
				else if (typ == typeof(int) || typ.IsEnum)
					aVar = (T)(object)EditorPrefs.GetInt(aName, (int)(object)aDefault);
				else if (typ == typeof(Color))
					aVar = (T)(object)ColorUtil.FromHex(EditorPrefs.GetString(aName, ColorUtil.ToHex((Color)(object)aDefault)));
				else
					Debug.LogError("Bad editor var type!");
			}
			return aVar.Value;
		}
		public static void SetEditorVar<T>(string aName, ref T? aVar, T aValue) where T : struct  {
			Type typ = typeof(T);
			if (!aVar.HasValue || !aVar.Value.Equals(aValue)) {
				if (typ == typeof(bool))
					EditorPrefs.SetBool(aName, (bool)(object)aValue);
				else if (typ == typeof(string))
					EditorPrefs.SetString(aName, (string)(object)aValue);
				else if (typ == typeof(float))
					EditorPrefs.SetFloat(aName, (float)(object)aValue);
				else if (typ == typeof(int) || typ.IsEnum)
					EditorPrefs.SetInt(aName, (int)(object)aValue);
				else if (typ == typeof(Color))
					EditorPrefs.SetString(aName, ColorUtil.ToHex((Color)(object)aValue));
				else
					Debug.LogError("Bad editor var type!");
				aVar = aValue;
			}
		}
		#endregion

        #region File and resource methods
		public static string    GetFerrDirectory   () {
			System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace( 0, true );
			System.Diagnostics.StackFrame frame      = stackTrace.GetFrame( 0 );
			string path = frame.GetFileName();

			path = path.Replace('\\', '/');
			int    start = path.IndexOf("Ferr/Common/Editor");
			if (start == -1) {
                Debug.LogError("You can put the 'Ferr' folder where you want, but the name should stay the same, and the tool folders must be inside it!");
				return "";
			}
	        string dir   = path.Substring(0, start);
			
			return "Assets/"+dir.Substring(Application.dataPath.Length+1);
		}
		public static Texture2D GetGizmo           (string aFileName) {
			string    path = GetFerrDirectory()+"Ferr/" + aFileName;
			Texture2D tex  = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
	        if (tex == null) {
	            tex = EditorGUIUtility.whiteTexture;
		        Debug.Log("Couldn't load Gizmo tex " + path);
	        }
	        return tex;
	    }
	    public static List<T>   GetPrefabsOfType<T>() where T:Component {
	        string[] fileNames  = System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories);
	        int      pathLength = Application.dataPath.Length + 1;
	        List<T>  result     = new List<T>();
	
	        for (int i = fileNames.Length; i > 0; i--) {
	            fileNames[i - 1] = "Assets\\" + fileNames[i - 1].Substring(pathLength);
	            GameObject go = UnityEditor.AssetDatabase.LoadAssetAtPath(fileNames[i - 1], typeof(GameObject)) as GameObject;
	            if (go != null) {
	                T source = go.GetComponent<T>();
	                if (source) result.Add(source);
	            }
	        }
	        return result;
	    }
        public static Material  GetDefaultMaterial () {
			System.Reflection.BindingFlags bfs                            = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;
	        System.Reflection.MethodInfo   getBuiltinExtraResourcesMethod = typeof( EditorGUIUtility ).GetMethod( "GetBuiltinExtraResource", bfs );
			#if UNITY_5_3_OR_NEWER
			string matName = "Default-Material.mat";
	        #else
	        string matName = "Default-Diffuse.mat";
	        #endif
	        return (Material)getBuiltinExtraResourcesMethod.Invoke( null, new object[] { typeof( Material ), matName } );
		}
        #endregion

        #region UI Drawing methods
        public static void DrawRect (Rect aRect) {
	        DrawRect(aRect, new Rect(0,0,1,1));
	    }
	    public static void DrawRect (Rect aRect, Rect aBounds) {
			float x      = aBounds.x + aRect.x * aBounds.width;
			float y      = aBounds.y + aRect.y * aBounds.height;
			float width  = aRect.width  * aBounds.width;
			float height = aRect.height * aBounds.height;
			
			GUI.DrawTexture(new Rect(x,       y,         width, 1     ), EditorGUIUtility.whiteTexture);
			GUI.DrawTexture(new Rect(x,      (y+height), width, 1     ), EditorGUIUtility.whiteTexture);
			GUI.DrawTexture(new Rect(x,       y,         1,     height), EditorGUIUtility.whiteTexture);
			GUI.DrawTexture(new Rect(x+width, y,         1,     height), EditorGUIUtility.whiteTexture);
		}
	    public static void DrawHLine(Vector2 aPos, float aLength) {
	        GUI.DrawTexture(new Rect(aPos.x, aPos.y, aLength, 1), EditorGUIUtility.whiteTexture);
	    }
	    public static void DrawVLine(Vector2 aPos, float aLength) {
	        GUI.DrawTexture(new Rect(aPos.x, aPos.y, 1, aLength), EditorGUIUtility.whiteTexture);
	    }
		public static void DrawDepthLine(Vector3 aP1, Vector3 aP2) {
			if (Event.current.type != EventType.Repaint) {
				return;
			}
			CapMaterial3D.SetPass(1);

			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(GL.LINES);
			GL.Color(Handles.color);
			GL.Vertex(aP1);
			GL.Vertex(aP2);
			GL.End();
			GL.PopMatrix();
		}
		public static void DrawLine(Vector3 aP1, Vector3 aP2, float aWidth) {
			if (Event.current.type != EventType.Repaint) {
				return;
			}
			CapMaterial2D.mainTexture = EditorGUIUtility.whiteTexture;
			CapMaterial2D.SetPass(0);

			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(GL.TRIANGLES);
			GL.Color(Handles.color);
			Vector3 norm = (aP2 - aP1).normalized;
			norm = new Vector3(-norm.y, norm.x, norm.z) * HandleUtility.GetHandleSize(aP2) * aWidth;

			GL.Vertex(aP1 - norm);
			GL.Vertex(aP1 + norm);
			GL.Vertex(aP2 + norm);

			GL.Vertex(aP2 + norm);
			GL.Vertex(aP2 - norm);
			GL.Vertex(aP1 - norm);
			GL.End();
			GL.PopMatrix();
		}
		public static void DrawDashedLine(Vector3 aP1, Vector3 aP2, float aWidth) {
			if (Event.current.type != EventType.Repaint) {
				return;
			}
			const float ratio = 20;
			const float gapRatio = 10;

			float size = ratio * aWidth;
			float linePercent = ((ratio-gapRatio)/ratio);
			float dist = Vector3.Distance(aP1, aP2);
			int   segs = (int)(dist / size);

			Vector3 curr = aP1;
			Vector3 step = (aP2 - aP1).normalized * size;
			for (int i = 0; i < segs; i++) {
				DrawLine(curr, curr+ step * linePercent, aWidth);
				curr += step;
			}

			float remaining = Mathf.Min(Vector3.Distance(aP2, curr), linePercent * size);
			DrawLine(curr, curr+remaining*step.normalized, aWidth);
		}
		public static void DrawPolyLine(Vector3[] aPts, float aWidth) {
			if (Event.current.type != EventType.Repaint) {
				return;
			}
			CapMaterial2D.mainTexture = EditorGUIUtility.whiteTexture;
			CapMaterial2D.SetPass(0);
			
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(GL.TRIANGLES);
			GL.Color(Handles.color);
			for (int i = 1; i < aPts.Length; i++) {
				Vector3 norm = (aPts[i] - aPts[i-1]).normalized;
				norm = new Vector3(-norm.y, norm.x, norm.z) * HandleUtility.GetHandleSize(aPts[i]) * aWidth;
				
				GL.Vertex(aPts[i-1] - norm);
				GL.Vertex(aPts[i-1] + norm);
				GL.Vertex(aPts[i] + norm);
				
				GL.Vertex(aPts[i] + norm);
				GL.Vertex(aPts[i] - norm);
				GL.Vertex(aPts[i-1] - norm);
			}
			GL.End();
			GL.PopMatrix();
		}

		public static void DrawPolyLine(List<Vector2> aPts, bool aClosed) {
			if (Event.current.type != EventType.Repaint || aPts.Count < 2) {
				return;
			}
			Material mat = CapMaterial2D;
			mat.mainTexture = EditorGUIUtility.whiteTexture;
			mat.SetPass(0);
			
			GL.PushMatrix();
			GL.MultMatrix(Handles.matrix);
			GL.Begin(GL.LINE_STRIP);
			GL.Color(Handles.color);

			int count = aPts.Count;
			for (int i = 0; i < count; i++) {
				GL.Vertex(aPts[i]);
			}
			if (aClosed)
				GL.Vertex(aPts[0]);

			GL.End();
			GL.PopMatrix();
		}

        public static void Box      (int aBorder, System.Action inside) {
            Box(aBorder, inside, 0, 0);
        }
	    public static void Box      (int aBorder, System.Action inside, int aWidthOverride, int aHeightOverride)
	    {
	        Rect r = EditorGUILayout.BeginHorizontal(GUILayout.Width(aWidthOverride));
	        if (aWidthOverride != 0)
	        {
	            r.width = aWidthOverride;
	        }
	        GUI.Box(r, GUIContent.none);
	        GUILayout.Space(aBorder);
	        if (aHeightOverride != 0)
	            EditorGUILayout.BeginVertical(GUILayout.Height(aHeightOverride));
	        else
	            EditorGUILayout.BeginVertical();
	        GUILayout.Space(aBorder);
	        inside();
	        GUILayout.Space(aBorder);
	        EditorGUILayout.EndVertical();
	        GUILayout.Space(aBorder);
	        EditorGUILayout.EndHorizontal();
	    }
        #endregion

        #region Custom handles
        public static Rect    UVRegionRect (Rect    aRect, Rect aBounds) {
	        Vector2 pos = RectHandle(new Vector2(aBounds.x+aRect.x, aBounds.y+aRect.y), aRect, aBounds);
	        aRect.x = pos.x - aBounds.x;
	        aRect.y = pos.y - aBounds.y;
	
	        float left  = MouseHandle(new Vector2(aBounds.x+aRect.x,   aBounds.y+aRect.y+aRect.height/2), 10).x - aBounds.x;
	        float right = MouseHandle(new Vector2(aBounds.x+aRect.xMax,aBounds.y+aRect.y+aRect.height/2), 10).x - aBounds.x;
	
	        float top    = MouseHandle(new Vector2(aBounds.x+aRect.x+aRect.width/2,aBounds.y+aRect.y   ), 10).y - aBounds.y;
	        float bottom = MouseHandle(new Vector2(aBounds.x+aRect.x+aRect.width/2,aBounds.y+aRect.yMax), 10).y - aBounds.y;
	
	        return new Rect(left, top, right-left, bottom-top);
	    }
	    public static Vector2 MouseHandle  (Vector2 aPos, int aSize) {
	        Rect button = new Rect(aPos.x-aSize/2, aPos.y-aSize/2, aSize, aSize);
	        GUI.DrawTexture(button, EditorGUIUtility.whiteTexture);
	        return RectHandle(aPos, button);
	    }
	    public static Vector2 RectHandle   (Vector2 aPos, Rect aRect) {
	        return RectHandle(aPos, aRect, new Rect(0,0,1,1));
	    }
	    public static Vector2 RectHandle   (Vector2 aPos, Rect aRect, Rect aBounds) {
	        handleID += 1;
	
	        EditorTools.DrawRect(new Rect(aBounds.x+aRect.x, aBounds.y+aRect.y, aRect.width, aRect.height));
	        if (Event.current.type == EventType.MouseDown) {
	            if (new Rect(aBounds.x+aRect.x, aBounds.y+aRect.y, aRect.width, aRect.height).Contains(Event.current.mousePosition)) {
	                selectedHandle = handleID;
	            }
	        }
	        if (selectedHandle == handleID && Event.current.type == EventType.MouseDrag) {
	            aPos += Event.current.delta;
	        }
	        return aPos;
	    }
        public static bool    ResetHandles () {
	        handleID = 0;
	        if (Event.current.type == EventType.MouseUp) {
	            selectedHandle = -1;
	            return true;
	        }
	        return false;
	    }
	    public static bool    HandlesMoving() {
	        return selectedHandle != -1;
	    }
		#endregion

		#region Cap methods
		public static void EmptyCap(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aEvent) {
            if (aEvent == EventType.Layout){
                HandleUtility.AddControl(aControlID, HandleUtility.DistanceToRectangle(aPosition, aRotation, aSize));
            }
		}
		public static void CircleCapBase(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aEvent) {
			if (aEvent == EventType.Repaint) {
				aPosition = Handles.matrix.MultiplyPoint(aPosition);
				Vector3 right = Camera.current.transform.right * aSize;
				Vector3 up    = Camera.current.transform.up    * aSize;
				CapMaterial2D.mainTexture = null;
				CapMaterial2D.SetPass(0);

				GL.Begin(GL.QUADS);
				GL.Color(Handles.color);
				GL.Vertex(aPosition + up);
				GL.Vertex(aPosition + right);
				GL.Vertex(aPosition - up);
				GL.Vertex(aPosition - right);

				GL.End();
			} else if (aEvent == EventType.Layout) {
				HandleUtility.AddControl(aControlID, HandleUtility.DistanceToRectangle(aPosition, aRotation, aSize));
			}
		}
		public static void ImageCapBase(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, Texture2D aTex, EventType aEvent) {
            if (aEvent == EventType.Repaint ) {
				Material  mat          = CapMaterial2D;
				Transform camTransform = Camera.current.transform;

			    aPosition = Handles.matrix.MultiplyPoint(aPosition);
			    Vector3 right = camTransform.right * aSize;
				Vector3 top   = camTransform.up    * aSize;
				Vector3 left = aPosition - right;
				right += aPosition;
			    
			    mat.mainTexture = aTex;
			    mat.SetPass(0);
			
			    GL.Begin(GL.QUADS);
			    GL.Color(Handles.color);
			    GL.TexCoord2(1, 1);
			    GL.Vertex(right + top);
			
			    GL.TexCoord2(1, 0);
			    GL.Vertex(right - top);
			
			    GL.TexCoord2(0, 0);
			    GL.Vertex(left - top);
			
			    GL.TexCoord2(0, 1);
			    GL.Vertex(left + top);
			
			    GL.End();
            } else if (aEvent == EventType.Layout) {
                HandleUtility.AddControl(aControlID, HandleUtility.DistanceToCircle(aPosition, aSize));
            }
		}
        public static void CubeCapDirBase(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, Vector3 aScale, EventType aEvent) {
			if (aEvent == EventType.Repaint) {
				aPosition  = Handles.matrix.MultiplyPoint(aPosition);
			
				for (int i = 0; i < CapMaterial3D.passCount; ++i) {
					CapMaterial3D.SetPass(i);
				
					GL.PushMatrix();
					GL.Begin(GL.QUADS);
					GL.Color(Handles.color);
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1,-1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1,-1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1,-1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1,-1) * aSize, aScale)));
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1, 1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1, 1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1, 1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1, 1) * aSize, aScale)));
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1,-1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1,-1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1, 1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1, 1) * aSize, aScale)));
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1, 1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1, 1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1,-1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1,-1) * aSize, aScale)));
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1, 1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1, 1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1, 1,-1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3(-1,-1,-1) * aSize, aScale)));
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1,-1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1,-1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1, 1, 1) * aSize, aScale)));
					GL.Vertex(aPosition + capDir.MultiplyPoint(Vector3.Scale(new Vector3( 1,-1, 1) * aSize, aScale)));
				
					GL.End();
					GL.PopMatrix();
				}
			} else if (aEvent == EventType.Layout) {
				HandleUtility.AddControl(aControlID, HandleUtility.DistanceToRectangle(aPosition, aRotation, aSize));
			}
		}
		public static void DiamondCapDir(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aEvent) {
			if (aEvent == EventType.Repaint) {
				aPosition  = Handles.matrix.MultiplyPoint(aPosition);
			
				for (int i = 0; i < CapMaterial3D.passCount; ++i) {
					CapMaterial3D.SetPass(i);
				
					GL.PushMatrix();
					GL.Begin(GL.TRIANGLES);
					GL.Color(Handles.color);
				
					GL.Color(Handles.color);
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0,-1) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0, 1) * aSize * .5f));
					GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, 1, 0) * aSize));
				
					GL.Color(Handles.color);
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0, 1) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0,-1) * aSize * .5f));
					GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, 1, 0) * aSize));
				
					GL.Color(Handles.color);
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0,-1) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0,-1) * aSize * .5f));
					GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, 1, 0) * aSize));
				
					GL.Color(Handles.color);
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0, 1) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0, 1) * aSize * .5f));
					GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, 1, 0) * aSize));
				
				
					GL.Color(Handles.color);
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0, 1) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0,-1) * aSize * .5f));
					GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, -1, 0) * aSize));
				
					GL.Color(Handles.color);
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0,-1) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0, 1) * aSize * .5f));
					GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, -1, 0) * aSize));
				
					GL.Color(Handles.color);
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0,-1) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0,-1) * aSize * .5f));
					GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, -1, 0) * aSize));
				
					GL.Color(Handles.color);
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 1,0, 1) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3(-1,0, 1) * aSize * .5f));
					GL.Color(Color.Lerp(Handles.color, Color.black, 0.35f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(new Vector3( 0, -1, 0) * aSize));
				
					GL.End();
					GL.PopMatrix();
				}
			} else if (aEvent == EventType.Layout) {
				HandleUtility.AddControl(aControlID, HandleUtility.DistanceToRectangle(aPosition, aRotation, aSize));
			}
		}
		public static void ArrowCapDirBase(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, Vector3 aDir, EventType aEvent) {
			if (aEvent == EventType.Repaint) {
				aPosition  = Handles.matrix.MultiplyPoint(aPosition);
				Quaternion rot = Quaternion.LookRotation(aDir);
			
				for (int i = 0; i < CapMaterial3D.passCount; ++i) {
					CapMaterial3D.SetPass(i);
			
					GL.PushMatrix();
					GL.Begin(GL.TRIANGLES);
					GL.Color(Handles.color);
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1, 1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1,-1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 0, 0, 1) * aSize));
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1,-1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1, 1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 0, 0, 1) * aSize));
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1,-1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1,-1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 0, 0, 1) * aSize));
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1, 1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1, 1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 0, 0, 1) * aSize));
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1, 1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1, 1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1,-1,0) * aSize * .5f));
				
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1, 1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3( 1,-1,0) * aSize * .5f));
					GL.Vertex(aPosition + capDir.MultiplyPoint(rot * new Vector3(-1,-1,0) * aSize * .5f));
				
					GL.End();
					GL.PopMatrix();
				}
			} else if (aEvent == EventType.Layout) {
				HandleUtility.AddControl(aControlID, HandleUtility.DistanceToRectangle(aPosition, aRotation, aSize));
			}
		}
		public static void ArrowCapXP(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(1, 0, 0), aType);
		}
		public static void ArrowCapXN(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(-1, 0, 0), aType);
		}
		public static void ArrowCapZP(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(0, 0, 1), aType);
		}
		public static void ArrowCapZN(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(0, 0, -1), aType);
		}
		public static void ArrowCapYP(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(0, 1, 0), aType);
		}
		public static void ArrowCapYN(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {
			ArrowCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(0, -1, 0), aType);
		}
		
		public static void BarCapX(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {
			CubeCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(1, .25f, .25f), aType);
		}
		public static void BarCapY(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {
			CubeCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(.25f, 1, .25f), aType);
		}
		public static void BarCapZ(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {
			CubeCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(.25f, .25f, 1), aType);
		}
		public static void BarCapXZ(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {
			CubeCapDirBase(aControlID, aPosition, aRotation, aSize, new Vector3(.25f, .25f, .25f), aType);
		}
        #endregion
    }
}