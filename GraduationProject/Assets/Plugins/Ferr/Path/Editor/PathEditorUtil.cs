using System;
using System.Collections.Generic;
using Ferr.Extensions;
using UnityEditor;
using UnityEngine;

namespace Ferr {
	public class PathEditorVisuals {
		public Color colorControlLine  = new Color(1,1,1,0.35f);
		public Color colorHandle       = new Color(1, 1, 1, 0.6f);
		public Color colorHandleDelete = new Color(1, 0, 0, 1f);
		public Color colorLine         = new Color(1,.6f,.6f,1);
		public Color colorSelectionTint   = new Color(.8f,1,.8f,1);
		public Color colorUnselectedTint  = new Color(.5f,.5f,.5f,1);
		public Color colorNoSelectionTint = new Color(1,1,1,1);
		public Color colorDragSelectInner = new Color(0, 0.5f, 0.25f, 0.25f);
		public Color colorDragSelectOuter = new Color(0, 0.5f, 0.25f, 0.25f);
		
		public Handles.CapFunction capVertex        = Handles.CircleHandleCap;
		public Handles.CapFunction capVertexMode    = Handles.CircleHandleCap;
		public Handles.CapFunction capVertexAdd     = EditorTools.EmptyCap;
		public Handles.CapFunction capVertexDelete  = Handles.CircleHandleCap;
		public Handles.CapFunction capControlHandle = Handles.CircleHandleCap;
		public Handles.CapFunction[] capVertexTypes = null;
		
		public float sizeVertex        = .1f;
		public float sizeVertexDelete  = .1f;
		public float sizeControlHandle = .05f;
		public float sizeVertexMode    = .1f;
		public float sizeVertexAdd     = .1f;
	}
	public enum PathSnap {
		Unity    = 0,
		Relative = 0,
		Local    = 1,
		World    = 2
	}

	public static class PathEditorUtil {
		#region Fields
		static PathEditorVisuals _defaultVisuals = new PathEditorVisuals();
		static Color _activeHandleTint;
		static bool  _vMode = false;
		static int   _recentInteract = -1;
		static GUIStyle _labelStyle;
		static GUIStyle _shadowStyle;
		static GUIStyle _centeredLabelStyle;
		static GUIStyle _centeredShadowStyle;

		public static GUIStyle LabelStyle { get{ 
				if (_labelStyle == null) {
					_labelStyle = new GUIStyle(GUI.skin.label);
					_labelStyle.normal.textColor = Color.white;
				}
				return _labelStyle;
			} }
		public static GUIStyle ShadowStyle { get{ 
				if (_shadowStyle == null) {
					_shadowStyle = new GUIStyle(GUI.skin.label);
					_shadowStyle.normal.textColor = new Color(0,0,0,0.5f);
					_shadowStyle.contentOffset = new Vector2(1,1);
				}
				return _shadowStyle;
			} }
		public static GUIStyle CenteredLabelStyle { get{ 
				if (_centeredLabelStyle == null) {
					_centeredLabelStyle = new GUIStyle(GUI.skin.label);
					_centeredLabelStyle.normal.textColor = Color.white;
					_centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
				}
				return _centeredLabelStyle;
			} }
		public static GUIStyle CenteredShadowStyle { get{ 
				if (_centeredShadowStyle == null) {
					_centeredShadowStyle = new GUIStyle(GUI.skin.label);
					_centeredShadowStyle.normal.textColor = new Color(0,0,0,0.5f);
					_centeredShadowStyle.contentOffset = new Vector2(1,1);
					_centeredShadowStyle.alignment = TextAnchor.MiddleCenter;
				}
				return _centeredShadowStyle;
			} }
		#endregion

		public static void OnSceneGUIEasy(SerializedProperty aPath, Path2D.Plane aPlane = Path2D.Plane.XY, PathSnap aSnapMode = PathSnap.Unity, float aSmartSnapDist = 0, PathEditorVisuals aVisuals = null) {
			SerializedObject obj = aPath.serializedObject;
			Transform        t   = ((Component)obj.targetObject).transform;
			Path2D           raw = EditorTools.GetTargetObjectOfProperty(aPath) as Path2D;
			
			OnSceneGUI(t.localToWorldMatrix, t.worldToLocalMatrix, aPath, raw, true, null, null, aPlane, aSnapMode, aSmartSnapDist, KeyCode.C, aVisuals);
			DoDragSelect(t.localToWorldMatrix, raw, new Rect(0,0,Screen.width,Screen.height), aVisuals);
			obj.ApplyModifiedProperties();
		}
		public static void OnSceneGUI(Matrix4x4 aTransform, Matrix4x4 aInvTransform, SerializedProperty aPath, Path2D aPathRaw, bool aShowShiftAdd = true, Action<SerializedProperty, int> aOnAddPoint = null,  Action<SerializedProperty, int> aOnRemovePoint = null, Path2D.Plane aPlane = Path2D.Plane.XY, PathSnap aSnapMode = PathSnap.Unity, float aSmartSnapDist = 0, KeyCode aVertModeKey = KeyCode.C, PathEditorVisuals aVisuals = null) {
			if (aVisuals == null)
				aVisuals = _defaultVisuals;

			// get the current selection list
			Selection selection = GetSelection(aPathRaw);

			bool showControls = !Event.current.shift && !Event.current.alt;

			// check for point type switching mode
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == aVertModeKey) {
				_vMode = true;
				Event.current.Use();
			}
			if (Event.current.type == EventType.KeyUp && Event.current.keyCode == aVertModeKey) {
				_vMode = false;
				Event.current.Use();
			}
			bool deleteMode = Event.current.alt;
			
			// draw all the curve and handle lines
			if (Event.current.type == EventType.Repaint)
				ShowPathLines(aPathRaw, aTransform, showControls, aPlane, aVisuals);

