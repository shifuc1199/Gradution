/*****************************
Created by 师鸿博
*****************************/
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(ToggleButton))]
public class ToggleButtonEditor : ToggleEditor
{
    private SerializedProperty yes_event;
    private SerializedProperty no_event;
    protected override void OnEnable()
    {
        base.OnEnable();
        yes_event = serializedObject.FindProperty("YesEvent");
        no_event = serializedObject.FindProperty("NoEvent");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        serializedObject.Update();
        EditorGUILayout.PropertyField(yes_event);
        EditorGUILayout.PropertyField(no_event);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif