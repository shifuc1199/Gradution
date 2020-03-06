using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public static class Ferr2DT_TerrainMaterialUtility {
    public static Rect AtlasField          (IFerr2DTMaterial aMat, Rect aRect, Texture aTexture) {
		EditorGUILayout.BeginHorizontal(GUILayout.Height(64));
		GUILayout.Space(5);
		GUILayout.Space(64);
		
		Rect  r   = GUILayoutUtility.GetLastRect();
		float aspect = aTexture.height/aTexture.width;
		float max = aRect.width == 0 && aRect.height == 1 ? 1 : Mathf.Max ( aRect.width, aRect.height * aspect );
		r.width   = Mathf.Max (1, (aRect.width  / max) * 64);
		r.height  = Mathf.Max (1, (aRect.height*aspect / max) * 64);
		
		GUI      .DrawTexture(new Rect(r.x-1,  r.y-1,    r.width+2, 1),          EditorGUIUtility.whiteTexture);
		GUI      .DrawTexture(new Rect(r.x-1,  r.yMax+1, r.width+2, 1),          EditorGUIUtility.whiteTexture);
		GUI      .DrawTexture(new Rect(r.x-1,  r.y-1,    1,         r.height+2), EditorGUIUtility.whiteTexture);
		GUI      .DrawTexture(new Rect(r.xMax, r.y-1,    1,         r.height+2), EditorGUIUtility.whiteTexture);
		GUI      .DrawTextureWithTexCoords(r, aTexture, aMat.ToUV(aRect));
		GUILayout.Space(10);

        Rect result = aMat.ToNative(EditorGUILayout.RectField(aMat.ToPixels(aRect)));
		EditorGUILayout.EndHorizontal();
		
		return result;
	}
    public static void ShowPreview         (IFerr2DTMaterial aMat, Ferr2DT_TerrainDirection aDir, bool aSimpleUVs, bool aEditable, float aWidth) {
		if (aMat.edgeMaterial == null || aMat.edgeMaterial.mainTexture == null) return;
		
		GUILayout.Label(aMat.edgeMaterial.mainTexture);
		
		Rect texRect   = GUILayoutUtility.GetLastRect();
        texRect.width  = Mathf.Min(Screen.width-aWidth, aMat.edgeMaterial.mainTexture.width);
        texRect.height = (texRect.width / aMat.edgeMaterial.mainTexture.width) * aMat.edgeMaterial.mainTexture.height;
		
		ShowPreviewDirection(aMat, aDir, texRect, aSimpleUVs, aEditable);
	}
	public static void ShowPreviewDirection(IFerr2DTMaterial aMat, Ferr2DT_TerrainDirection aDir, Rect aBounds, bool aSimpleUVs, bool aEditable) {
		Ferr2DT_SegmentDescription desc = aMat.GetDescriptor(aDir);
        if (!aMat.Has(aDir)) return;

        if (!aEditable) {
            for (int i = 0; i < desc.body.Length; i++)
            {
                Ferr.EditorTools.DrawRect(aMat.ToScreen( desc.body[i]  ), aBounds);    
            }
		    Ferr.EditorTools.DrawRect(aMat.ToScreen( desc.leftCap  ), aBounds);
	        Ferr.EditorTools.DrawRect(aMat.ToScreen( desc.rightCap ), aBounds);
	        Ferr.EditorTools.DrawRect(aMat.ToScreen( desc.innerLeftCap ), aBounds);
	        Ferr.EditorTools.DrawRect(aMat.ToScreen( desc.innerRightCap), aBounds);
        }
        else if (aSimpleUVs) {
	        float   height    = MaxHeight(desc);
            float   capWidth  = Mathf.Max(desc.leftCap.width, desc.rightCap.width);
            float   bodyWidth = desc.body[0].width;
	        int     bodyCount = desc.body.Length;
	        float   texWidth  = aMat.edgeMaterial.mainTexture != null ? aMat.edgeMaterial.mainTexture.width  : 1;
	        float   texHeight = aMat.edgeMaterial.mainTexture != null ? aMat.edgeMaterial.mainTexture.height : 1;
            Vector2 pos       = new Vector2(desc.leftCap.x, desc.leftCap.y);
            if (desc.leftCap.width == 0 && desc.leftCap.height == 0) pos = new Vector2(desc.body[0].x, desc.body[0].y);

            Rect bounds = new Rect(pos.x, pos.y, capWidth*2+bodyWidth*bodyCount, height);
            bounds = aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels(bounds),  aBounds));
	        bounds = ClampRect(bounds, (Texture2D)aMat.edgeMaterial.mainTexture);
	        
	        Ferr.EditorTools.DrawVLine(new Vector2((pos.x + capWidth)* texWidth + aBounds.x, (pos.y * texHeight)+2), height * texHeight);
	        for (int i = 1; i <= desc.body.Length; i++) {
		        Ferr.EditorTools.DrawVLine(new Vector2((pos.x + capWidth + bodyWidth*i) * texWidth + aBounds.x, (pos.y * texHeight)+2), height * texHeight);
            }

            height    = bounds.height;
            bodyWidth = (bounds.width - capWidth * 2) / bodyCount;
            pos.x     = bounds.x;
            pos.y     = bounds.y;

            float currX = pos.x;
			Rect leftCap = desc.leftCap;
            leftCap.x      = currX;
            leftCap.y      = pos.y;
            leftCap.width  = capWidth;
            leftCap.height = capWidth == 0 ? 0 : height;
			desc.leftCap = leftCap;
            currX += capWidth;

            for (int i = 0; i < desc.body.Length; i++)
            {
                desc.body[i].x      = currX;
                desc.body[i].y      = pos.y;
                desc.body[i].width  = bodyWidth;
                desc.body[i].height = height;
                currX += bodyWidth;
            }

			Rect rightCap = desc.rightCap;
            rightCap.x      = currX;
            rightCap.y      = pos.y;
            rightCap.width  = capWidth;
            rightCap.height = capWidth == 0 ? 0 : height;
			desc.rightCap = rightCap;

        } else {
            for (int i = 0; i < desc.body.Length; i++) {
                desc.body[i]  = ClampRect(aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels( desc.body[i] ), aBounds)),  (Texture2D)aMat.edgeMaterial.mainTexture);
            }
            if (desc.leftCap.width  != 0 && desc.leftCap.height  != 0)
                desc.leftCap  = ClampRect(aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels( desc.leftCap ),  aBounds)), (Texture2D)aMat.edgeMaterial.mainTexture);
            if (desc.rightCap.width != 0 && desc.rightCap.height != 0)
	            desc.rightCap = ClampRect(aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels( desc.rightCap ), aBounds)), (Texture2D)aMat.edgeMaterial.mainTexture);
	        
	        if (desc.innerLeftCap.width  != 0 && desc.innerLeftCap.height  != 0)
		        desc.innerLeftCap  = ClampRect(aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels( desc.innerLeftCap ),  aBounds)), (Texture2D)aMat.edgeMaterial.mainTexture);
	        if (desc.innerRightCap.width != 0 && desc.innerRightCap.height != 0)
		        desc.innerRightCap = ClampRect(aMat.ToNative(Ferr.EditorTools.UVRegionRect(aMat.ToPixels( desc.innerRightCap ), aBounds)), (Texture2D)aMat.edgeMaterial.mainTexture);
        }
	}
    public static void ShowSample          (IFerr2DTMaterial aMat, Ferr2DT_TerrainDirection aDir, float aWidth) {
        if (aMat.edgeMaterial == null || aMat.edgeMaterial.mainTexture == null)  return;

        Ferr2DT_SegmentDescription desc = aMat.GetDescriptor(aDir);
        float   totalWidth              = desc.leftCap.width + desc.rightCap.width + (Mathf.Max(0,desc.body[0].width) * 3);
        float   sourceHeight            = MaxHeight(desc);
		Texture tex                     = aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture;

        float scale = Mathf.Min(aWidth/totalWidth, 64 / sourceHeight);
		float aspect = tex.height/tex.width;

        GUILayout.Space(sourceHeight*scale*aspect);
        float x = GUILayoutUtility.GetLastRect().x;
        float y = GUILayoutUtility.GetLastRect().y;
        if (desc.leftCap.width != 0) {
            float yOff = ((sourceHeight - desc.leftCap.height) / 2) * scale;
            GUI.DrawTextureWithTexCoords(new Rect(x,y+yOff,desc.leftCap.width * scale, desc.leftCap.height * scale * aspect), tex, aMat.ToUV(desc.leftCap));
            x += desc.leftCap.width * scale;
        }
        for (int i = 0; i < 3; i++)
        {
            int id = (2-i) % desc.body.Length;
            float yOff = ((sourceHeight - desc.body[id].height) / 2) * scale;
            GUI.DrawTextureWithTexCoords(new Rect(x,y+yOff,desc.body[id].width * scale, desc.body[id].height * scale * aspect), tex, aMat.ToUV(desc.body[id]));
            x += desc.body[id].width * scale;
        }
        if (desc.leftCap.width != 0) {
            float yOff = ((sourceHeight - desc.rightCap.height) / 2) * scale;
            GUI.DrawTextureWithTexCoords(new Rect(x,y+yOff,desc.rightCap.width * scale, desc.rightCap.height * scale * aspect), tex, aMat.ToUV(desc.rightCap));
        }
    }
    
    public static bool IsSimple      (Ferr2DT_SegmentDescription aDesc) {
        float y       = aDesc.body[0].y;
        bool  hasCaps = aDesc.leftCap.width != 0 || aDesc.rightCap.width != 0;
        float height  = aDesc.body[0].height;

		if (aDesc.innerLeftCap.width != 0 || aDesc.innerRightCap.width != 0) return false;
		if (aDesc.EditorLeftCapType         != aDesc.EditorRightCapType      || 
			aDesc.EditorInnerLeftCapType    != aDesc.EditorInnerRightCapType ||
			aDesc.EditorInnerRightCapType   != aDesc.EditorRightCapType      ||
			aDesc.EditorInnerLeftCapType    != aDesc.EditorLeftCapType) return false;
		if (aDesc.EditorLeftCapOffset       != aDesc.EditorRightCapOffset      ||
			aDesc.EditorInnerLeftCapOffset  != aDesc.EditorInnerRightCapOffset ||
			aDesc.EditorLeftCapOffset       != aDesc.EditorInnerLeftCapOffset  ||
			aDesc.EditorRightCapOffset      != aDesc.EditorInnerRightCapOffset) return false;
		if (aDesc.EditorLeftCapColliderSize      != aDesc.EditorRightCapColliderSize      ||
			aDesc.EditorInnerLeftCapColliderSize != aDesc.EditorInnerRightCapColliderSize ||
			aDesc.EditorLeftCapColliderSize      != aDesc.EditorInnerLeftCapColliderSize  ||
			aDesc.EditorRightCapColliderSize     != aDesc.EditorInnerRightCapColliderSize) return false;

        if (hasCaps && (aDesc.leftCap.y      != y      || aDesc.rightCap.y != y))           return false;
        if (hasCaps &&  aDesc.leftCap.xMax   != aDesc.body[0].x)                            return false;
        if (hasCaps &&  aDesc.rightCap.x     != aDesc.body[aDesc.body.Length - 1].xMax)     return false;
        if (hasCaps && (aDesc.leftCap.height != height || aDesc.rightCap.height != height)) return false;

        for (int i = 0; i < aDesc.body.Length; i++) {
            if (hasCaps && aDesc.body[i].y != aDesc.leftCap.y)                          return false;
            if (aDesc.body[i].height != height)                                         return false;
            if (i + 1 < aDesc.body.Length && aDesc.body[i].xMax != aDesc.body[i + 1].x) return false;
        }
        return true;
    }
    public static void EditUVsSimple (IFerr2DTMaterial    aMat, Ferr2DT_SegmentDescription desc)
    {
        Rect cap  = aMat.ToPixels(desc.leftCap);
        Rect body = aMat.ToPixels(desc.body[0]);

        float   height    = body.height;
        float   capWidth  = cap .width;
        float   bodyWidth = body.width;
        int     bodyCount = desc.body.Length;
        Vector2 pos       = new Vector2(cap.x, cap.y);
        if (cap.width == 0 && cap.height == 0) pos = new Vector2(body.x, body.y);

		Ferr.EditorTools.Box(2, ()=>{
			pos       = EditorGUILayout.Vector2Field("Position",    pos      );
			height    = EditorGUILayout.FloatField  ("Height",      height   );
			capWidth  = EditorGUILayout.FloatField  ("Cap Width",   capWidth );
			desc.SingleColliderCapOffset = EditorGUILayout.Slider("Cap Offset", desc.SingleColliderCapOffset, -1, 1);
			bodyWidth = Mathf.Max(1, EditorGUILayout.FloatField  ("Body Width",  bodyWidth));
			bodyCount = Mathf.Max(1, EditorGUILayout.IntField    ("Body slices", bodyCount));

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Colliders", EditorStyles.boldLabel);
			
			desc.SingleColliderCapType   = (Ferr2D_CapColliderType)EditorGUILayout.EnumPopup("Collider Type", desc.SingleColliderCapType);
			desc.SingleColliderCapSize   = EditorGUILayout.Slider("Collider Size", desc.SingleColliderCapSize, -1, 2);

			if (bodyCount != desc.body.Length) {
				Rect[] bodies = desc.body;
				Array.Resize<Rect>(ref bodies, bodyCount);
				desc.body = bodies;
			}

			float currX = pos.x;
			Rect leftCap = desc.leftCap;
			leftCap.x      = currX;
			leftCap.y      = pos.y;
			leftCap.width  = capWidth;
			leftCap.height = capWidth == 0 ? 0 : height;
			desc.leftCap = aMat.ToNative(leftCap);
			currX += capWidth;

			for (int i = 0; i < desc.body.Length; i++)
			{
				desc.body[i].x      = currX;
				desc.body[i].y      = pos.y;
				desc.body[i].width  = bodyWidth;
				desc.body[i].height = height;
				desc.body[i] = aMat.ToNative(desc.body[i]);
				currX += bodyWidth;
			}

			Rect rightCap = desc.rightCap;
			rightCap.x      = currX;
			rightCap.y      = pos.y;
			rightCap.width  = capWidth;
			rightCap.height = capWidth == 0 ? 0 : height;
			desc.rightCap = aMat.ToNative(rightCap);
		});
    }
    public static void EditUVsComplex(IFerr2DTMaterial    aMat, Ferr2DT_SegmentDescription desc, float aWidth, ref int aCurrBody)
    {
		int currBody = aCurrBody;
		int bodyID = 0;
		Ferr.EditorTools.Box(2, ()=>{
			
			if ( desc.leftCap .width == 0 && desc.leftCap .height == 0 && 
				 desc.rightCap.width == 0 && desc.rightCap.height == 0 ) {
				desc.EditorLeftCapOffset  = EditorGUILayout.Slider("Left Offset",  desc.EditorLeftCapOffset,  -1, 1);
				desc.EditorRightCapOffset = EditorGUILayout.Slider("Right Offset", desc.EditorRightCapOffset, -1, 1);
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Body", GUILayout.Width(40f));

			bodyID = Mathf.Clamp(currBody, 0, desc.body.Length);
			if (GUILayout.Button("<", GUILayout.Width(20f))) currBody = Mathf.Clamp(currBody - 1, 0, desc.body.Length - 1);
			EditorGUILayout.LabelField("" + (bodyID + 1), GUILayout.Width(12f));
			if (GUILayout.Button(">", GUILayout.Width(20f))) currBody = Mathf.Clamp(currBody + 1, 0, desc.body.Length - 1);
			bodyID = Mathf.Clamp(currBody, 0, desc.body.Length - 1);
			int length = Math.Max(1, EditorGUILayout.IntField(desc.body.Length, GUILayout.Width(32f)));
			EditorGUILayout.LabelField("Total", GUILayout.Width(40f));
			if (length != desc.body.Length) {
				Rect[] bodies = desc.body;
				Array.Resize<Rect>(ref bodies, length);
				desc.body = bodies;
			}
			
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			desc.body[bodyID] = AtlasField(aMat, desc.body[bodyID], aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture);

			float pixelWidth = aMat.edgeMaterial == null || aMat.edgeMaterial.mainTexture == null ? 1f/256 : 1f/aMat.edgeMaterial.mainTexture.width;
			if (desc.body[bodyID].width < pixelWidth)
				desc.body[bodyID].width = pixelWidth;
		});
		aCurrBody = currBody;

		EditorGUILayout.Space();
		Ferr.EditorTools.Box(2, ()=>{
			if (desc.leftCap.width == 0 && desc.leftCap.height == 0) {
				if (EditorGUILayout.Toggle("Left Cap", false)) {
					desc.leftCap = aMat.ToNative(new Rect(0, 0, 50, 50));
				}
			} else {
				if (EditorGUILayout.Toggle("Left Cap", true)) {
					desc.EditorLeftCapOffset       = EditorGUILayout.Slider("Cap Offset", desc.EditorLeftCapOffset, -1, 1);
					desc.leftCap = AtlasField(aMat, desc.leftCap, aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture);
				} else {
					desc.leftCap = new Rect(0, 0, 0, 0);
				}
			}
			EditorGUI.indentLevel += 1;
			desc.EditorLeftCapType         = Convert.ToInt32(EditorGUILayout.EnumPopup("Collider Type", (Ferr2D_CapColliderType)desc.EditorLeftCapType));
			desc.EditorLeftCapColliderSize = EditorGUILayout.Slider("Collider Size", desc.EditorLeftCapColliderSize, -1, 2);
			EditorGUI.indentLevel -= 1;
		});

		EditorGUILayout.Space();
		Ferr.EditorTools.Box(2, ()=>{
			if (desc.innerLeftCap.width == 0 && desc.innerLeftCap.height == 0) {
				if (EditorGUILayout.Toggle("Inner Left Cap", false)) {
					desc.innerLeftCap = aMat.ToNative(new Rect(0, 0, 50, 50));
				}
			} else {
				if (EditorGUILayout.Toggle("Inner Left Cap", true)) {
					Ferr.EditorTools.Box(2, ()=>{
						desc.EditorInnerLeftCapOffset       = EditorGUILayout.Slider("Cap Offset", desc.EditorInnerLeftCapOffset, -1, 1);
						desc.innerLeftCap = AtlasField(aMat, desc.innerLeftCap, aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture);
					});
				} else {
					desc.innerLeftCap = new Rect(0, 0, 0, 0);
				}
			}
			EditorGUI.indentLevel += 1;
			desc.EditorInnerLeftCapType         = Convert.ToInt32(EditorGUILayout.EnumPopup("Collider Type", (Ferr2D_CapColliderType)desc.EditorInnerLeftCapType));
			desc.EditorInnerLeftCapColliderSize = EditorGUILayout.Slider("Collider Size", desc.EditorInnerLeftCapColliderSize, -1, 2);
			EditorGUI.indentLevel -= 1;
		});

	    EditorGUILayout.Space();
		Ferr.EditorTools.Box(2, ()=>{
			if (desc.rightCap.width == 0 && desc.rightCap.height == 0) {
				if (EditorGUILayout.Toggle("Right Cap", false)) {
					desc.rightCap = aMat.ToNative(new Rect(0, 0, 50, 50));
				}
			} else  {
				if (EditorGUILayout.Toggle("Right Cap", true)) {
					Ferr.EditorTools.Box(2, ()=>{
						desc.EditorRightCapOffset       = EditorGUILayout.Slider("Cap Offset", desc.EditorRightCapOffset, -1, 1);
						desc.rightCap = AtlasField(aMat, desc.rightCap, aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture);
					});
				} else {
					desc.rightCap = new Rect(0, 0, 0, 0);
				}
			}
			EditorGUI.indentLevel += 1;
			desc.EditorRightCapType         = Convert.ToInt32(EditorGUILayout.EnumPopup("Collider Type", (Ferr2D_CapColliderType)desc.EditorRightCapType));
			desc.EditorRightCapColliderSize = EditorGUILayout.Slider("Collider Size", desc.EditorRightCapColliderSize, -1, 2);
			EditorGUI.indentLevel -= 1;
		});

		EditorGUILayout.Space();
		Ferr.EditorTools.Box(2, ()=>{
			if (desc.innerRightCap.width == 0 && desc.innerRightCap.height == 0) {
				if (EditorGUILayout.Toggle("Inner Right Cap", false)) {
					desc.innerRightCap = aMat.ToNative(new Rect(0, 0, 50, 50));
				}
			} else  {
				if (EditorGUILayout.Toggle("Inner Right Cap", true)) {
					Ferr.EditorTools.Box(2, ()=>{
						desc.EditorInnerRightCapOffset       = EditorGUILayout.Slider("Cap Offset", desc.EditorInnerRightCapOffset, -1, 1);
						desc.innerRightCap = AtlasField(aMat, desc.innerRightCap, aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture);
					});
				} else {
					desc.innerRightCap = new Rect(0, 0, 0, 0);
				}
			}
			EditorGUI.indentLevel += 1;
			desc.EditorInnerRightCapType         = Convert.ToInt32(EditorGUILayout.EnumPopup("Collider Type", (Ferr2D_CapColliderType)desc.EditorInnerRightCapType));
			desc.EditorInnerRightCapColliderSize = EditorGUILayout.Slider("Collider Size", desc.EditorInnerRightCapColliderSize, -1, 2);
			EditorGUI.indentLevel -= 1;
		});
    }
	public static void EditColliders(Ferr2DT_SegmentDescription desc) {
		EditorGUILayout.LabelField("Colliders", EditorStyles.boldLabel);
		EditorGUI.indentLevel = 1;
		
		desc.ColliderOffset    = EditorGUILayout.Slider("Vertical Offset", desc.ColliderOffset,   -1, 1);
		desc.ColliderThickness = EditorGUILayout.Slider("Thickness",       desc.ColliderThickness, 0, 2);
		desc.PhysicsMaterial2D = (PhysicsMaterial2D)EditorGUILayout.ObjectField("Physics Material 2D", desc.PhysicsMaterial2D, typeof(PhysicsMaterial2D), false);
		desc.PhysicsMaterial3D = (PhysicMaterial   )EditorGUILayout.ObjectField("Physics Material 3D", desc.PhysicsMaterial3D, typeof(PhysicMaterial),    false);

		EditorGUI.indentLevel = 0;
	}

    public static float MaxHeight (Ferr2DT_SegmentDescription aDesc) {
        float sourceHeight = Mathf.Max( aDesc.leftCap.height, aDesc.rightCap.height );
        float max          = 0;
        for (int i = 0; i < aDesc.body.Length; i++)
        {
            if (aDesc.body[i].height > max) max = aDesc.body[i].height;
        }
        return Mathf.Max(max, sourceHeight);
    }
    public static Rect  ClampRect (Rect aRect, Texture2D aTex) {
        if (aRect.width  > 1) aRect.width  = 1;
        if (aRect.height > 1) aRect.height = 1;
        if (aRect.xMax   > 1) aRect.xMax   = 1;
        if (aRect.yMax   > 1) aRect.yMax   = 1;
        if (aRect.x      < 0) aRect.x      = 0;
        if (aRect.y      < 0) aRect.y      = 0;
        if (aRect.width  < 0) aRect.width  = 0;
        if (aRect.height < 0) aRect.height = 0;
        return aRect;
    }
	public static void CheckMaterialMode(Material aMat, TextureWrapMode aDesiredMode) {
		if (aMat != null && aMat.mainTexture != null && aMat.mainTexture.wrapMode != aDesiredMode) {
			if (EditorUtility.DisplayDialog("Ferr2D Terrain", "The Material's texture 'Wrap Mode' generally works best when set to "+aDesiredMode+"! Would you like this texture to be updated?", "Yes", "No")) {
				string          path = AssetDatabase.GetAssetPath(aMat.mainTexture);
				TextureImporter imp  = AssetImporter.GetAtPath   (path) as TextureImporter;
				if (imp != null) {
					imp.wrapMode = aDesiredMode;
					AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
				}
			}
		}
	}
}

public class Ferr2DT_TerrainMaterialWindow : EditorWindow {
	[SerializeField] UnityEngine.Object _selectedMaterial; 
	private IFerr2DTMaterial material { get { return (IFerr2DTMaterial)_selectedMaterial; } set { _selectedMaterial=(UnityEngine.Object)value; } }
	
    int                      currBody  = 0;
    bool                     simpleUVs = true;
    const int                width     = 300;
    int                      currDir   = (int)Ferr2DT_TerrainDirection.Top;
    int                      prevDir   = (int)Ferr2DT_TerrainDirection.Top;
    GUIStyle                 foldoutStyle;
    Vector2                  scroll;

    public static void Show(IFerr2DTMaterial aMaterial) {
        Ferr2DT_TerrainMaterialWindow window = EditorWindow.GetWindow<Ferr2DT_TerrainMaterialWindow>();
        window.material       = aMaterial;
        window.wantsMouseMove = true;
        #if UNITY_5_4_OR_NEWER
	    window.titleContent   = new GUIContent("Ferr2DT Editor");
        #else
        window.title          = "Ferr2DT Editor";
        #endif
        if (aMaterial != null && aMaterial.edgeMaterial != null) {
	        window.minSize = new Vector2(400, 400);
        }
        window.foldoutStyle           = EditorStyles.foldout;
        window.foldoutStyle.fontStyle = FontStyle.Bold;
        window.currDir                = 0;
	    window.prevDir                = -1;
    }