			// create the plane on which all edits occur
			Plane editPlane;
			if (aPlane == Path2D.Plane.XY)
				editPlane = new Plane(aTransform.MultiplyPoint3x4(Vector3.zero), aTransform.MultiplyPoint3x4(Vector3.right), aTransform.MultiplyPoint3x4(Vector3.up));
			else
				editPlane = new Plane(aTransform.MultiplyPoint3x4(Vector3.zero), aTransform.MultiplyPoint3x4(Vector3.right), aTransform.MultiplyPoint3x4(Vector3.forward));
			
			// check for shift-add point to path
			if (Event.current.shift && aShowShiftAdd && Event.current.button != 1 && !Event.current.control) {
				ShowShiftAdd(aPathRaw, aTransform, aInvTransform, aPath, aOnAddPoint, aPlane, editPlane, aVisuals);
			}
			
			// draw the handles for each point
			for (int i = 0; i < aPathRaw.Count; i++) {
				if (selection.Count > 0)
					_activeHandleTint = selection.IsSelected(i) ? aVisuals.colorSelectionTint : aVisuals.colorUnselectedTint;
				else
					_activeHandleTint = aVisuals.colorNoSelectionTint;

				if (deleteMode) {
					ShowPointDeleteMode(aPathRaw, i, selection, aTransform, aPath, aOnRemovePoint, aPlane, aVisuals);
				} else if (_vMode) {
					ShowPointTypeMode  (aPathRaw, i, selection, aTransform, aInvTransform, aPath, aPlane, aVisuals);
				} else {
					ShowPoint          (aPathRaw, i, selection, aTransform, aInvTransform, aPath, aPlane, editPlane, aSnapMode, aSmartSnapDist, aVisuals);
				}
				if (showControls) {
					ShowHandles        (aPathRaw, i, aTransform, aInvTransform, aPath, i==_recentInteract, _vMode, aPlane, editPlane, aVisuals);
				}
			}
			
			if (GUI.changed)
				SetDirty(aPath);

			Handles.color = Color.white;
		}

		#region Selection Code
		public class Selection {
			public List<int> ids = new List<int>();
			public int Count { get { return ids.Count; } }

			public bool IsSelected       (int aIndex) {
				if (ids.Count == 0)
					return false;
				return ids.BinarySearch(aIndex) >= 0;
			}
			public bool IsSegmentSelected(int aSegmentIndex, int aPointCount, bool aClosed) {
				int start = ids.BinarySearch(aSegmentIndex);
				if (start < 0)
					return false;

				int next  = (start + 1) % ids.Count;
				if (ids[start]+1 == ids[next] || ( aClosed && ids[start] == aPointCount-1 && ids[next] == 0 ))
					return true;
				return false;
			}
			public void Each             (int aEnsureId, Action<int> aEachIndex) {
				if (ids.BinarySearch(aEnsureId)<0)
					ids.Clear();
				if (ids.Count == 0)
					aEachIndex(aEnsureId);
				for (int i = ids.Count-1; i >= 0; i--) {
					aEachIndex(ids[i]);
				}
			}
			public void EachSegment      (int aEnsureId, int aPointCount, bool aClosed, Action<int> aEachSegIndex) {
				if (ids.BinarySearch(aEnsureId)<0)
					ids.Clear();
				if (ids.Count == 0)
					aEachSegIndex(aEnsureId);
				for (int i = ids.Count-1; i >= 0; i--) {
					if (IsSegmentSelected(ids[i], aPointCount, aClosed))
						aEachSegIndex(ids[i]);
				}
			}
		}
		static bool                          _isMouseDown;
		static Vector2                       _mouseDownPos;
		static Dictionary<Path2D, Selection> _selections = new Dictionary<Path2D, Selection>();

		public static void      DoDragSelect(Matrix4x4 aTransform, Path2D aPath, Rect aValidDragArea, PathEditorVisuals aVisuals) {
			if (aVisuals == null)
				aVisuals = _defaultVisuals;
			int   id   = GUIUtility.GetControlID("PathEditorDragSelect".GetHashCode(), FocusType.Keyboard);
			Event curr = Event.current;

			// get the current selection list
			Selection selection = GetSelection(aPath);
			Vector2   mouse     = curr.mousePosition;
			EventType type      = curr.GetTypeForControl(id);
			switch (type) {
				case EventType.MouseDown:
					if (curr.button == 0 && !curr.alt && !curr.control && aValidDragArea.Contains(mouse)) {
						GUIUtility.hotControl = id;
						_isMouseDown  = true;
						_mouseDownPos = curr.mousePosition;
						curr.Use();
					}
					break;
				case EventType.MouseUp:
					if (curr.button == 0)
						_isMouseDown  = false;
					
					if (curr.button == 0 && GUIUtility.hotControl == id && curr.mousePosition != _mouseDownPos) {
						selection.ids.Clear();
						
						for	(int i=0;i<aPath.Count;i+=1) {
							float left   = Mathf.Min(_mouseDownPos.x, _mouseDownPos.x + (mouse.x - _mouseDownPos.x));
							float right  = Mathf.Max(_mouseDownPos.x, _mouseDownPos.x + (mouse.x - _mouseDownPos.x));
							float top    = Mathf.Min(_mouseDownPos.y, _mouseDownPos.y + (mouse.y - _mouseDownPos.y));
							float bottom = Mathf.Max(_mouseDownPos.y, _mouseDownPos.y + (mouse.y - _mouseDownPos.y));
					
							Rect r = new Rect(left, top, right-left, bottom-top);
							if (r.Contains(HandleUtility.WorldToGUIPoint(aTransform.MultiplyPoint( aPath[i]) ) )) {
								selection.ids.Add(i);
							}
						}
					}

					if (curr.button == 0 && GUIUtility.hotControl == id) { 
						GUIUtility.hotControl = 0;
						curr.Use();
					}

					// if the mouse hasn't moved, emulate Unity's default mouse behaviour
					if (curr.mousePosition == _mouseDownPos) {
						if (selection.Count > 0) {
							selection.ids.Clear();
							SceneView.RepaintAll();
						} else {
							GameObject go = HandleUtility.PickGameObject(curr.mousePosition, false);
							UnityEditor.Selection.activeGameObject = go;
						}
					}
					break;
				case EventType.MouseDrag:
					if (_isMouseDown && GUIUtility.hotControl == id) {
						SceneView.RepaintAll();
						GUI.changed = true;
						curr.Use();
					}
					break;
				case EventType.Repaint:
					if (curr.button == 0 && !curr.alt && !curr.control && _isMouseDown) {
						Vector3 pt1 = HandleUtility.GUIPointToWorldRay(_mouseDownPos).GetPoint(0.2f);
						Vector3 pt2 = HandleUtility.GUIPointToWorldRay(mouse).GetPoint(0.2f);
						Vector3 pt3 = HandleUtility.GUIPointToWorldRay(new Vector2(_mouseDownPos.x, mouse.y)).GetPoint(0.2f);
						Vector3 pt4 = HandleUtility.GUIPointToWorldRay(new Vector2(mouse.x, _mouseDownPos.y)).GetPoint(0.2f);
						Handles.DrawSolidRectangleWithOutline(new Vector3[] { pt1, pt3, pt2, pt4 }, aVisuals.colorDragSelectInner, aVisuals.colorDragSelectOuter);
					}
					break;
			}
		}
		public static Selection GetSelection(Path2D aPath) {
			Selection selection;
			if (!_selections.TryGetValue(aPath, out selection)) {
				selection = new Selection();
				_selections[aPath] = selection;
			}
			return selection;
		}
		public static void      ClearSelections() {
			_selections.Clear();
		}
		#endregion

