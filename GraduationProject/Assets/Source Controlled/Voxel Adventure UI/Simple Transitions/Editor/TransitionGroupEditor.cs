#define StoreVersion

using UnityEngine;
using System.Collections;
using UnityEditor;

#if(!StoreVersion)
namespace K2Framework
{
#endif
[CustomEditor(typeof(TransitionGroup))]
public class TransitionGroupEditor : Editor
{
    SerializedProperty transitionInTime, transitionTimeOperator, delay, displayTime, delayOperator, displayTimeOperator, label,
        fadeOutTime, fadeOutTimeOperator, fadeOutDelay, fadeOutDelayOperator,
        triggerInstantly;

    SerializedObject transitions;
    TransitionGroup current;

    void OnEnable()
    {
        label = serializedObject.FindProperty("label");

        transitionInTime = serializedObject.FindProperty("transitionInTime");
        transitionTimeOperator = serializedObject.FindProperty("transitionTimeOperator");

        delay = serializedObject.FindProperty("delay");
        delayOperator = serializedObject.FindProperty("delayOperator");

        displayTime = serializedObject.FindProperty("displayTime");
        displayTimeOperator = serializedObject.FindProperty("displayTimeOperator");

        fadeOutTime = serializedObject.FindProperty("fadeOutTime");
        fadeOutTimeOperator = serializedObject.FindProperty("fadeOutTimeOperator");

        fadeOutDelay = serializedObject.FindProperty("fadeOutDelay");
        fadeOutDelayOperator = serializedObject.FindProperty("fadeOutDelayOperator");

        triggerInstantly = serializedObject.FindProperty("triggerInstantly");

        transitions = new SerializedObject(((TransitionGroup)target));//handled differently because its an array
    }

    public override void OnInspectorGUI()
    {
        current = (TransitionGroup)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(label, new GUIContent("Label", "A dev feature, stripped at runtime"));

        EditorGUILayout.PropertyField(triggerInstantly, new GUIContent("Trigger Instantly"));


        GUILayout.Space(10);

        GUIHelper.ArrayGUI(transitions, "transitions");
        GUIHelper.ArrayGUI(transitions, "transitionGroups");

        #region Delay
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(delay, new GUIContent("Delay", "Use this value to override all delays. 0 will mean no valus are edited"));

        if(delay.floatValue != 0)
            EditorGUILayout.PropertyField(delayOperator, new GUIContent("Operator Type", "How to apply this delay to other transitions and groups"));
        #endregion

        #region Transition Time
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(transitionInTime, new GUIContent("Transition Time", "Use this value to override all transition times. 0 will mean no valus are edited"));

        if(transitionInTime.floatValue > 0)
            EditorGUILayout.PropertyField(transitionTimeOperator, new GUIContent("Operator Type", "How to apply this time to other transitions and groups"));
        #endregion

        #region Display Time
        GUILayout.Space(10);

        #region Check If Needs Edited
        bool editDisplayTime = false;

        if(current.transitions == null)
            current.transitions = new TransitionalObject[0];

        for(int i = 0; i < current.transitions.Length; i++)
            if(current.transitions[i] != null)// && current.transitions[i].DisplayTime >= 0)
            {
                editDisplayTime = true;
                break;
            }

        if(editDisplayTime)
            for(int i = 0; i < current.transitionGroups.Length; i++)
                for(int ii = 0; ii < current.transitionGroups[i].transitions.Length; ii++)
                    if(current.transitionGroups[i] != null)// && current.transitionGroups[i].transitions[ii].displayTime >= 0)
                    {
                        editDisplayTime = true;
                        i = current.transitionGroups.Length;//end the outer loop
                        break;
                    }
        #endregion

        if(editDisplayTime)//basically only display this if at least one of the transitions uses this varaible 
        {
            EditorGUILayout.PropertyField(displayTime, new GUIContent("Display Time", "Use this value to override all transition times. 0 will mean no valus are edited"));

            if(displayTime.floatValue != 0)
                EditorGUILayout.PropertyField(displayTimeOperator, new GUIContent("Operator Type", "How to apply this display time to other transitions and groups"));
        }
        #endregion

        #region Fade Out Delay
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(fadeOutDelay, new GUIContent("Fade Out Delay", "Use this value to override all delays. 0 will mean no valus are edited"));

        if(fadeOutDelay.floatValue != 0)
            EditorGUILayout.PropertyField(fadeOutDelayOperator, new GUIContent("Operator Type", "How to apply this delay to other transitions and groups"));
        #endregion

        #region Transition Time
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(fadeOutTime, new GUIContent("Fade Out Time", "Use this value to override all transition times. 0 will mean no valus are edited"));

        if(fadeOutTime.floatValue > 0)
            EditorGUILayout.PropertyField(fadeOutTimeOperator, new GUIContent("Operator Type", "How to apply this time to other transitions and groups"));
        #endregion

        #region Buttons
        GUILayout.Space(10);

        if(GUILayout.Button(new GUIContent("View Start")))
            for(int i = 0; i < current.transitions.Length; i++)
                if(current.transitions[i] != null)
                    current.transitions[i].ViewPosition(TransitionalObject.MovingDataType.StartPoint);

        if(GUILayout.Button(new GUIContent("View End")))
            for(int i = 0; i < current.transitions.Length; i++)
                if(current.transitions[i] != null)
                    current.transitions[i].ViewPosition(TransitionalObject.MovingDataType.EndPoint);
        #endregion

        if(GUI.changed)
            EditorUtility.SetDirty(target);

        serializedObject.ApplyModifiedProperties();// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
    }
}
#if(!StoreVersion)
}
#endif