    void OnGUI        () {
        if (_selectedMaterial == null) return;

        // if this was an undo, repaint it
        if (Event.current.type == EventType.ValidateCommand)  {
            switch (Event.current.commandName) {
                case "UndoRedoPerformed":
					GUI.changed = true;
                    Repaint ();
					break;
            }
        }

	    Undo.RecordObject((UnityEngine.Object)material, "Modified Terrain Material");

        if (Ferr.EditorTools.ResetHandles()) {
            GUI.changed = true;
        }
        
        EditorGUILayout .BeginHorizontal ();
        EditorGUILayout .BeginVertical   (GUILayout.Width(width));
		GUI.Box(new Rect(0,0, width, position.height), GUIContent.none);

		bool delete = false;
		List<string> segments = new List<string>();
		for (int i = 0; i < material.descriptorCount; i++) {
			if      (i==0) segments.Add("Top");
			else if (i==1) segments.Add("Left");
			else if (i==2) segments.Add("Right");
			else if (i==3) segments.Add("Bottom");
			else segments.Add((i-3).ToString());
		}
		segments.Add("+New edge");
			
		currDir = EditorGUILayout.Popup(currDir, segments.ToArray());
		// new edge was selected!
		if (currDir == segments.Count-1) {
			segments.Insert(segments.Count-1, currDir.ToString());
			material.Add();
		}

		if (currDir != (int)Ferr2DT_TerrainDirection.None) {
            Ferr2DT_TerrainMaterialUtility.ShowSample(material, (Ferr2DT_TerrainDirection)currDir, width-10);
        }

		if (prevDir != currDir) simpleUVs = Ferr2DT_TerrainMaterialUtility.IsSimple(material.GetDescriptor((Ferr2DT_TerrainDirection)currDir));
		EditorGUILayout.BeginHorizontal();
        bool show = GUILayout.Toggle(material.Has((Ferr2DT_TerrainDirection)currDir), "Use " + segments[currDir]);
		delete = GUILayout.Button("Delete");
		EditorGUILayout.EndHorizontal();
        material.Set((Ferr2DT_TerrainDirection)currDir, show);
		if (show) 
            ShowDirection(material, (int)currDir);

        EditorGUILayout.EndVertical  ();
        EditorGUILayout.BeginVertical();
        scroll = EditorGUILayout.BeginScrollView(scroll);
        if (currDir != (int)Ferr2DT_TerrainDirection.None) {
            Ferr2DT_TerrainMaterialUtility.ShowPreview(material, (Ferr2DT_TerrainDirection)currDir, simpleUVs, true, width);
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical  ();
        EditorGUILayout.EndHorizontal();

        if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag)
			Repaint ();

        if (GUI.changed) {
			Ferr2DT_PathTerrainEditor.cachedColliders = null;
	        EditorUtility.SetDirty((UnityEngine.Object)material);

			bool updatedTerrain = false;
            Ferr2DT_PathTerrain[] terrain = GameObject.FindObjectsOfType(typeof(Ferr2DT_PathTerrain)) as Ferr2DT_PathTerrain[];
            for (int i = 0; i < terrain.Length; i++) {
				if (terrain[i].TerrainMaterial == material) {
					terrain[i].Build(true);
					updatedTerrain = true;
				}
            }

			if (updatedTerrain && !Application.isPlaying) {
				EditorSceneManager.MarkAllScenesDirty();
			}
		}

		prevDir = currDir;
		if (delete) {
			material.Remove((Ferr2DT_TerrainDirection)currDir);
			if (currDir > 3)
				currDir -= 1;
		}
    }