		#region GUI Functions
		private static void ShowShiftAdd       (Path2D aPathRaw, Matrix4x4 aTransform, Matrix4x4 aInvTransform, SerializedProperty aPath, Action<SerializedProperty, int> aOnAddPoint, Path2D.Plane aPlane, Plane aEditPlane, PathEditorVisuals aVisuals) {
			if (Event.current.type == EventType.MouseMove)
				SceneView.RepaintAll();

			Ray   r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			float dist = 0;
			
			if (aEditPlane.Raycast(r, out dist)) {
				Vector3       worldPt = r.GetPoint(dist);
				Vector3       localPt = aInvTransform.MultiplyPoint3x4(worldPt);
				List<Vector2> points  = aPathRaw.GetPathRaw();
				int           segment = PathUtil.GetClosestSegment(points, Deplane(localPt, aPlane), aPathRaw.Closed);

				Vector3 p1 = points.Count == 0 ? worldPt : aTransform.MultiplyPoint3x4(Plane(points[segment], aPlane));
				Vector3 p2 = points.Count == 0 ? worldPt : aTransform.MultiplyPoint3x4(Plane(points[PathUtil.WrapIndex(segment+1, points.Count, aPathRaw.Closed)], aPlane));
				float   d1 = Vector3.Distance(p1, worldPt);
				float   d2 = Vector3.Distance(p2, worldPt);
				int insertIndex = 0;

				EditorGUIUtility.AddCursorRect(new Rect(0,0,Screen.width, Screen.height), MouseCursor.ArrowPlus);

				// draw dotted lines, and figure out what index we'll be adding to
				if (!aPathRaw.Closed && segment == 0 && d1 < d2) {
					Handles.DrawDottedLine(p1, worldPt, 4);
					insertIndex = 0;
				} else if (!aPathRaw.Closed && segment == points.Count-2 && d2 < d1) {
					Handles.DrawDottedLine(p2, worldPt, 4);
					insertIndex = points.Count;
				} else {
					Handles.DrawDottedLine(p1, worldPt, 4);
					Handles.DrawDottedLine(p2, worldPt, 4);
					insertIndex = segment+1;
				}

				// do the add point button
				if (Handles.Button(worldPt,
						SceneView.lastActiveSceneView.camera.transform.rotation,
						HandleUtility.GetHandleSize(worldPt) * aVisuals.sizeVertexAdd,
						HandleUtility.GetHandleSize(worldPt) * aVisuals.sizeVertexAdd,
						aVisuals.capVertexAdd)) {

					AddPoint(aPath, Deplane(localPt, aPlane), insertIndex);
					if (aOnAddPoint != null) {
						aOnAddPoint(aPath, insertIndex);
					}
					
					_recentInteract = insertIndex;
					GUI.changed = true;
				}
			}
		}
		public  static void ShowPathLines      (Path2D aPathRaw, Matrix4x4 aTransform, bool aShowControls, Path2D.Plane aPlane, PathEditorVisuals aVisuals) {
			if (aShowControls) {
				Handles.color = aVisuals.colorControlLine;
				for (int i = 0; i < aPathRaw.Count; i++) {
					PointControl c  = aPathRaw.GetControls(i);
					Vector3      pt = aTransform.MultiplyPoint3x4(Plane(aPathRaw[i], aPlane));

					if (c.type == PointType.Auto || c.type == PointType.AutoSymmetrical)
						Handles.DrawDottedLines(new Vector3[] { pt + aTransform.MultiplyVector(Plane(c.controlPrev, aPlane)), pt, pt, pt+aTransform.MultiplyVector(Plane(c.controlNext, aPlane)) }, 4);
					else if (c.type == PointType.Free)
						Handles.DrawPolyLine(pt + aTransform.MultiplyVector(Plane(c.controlPrev, aPlane)), pt, pt+aTransform.MultiplyVector(Plane(c.controlNext, aPlane)));
					else if (c.type == PointType.Locked)
						Handles.DrawPolyLine(pt + aTransform.MultiplyVector(Plane(c.controlPrev, aPlane)), pt + aTransform.MultiplyVector(Plane(-c.controlPrev, aPlane)));
					else if (c.type == PointType.CircleCorner) {
						Vector3 end1 =  PathUtil.GetRoundedCornerEnd(i, aPathRaw.GetPathRaw(), aPathRaw.GetControls(), aPathRaw.Closed, c.radius, true);
						Handles.DrawDottedLine(pt, aTransform.MultiplyPoint(Plane(end1, aPlane)), 4);
						Vector3 end2 = PathUtil.GetRoundedCornerEnd(i, aPathRaw.GetPathRaw(), aPathRaw.GetControls(), aPathRaw.Closed, c.radius, false);
						Handles.DrawDottedLine(pt, aTransform.MultiplyPoint(Plane(end2, aPlane)), 4);

						Vector3 center = Vector2.Lerp(end1.xy()-aPathRaw[i], end2.xy()-aPathRaw[i],0.5f).normalized * (c.radius + 0.2f);
						Handles.DrawLine(pt, pt + aTransform.MultiplyVector(Plane(center, aPlane)));
					}
				}
			}

			Handles.color = aVisuals.colorLine;
			Handles.matrix = aTransform;
			EditorTools.DrawPolyLine(aPathRaw.GetFinalPath(), aPathRaw.Closed);
			Handles.matrix = Matrix4x4.identity;
		}
		private static void ShowPoint          (Path2D aPathRaw, int aIndex, Selection aSelection, Matrix4x4 aTransform, Matrix4x4 aInvTransform, SerializedProperty aPath, Path2D.Plane aPlane, Plane aEditPlane, PathSnap aSnapMode, float aSmartSnapDist, PathEditorVisuals aVisuals) {
			Handles.color = aVisuals.colorHandle * _activeHandleTint;
			Handles.CapFunction cap = aVisuals.capVertex;
			if (aVisuals.capVertexTypes != null)
				cap = aVisuals.capVertexTypes[(int)aPathRaw.GetControls(aIndex).type];

			Vector3 v  = aTransform.MultiplyPoint3x4(Plane(aPathRaw[aIndex], aPlane));
			Vector3 nV = Handles.FreeMoveHandle(v,
				SceneView.lastActiveSceneView.camera.transform.rotation,
				HandleUtility.GetHandleSize(v) * aVisuals.sizeVertex,
				Vector3.zero,
				cap);

			if (nV != v) {
				nV = EditorTools.ProjectPoint(nV, aEditPlane);
				nV = aInvTransform.MultiplyPoint3x4(nV);
				Vector2 newPos = Deplane(nV, aPlane);
				newPos = Snap(aPathRaw[aIndex], newPos, aTransform, aInvTransform, aSnapMode);
				if (!Event.current.control && aSmartSnapDist != 0)
					newPos = SmartSnap(newPos, aPathRaw, aIndex, aSelection.ids, aSmartSnapDist);

				Vector2 delta = newPos - aPathRaw[aIndex];
				aSelection.Each(aIndex, i => aPath.FindPropertyRelative("_points").GetArrayElementAtIndex(i).vector2Value += delta);
				
				GUI.changed = true;
				_recentInteract = aIndex;
			}
		}
		private static void ShowPointTypeMode  (Path2D aPathRaw, int aIndex, Selection aSelection, Matrix4x4 aTransform, Matrix4x4 aInvTransform, SerializedProperty aPath, Path2D.Plane aPlane, PathEditorVisuals aVisuals) {
			Handles.color = aVisuals.colorHandle * _activeHandleTint;
			Handles.CapFunction cap = aVisuals.capVertex;
			if (aVisuals.capVertexTypes != null)
				cap = aVisuals.capVertexTypes[(int)aPathRaw.GetControls(aIndex).type];
			Vector3 v = aTransform.MultiplyPoint3x4(Plane(aPathRaw[aIndex], aPlane));

			EditorGUIUtility.AddCursorRect(new Rect(0,0,Screen.width, Screen.height), MouseCursor.RotateArrow);
			if (Handles.Button(v,
				SceneView.lastActiveSceneView.camera.transform.rotation,
				HandleUtility.GetHandleSize(v) * aVisuals.sizeVertexMode,
				HandleUtility.GetHandleSize(v) * aVisuals.sizeVertexMode,
				cap)) {
				
				PointType t = aPathRaw.GetControls(aIndex).type;
				if (t == PointType.Auto)
					t = PointType.Locked;
				else if (t == PointType.Free)
					t = PointType.Locked;
				else if (t == PointType.Locked)
					t = PointType.CircleCorner;
				else if (t == PointType.CircleCorner)
					t = PointType.Sharp;
				else if (t == PointType.Sharp)
					t = PointType.Auto;

				SerializedProperty pointControls = aPath.FindPropertyRelative("_pointControls");
				aSelection.Each(aIndex, i => pointControls.GetArrayElementAtIndex(i).FindPropertyRelative("type").enumValueIndex=(int)t);
				
				_recentInteract = aIndex;
				GUI.changed = true;
			}
		}
		private static void ShowPointDeleteMode(Path2D aPathRaw, int aIndex, Selection aSelection, Matrix4x4 aTransform, SerializedProperty aPath, Action<SerializedProperty, int> aOnRemovePoint, Path2D.Plane aPlane, PathEditorVisuals aVisuals) {
			Handles.color = aVisuals.colorHandleDelete * _activeHandleTint;
			Vector3 aAt = aTransform.MultiplyPoint3x4(Plane(aPathRaw[aIndex], aPlane));

			if (Handles.Button(aAt,
				SceneView.lastActiveSceneView.camera.transform.rotation,
				HandleUtility.GetHandleSize(aAt) * aVisuals.sizeVertexDelete,
				HandleUtility.GetHandleSize(aAt) * aVisuals.sizeVertexDelete,
				aVisuals.capVertexDelete)) {

				SerializedProperty points   = aPath.FindPropertyRelative("_points");
				SerializedProperty controls = aPath.FindPropertyRelative("_pointControls");
				aSelection.Each(aIndex, i => {
					points  .DeleteArrayElementAtIndex(i);
					controls.DeleteArrayElementAtIndex(i);
					if (aOnRemovePoint != null)
						aOnRemovePoint(aPath, i);
				});
				aSelection.ids.Clear();
				
				if (_recentInteract == aIndex)
					_recentInteract = -1;
				GUI.changed = true;
			}
		}
		private static void ShowHandles        (Path2D aPathRaw, int aIndex, Matrix4x4 aTransform, Matrix4x4 aInvTransform, SerializedProperty aPath, bool aShowMeta, bool aUnlock, Path2D.Plane aPlane, Plane aEditPlane, PathEditorVisuals aVisuals) {
			PointControl ctrl   = aPathRaw.GetControls(aIndex);
			bool         locked = ctrl.type == PointType.Locked;
			Vector3      at     = aTransform.MultiplyPoint3x4(Plane(aPathRaw[aIndex], aPlane));

			if (ctrl.type == PointType.Auto || ctrl.type == PointType.AutoSymmetrical || ctrl.type == PointType.Sharp) {

			} else if (ctrl.type == PointType.CircleCorner) {
				if (aPathRaw.Closed || (aIndex != 0 && aIndex != aPathRaw.Count-1)) {
					float size         = HandleUtility.GetHandleSize(at);
					float radiusOffset = .2f;

					Vector3 end1 = PathUtil.GetRoundedCornerEnd(aIndex, aPathRaw.GetPathRaw(), aPathRaw.GetControls(), aPathRaw.Closed, ctrl.radius, true);
					Vector3 end2 = PathUtil.GetRoundedCornerEnd(aIndex, aPathRaw.GetPathRaw(), aPathRaw.GetControls(), aPathRaw.Closed, ctrl.radius, false);
					Vector3 normal = Vector2.Lerp(end1.xy()-aPathRaw[aIndex], end2.xy()-aPathRaw[aIndex],0.5f).normalized;
				
					Vector3 v = aTransform.MultiplyVector(Plane(normal*(ctrl.radius + radiusOffset), aPlane)) + at;
					Vector3 nV = Handles.FreeMoveHandle(v,
						SceneView.lastActiveSceneView.camera.transform.rotation,
						size * aVisuals.sizeControlHandle,
						Vector3.zero,
						aVisuals.capControlHandle);

					if (nV != v) {
						SerializedProperty radius = aPath.FindPropertyRelative("_pointControls").GetArrayElementAtIndex(aIndex).FindPropertyRelative("radius");
						nV = EditorTools.ProjectPoint(nV, aEditPlane);
						nV -= at;
						nV = aInvTransform.MultiplyVector(nV);
						Vector2 newPos = Deplane(nV, aPlane);
						radius.floatValue = SnapScale(radius.floatValue, Mathf.Max(0,newPos.magnitude - radiusOffset), PathSnap.World);
						_recentInteract = aIndex;
						GUI.changed = true;
					}
				
					if (aShowMeta) {
						string txt = Math.Round(ctrl.radius,2).ToString();
						Handles.Label(v, txt, ShadowStyle);
						Handles.Label(v, txt, LabelStyle);
					}
				}
			} else {
				Vector2 newPos;
				Vector3 v = aTransform.MultiplyVector(Plane(ctrl.controlPrev, aPlane)) + at;
				Vector3 nV = Handles.FreeMoveHandle(v,
					SceneView.lastActiveSceneView.camera.transform.rotation,
					HandleUtility.GetHandleSize(v) * aVisuals.sizeControlHandle,
					Vector3.zero,
					aVisuals.capControlHandle);

				if (nV != v) {
					SerializedProperty controlProp = aPath.FindPropertyRelative("_pointControls").GetArrayElementAtIndex(aIndex);
					SerializedProperty type = controlProp.FindPropertyRelative("type");
					SerializedProperty prev = controlProp.FindPropertyRelative("controlPrev");

					nV = EditorTools.ProjectPoint(nV, aEditPlane);
					nV -= at;
					nV = aInvTransform.MultiplyVector(nV);
					newPos = Deplane(nV, aPlane);
					prev.vector2Value = SnapRadial(v, prev.vector2Value, newPos, aTransform, aInvTransform, PathSnap.World);
					if (aUnlock) {
						ctrl.type = PointType.Free;
						type.enumValueIndex = (int)ctrl.type;
					}
					_recentInteract = aIndex;
					GUI.changed = true;
				}

				if (ctrl.type == PointType.Locked)
					v = -aTransform.MultiplyVector(Plane(ctrl.controlPrev, aPlane)) + at;
				else
					v =  aTransform.MultiplyVector(Plane(ctrl.controlNext, aPlane)) + at;

				nV = Handles.FreeMoveHandle(v,
					SceneView.lastActiveSceneView.camera.transform.rotation,
					HandleUtility.GetHandleSize(v) * aVisuals.sizeControlHandle,
					Vector3.zero,
					aVisuals.capControlHandle);

				if (nV != v) {
					SerializedProperty controlProp = aPath.FindPropertyRelative("_pointControls").GetArrayElementAtIndex(aIndex);
					SerializedProperty type = controlProp.FindPropertyRelative("type");
					SerializedProperty prev = controlProp.FindPropertyRelative("controlPrev");
					SerializedProperty next = controlProp.FindPropertyRelative("controlNext");

					nV = EditorTools.ProjectPoint(nV, aEditPlane);
					nV -= at;
					nV = aInvTransform.MultiplyVector(nV);
					if (aUnlock) {
						ctrl.type = PointType.Free;
						type.enumValueIndex = (int)ctrl.type;
					}
					
					if (ctrl.type == PointType.Locked) {
						newPos = Deplane(-nV, aPlane);
						prev.vector2Value = SnapRadial(v, prev.vector2Value, newPos, aTransform, aInvTransform, PathSnap.World);
					} else {
						newPos = Deplane( nV, aPlane);
						next.vector2Value = SnapRadial(v, next.vector2Value, newPos, aTransform, aInvTransform, PathSnap.World);
					}
					_recentInteract = aIndex;
					GUI.changed = true;
				}

				if (aShowMeta) {
					bool xz = aPlane == Path2D.Plane.XZ;

					Vector2 prop2 = (locked?ctrl.controlPrev:ctrl.controlNext);
					Vector3 handle1 = aTransform.MultiplyVector(Plane(ctrl.controlPrev, aPlane)) + at;
					Vector3 handle2 = (locked?-1:1) * aTransform.MultiplyVector(Plane((locked?ctrl.controlPrev:ctrl.controlNext), aPlane)) + at;
					float ang1 = PathUtil.ClockwiseAngle(ctrl.controlPrev,  Vector2.right);//Vector2.Angle(prev.vector2Value, Vector2.right);
					float ang2 = PathUtil.ClockwiseAngle((locked?-1:1) * prop2, Vector2.right);//Vector2.Angle(prop2.vector2Value, Vector2.right);
					float mag1 = ctrl.controlPrev.magnitude;
					float mag2 = prop2.magnitude;

					Vector3 pos = Vector3.Lerp(handle1, at, 0.5f);
					string  txt = Math.Round(mag1,2).ToString();
					Handles.Label(pos, txt, ShadowStyle); Handles.Label(pos, txt, LabelStyle);
					pos = Vector3.Lerp(handle2, at, 0.5f);
					txt = Math.Round(mag2,2).ToString();
					Handles.Label(pos, txt, ShadowStyle); Handles.Label(pos, txt, LabelStyle);

					pos = handle1;
					txt = "\u00B0"+Mathf.Round(ang1);
					Handles.Label(pos, txt, ShadowStyle); Handles.Label(pos, txt, LabelStyle);
					pos = handle2;
					txt = "\u00B0"+Mathf.Round(ang2);
					Handles.Label(pos, txt, ShadowStyle); Handles.Label(pos, txt, LabelStyle);
					
					if (ctrl.type == PointType.Free) {
						if (ang2 < ang1)
							ang2 += 360;
						float ang = ang2 - ang1;
						float halfAng = ang1 + ang/2;

						float arcRadius = HandleUtility.GetHandleSize(at) * aVisuals.sizeVertex * 1.75f;
						Vector3 centerArc = new Vector3(Mathf.Cos(halfAng*Mathf.Deg2Rad),Mathf.Sin(halfAng*Mathf.Deg2Rad),0);
						if (xz)
							centerArc = new Vector3(centerArc.x, 0, centerArc.y);
						centerArc = aTransform.MultiplyVector(centerArc);

						var centeredStyle = new GUIStyle(GUI.skin.label);
						centeredStyle.contentOffset = new Vector2(-9,-9);
						centeredStyle.normal.textColor = new Color(0,0,0,0.5f);
						Handles.Label(at + centerArc * arcRadius * 2f, "\u00B0"+Mathf.Round(ang), centeredStyle);
						centeredStyle.contentOffset = new Vector2(-10,-10);
						centeredStyle.normal.textColor = Color.white;
						Handles.Label(at + centerArc * arcRadius * 2f, "\u00B0"+Mathf.Round(ang), centeredStyle);

						Handles.DrawWireArc(at, aPlane == Path2D.Plane.XY?Vector3.forward:Vector3.up, ((xz?handle2:handle1)-at).normalized, ang, arcRadius);
					}
				}
			}
		}
		#endregion

