using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

/// <summary>
/// A direction used to describe the surface of terrain.
/// </summary>
public enum Ferr2DT_TerrainDirection
{
	Top    = 0,
	Left   = 1,
	Right  = 2,
	Bottom = 3,
	None   = 100
}

/// <summary>
/// Describes a terrain segment, and how it should be drawn.
/// </summary>
[System.Serializable]
public partial class Ferr2DT_SegmentDescription {
	#region Legacy Access Properties
	/// <summary>
	/// [Legacy] Applies only to terrain segments facing this direction.
	/// </summary>
	public Ferr2DT_TerrainDirection applyTo { get { return _applyTo; } set { _applyTo = value; } }
    /// <summary>
    /// [Legacy] Z Offset, for counteracting depth issues.
    /// </summary>
	public float  zOffset { get { return _zOffset; } set { _zOffset = value; } }
    /// <summary>
    /// [Legacy] Just in case you want to adjust the height of the segment
    /// </summary>
	public float  yOffset { get { return _yOffset; } set { _yOffset = value; } }
	/// <summary>
    /// [Legacy] How much should the end of the path slide to make room for the caps? (Unity units)
    /// </summary>
	public float  capOffset { get { return _capOffset; } set{ _capOffset = value; } }
    /// <summary>
    /// [Legacy] UV coordinates for the left ending cap.
    /// </summary>
	public Rect   leftCap { 
		get { if ( _leftCaps==null || _leftCaps.Length<=0) return _legacyLeftCap; return _leftCaps[0]; } 
		set { if ( _leftCaps==null || _leftCaps.Length<=0) _leftCaps = new Rect[1]; _leftCaps[0]=value;} }
	/// <summary>
    /// [Legacy] UV coordinates for the left ending cap.
    /// </summary>
	public Rect   innerLeftCap { 
		get { if ( _innerLeftCaps==null || _innerLeftCaps.Length<=0) return _legacyInnerLeftCap; return _innerLeftCaps[0]; } 
		set { if ( _innerLeftCaps==null || _innerLeftCaps.Length<=0) _innerLeftCaps = new Rect[1]; _innerLeftCaps[0]=value;} }
    /// <summary>
    /// [Legacy] UV coordinates for the right ending cap.
    /// </summary>
	public Rect   rightCap { 
		get { if ( _rightCaps==null || _rightCaps.Length<=0) return _legacyRightCap; return _rightCaps[0]; } 
		set { if ( _rightCaps==null || _rightCaps.Length<=0) _rightCaps = new Rect[1]; _rightCaps[0]=value;} }
	/// <summary>
    /// [Legacy] UV coordinates for the right ending cap.
    /// </summary>
	public Rect   innerRightCap { 
		get { if ( _innerRightCaps==null || _innerRightCaps.Length<=0) return _legacyInnerRightCap; return _innerRightCaps[0]; } 
		set { if ( _innerRightCaps==null || _innerRightCaps.Length<=0) _innerRightCaps = new Rect[1]; _innerRightCaps[0]=value;} }
    /// <summary>
    /// [Legacy] A list of body UVs to randomly pick from.
    /// </summary>
	public Rect[] body{ get{ return _bodies; } set{ _bodies=value; } }
	#endregion

	#region Fields
	[FormerlySerializedAs("applyTo")]
	[SerializeField] Ferr2DT_TerrainDirection _applyTo;
	[FormerlySerializedAs("zOffset")]
	[SerializeField] float  _zOffset;
	[FormerlySerializedAs("yOffset")]
	[SerializeField] float  _yOffset;
	[FormerlySerializedAs("capOffset")]
	[SerializeField] float  _capOffset = 0f;

	[FormerlySerializedAs("leftCap")]
	[SerializeField] Rect    _legacyLeftCap;
	[SerializeField] Rect [] _leftCaps;
	[SerializeField] float[] _leftCapWeights;
	[SerializeField] float[] _leftCapColliderSize;
	[SerializeField] int[]   _leftCapColliderType;
	[SerializeField] float[] _leftCapOffset;

	[FormerlySerializedAs("innerLeftCap")]
	[SerializeField] Rect    _legacyInnerLeftCap;
	[SerializeField] Rect [] _innerLeftCaps;
	[SerializeField] float[] _innerLeftCapWeights;
	[SerializeField] float[] _innerLeftCapColliderSize;
	[SerializeField] int[]   _innerLeftCapColliderType;
	[SerializeField] float[] _innerLeftCapOffset;

	[FormerlySerializedAs("rightCap")]
	[SerializeField] Rect    _legacyRightCap;
	[SerializeField] Rect [] _rightCaps;
	[SerializeField] float[] _rightCapWeights;
	[SerializeField] float[] _rightCapColliderSize;
	[SerializeField] int[]   _rightCapColliderType;
	[SerializeField] float[] _rightCapOffset;

	[FormerlySerializedAs("innerRightCap")]
	[SerializeField] Rect    _legacyInnerRightCap;
	[SerializeField] Rect [] _innerRightCaps;
	[SerializeField] float[] _innerRightCapWeights;
	[SerializeField] float[] _innerRightCapColliderSize;
	[SerializeField] int[]   _innerRightCapColliderType;
	[SerializeField] float[] _innerRightCapOffset;

	[FormerlySerializedAs("body")]
	[SerializeField] Rect [] _bodies;
	[SerializeField] float[] _bodyWeights;
	
	[SerializeField] float _yOffsetPercent;
	// Ferr2D includes a partial class that uses these fields, warning won't be present when Ferr2D is also present
	#pragma warning disable 0414
	[SerializeField] float _colliderOffset    = 0;
	[SerializeField] float _colliderThickness = 1;
	#pragma warning restore 0414
	[SerializeField] PhysicsMaterial2D _physicsMaterial2D;
	[SerializeField] PhysicMaterial    _physicsMaterial3D;
	#endregion
	
	public Ferr2DT_SegmentDescription() {
		body    = new Rect[] { new Rect(0,0,50,50) };
		applyTo = Ferr2DT_TerrainDirection.Top;
	}
}