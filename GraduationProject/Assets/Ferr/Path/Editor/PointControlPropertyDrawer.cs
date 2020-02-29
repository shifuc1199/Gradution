using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ferr {
    [CustomPropertyDrawer(typeof(PointControl), true)]
    public class PointControlPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            Rect curr = position;
            curr.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty radius      = property.FindPropertyRelative("radius");
            SerializedProperty controlNext = property.FindPropertyRelative("controlNext");
			SerializedProperty controlPrev = property.FindPropertyRelative("controlPrev");
            SerializedProperty type        = property.FindPropertyRelative("type");

			float step = curr.width/5;
			float width = step;
			if (type.enumValueIndex == (int)PointType.Sharp)
				width = step * 3;
			type.enumValueIndex = (int)(PointType)EditorGUI.EnumPopup(new Rect(curr.x, curr.y, width, curr.height), (PointType)type.enumValueIndex);
			
			if (type.enumValueIndex == (int)PointType.Auto || type.enumValueIndex == (int)PointType.AutoSymmetrical )
				EditorGUI.LabelField(new Rect(curr.x+step, curr.y, step*2, curr.height), controlPrev.vector2Value.ToString());

			if (type.enumValueIndex == (int)PointType.Auto || type.enumValueIndex == (int)PointType.AutoSymmetrical || type.enumValueIndex == (int)PointType.Locked )
				EditorGUI.LabelField(new Rect(curr.x+step*3, curr.y, step*2, curr.height), controlNext.vector2Value.ToString());
			

			if (type.enumValueIndex == (int)PointType.Free || type.enumValueIndex == (int)PointType.Locked)
				controlPrev.vector2Value = EditorGUI.Vector2Field(new Rect(curr.x+step, curr.y, step*2, curr.height), "", controlPrev.vector2Value);

			if (type.enumValueIndex == (int)PointType.Free )
				controlNext.vector2Value = EditorGUI.Vector2Field(new Rect(curr.x+step*3, curr.y, step*2, curr.height), "", controlNext.vector2Value);

			if (type.enumValueIndex == (int)PointType.CircleCorner )
				radius.floatValue = EditorGUI.FloatField(new Rect(curr.x+step, curr.y, step*2, curr.height), "", radius.floatValue);
			
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * 1;
        }
    }
}