		public static List<Vector3> GetAllHandleLocations(Matrix4x4 aTransform, Path2D aPathRaw, bool aShowShiftAdd = true, Path2D.Plane aPlane = Path2D.Plane.XY) {
			List<Vector3>      result   = new List<Vector3>();
			List<Vector2>      points   = aPathRaw.GetPathRaw();
			List<PointControl> controls = aPathRaw.GetControls();
			bool showControls = !Event.current.shift && !Event.current.alt;

			// add all points
			List<Vector3> worldPoints = PathUtil.To3D(points, aTransform, aPlane == Path2D.Plane.XY ? PathUtil.ConvertOptions.XY : PathUtil.ConvertOptions.XZ );
			result.AddRange(worldPoints);

			// add the control points
			if (showControls) {
				for (int i = 0; i < points.Count; i++) {
					PointControl ctrl = controls[i];
					PointType t  = ctrl.type;
					Vector3   at = worldPoints[i];

					if (t == PointType.CircleCorner) {
						Vector3 end1   = PathUtil.GetRoundedCornerEnd(i, points, controls, aPathRaw.Closed, ctrl.radius, true);
						Vector3 end2   = PathUtil.GetRoundedCornerEnd(i, points, controls, aPathRaw.Closed, ctrl.radius, false);
						Vector3 normal = Vector2.Lerp(end1.xy()-points[i], end2.xy()-points[i],0.5f).normalized;

						result.Add(at + aTransform.MultiplyVector(Plane(normal * (ctrl.radius + 0.2f), aPlane)));
					} else if (t == PointType.Free || t == PointType.Locked) {
						Vector3 v = aTransform.MultiplyVector(Plane(ctrl.controlPrev, aPlane)) + at;
						result.Add(v);

						if (t == PointType.Locked)
							v = -aTransform.MultiplyVector(Plane(ctrl.controlPrev, aPlane)) + at;
						else
							v =  aTransform.MultiplyVector(Plane(ctrl.controlNext, aPlane)) + at;
						result.Add(v);
					}
				}
			}

			// create the plane on which all edits occur
			Plane editPlane;
			if (aPlane == Path2D.Plane.XY)
				editPlane = new Plane(aTransform.MultiplyPoint3x4(Vector3.zero), aTransform.MultiplyPoint3x4(Vector3.right), aTransform.MultiplyPoint3x4(Vector3.up));
			else
				editPlane = new Plane(aTransform.MultiplyPoint3x4(Vector3.zero), aTransform.MultiplyPoint3x4(Vector3.right), aTransform.MultiplyPoint3x4(Vector3.forward));

			// add shift-add handle
			if (Event.current.shift && aShowShiftAdd) {
				Ray   r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				float dist = 0;
				if (editPlane.Raycast(r, out dist))
					result.Add(r.GetPoint(dist));
			}

			return result;
		}
		public static SerializedProperty AddPoint(SerializedProperty aPath, Vector2 aPt, int aIndex = -1) {
			Path2D path = (Path2D)EditorTools.GetTargetObjectOfProperty(aPath);
			GetSelection(path).ids.Clear();

			SerializedProperty _points        = aPath.FindPropertyRelative("_points");
			SerializedProperty _pointControls = aPath.FindPropertyRelative("_pointControls");
			bool closed = aPath.FindPropertyRelative("_closed").boolValue;

			if (_points.arraySize == 0) {
				return AddPoint(aPath, aPt, PointType.Sharp);
			}

			int segment = aIndex == -1 ? _points.arraySize-1 : aIndex;
			SerializedProperty ctrlPrev = _pointControls.GetArrayElementAtIndex(PathUtil.WrapIndex(segment,   _points.arraySize, closed));
			SerializedProperty ctrlNext = _pointControls.GetArrayElementAtIndex(PathUtil.WrapIndex(segment+1, _points.arraySize, closed));

			return AddPoint(aPath, aPt, 
				(PointType)ctrlPrev.FindPropertyRelative("type").enumValueIndex,
				Mathf.Max(ctrlPrev.FindPropertyRelative("radius").floatValue, ctrlNext.FindPropertyRelative("radius").floatValue),
				ctrlPrev.FindPropertyRelative("controlPrev").vector2Value,
				ctrlPrev.FindPropertyRelative("controlNext").vector2Value, aIndex);
		}
		public static SerializedProperty AddPoint(SerializedProperty aPath, Vector2 aPt, PointType aType, float aRadius = 1, Vector2 aControlPointPrev = default(Vector2), Vector2 aControlPointNext = default(Vector2), int aIndex = -1) {
			SerializedProperty result;
			SerializedProperty points        = aPath.FindPropertyRelative("_points");
			SerializedProperty pointControls = aPath.FindPropertyRelative("_pointControls");

			if (aIndex == -1) {
				points       .arraySize += 1;
				pointControls.arraySize += 1;
				aIndex = points.arraySize-1;
			} else {
				points       .InsertArrayElementAtIndex(aIndex);
				pointControls.InsertArrayElementAtIndex(aIndex);
			}

			result = points.GetArrayElementAtIndex(aIndex);
			result.vector2Value = aPt;

			SerializedProperty controls = pointControls.GetArrayElementAtIndex(aIndex);
			controls.FindPropertyRelative("type"       ).enumValueIndex = (int)aType;
			controls.FindPropertyRelative("radius"     ).floatValue     = aRadius;
			controls.FindPropertyRelative("controlNext").vector2Value   = aControlPointNext;
			controls.FindPropertyRelative("controlPrev").vector2Value   = aControlPointPrev;

			SetDirty(aPath);
			return result;
		}
		public static void SetDirty(SerializedProperty aPath) {
			Path2D p = EditorTools.GetTargetObjectOfProperty(aPath) as Path2D;
			if (p != null)
				p.SetDirty();
		}
		public static void Clear   (SerializedProperty aPath) {
			aPath.FindPropertyRelative("_points").ClearArray();
			aPath.FindPropertyRelative("_pointControls").ClearArray();
			SerializedProperty data = aPath.FindPropertyRelative("_data");
			if (data != null) data.ClearArray();

			SetDirty(aPath);
		}

