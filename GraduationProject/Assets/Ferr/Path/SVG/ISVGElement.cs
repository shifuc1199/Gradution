using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ferr {
	public interface ISVGElement {
		Vector2 Start  { get; }
		Vector2 End    { get; }
		Rect    Bounds { get; }
		bool    Closed { get; }
		ISVGElement   Reverse     ();
		List<Vector2> GetPoints   (int aResolution);
		string        ToSVGElement();
	}
}