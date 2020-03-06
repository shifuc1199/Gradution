using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Describes a material that can be applied to a Ferr2DT_PathTerrain
/// </summary>
public class Ferr2DT_TerrainMaterial : MonoBehaviour, IFerr2DTMaterial
{
    #region Fields
	[SerializeField, UnityEngine.Serialization.FormerlySerializedAs("fillMaterial")]
	private Material                     _fillMaterial;
	[SerializeField, UnityEngine.Serialization.FormerlySerializedAs("edgeMaterial")]
	private Material                     _edgeMaterial;
    [SerializeField]
    private Ferr2DT_SegmentDescription[] descriptors = new Ferr2DT_SegmentDescription[4];
    [SerializeField]
	private bool isPixel = true;
	
	/// <summary>
    /// The material of the interior of the terrain.
    /// </summary>
	public Material fillMaterial { get{return _fillMaterial;} set{_fillMaterial = value;} }
	/// <summary>
    /// The material of the edges of the terrain.
    /// </summary>
	public Material edgeMaterial { get{return _edgeMaterial;} set{_edgeMaterial = value;} }
	/// <summary>
	/// How many edge descriptors are present in the material?
	/// </summary>
	public int descriptorCount { get{ return descriptors.Length; } }
    #endregion

    #region Constructor
    public Ferr2DT_TerrainMaterial() {
        for (int i = 0; i < descriptors.Length; i++) {
            descriptors[i] = new Ferr2DT_SegmentDescription();
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Gets the edge descriptor for the given edge, defaults to the Top, if none by that type exists, or an empty one, if none are defined at all.
    /// </summary>
    /// <param name="aDirection">Direction to get.</param>
    /// <returns>The given direction, or the first direction, or a default, based on what actually exists.</returns>
    public Ferr2DT_SegmentDescription GetDescriptor(Ferr2DT_TerrainDirection aDirection) {
        ConvertToPercentage();
        for (int i = 0; i < descriptors.Length; i++) {
            if (descriptors[i].applyTo == aDirection) return descriptors[i];
        }
        if (descriptors.Length > 0) {
            return descriptors[0];
        }
        return new Ferr2DT_SegmentDescription();
    }
    /// <summary>
    /// Finds out if we actually have a descriptor for the given direction
    /// </summary>
    /// <param name="aDirection">Duh.</param>
    /// <returns>is it there, or is it not?</returns>
	public bool                       Has          (Ferr2DT_TerrainDirection aDirection) {
		for (int i = 0; i < descriptors.Length; i++) {
            if (descriptors[i].applyTo == aDirection) return true;
        }
		return false;
	}
    /// <summary>
    /// Sets a particular direction as having a valid descriptor. Or not. That's a bool.
    /// </summary>
    /// <param name="aDirection">The direction!</param>
    /// <param name="aActive">To active, or not to active? That is the question!</param>
	public void                       Set          (Ferr2DT_TerrainDirection aDirection, bool aActive) {
		if (aActive) {
			if (descriptors[(int)aDirection].applyTo != aDirection) {
				descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
				descriptors[(int)aDirection].applyTo = aDirection;
			}
		} else if (descriptors[(int)aDirection].applyTo != Ferr2DT_TerrainDirection.Top) {
			descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
		}
	}
    /// <summary>
    /// Converts our internal pixel UV coordinates to UV values Unity will recognize.
    /// </summary>
    /// <param name="aNativeRect">A UV rect, using pixels.</param>
    /// <returns>A UV rect using Unity coordinates.</returns>
	public Rect                       ToUV    (Rect aNativeRect) {
		if (edgeMaterial == null) return aNativeRect;
        return new Rect(
            aNativeRect.x ,
            (1.0f - aNativeRect.height) - aNativeRect.y,
            aNativeRect.width,
            aNativeRect.height);
	}
    /// <summary>
    /// Converts our internal pixel UV coordinates to UV values we can use on the screen! As 0-1.
    /// </summary>
    /// <param name="aNativeRect">A UV rect, using pixels.</param>
    /// <returns>A UV rect using standard UV coordinates.</returns>
	public Rect                       ToScreen(Rect aNativeRect) {
		if (edgeMaterial == null) return aNativeRect;
        return aNativeRect;
    }

    public Rect GetBody     (Ferr2DT_TerrainDirection aDirection, int aBodyID) {
        return GetDescriptor(aDirection).body[aBodyID];
    }

    private void ConvertToPercentage() {
        if (isPixel) {
            for (int i = 0; i < descriptors.Length; i++) {
                for (int t = 0; t < descriptors[i].body.Length; t++) {
                    descriptors[i].body[t] = ToNative(descriptors[i].body[t]);
                }
                descriptors[i].leftCap  = ToNative(descriptors[i].leftCap );
                descriptors[i].rightCap = ToNative(descriptors[i].rightCap);
            }
            isPixel = false;
        }
    }
    public Rect ToNative(Rect aPixelRect) {
        if (edgeMaterial == null) return aPixelRect;
        return new Rect(
            aPixelRect.x      / edgeMaterial.mainTexture.width,
            aPixelRect.y      / edgeMaterial.mainTexture.height,
            aPixelRect.width  / edgeMaterial.mainTexture.width,
            aPixelRect.height / edgeMaterial.mainTexture.height);
    }
    public Rect ToPixels(Rect aNativeRect) {
        if (edgeMaterial == null) return aNativeRect;
        return new Rect(
            aNativeRect.x      * edgeMaterial.mainTexture.width,
            aNativeRect.y      * edgeMaterial.mainTexture.height,
            aNativeRect.width  * edgeMaterial.mainTexture.width,
            aNativeRect.height * edgeMaterial.mainTexture.height);
    }

	public void Add() {
		Array.Resize(ref descriptors, descriptors.Length+1);
		var newSegment = new Ferr2DT_SegmentDescription();
		newSegment.applyTo = (Ferr2DT_TerrainDirection)descriptors.Length-1;
		descriptors[descriptors.Length-1] = newSegment;
	}
	public void Remove(Ferr2DT_TerrainDirection aDirection) {
		if ((int)aDirection <= 3) {
			Set(aDirection, false);
			return;
		}

		for (int i = (int)aDirection; i < descriptors.Length-1; i++) {
			descriptors[i] = descriptors[i+1];
		}
		Array.Resize(ref descriptors, descriptors.Length-1);
	}

    public Ferr2DT_Material CreateNewFormatMaterial() {
        Ferr2DT_Material result = ScriptableObject.CreateInstance<Ferr2DT_Material>();

        result.edgeMaterial = _edgeMaterial;
        result.fillMaterial = _fillMaterial;

        for (int i = 0; i<4; i+=1) {
            Ferr2DT_TerrainDirection   dir  = (Ferr2DT_TerrainDirection)i;
            if (!Has(dir)) continue;
            result.Set(dir, true);
            Ferr2DT_SegmentDescription dest = result.GetDescriptor(dir);
            Ferr2DT_SegmentDescription src  = GetDescriptor(dir);
            dest.applyTo       = src.applyTo;
            dest.body          = src.body;
            dest.capOffset     = src.capOffset;
            dest.innerLeftCap  = src.innerLeftCap;
            dest.innerRightCap = src.innerRightCap;
            dest.leftCap       = src.leftCap;
            dest.rightCap      = src.rightCap;
            dest.yOffset       = src.yOffset;
            dest.zOffset       = src.zOffset;
        }

        return result;
    }
    #endregion
}
