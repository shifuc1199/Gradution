using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ferr2D_Visual {
	public const float LineWidth       = 0.05f;
	public const float HandleSize      = 0.75f;
	public const float SmallHandleSize = 0.5f;

	public readonly static Color ColliderColor = new Color(79/255f, 197/255f, 87/255f, 1);
	public readonly static Color PathColor     = new Color(1, 1, 1, 1);
	public readonly static Color HandleColor   = new Color(1, 1, 1, 1);

	public readonly static Color DragBoxInnerColor = new Color(0, 0.5f, 0.25f, 0.25f);
	public readonly static Color DragBoxOuterColor = new Color(0, 0.5f, 0.25f, 0.5f);
	public readonly static Color HelpBoxColor      = new Color(0,0,0,.6f);

	public readonly static Color IndicesColor = new Color(1,1,1,1);
}