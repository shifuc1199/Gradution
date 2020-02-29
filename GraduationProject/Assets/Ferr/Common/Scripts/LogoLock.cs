using UnityEngine;
using System.Collections;

namespace Ferr {
	public class LogoLock : MonoBehaviour {
		private enum LockPosition {
			Left,
			Center,
			Right
		}
		
		[SerializeField]
		private Camera       mCamera;
		[SerializeField]
		private LockPosition mLockHorizontal;
		[SerializeField]
		private LockPosition mLockVertical;
		[SerializeField]
		private float        mPadding;
		[SerializeField]
		private float        mScale = 1;
		
		void Awake () {
			if (mCamera == null) {
				mCamera = Camera.main;
			}
			transform.parent        = mCamera.transform;
			transform.localPosition = GetLockPosition(mCamera, mLockHorizontal, mLockVertical, mPadding);
			float s                 = GetPixelScale  (mCamera, GetComponent<SpriteRenderer>().sprite);
			transform.localScale    = new Vector3(s,s,s) * mScale;
			transform.localRotation = Quaternion.identity;
		}
		
		private static float   GetPixelScale  (Camera aCam, Sprite aSprite) {
			float   result   = 1;
			Vector2 viewSize = GetViewSizeAtDistance(1, aCam);
			float   ratio    = aSprite.textureRect.width / Screen.width;
			
			result = (viewSize.x * ratio) / (aSprite.bounds.extents.x * 2);
			
			return result;
		}
		
		private static Vector3 GetLockPosition(Camera aCam, LockPosition aHLock, LockPosition aVLock, float aPadding) {
			Vector3 result   = Vector3.zero;
			Vector2 viewSize = GetViewSizeAtDistance(1, aCam);
			result.z = 1;
			aPadding = aPadding * ((1f/Screen.width) * viewSize.x);
			
			if      (aHLock == LockPosition.Left  ) result.x = -viewSize.x/2 + aPadding;
			else if (aHLock == LockPosition.Center) result.x = 0;
			else if (aHLock == LockPosition.Right ) result.x =  viewSize.x/2 - aPadding;
			
			if      (aVLock == LockPosition.Left  ) result.y =  viewSize.y/2 - aPadding;
			else if (aVLock == LockPosition.Center) result.y = 0;
			else if (aVLock == LockPosition.Right ) result.y = -viewSize.y/2 + aPadding;
			
			return result;
		}
		
		private static Vector2 GetViewSizeAtDistance(float aDist, Camera aCamera) {
			if (aCamera == null) aCamera = Camera.main;
			if (aCamera.orthographic)
				return new Vector2(((float)Screen.width / Screen.height) * aCamera.orthographicSize * 2, aCamera.orthographicSize * 2);
			float frustumHeight = 2f * aDist * Mathf.Tan(aCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
			return new Vector2(frustumHeight * aCamera.aspect, frustumHeight);
		}
	}
}