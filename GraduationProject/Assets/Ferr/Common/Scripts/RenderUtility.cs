using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ferr {
	public class RenderUtility {
		static List<int> mReservedLayers = null;
		static Camera    mCamera         = null;
		
		public static int  GetFreeLayer() {
			for (int i = 16; i < 32; i += 1) {
				if (LayerMask.LayerToName(i) == "" && (mReservedLayers == null || !mReservedLayers.Contains(i)))
					return i;
			}
			Debug.LogError("Ferr is looking for an unnamed render layer after 15, but none are free!");
			return -1;
		}
		public static void ReserveLayer(int aLayerID) {
			if (mReservedLayers == null) mReservedLayers = new List<int>();
			mReservedLayers.Add(aLayerID);
		}
		public static Camera CreateRenderCamera() {
			if (mCamera == null) {
				GameObject go = new GameObject("Ferr Render Cam");
				mCamera = go.AddComponent<Camera>();
				mCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
				mCamera.enabled              = false;
			}
			return mCamera;
		}
	}
}