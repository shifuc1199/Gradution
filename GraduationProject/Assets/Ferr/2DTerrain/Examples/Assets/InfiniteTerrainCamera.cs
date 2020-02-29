using UnityEngine;
using System.Collections;

namespace Ferr.Example {
	public class InfiniteTerrainCamera : MonoBehaviour {
		[SerializeField] Transform _targetObject;
		[SerializeField] float     _maxXOffset;
		[SerializeField] float     _maxSpeed = 2;
		[SerializeField] float     _maxDistance = 100;
   
		float _screenWidth;
		float _xPosition;

		void Start      () {
			_screenWidth = GetViewSizeAtDistance(Mathf.Abs(_targetObject.transform.position.z - transform.position.z)).x;
		}
		void FixedUpdate () {
			if (_targetObject.transform.position.x < _xPosition - _screenWidth / 2) {
				_targetObject.transform.position = new Vector3(0, 9, 0);
				transform.position = new Vector3(0, 9, transform.position.z);
				_xPosition = 0;
			}

			_xPosition += GetSpeed() * Time.deltaTime;
			if (_targetObject.transform.position.x > _xPosition + _maxXOffset) {
				_xPosition = _targetObject.transform.position.x - _maxXOffset;
			}
			transform.position = new Vector3(_xPosition, _targetObject.transform.position.y, transform.position.z);
		}

		float GetSpeed() {
			return Mathf.Min(1,(_xPosition/_maxDistance)) * _maxSpeed;
		}

		public static Vector2 GetViewSizeAtDistance(float aDist) {
			float frustumHeight = 2f * aDist * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
			return new Vector2(frustumHeight * Camera.main.aspect, frustumHeight);
		}
	}
}