		#region Handle Helpers
		private static Vector3 Plane(Vector2 aPt, Path2D.Plane aPlane) {
			return aPlane == Path2D.Plane.XY ? aPt.xy0() : aPt.x0y();
		}
		private static Vector2 Deplane(Vector3 aPt, Path2D.Plane aPlane) {
			return aPlane == Path2D.Plane.XY ? aPt.xy() : aPt.xz();
		}
		private static Vector2 SnapRadial(Vector3 aWorldPt, Vector2 aOriginalPoint, Vector2 aNewPoint, Matrix4x4 aTransform, Matrix4x4 aInvTransform, PathSnap aSnapMode = PathSnap.Unity) {
			Vector2 result = aNewPoint;

			if (!Event.current.control)
				return result;
			
			float angleSnap     = EditorTools.GetUnityRotationSnap();
			float magnitudeSnap = EditorTools.GetUnityScaleSnap();
			Vector2 src = aNewPoint;

			if (aSnapMode == PathSnap.Local)
				src = aNewPoint;
			if (aSnapMode == PathSnap.World) 
				src = aInvTransform.MultiplyVector(aNewPoint);
			if (aSnapMode == PathSnap.Relative)
				src = aNewPoint;

			Vector2 polar     = new Vector2(PathUtil.ClockwiseAngle(src, Vector2.right), src.magnitude);
			Vector3 polarSnap = new Vector3(angleSnap, magnitudeSnap, 0);

			polar = SnapVector(polar, polarSnap);
			polar.x *= Mathf.Deg2Rad;

			result = new Vector2(Mathf.Cos(polar.x) * polar.y, Mathf.Sin(polar.x) * polar.y);
				
			if (aSnapMode == PathSnap.World) 
				result = aInvTransform.MultiplyVector(result);
			
			return result;
		}
		private static Vector2 Snap(Vector2 aOriginalPoint, Vector2 aNewPoint, Matrix4x4 aTransform, Matrix4x4 aInvTransform, PathSnap aSnapMode = PathSnap.Unity) {
			Vector2 result = aNewPoint;

			if (!Event.current.control)
				return result;

			Vector3 snap = EditorTools.GetUnitySnap();

			if (aSnapMode == PathSnap.Local)
				result = SnapVector(aNewPoint, snap);
			
			if (aSnapMode == PathSnap.World) 
				result = aInvTransform.MultiplyPoint(SnapVector( aTransform.MultiplyPoint(aNewPoint), snap ));

			if (aSnapMode == PathSnap.Relative)
				result = aOriginalPoint + SnapVector(aNewPoint-aOriginalPoint, snap);
			
			return result;
		}
		private static float   SnapScale(float aOriginalScale, float aNewScale, PathSnap aSnapMode = PathSnap.Unity) {
			float result = aNewScale;

			if (!Event.current.control)
				return result;

			float snap = EditorTools.GetUnityScaleSnap();

			if (aSnapMode == PathSnap.Local || aSnapMode == PathSnap.World)
				result = SnapScalar(aNewScale, snap);

			if (aSnapMode == PathSnap.Relative)
				result = aOriginalScale + SnapScalar(aNewScale-aOriginalScale, snap);
			
			return result;
		}
		private static Vector3 SnapVector(Vector3 aVector, Vector3 aSnap) {
			return new Vector3(
				((int)(aVector.x / aSnap.x + (aVector.x > 0 ? 0.5f : -0.5f))) * aSnap.x,
				((int)(aVector.y / aSnap.y + (aVector.y > 0 ? 0.5f : -0.5f))) * aSnap.y,
				((int)(aVector.z / aSnap.z + (aVector.z > 0 ? 0.5f : -0.5f))) * aSnap.z);
		}
		private static Vector2 SnapVector(Vector2 aVector, Vector3 aSnap) {
			return new Vector2(
				((int)(aVector.x / aSnap.x + (aVector.x > 0 ? 0.5f : -0.5f))) * aSnap.x,
				((int)(aVector.y / aSnap.y + (aVector.y > 0 ? 0.5f : -0.5f))) * aSnap.y);
		}
		private static float   SnapScalar(float   aScalar, float aSnap) {
			return ((int)(aScalar / aSnap + (aScalar > 0 ? 0.5f : -0.5f))) * aSnap;
		}
		private static Vector3 SmartSnap(Vector3 aPoint, Path2D aPath, int aIgnoreId, List<int> aIgnoreList, float aSnapDist) {
			float   minXDist = aSnapDist;
			float   minYDist = aSnapDist;
			Vector3 result   = aPoint;
		
			for (int i = 0; i < aPath.Count; ++i) {
				if (i == aIgnoreId || aIgnoreList.Contains(i)) continue;
			
				float xDist = Mathf.Abs(aPoint.x - aPath[i].x);
				float yDist = Mathf.Abs(aPoint.y - aPath[i].y);
			
				if (xDist < minXDist) {
					minXDist = xDist;
					result.x = aPath[i].x;
				}
			
				if (yDist < minYDist) {
					minYDist = yDist;
					result.y = aPath[i].y;
				}
			}
			return result;
		}
		#endregion

