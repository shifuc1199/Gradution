using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Ferr {
	[CustomPropertyDrawer(typeof(InspectorName))]
	public class InspectorNameDrawer : PropertyDrawer {
	    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
	        EditorGUI.PropertyField(position, property, new GUIContent(((InspectorName)attribute).mName));
	    }
	}
}