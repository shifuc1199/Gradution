using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ferr {
    [CustomPropertyDrawer(typeof(Path2D), true)]
    public class Path2DPropertyDrawer : PropertyDrawer {
        bool _fold;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            Rect curr = position;
            curr.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty points        = property.FindPropertyRelative("_points");
            SerializedProperty closed        = property.FindPropertyRelative("_closed");
            SerializedProperty splitDistance = property.FindPropertyRelative("_splitDistance");
			
            Rect remainder = EditorGUI.PrefixLabel(curr,  new GUIContent(" "));
			remainder.x -= curr.x-4;
            _fold = EditorGUI.Foldout(
				new Rect(curr.x, curr.y, curr.width-remainder.width, curr.height), 
				_fold, string.Format("{0}: {1}.", property.displayName, points.arraySize));
			
            if (_fold) {
                EditorGUI.indentLevel += 1;
				curr.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(curr, closed);
                curr.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(curr, splitDistance);
                curr.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.indentLevel -= 1;
            } else {
				closed.boolValue = EditorGUI.ToggleLeft(
					new Rect(remainder.x, remainder.y, 50, remainder.height),
					new GUIContent(closed.boolValue?"O":"C"), closed.boolValue);
				splitDistance.floatValue = EditorGUI.FloatField(
					new Rect(remainder.x + 40, remainder.y, 50, remainder.height),
					splitDistance.floatValue);
			}
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (!_fold)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 3;
        }
    }
}