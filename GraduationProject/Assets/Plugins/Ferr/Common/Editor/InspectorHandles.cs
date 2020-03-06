using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ferr {
	public static class InspectorHandles {
		static Vector2 currMousePos;
		static Vector3 startPos;
		static Vector2 startMousePos;

		public static void DrawGrid(Rect gui, Rect aRange, float aIncrements = 0.25f) {
			Handles.color = new Color(1, 1, 1, 0.25f);
			float startx = (int)(aRange.xMin / aIncrements) * aIncrements;
			while (startx < aRange.xMax) {
				Handles.DrawLine(ToGUIPos(gui, aRange, new Vector2(startx, aRange.yMin)), ToGUIPos(gui, aRange, new Vector2(startx, aRange.yMax)));
				startx += aIncrements;
			}
			float starty = (int)(aRange.yMin / aIncrements) * aIncrements;
			while (starty < aRange.yMax) {
				Handles.DrawLine(ToGUIPos(gui, aRange, new Vector2(aRange.xMin, starty)), ToGUIPos(gui, aRange, new Vector2(aRange.xMax, starty)));
				starty += aIncrements;
			}
			Handles.color = new Color(1, 1, 1, 1);
			Handles.DrawLine(ToGUIPos(gui, aRange, new Vector2(0, aRange.yMax)), ToGUIPos(gui, aRange, new Vector2(0, aRange.yMin)));
			Handles.DrawLine(ToGUIPos(gui, aRange, new Vector2(aRange.xMin, 0)), ToGUIPos(gui, aRange, new Vector2(aRange.xMax, 0)));
		}
		public static Vector3 ToGUIPos(Rect gui, Rect aRange, Vector2 aPt) {
			float px = (aPt.x - aRange.xMin) / aRange.width;
			float py = 1-(aPt.y - aRange.yMin) / aRange.height;
			return new Vector3(gui.xMin + px * gui.width, (gui.yMin + py * gui.height), 0);
		}
		public static Vector3 FromGUIPos(Rect gui, Rect aRange, Vector2 aPt) {
			float px = (aPt.x - gui.xMin) / gui.width;
			float py = 1-(aPt.y - gui.yMin) / gui.height;
			Vector3 result = new Vector3(aRange.xMin + px * aRange.width, (aRange.yMin + py * aRange.height), 0);
			result.x = (float)System.Math.Round(result.x, 5);
			result.y = (float)System.Math.Round(result.y, 5);
			result.z = (float)System.Math.Round(result.z, 5);
			return result;
		}

		public static Vector3 FreeMove(Vector3 position, float size, Handles.CapFunction handleFunction) {
			int       id        = GUIUtility.GetControlID("InspectorFreeMoveHash".GetHashCode(), FocusType.Keyboard);
			Vector3   position2 = Handles.matrix.MultiplyPoint(position);
			Matrix4x4 matrix    = Handles.matrix;
			Event current = Event.current;
			
			switch (current.GetTypeForControl(id)) {
				case EventType.MouseDown:
					if (HandleUtility.DistanceToRectangle(position, Quaternion.identity, size) < size && current.button == 0) {
						GUIUtility.keyboardControl = id;
						GUIUtility.hotControl      = id;
						currMousePos = (startMousePos = current.mousePosition);
						startPos     = position;
						current.Use();
						EditorGUIUtility.SetWantsMouseJumping(1);
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == id && (current.button == 0)) {
						GUIUtility.hotControl = 0;
						current.Use();
						EditorGUIUtility.SetWantsMouseJumping(0);
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == id) {
						currMousePos += new Vector2(current.delta.x, current.delta.y);
						position = startPos + (Vector3)(currMousePos - startMousePos);

						GUI.changed = true;
						current.Use();
					}
					break;
				case EventType.Repaint: {
						Color color = Color.white;
						if (id == GUIUtility.keyboardControl) {
							color = Handles.color;
							Handles.color = Handles.selectedColor;
						}
						Handles.matrix = Matrix4x4.identity;
						handleFunction(id, position2, Quaternion.identity, size, EventType.Repaint);
						Handles.matrix = matrix;
						if (id == GUIUtility.keyboardControl) {
							Handles.color = color;
						}
						break;
					}
				case EventType.Layout:
					Handles.matrix = Matrix4x4.identity;
					handleFunction(id, position2, Quaternion.identity, size, EventType.Layout);
					Handles.matrix = matrix;
					break;
			}
			return position;
		}
		public static bool Button(Vector3 position, float size, float pickSize, Handles.CapFunction capFunction) {
			int   id      = GUIUtility.GetControlID("InspectorButtonHandleHash".GetHashCode(), FocusType.Keyboard);
			Event current = Event.current;
			bool  result;

			switch (current.GetTypeForControl(id)) {
				case EventType.MouseDown:
					if (HandleUtility.DistanceToRectangle(position, Quaternion.identity, size) < size && (current.button == 0 || current.button == 2)) {
						GUIUtility.hotControl = id;
						current.Use();
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == id && HandleUtility.DistanceToRectangle(position, Quaternion.identity, size) < size  && (current.button == 0 || current.button == 2)) {
						GUIUtility.hotControl = 0;
						current.Use();

						result = true;
						return result;
					}
					break;
				case EventType.MouseMove:
					if ((GUIUtility.hotControl == id && current.button == 0) || (GUIUtility.keyboardControl == id && current.button == 2)) {
						HandleUtility.Repaint();
					}
					break;
				case EventType.Repaint: {
						Color color = Handles.color;
						if (GUIUtility.hotControl == id && GUI.enabled) {
							Handles.color = Handles.selectedColor;
						}
						capFunction(id, position, Quaternion.identity, size, EventType.Repaint);
						Handles.color = color;
						break;
					}
				case EventType.Layout:
					if (GUI.enabled) {
						capFunction(id, position, Quaternion.identity, pickSize, EventType.Layout);
					}
					break;
			}

			result = false;
			return result;
		}
		public static Rect GridScroller(Rect position, Rect guiArea) {
			int       id      = GUIUtility.GetControlID("GridScrollerHash".GetHashCode(), FocusType.Keyboard);
			Matrix4x4 matrix  = Handles.matrix;
			Event     current = Event.current;
			
			switch (current.GetTypeForControl(id)) {
				case EventType.MouseDown:
					if (guiArea.Contains(current.mousePosition) && (current.button == 2 || current.button == 1)) {
						GUIUtility.keyboardControl = id;
						GUIUtility.hotControl      = id;
						currMousePos = (startMousePos = current.mousePosition);
						startPos     = position.center;
						current.Use();
						EditorGUIUtility.SetWantsMouseJumping(1);
					}
					break;
				case EventType.MouseUp:
					if (GUIUtility.hotControl == id && (current.button == 2 || current.button == 1)) {
						GUIUtility.hotControl = 0;
						current.Use();
						EditorGUIUtility.SetWantsMouseJumping(0);
					}
					break;
				case EventType.MouseDrag:
					if (GUIUtility.hotControl == id) {
						currMousePos += new Vector2(current.delta.x, -current.delta.y);
						Vector3 change = (Vector3)(currMousePos - startMousePos);
						change = Vector3.Scale(change, new Vector3(position.width/guiArea.width, position.height/guiArea.height,0));
						position.center = startPos - change;

						GUI.changed = true;
						current.Use();
					}
					break;
				case EventType.ScrollWheel:
					if (guiArea.Contains(current.mousePosition)) {
						float delta = current.delta.y * 0.05f;
						position.width  = position.width  * (1 + delta);
						position.height = position.height * (1 + delta);
						position.x -= position.width * delta * 0.5f;
						position.y -= position.height * delta * 0.5f;
					}
					break;
				case EventType.Repaint: {
						break;
					}
				case EventType.Layout:
					break;
			}
			return position;
		}
	}
}