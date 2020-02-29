using System.Collections;
using System.Collections.Generic;
using Ferr;
using UnityEditor;
using UnityEngine;

public static class Ferr2DT_Caps {
	static Texture2D texMinus;
    static Texture2D texMinusSelected;
	static Texture2D texControl;
    static Texture2D texDot;
    static Texture2D texDotSnap;
    static Texture2D texDotPlus;
    static Texture2D texDotSelected;
    static Texture2D texDotSelectedSnap;
	
	static Texture2D texDot1;
	static Texture2D texDot2;
	static Texture2D texDot3;
	static Texture2D texDot4;
	static Texture2D texDot5;
	static Texture2D texDotN;

	static Texture2D texLeft;
    static Texture2D texRight;
    static Texture2D texTop;
    static Texture2D texBottom;
	static Texture2D texAuto;
	static Texture2D texReset;

	static Texture2D texScaleV;
	static Texture2D texScaleH;
	static Texture2D texScaleDL;
	static Texture2D texScaleDR;
	
	static Texture2D texDotBezier;
	static Texture2D texDotAutoBezier;
	static Texture2D texDotArc;
	
    public static void CapDotMinus        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texMinus,           aType);}
    public static void CapDotMinusSelected(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texMinusSelected,   aType);}
	public static void CapControl         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texControl,         aType);}
    public static void CapDot             (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot,             aType);}
    public static void CapDotSnap         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSnap,         aType);}
    public static void CapDotPlus         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotPlus,         aType);}
    public static void CapDotSelected     (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSelected,     aType);}
    public static void CapDotSelectedSnap (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSelectedSnap, aType);}
    public static void CapDotLeft         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texLeft,            aType);}
    public static void CapDotRight        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texRight,           aType);}
    public static void CapDotTop          (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texTop,             aType);}
    public static void CapDotBottom       (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texBottom,          aType);}
    public static void CapDotAuto         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texAuto,            aType);}
    public static void CapDotScaleV       (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texScaleV,          aType);}
	public static void CapDotScaleH       (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texScaleH,          aType);}
	public static void CapDotScaleDL      (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texScaleDL,         aType);}
	public static void CapDotScaleDR      (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texScaleDR,         aType);}
    public static void CapDotReset        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texReset,           aType);}
	public static void CapDot1            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot1,            aType);}
	public static void CapDot2            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot2,            aType);}
	public static void CapDot3            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot3,            aType);}
	public static void CapDot4            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot4,            aType);}
	public static void CapDot5            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot5,            aType);}
	public static void CapDotN            (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotN,            aType);}
	public static void CapDotBezier       (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotBezier,       aType);}
	public static void CapDotAutoBezier   (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotAutoBezier,   aType);}
	public static void CapDotArc          (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize, EventType aType) {EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotArc,          aType);}
	
	public static void LoadImages() {
		if (texMinus != null) return;

        texMinus           = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-minus.png"         );
        texMinusSelected   = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-minus-selected.png");
		texControl         = EditorTools.GetGizmo("2DTerrain/Gizmos/control.png"           );
        texDot             = EditorTools.GetGizmo("2DTerrain/Gizmos/dot.png"               );
        texDotSnap         = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-snap.png"          );
        texDotPlus         = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-plus.png"          );
        texDotSelected     = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-selected.png"      ); 
        texDotSelectedSnap = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-selected-snap.png" );

		texDot1 = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-1.png");
		texDot2 = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-2.png");
		texDot3 = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-3.png");
		texDot4 = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-4.png");
		texDot5 = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-5.png");
		texDotN = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-n.png");

		texLeft   = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-left.png" );
        texRight  = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-right.png");
        texTop    = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-top.png"  );
        texBottom = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-down.png" );
	    texAuto   = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-auto.png" );
	    texReset  = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-reset.png");

	    texScaleV  = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-scale-v.png");
		texScaleH  = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-scale-h.png");
		texScaleDL = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-scale-dl.png");
		texScaleDR = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-scale-dr.png");
		
		texDotAutoBezier = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-autobezier.png");
		texDotBezier     = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-bezier.png");
		texDotArc        = EditorTools.GetGizmo("2DTerrain/Gizmos/dot-arc.png");
	}

	public static Handles.CapFunction GetScaleCap(Vector2 aNormal) {
		float angle = PathUtil.ClockwiseAngle(Vector2.right, aNormal);

		if (angle < 22.5f || angle > 337.5f || (angle > 157.5f && angle < 202.5f))
			return CapDotScaleH;
		else if ((angle > 67.5f && angle < 112.5f) || (angle > 247.5f && angle < 292.5f))
			return CapDotScaleV;
		else if ((angle >= 22.5f && angle <= 67.5f) || (angle >= 202.5f && angle <= 247.5f))
			return CapDotScaleDR;
		else
			return CapDotScaleDL;
	}
	public static Handles.CapFunction GetEdgeCap(int aDirection) {
		switch (aDirection) {
			case (int)Ferr2DT_TerrainDirection.None:   return CapDotAuto;
			case (int)Ferr2DT_TerrainDirection.Top:    return CapDotTop;
			case (int)Ferr2DT_TerrainDirection.Left:   return CapDotLeft;
			case (int)Ferr2DT_TerrainDirection.Right:  return CapDotRight;
			case (int)Ferr2DT_TerrainDirection.Bottom: return CapDotBottom;
			case 4: return CapDot1;
			case 5: return CapDot2;
			case 6: return CapDot3;
			case 7: return CapDot4;
			case 8: return CapDot5;
			default: return CapDotN;
		}
	}
	public static Handles.CapFunction GetNumberCap(int aNumber) {
		switch (aNumber) {
			case 0: return CapDotAuto;
			case 1: return CapDot1;
			case 2: return CapDot2;
			case 3: return CapDot3;
			case 4: return CapDot4;
			case 5: return CapDot5;
			default: return CapDotN;
		}
	}
}
