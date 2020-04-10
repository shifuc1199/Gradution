/*****************************
Created by 师鸿博
*****************************/
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(DreamerButton))]
public class DreamerButtonEditor : ButtonEditor
{
    private SerializedProperty on_release;
    private SerializedProperty on_down;
    protected override void OnEnable()
    {
        base.OnEnable();
        on_release = serializedObject.FindProperty("OnRelease");
        on_down = serializedObject.FindProperty("OnDown");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        EditorGUILayout.PropertyField(on_release);
        EditorGUILayout.PropertyField(on_down);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif