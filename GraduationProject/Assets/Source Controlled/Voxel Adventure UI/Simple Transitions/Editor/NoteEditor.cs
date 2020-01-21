using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Note))]
public class NoteEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Note current = (Note)target;

        EditorGUILayout.HelpBox(current.note, MessageType.Info);

        if(current.transform.localPosition.z < 0)
            current.note = EditorGUILayout.TextField(current.note);
    }
}
