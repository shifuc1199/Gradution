using UnityEngine;
using UnityEditor;

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using Object = UnityEngine.Object;

public class Ferr2DT_MaterialSelector : ScriptableWizard
{
    const int    cMaxRecent  = 10;
    const string cHistoryKey = "Ferr2DT_MaterialHistory";
    
    Vector2                  _scroll;
    Object                   _selectedObject;
    List<string>             _recentGUIDs = null;
    Action<IFerr2DTMaterial> _onPickMaterial;
    
    public static void Show(Action<IFerr2DTMaterial> aOnPickMaterial) {
        Ferr2DT_MaterialSelector wiz = ScriptableWizard.DisplayWizard<Ferr2DT_MaterialSelector>("Select Terrain Material");
        wiz._onPickMaterial = aOnPickMaterial;
        wiz._selectedObject = null;
    }
    
    void OnGUI() {
        EditorGUILayout.BeginVertical();
        
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Select a Material \u25BC", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (_selectedObject != null) {
            IFerr2DTMaterial mat = _selectedObject as IFerr2DTMaterial;
            if (mat != null && mat.edgeMaterial != null && mat.edgeMaterial.mainTexture != null) {
                GUILayout.Label(mat.edgeMaterial.mainTexture, GUILayout.Width(32), GUILayout.Height(32));
            }
        }
        _selectedObject = EditorGUILayout.ObjectField(_selectedObject, typeof(Ferr2DT_Material), false, GUILayout.Height(32)) ;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        
		EditorGUILayout.Space();
        EditorGUILayout.LabelField("Or Pick From Recent Materials", EditorStyles.boldLabel);
        _scroll = EditorGUILayout.BeginScrollView(_scroll, EditorStyles.helpBox);
        List<UnityEngine.Object> history = GetRecentList();
        for (int i = 0; i < history.Count; i++) {
            if (DrawObject( history[i] as IFerr2DTMaterial )) {
                _selectedObject = history[i];
            }
        }
        EditorGUILayout.EndScrollView();

        IFerr2DTMaterial obj = _selectedObject as IFerr2DTMaterial;
        if (GUILayout.Button("Confirm") && obj != null) {
            AddToRecentList(_selectedObject);
            if (_onPickMaterial != null)
                _onPickMaterial(obj);
            Close();
        }
        return;
    }

    List<Object> GetRecentList() {
        if (_recentGUIDs == null)
            LoadList();

        List<Object> result = new List<Object>();

        for (int i = 0; i < _recentGUIDs.Count; i++) {
            result.Add(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(_recentGUIDs[i]), typeof(Object)));
        }
        return result;
    }
    void AddToRecentList(Object aObject) {
        List<Object> recent = GetRecentList();
        int index = recent.IndexOf(aObject);
        if (index == -1) {
            _recentGUIDs.Insert(0, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(aObject)));
        } else {
            string guid = _recentGUIDs[index];
            _recentGUIDs.RemoveAt(index);
            _recentGUIDs.Insert(0, guid);
        }

        // cap it at a maximum number
        while (_recentGUIDs.Count > cMaxRecent) {
            _recentGUIDs.RemoveAt(_recentGUIDs.Count-1);
        }
        SaveList();
    }
    void SaveList() {
        string data = "";
        for (int i = 0; i < _recentGUIDs.Count; i++) {
            data += _recentGUIDs[i];
            if (i < _recentGUIDs.Count-1)
                data += "|";
        }

        EditorPrefs.SetString(cHistoryKey, data);
    }
    void LoadList() {
        string data = EditorPrefs.GetString(cHistoryKey, "");

        if (string.IsNullOrEmpty(data)) {
            _recentGUIDs = new List<string>();
            return;
        }

        _recentGUIDs= new List<string>( data.Split('|') );
    }

    bool DrawObject(IFerr2DTMaterial mb)
    {
        if (mb == null)
            return false;

        bool retVal = false;

        GUILayout.BeginHorizontal();
        {
            if (mb.edgeMaterial != null && mb.edgeMaterial.mainTexture != null) {
                retVal = GUILayout.Button(mb.edgeMaterial.mainTexture, GUILayout.Width(48), GUILayout.Height(48));
            } else {
                retVal = GUILayout.Button("Select",  GUILayout.Width(48), GUILayout.Height(48));
            }
            GUILayout.Label(mb.name, GUILayout.Height(48), GUILayout.Width(160f));

            GUI.color = Color.white;
        }
        GUILayout.EndHorizontal();
        return retVal;
    }

    void DrawDivider(Color aColor, float aSize = 2, float aPadding = 4) {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(0);
        EditorGUILayout.EndHorizontal();

        Rect prev = GUILayoutUtility.GetLastRect();
        prev.height = aSize;
        prev.y += aPadding;
        GUI.contentColor = aColor;
        GUILayout.BeginVertical();
        GUILayout.Space(aPadding+aSize);
        GUI.DrawTexture(prev, EditorGUIUtility.whiteTexture);
        GUILayout.Space(aPadding);
        GUILayout.EndVertical();
        GUI.contentColor = Color.white;
    }
}