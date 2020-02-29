using UnityEngine;
using System.Collections;

namespace Ferr {
	public class CameraShake : MonoBehaviour {
		#region Singleton
		private static CameraShake instance = null;
		private static CameraShake Instance {
			get{if (instance == null) instance = Create(); return instance;}
		}
		private CameraShake() {
		}
		private static CameraShake Create() {
			return Camera.main.gameObject.AddComponent<CameraShake>();
		}
		#endregion
	
		Vector3 magnitude;
		float   duration;
		float   start;
	
		Vector3 offset;
		AnimationCurve curve;
	
		void LateUpdate () {
			float percent = (Time.time - start) / duration;
			if (percent <= 1) {
				transform.position -= offset;
				offset = new Vector3(
					Random.Range(-magnitude.x, magnitude.x),
					Random.Range(-magnitude.y, magnitude.y),
					Random.Range(-magnitude.z, magnitude.z)) * curve.Evaluate(percent);
				transform.position += offset;
			} else {
				transform.position -= offset;
				offset  = Vector2.zero;
				enabled = false;
			}
		}
	
		public static void Shake(Vector3 aMagnitude, float aDuration) {
			Instance.magnitude = aMagnitude;
			Instance.duration  = aDuration;
			Instance.start     = Time.time;
	
			Instance.transform.position -= Instance.offset;
			Instance.offset  = Vector3.zero;
			Instance.enabled = true;
			Instance.curve   = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1,0));
		}
	}
}