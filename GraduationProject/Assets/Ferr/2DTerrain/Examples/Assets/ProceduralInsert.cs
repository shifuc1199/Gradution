using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ferr.Example {
	public class ProceduralInsert : MonoBehaviour {
		#region Fields
		[SerializeField] Ferr2DT_PathTerrain _terrain;
		[SerializeField] Texture2D           _handle;
		[SerializeField] bool                _showHelpWindow = true;
		
		bool    _mouseDown;
		int     _selected = -1;
		Vector2 _downPos;
		Vector2 _ptStartPos;
		List<Vector2> _original = new List<Vector2>();
		#endregion

		#region Properties
		public Ferr2DT_PathTerrain Terrain {
			get{ return _terrain; }
			set{ _terrain = value; Save(); }
		}
		#endregion

		#region Unity Events
		private void Start() {
			Save();
		}
		private void OnGUI() {
			if (!enabled)
				return;

			if (_showHelpWindow) {
				GUILayout.Window(0, new Rect(10,10,200,100), (id)=>{
					GUILayout.Label("\u2022 Click to Add");
					GUILayout.Label("\u2022 Right-Click to Remove");
					GUILayout.Label("\u2022 Drag to Move");
					if (_terrain == null)
						GUILayout.Label("==No Terrains Selected!==");
					if (GUILayout.Button("Reset")) {
						Reset();
					}
				}, "Runtime Edit Example");
			}
			if (_terrain != null) {
				DoHandleEditor();
			}
		}
		#endregion

		#region Editor
		private void DoHandleEditor() {
			Path2D    path  = _terrain.PathData;
			Matrix4x4 mat   = _terrain.transform.localToWorldMatrix;
			Vector2   mouse = Event.current.mousePosition;
			bool      justDown = false;
			bool      justUp   = false;

			// find where the mouse is in coordinates relative to the terrain
			Vector2 mouseLocal = GetMouseLocal(mat);

			// do mouse events
			if (Event.current.type == EventType.MouseDown) {
				_mouseDown = true;
				_downPos   = mouseLocal;
				justDown   = true;
			} else if (Event.current.type == EventType.MouseUp) {
				_mouseDown = false;
				justUp     = true;
			}

			for (int i = 0; i < path.Count; i++) {
				// if it's selected, apply movement
				if (_selected == i && Event.current.button == 0) {
					path[i] = _ptStartPos + (mouseLocal-_downPos);
				}

				// find the screen location
				Vector3 world  = mat.MultiplyPoint( path[i] );
				Vector3 screen = Camera.main.WorldToScreenPoint(world);
				screen.y = Screen.height-screen.y;

				// select this point if they just clicked, and we're close enough
				if (justDown && Vector2.Distance(screen, mouse) < _handle.width) {
					_selected = i;
					_ptStartPos = path[i];
				}
				
				// draw the handle on screen
				GUI.color = Color.Lerp(Color.white, new Color(1,1,1,0.0f), Vector2.Distance(mouse, screen)/300f);
				GUI.DrawTexture(new Rect(screen.x-_handle.width/2, screen.y-_handle.height/2, _handle.width, _handle.height), _handle);
			}

			// rebuild if the mouse moved with something selected
			if (_mouseDown && Event.current.type == EventType.MouseDrag && _selected != -1) {
				_terrain.Build(false);
			}
			// check for adding a new point
			if (justUp) {
				if (_selected == -1 && Event.current.button==0) {
					_terrain.AddAutoPoint(mouseLocal);
				}
				_selected  = -1;
				_terrain.Build(false);
			}
			// check for deleting a point
			if (justDown && Event.current.button == 1 && _selected != -1) {
				path.RemoveAt(_selected);
				_selected = -1;
			}
		}
		private Vector2 GetMouseLocal(Matrix4x4 aLocalToWorld) {
			Vector2 mouse      = Event.current.mousePosition;
			Plane   p          = new Plane(aLocalToWorld.MultiplyVector(new Vector3(0,0,1)), aLocalToWorld.MultiplyPoint(Vector3.zero));
			Ray     mouseRay   = Camera.main.ScreenPointToRay( new Vector3(mouse.x, Screen.height-mouse.y, 0) );
			Vector2 mouseLocal = Vector2.zero;
			float   dist = 0;
			if (p.Raycast(mouseRay, out dist)) {
				mouseLocal = mouseRay.GetPoint(dist);
				mouseLocal = _terrain.transform.worldToLocalMatrix.MultiplyPoint(mouseLocal);
			}
			return mouseLocal;
		}
		#endregion

		#region Helper Methods
		private void Save () {
			if (_terrain != null) {
				_original = _terrain.PathData.GetPathRawCopy();
			} else {
				_original.Clear();
			}
		}
		private void Reset() {
			if (_terrain == null) 
				return;

			_terrain.ClearPoints();
			for (int i = 0; i < _original.Count; i++) {
				_terrain.AddPoint(_original[i]);
			}
			_terrain.Build(false);
		}
		#endregion
	}
}