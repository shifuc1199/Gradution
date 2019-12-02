using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class Ferr2DT_SceneOverlay {
	#region Fields
	const float _dist = 100;

	static Texture2D _ferr2DIcon = Ferr.EditorTools.GetGizmo("2DTerrain/Gizmos/Ferr2DTIconSmall.png");
	static GUIStyle  _helpBoxStyle;
	static GUIStyle  _helpLabelStyle;

    static bool _showTop = true;
    static int  _top     = 0;

    public static bool showIndices  = false;
	public static bool segmentLockMode = false;
	#endregion

	#region Properties
	private static GUIStyle HelpBoxStyle { get { 
		if (_helpBoxStyle == null) {
			_helpBoxStyle = new GUIStyle( GUI.skin.box );
			_helpBoxStyle.normal.background = EditorGUIUtility.whiteTexture;
		}
		return _helpBoxStyle;
	} }
	private static GUIStyle HelpLabelStyle { get { 
		if (_helpLabelStyle == null) {
			_helpLabelStyle = new GUIStyle( GUI.skin.label );
			_helpLabelStyle.normal.textColor = new Color(.8f,.8f,.8f,1);
		}
		return _helpLabelStyle;
	} }
	#endregion

    public static void OnGUI() {
        Handles.BeginGUI();

	    int size  = 16;
	    int currX = 2;
        if (!_showTop) _top = (int)Screen.height - size*3 - 8;

	    GUI.Box(new Rect(0, _top, Screen.width, size), "", EditorStyles.toolbar);
	    
	    // if it's not the pro skin, the icons are too bright, almost unseeable
	    if (!EditorGUIUtility.isProSkin) {
		    GUI.contentColor = new Color(0,0,0,1);
	    }
	    
	    // Draw the Ferr2D icon
	    GUI.Label(new Rect(currX, 1, size, size), _ferr2DIcon);
	    currX += size+2;
	    
	    // reset the color back to normal
	    GUI.contentColor = Color.white;
	    
		Ferr2DT_Menu.ShowHelp   = GUI.Toggle(new Rect(currX, _top, size * 5, size), Ferr2DT_Menu.ShowHelp, new GUIContent("Show Help" + (Ferr2DT_Menu.ShowHelp?" \u25BC":"")), EditorStyles.toolbarButton);
	    currX += size * 5 + 6;

	    Ferr2DT_Menu.SnapMode   = (Ferr2DT_Menu.SnapType)EditorGUI.EnumPopup(new Rect(currX, _top, size * 6, size), Ferr2DT_Menu.SnapMode, EditorStyles.toolbarPopup);
	    currX += size * 6;
	    Ferr2DT_Menu.SmartSnap  = GUI.Toggle(new Rect(currX, _top, size * 5, size), Ferr2DT_Menu.SmartSnap, new GUIContent("Smart Snap", "[Ctrl+R]"), EditorStyles.toolbarButton);
	    currX += size * 5 + 6;

		segmentLockMode = GUI.Toggle(new Rect(currX, _top, size * 8, size), segmentLockMode, new GUIContent("Segment Lock Mode", "[Ctrl+L]"), EditorStyles.toolbarButton);
		currX += size * 8 + 6;
		
		#if UNITY_5_5_OR_NEWER
		Ferr2DT_Menu.HideMeshes = !GUI.Toggle(new Rect(currX, _top, size * 6, size), !Ferr2DT_Menu.HideMeshes, "Show Highlight",    EditorStyles.toolbarButton);
	    currX += size * 6;
	    #else
	    Ferr2DT_Menu.HideMeshes = !GUI.Toggle(new Rect(currX, top, size * 5, size), !Ferr2DT_Menu.HideMeshes, "Show Meshes",       EditorStyles.toolbarButton);
	    currX += size * 5;
	    #endif
	    
	    Ferr2DT_Menu.ShowCollider= GUI.Toggle(new Rect(currX, _top, size * 6, size), Ferr2DT_Menu.ShowCollider,"Show Colliders",    EditorStyles.toolbarButton);
	    currX += size * 6;
	    showIndices             =  GUI.Toggle(new Rect(currX, _top, size * 2, size), showIndices,              "123",               EditorStyles.toolbarButton);
	    currX += size * 2;
	    
		if (Event.current.control && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.R) {
	        Ferr2DT_Menu.SmartSnap = !Ferr2DT_Menu.SmartSnap;
        }
		if (Event.current.control && Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.L) {
			segmentLockMode = !segmentLockMode;
		}

		if (Ferr2DT_Menu.ShowHelp) {
			GUI.color = Ferr2D_Visual.HelpBoxColor;
			GUI.Box(new Rect(1,EditorGUIUtility.singleLineHeight + 4,205,EditorGUIUtility.singleLineHeight * 5), "", HelpBoxStyle);
			GUI.color = Color.white;

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("\u2022 CLICK+DRAG to select multiple", HelpLabelStyle);
			EditorGUILayout.LabelField("\u2022 SHIFT to add", HelpLabelStyle);
			EditorGUILayout.LabelField("\u2022 ALT to delete or reset", HelpLabelStyle);
			EditorGUILayout.LabelField("\u2022 C to change control point mode", HelpLabelStyle);
		}
		Handles.EndGUI();
    }
}