		public static void MakePointAuto(SerializedProperty aPath, int aIndex) {
			SerializedProperty ctrl = aPath.FindPropertyRelative("_controls").GetArrayElementAtIndex(aIndex);
			ctrl.FindPropertyRelative("type").enumValueIndex = (int)Ferr.PointType.Auto;
		}
		public static void MakePointSharp(SerializedProperty aPath, int aIndex) {
			SerializedProperty ctrl = aPath.FindPropertyRelative("_controls").GetArrayElementAtIndex(aIndex);
			ctrl.FindPropertyRelative("type").enumValueIndex = (int)Ferr.PointType.Sharp;
		}
		
		/// <summary>
		/// Takes a path, and switches Auto/Sharp marked control points based on their angles.
		/// </summary>
		/// <param name="aPath">A SerializedProperty pointing to a Path2D object.</param>
		/// <param name="aTolerance">How far can the angle deviate from right angles before it switches? Degrees.</param>
		public static void AutoSharpen(SerializedProperty aPath, float aTolerance=45) {
			SerializedProperty aPoints        = aPath.FindPropertyRelative("_points");
			SerializedProperty aPointControls = aPath.FindPropertyRelative("_pointControls");

			int     size = aPoints.arraySize;
			Vector2 prev = aPoints.GetArrayElementAtIndex(size - 1).vector2Value;
			Vector2 curr = aPoints.GetArrayElementAtIndex(0).vector2Value;
			Vector2 next = aPoints.GetArrayElementAtIndex(1).vector2Value;

			for (int i = 0; i < size; i++) {
				SerializedProperty ctrl = aPointControls.GetArrayElementAtIndex(i);
				SerializedProperty type = ctrl.FindPropertyRelative("type");

				if (type.enumValueIndex == (int)PointType.Auto || type.enumValueIndex == (int)PointType.Sharp) {
					float angle = PathUtil.ClockwiseAngle(prev - curr, next - curr);

					if (angle < 90+aTolerance || angle > 270-aTolerance) {
						type.enumValueIndex = (int)PointType.Sharp;
					} else {
						type.enumValueIndex = (int)PointType.Auto;
					}
				}

				prev = curr;
				curr = next;
				next = aPoints.GetArrayElementAtIndex(PathUtil.WrapIndex(i+2, size, true)).vector2Value;
			}
		}
	}
}