	static Vector2 settingsScroll;
    void ShowDirection(IFerr2DTMaterial aMat, int aDir) {
		Ferr2DT_SegmentDescription desc = aMat.GetDescriptor((Ferr2DT_TerrainDirection)aDir);
		
		settingsScroll = EditorGUILayout.BeginScrollView(settingsScroll);
		EditorGUILayout.LabelField("Edge Placement", EditorStyles.boldLabel);
		EditorGUI.indentLevel = 1;
		desc.zOffset        = EditorGUILayout.IntField  ( "Draw Order", Mathf.RoundToInt(desc.zOffset*1000)  )/1000f;
		desc.YOffsetPercent = EditorGUILayout.Slider    ( "Y Offset",            desc.YOffsetPercent, -.5f, .5f);
		desc.yOffset        = EditorGUILayout.FloatField( "[Legacy] Y Offset",   desc.yOffset);
        desc.capOffset      = EditorGUILayout.FloatField( "[Legacy] Cap Offset", desc.capOffset);
		EditorGUI.indentLevel = 0;

		Ferr2DT_TerrainMaterialUtility.EditColliders(desc);

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Edge Segments", EditorStyles.boldLabel);
        simpleUVs = EditorGUILayout.Toggle("Simple Mode", simpleUVs);
        if (simpleUVs) {
            Ferr2DT_TerrainMaterialUtility.EditUVsSimple(aMat, desc);
        } else {
            Ferr2DT_TerrainMaterialUtility.EditUVsComplex(aMat, desc, width, ref currBody);
        }
		EditorGUILayout.EndScrollView();
    }
}