using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Ferr2DT_Material {
	#if UNITY_EDITOR
	const string editorMenuName = "Terrain Material";
	[UnityEditor.MenuItem("GameObject/Create Ferr2D/" + editorMenuName, false, 11 ), 
	 UnityEditor.MenuItem("Assets/Create/Ferr2D/"     + editorMenuName, false, 101)]
	public static void CreateAsset() {
		Ferr2DT_Material mat = (Ferr2DT_Material)Ferr.SOUtil.CreateAsset(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, editorMenuName);
		// top
		mat.Set(Ferr2DT_TerrainDirection.Top, true);
		Ferr2DT_SegmentDescription curr = mat._descriptors[0];
		curr.SingleColliderCapType = Ferr2D_CapColliderType.Connected;

		// left
		mat.Set(Ferr2DT_TerrainDirection.Left, true);
		curr = mat._descriptors[1];
		curr.ZOffset = 2f/1000f;
		curr.SingleColliderCapType = Ferr2D_CapColliderType.Rectangle;

		// right
		mat.Set(Ferr2DT_TerrainDirection.Right, true);
		curr = mat._descriptors[2];
		curr.ZOffset = 2f/1000f;
		curr.SingleColliderCapType = Ferr2D_CapColliderType.Rectangle;

		// right
		mat.Set(Ferr2DT_TerrainDirection.Bottom, true);
		curr = mat._descriptors[3];
		curr.ZOffset = 1f/1000f;
		curr.SingleColliderCapType = Ferr2D_CapColliderType.Connected;
	}
	#endif
}
