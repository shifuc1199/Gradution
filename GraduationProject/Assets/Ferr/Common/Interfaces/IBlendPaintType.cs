using UnityEngine;
using System.Collections.Generic;

namespace Ferr {
	public interface IBlendPaintType {
		Color Color    { get; set; }
		float Size     { get; set; }
		float Strength { get; set; }
		float Falloff  { get; set; }
		bool  Backfaces{ get; set; }

		Texture2D Cursor            { get; }
		Vector2   CursorHotspot     { get; }
		bool      ShowColorSettings { get; }
		bool      ShowBrushSettings { get; }
		string    Name              { get; }

		void PaintObjectsBegin(List<GameObject> aObjects, RaycastHit aHit, RaycastHit? aPreviousHit);
		void PaintObjects     (List<GameObject> aObjects, RaycastHit aHit, RaycastHit? aPreviousHit);
		void PaintObjectsEnd  (List<GameObject> aObjects, RaycastHit aHit, RaycastHit? aPreviousHit);

		void PaintBegin(GameObject aObject, RaycastHit aHit, RaycastHit? aPreviousHit);
		void Paint     (GameObject aObject, RaycastHit aHit, RaycastHit? aPreviousHit);
		void PaintEnd  (GameObject aObject, RaycastHit aHit, RaycastHit? aPreviousHit);

		float GetPointInfluence(Vector3 aObjScale, Vector3 aHitPt, Vector3 aHitDirection, Vector3 aVert, Vector3 aVertNormal);
		void RenderScenePreview(Camera aSceneCamera, RaycastHit aHit, List<GameObject> aObjects);
		void RenderScenePreview(Camera aSceneCamera, RaycastHit aHit, GameObject aObject);

		int  CheckPriority(GameObject aOfObject);
		void OnSelect  (List<GameObject> aObjects);
		void OnUnselect(List<GameObject> aObjects);
		void DrawToolGUI();
		bool GUIInput   ();
	}
}