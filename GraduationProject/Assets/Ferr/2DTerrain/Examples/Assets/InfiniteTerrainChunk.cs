using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr.Example {
	public class InfiniteTerrainChunk : MonoBehaviour {
		[SerializeField] Transform _rightHook;
		[SerializeField] Transform _leftHook;

		public Vector3 RightHook { get{ return _rightHook==null?transform.position:_rightHook.transform.position; } }
		public Vector3 LeftHook  { get{ return _leftHook ==null?transform.position:_leftHook .transform.position; } }

		public void ConnectTo(Vector3 aToHook, Side aToHookSide) {
			if (aToHookSide == Side.Left)
				transform.position = aToHook + (transform.position - RightHook);
			else if (aToHookSide == Side.Right)
				transform.position = aToHook + (transform.position - LeftHook);
		}

		private void OnDrawGizmos() {
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(RightHook, 1);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(LeftHook, 1);
		}
	}
}