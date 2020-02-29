using UnityEditor;
using UnityEngine;

namespace Ferr {
	[CustomPropertyDrawer(typeof(ClampAttribute))]
	public class ClampDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			ClampAttribute clamp = (ClampAttribute)base.attribute;
			
			if (property.propertyType == SerializedPropertyType.Float) {
				
				EditorGUI.BeginChangeCheck();
				float result = EditorGUI.FloatField(position, label, property.floatValue);
				if (EditorGUI.EndChangeCheck())
					property.floatValue = Mathf.Clamp(result, clamp.mMin, clamp.mMax);
				
			} else if (property.propertyType == SerializedPropertyType.Integer) {
				
				EditorGUI.BeginChangeCheck();
				int result = EditorGUI.IntField(position, label, property.intValue);
				if (EditorGUI.EndChangeCheck())
					property.intValue = Mathf.Clamp(result, (int)clamp.mMin, (int)clamp.mMax);
				
			} else {
				EditorGUI.LabelField(position, label.text, "Use Clamp with float or int.");
			}
		}
	}
}