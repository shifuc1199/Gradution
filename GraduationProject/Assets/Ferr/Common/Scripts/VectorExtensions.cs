using UnityEngine;
using System.Collections;

namespace Ferr.Extensions {
	public static class VectorExtensions {
		public static Vector2 xz(this Vector3 aVec) {
			return new Vector2(aVec.x, aVec.z);
		}
		public static Vector2 xy(this Vector3 aVec) {
			return new Vector2(aVec.x, aVec.y);
		}
		public static Vector2 yz(this Vector3 aVec) {
			return new Vector2(aVec.y, aVec.z);
		}
	
		public static Vector3 x0y(this Vector2 aVec) {
			return new Vector3(aVec.x, 0, aVec.y);
		}
		public static Vector3 xy0(this Vector2 aVec) {
			return new Vector3(aVec.x, aVec.y, 0);
		}
	
		public static Vector3 xty(this Vector2 aVec, float t) {
			return new Vector3(aVec.x, t, aVec.y);
		}
		public static Vector3 xyt(this Vector2 aVec, float t) {
			return new Vector3(aVec.x, aVec.y, t);
		}
	
		public static Vector3 xxx(this Vector3 aVec) {
			return new Vector3(aVec.x, aVec.x, aVec.x);
		}
		public static Vector3 yyy(this Vector3 aVec) {
			return new Vector3(aVec.y, aVec.y, aVec.y);
		}
		public static Vector3 zzz(this Vector3 aVec) {
			return new Vector3(aVec.z, aVec.z, aVec.z);
		}
	
		public static Vector3 WithX(this Vector3 aVec, float aX) {
			return new Vector3(aX, aVec.y, aVec.z);
		}
		public static Vector3 WithY(this Vector3 aVec, float aY) {
			return new Vector3(aVec.x, aY, aVec.z);
		}
		public static Vector3 WithZ(this Vector3 aVec, float aZ) {
			return new Vector3(aVec.x, aVec.y, aZ);
		}
	
		public static void SetX(this Vector3 aVec, float aX) {
			aVec.x = aX;
		}
		public static void SetY(this Vector3 aVec, float aY) {
			aVec.y = aY;
		}
		public static void SetZ(this Vector3 aVec, float aZ) {
			aVec.z = aZ;
		}

		public static Vector2 Rot90CW(this Vector2 aVec) {
			return new Vector2(aVec.y, -aVec.x);
		}
		public static Vector2 Rot90CCW(this Vector2 aVec) {
			return new Vector2(-aVec.y, aVec.x);
		}
	}
}