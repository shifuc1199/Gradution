using UnityEngine;
using System.Collections;
using System;

public partial class Ferr2DT_Material : ScriptableObject, IFerr2DTMaterial {
	#region Fields
	[SerializeField] Material                     _fillMaterial;
	[SerializeField] Material                     _edgeMaterial;
	[SerializeField] private Ferr2DT_SegmentDescription[] _descriptors = new Ferr2DT_SegmentDescription[4];
	[SerializeField] private bool isPixel = true;
	[SerializeField] private bool _isPerfect = false;
	
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
	public int descriptorCount { get{ return _descriptors.Length; } }
	/// <summary>
	/// Is this material ok to use for perfect edges?
	/// </summary>
	public bool IsPerfect{ get { return _isPerfect; } }
    #endregion
	
    #region Constructor
	public Ferr2DT_Material() {
		for (int i = 0; i < _descriptors.Length; i++) {
			_descriptors[i] = new Ferr2DT_SegmentDescription();
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
		for (int i = 0; i < _descriptors.Length; i++) {
			if (_descriptors[i].applyTo == aDirection) return _descriptors[i];
		}
		if (_descriptors.Length > 0) {
			return _descriptors[0];
		}
		return new Ferr2DT_SegmentDescription();
	}
    /// <summary>
    /// Finds out if we actually have a descriptor for the given direction
    /// </summary>
    /// <param name="aDirection">Duh.</param>
    /// <returns>is it there, or is it not?</returns>
	public bool                       Has          (Ferr2DT_TerrainDirection aDirection) {
		for (int i = 0; i < _descriptors.Length; i++) {
			if (_descriptors[i].applyTo == aDirection) return true;
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
			if (_descriptors[(int)aDirection].applyTo != aDirection) {
				_descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
				_descriptors[(int)aDirection].applyTo = aDirection;
			}
		} else if (_descriptors[(int)aDirection].applyTo != Ferr2DT_TerrainDirection.Top) {
			_descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
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
			for (int i = 0; i < _descriptors.Length; i++) {
				for (int t = 0; t < _descriptors[i].body.Length; t++) {
					_descriptors[i].body[t] = ToNative(_descriptors[i].body[t]);
				}
				_descriptors[i].leftCap  = ToNative(_descriptors[i].leftCap );
				_descriptors[i].rightCap = ToNative(_descriptors[i].rightCap);
			}
			isPixel = false;
		}
	}
	public Rect ToNative(Rect aPixelRect) {
		if (edgeMaterial == null) return aPixelRect;
		
		int w = edgeMaterial.mainTexture == null ? 1 : edgeMaterial.mainTexture.width;
		int h = edgeMaterial.mainTexture == null ? 1 : edgeMaterial.mainTexture.height;
		
		return new Rect(
			aPixelRect.x      / w,
			aPixelRect.y      / h,
			aPixelRect.width  / w,
			aPixelRect.height / h);
	}
	public Rect ToPixels(Rect aNativeRect) {
		if (edgeMaterial == null) return aNativeRect;
		
		int w = edgeMaterial.mainTexture == null ? 1 : edgeMaterial.mainTexture.width;
		int h = edgeMaterial.mainTexture == null ? 1 : edgeMaterial.mainTexture.height;
		
		return new Rect(
			aNativeRect.x      * w,
			aNativeRect.y      * h,
			aNativeRect.width  * w,
			aNativeRect.height * h);
	}

	public void Add() {
		Array.Resize(ref _descriptors, _descriptors.Length+1);
		var newSegment = new Ferr2DT_SegmentDescription();
		newSegment.applyTo = (Ferr2DT_TerrainDirection)_descriptors.Length-1;
		_descriptors[_descriptors.Length-1] = newSegment;
	}
	public void Remove(Ferr2DT_TerrainDirection aDirection) {
		if ((int)aDirection <= 3) {
			Set(aDirection, false);
			return;
		}

		for (int i = (int)aDirection; i < _descriptors.Length-1; i++) {
			_descriptors[i] = _descriptors[i+1];
		}
		Array.Resize(ref _descriptors, _descriptors.Length-1);
	}
    #endregion
}