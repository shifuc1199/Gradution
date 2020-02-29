using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr {
	public static class GizmoUtil {
		public static void DrawWireCircle(Vector3 aPos, float aRadius) {
			DrawWireArc(aPos, aRadius, 0, 360);
		}
		public static void DrawWireArc(Vector3 aPos, float aRadius, float aAngle, float aAngleWidth) {
			float length = 2*Mathf.PI*aRadius * (aAngleWidth/360f);
			int   sides  = (int)(length / 0.4f);

			float angle = aAngleWidth * Mathf.Deg2Rad;
			float step  = angle / sides;
			float curr  = (aAngle-aAngleWidth/2f) * Mathf.Deg2Rad;
			for (int i = 0; i < sides; i++) {
				Gizmos.DrawLine(
					aPos + new Vector3(Mathf.Cos(curr), 0, Mathf.Sin(curr))*aRadius,
					aPos + new Vector3(Mathf.Cos(curr+step), 0, Mathf.Sin(curr+step))*aRadius);
				curr += step;
			}
		}
	}
}