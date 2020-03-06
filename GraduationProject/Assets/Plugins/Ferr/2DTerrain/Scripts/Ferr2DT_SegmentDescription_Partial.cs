using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Ferr2DT_SegmentDescription {
	/// <summary> Applies only to terrain segments facing this direction. </summary>
	public Ferr2DT_TerrainDirection ApplyTo { get { return _applyTo; } set { _applyTo = value; } }
    /// <summary> Z Offset, for counteracting depth issues. </summary>
	public float  ZOffset { get { return _zOffset; } set { _zOffset = value; } }
    /// <summary> [Legacy, use YOffsetPercent] Just in case you want to adjust the height of the segment </summary>
	public float  YOffset { get { return _yOffset; } set { _yOffset = value; } }
	/// <summary> Just in case you want to adjust the height of the segment </summary>
	public float  YOffsetPercent { get { return _yOffsetPercent; } set { _yOffsetPercent = value; } }
	/// <summary> [Legacy] How much should the end of the path slide to make room for the caps? (Unity units) </summary>
	public float  CapOffset { get { return _capOffset; } set{ _capOffset = value; } }

	public float  ColliderOffset    { get { return _colliderOffset;    } set{ _colliderOffset    = value; } }
	public float  ColliderThickness { get { return _colliderThickness; } set{ _colliderThickness = value; } }

	public PhysicMaterial    PhysicsMaterial3D { get{ return _physicsMaterial3D; } set { _physicsMaterial3D = value; } }
	public PhysicsMaterial2D PhysicsMaterial2D { get{ return _physicsMaterial2D; } set { _physicsMaterial2D = value; } }

	public float SingleColliderCapSize { 
		get { return SafeGet(_leftCapColliderSize,    1, 0); }
		set { SafeSet(ref _leftCapColliderSize,       value, 0);
			  SafeSet(ref _innerLeftCapColliderSize,  value, 0);
			  SafeSet(ref _rightCapColliderSize,      value, 0);
			  SafeSet(ref _innerRightCapColliderSize, value, 0);} }
	public float SingleColliderCapOffset { 
		get { return SafeGet(_leftCapOffset,    0, 0); }
		set { SafeSet(ref _leftCapOffset,       value, 0);
			  SafeSet(ref _innerLeftCapOffset,  value, 0);
			  SafeSet(ref _rightCapOffset,      value, 0);
			  SafeSet(ref _innerRightCapOffset, value, 0);} }
	public Ferr2D_CapColliderType SingleColliderCapType { 
		get { return (Ferr2D_CapColliderType)SafeGet(_leftCapColliderType,    (int)Ferr2D_CapColliderType.Rectangle, 0); }
		set { SafeSet(ref _leftCapColliderType,       (int)value, 0);
			  SafeSet(ref _innerLeftCapColliderType,  (int)value, 0);
			  SafeSet(ref _rightCapColliderType,      (int)value, 0);
			  SafeSet(ref _innerRightCapColliderType, (int)value, 0);} }

	public float EditorLeftCapOffset       { get{ return SafeGet(_leftCapOffset, 0,0); } set { SafeSet(ref _leftCapOffset,  value, 0); } }
	public float EditorRightCapOffset      { get{ return SafeGet(_rightCapOffset,0,0); } set { SafeSet(ref _rightCapOffset, value, 0); } }
	public float EditorInnerLeftCapOffset  { get{ return SafeGet(_innerLeftCapOffset, 0,0); } set { SafeSet(ref _innerLeftCapOffset,  value, 0); } }
	public float EditorInnerRightCapOffset { get{ return SafeGet(_innerRightCapOffset,0,0); } set { SafeSet(ref _innerRightCapOffset, value, 0); } }

	public int EditorLeftCapType       { get{ return SafeGet(_leftCapColliderType, 0, 0); } set { SafeSet(ref _leftCapColliderType,  value, 0); } }
	public int EditorRightCapType      { get{ return SafeGet(_rightCapColliderType,0, 0); } set { SafeSet(ref _rightCapColliderType, value, 0); } }
	public int EditorInnerLeftCapType  { get{ return SafeGet(_innerLeftCapColliderType, 0,0); } set { SafeSet(ref _innerLeftCapColliderType,  value, 0); } }
	public int EditorInnerRightCapType { get{ return SafeGet(_innerRightCapColliderType,0,0); } set { SafeSet(ref _innerRightCapColliderType, value, 0); } }

	public float EditorLeftCapColliderSize       { get{ return SafeGet(_leftCapColliderSize, 0,0); } set { SafeSet(ref _leftCapColliderSize,  value, 0); } }
	public float EditorRightCapColliderSize      { get{ return SafeGet(_rightCapColliderSize,0,0); } set { SafeSet(ref _rightCapColliderSize, value, 0); } }
	public float EditorInnerLeftCapColliderSize  { get{ return SafeGet(_innerLeftCapColliderSize, 0,0); } set { SafeSet(ref _innerLeftCapColliderSize,  value, 0); } }
	public float EditorInnerRightCapColliderSize { get{ return SafeGet(_innerRightCapColliderSize,0,0); } set { SafeSet(ref _innerRightCapColliderSize, value, 0); } }
	
	public int BodyCount { get { return _bodies.Length; } }
	
	public bool HasInnerLeft() {
		Rect innerCap = SafeGet(_innerLeftCaps, innerLeftCap, 0);
		return innerCap.width!=0 || innerCap.height!=0;
	}
	public bool HasInnerRight() {
		Rect innerCap = SafeGet(_innerRightCaps, innerRightCap, 0);
		return innerCap.width!=0 || innerCap.height!=0;
	}
	public Ferr2D_CapColliderType GetLeftCapType (bool aInner, int aId=-1) {
		int[] from = (aInner && HasInnerLeft()) ? 
			_innerLeftCapColliderType : 
			_leftCapColliderType;
		return (Ferr2D_CapColliderType)SafeGet(from, (int)Ferr2D_CapColliderType.Rectangle, aId==-1?0:aId);
	}
	public Ferr2D_CapColliderType GetRightCapType(bool aInner, int aId=-1) {
		int[] from = (aInner && HasInnerRight()) ? 
			_innerRightCapColliderType : 
			_rightCapColliderType;
		return (Ferr2D_CapColliderType)SafeGet(from, (int)Ferr2D_CapColliderType.Rectangle, aId==-1?0:aId);
	}
	public float GetLeftCapOffset (bool aInner, Vector2 aUnitsPerUV, int aId=-1) {
		Rect result = default(Rect);
		if (aId == -1) aId = 0;

		if (aInner && HasInnerLeft()) {
			result           = SafeGet(_innerLeftCaps, innerLeftCap, aId);
			return capOffset + SafeGet(_innerLeftCapOffset, 0, aId) * (result.width==0?GetBodyMaxHeight()*2:result.width) * aUnitsPerUV.x;
		} else {
			result           = SafeGet(_leftCaps,      leftCap, aId);
			return capOffset + SafeGet(_leftCapOffset,      0, aId) * (result.width==0?GetBodyMaxHeight()*2:result.width) * aUnitsPerUV.x;
		}
	}
	public float GetRightCapOffset(bool aInner, Vector2 aUnitsPerUV, int aId=-1) {
		Rect result = default(Rect);
		if (aId == -1) aId = 0;

		if (aInner && HasInnerRight()) {
			result           = SafeGet(_innerRightCaps, innerRightCap, aId);
			return capOffset + SafeGet(_innerRightCapOffset, 0, aId) * (result.width==0?GetBodyMaxHeight()*2:result.width) * aUnitsPerUV.x;
		} else {
			result           = SafeGet(_rightCaps,      rightCap, aId);
			return capOffset + SafeGet(_rightCapOffset,      0, aId) * (result.width==0?GetBodyMaxHeight()*2:result.width) * aUnitsPerUV.x;
		}
	}
	public float GetLeftCapSize   (bool aInner, Vector2 aUnitsPerUV, int aId=-1) {
		Rect result = default(Rect);
		if (aId == -1) aId = 0;

		if (aInner && HasInnerLeft()) {
			result = SafeGet(_innerLeftCaps, innerLeftCap, aId);
			float size = SafeGet(_innerLeftCapColliderSize, 1, aId);
			return ( result.width==0 ? (size-1)*GetBodyMaxHeight()*2 : size*result.width ) * aUnitsPerUV.x;
		} else {
			result = SafeGet(_leftCaps,      leftCap, aId);
			float size = SafeGet(_leftCapColliderSize, 1, aId);
			return ( result.width==0 ? (size-1)*GetBodyMaxHeight()*2 : size*result.width ) * aUnitsPerUV.x;
		}
	}
	public float GetRightCapSize  (bool aInner, Vector2 aUnitsPerUV, int aId=-1) {
		Rect result = default(Rect);
		if (aId == -1) aId = 0;

		if (aInner && HasInnerRight()) {
			result = SafeGet(_innerRightCaps, innerRightCap, aId);
			float size = SafeGet(_innerRightCapColliderSize, 1, aId);
			return ( result.width==0 ? (size-1)*GetBodyMaxHeight()*2 : size*result.width ) * aUnitsPerUV.x;
		} else {
			result = SafeGet(_rightCaps, rightCap, aId);
			float size = SafeGet(_rightCapColliderSize, 1, aId);
			return ( result.width==0 ? (size-1)*GetBodyMaxHeight()*2 : size*result.width ) * aUnitsPerUV.x;
		}
	}

	public Rect GetLeftCap (bool aInner, Vector2 aUnitsPerUV, out Ferr2D_CapColliderType aColliderType, out float aColliderCapSize, out float aCapOffset, int aId=-1) {
		Rect result = default(Rect);
		aColliderCapSize = 1;
		aColliderType = 0;
		aCapOffset = 0;
		if (aId == -1) aId = 0;

		if (aInner) {
			result = SafeGet(_innerLeftCaps, innerLeftCap, aId);
			float width = (result.width==0?GetBodyMaxHeight()*2:result.width) * aUnitsPerUV.x;

			aColliderCapSize =             SafeGet(_innerLeftCapColliderSize, 1, aId);
			aColliderCapSize = (result.width==0?aColliderCapSize-1:aColliderCapSize) * width;
			aCapOffset       = capOffset + SafeGet(_innerLeftCapOffset,       0, aId) * width;
			aColliderType    = (Ferr2D_CapColliderType)SafeGet(_innerLeftCapColliderType, (int)Ferr2D_CapColliderType.Rectangle, aId);
			
		}
		if (!aInner || (result.width==0 && result.height==0)) {
			result = SafeGet(_leftCaps, leftCap, aId);
			float width = (result.width==0?GetBodyMaxHeight()*2:result.width) * aUnitsPerUV.x;

			aColliderCapSize =             SafeGet(_leftCapColliderSize, 1, aId);
			aColliderCapSize = (result.width==0?aColliderCapSize-1:aColliderCapSize) * width;
			aCapOffset       = capOffset + SafeGet(_leftCapOffset,       0, aId) * width;
			aColliderType    = (Ferr2D_CapColliderType)SafeGet(_leftCapColliderType, (int)Ferr2D_CapColliderType.Rectangle, aId);
		}
		return result;
	}
	public Rect GetRightCap(bool aInner, Vector2 aUnitsPerUV, out Ferr2D_CapColliderType aColliderType, out float aColliderCapSize, out float aCapOffset, int aId=-1) {
		Rect result = default(Rect);
		aColliderCapSize = 1;
		aColliderType = 0;
		aCapOffset = 0;
		if (aId == -1) aId = 0;

		if (aInner) {
			result = SafeGet(_innerRightCaps, innerRightCap, aId);
			float width = (result.width==0?GetBodyMaxHeight()*2:result.width) * aUnitsPerUV.x;

			aColliderCapSize =             SafeGet(_innerRightCapColliderSize, 1, aId);
			aColliderCapSize = (result.width==0?aColliderCapSize-1:aColliderCapSize) * width;
			aCapOffset       = capOffset + SafeGet(_innerRightCapOffset,       0, aId) * width;
			aColliderType    = (Ferr2D_CapColliderType)SafeGet(_innerRightCapColliderType, (int)Ferr2D_CapColliderType.Rectangle, aId);
		}
		if (!aInner || (result.width==0 && result.height==0)) {
			result = SafeGet(_rightCaps, rightCap, aId);
			float width = (result.width==0?GetBodyMaxHeight()*2:result.width) * aUnitsPerUV.x;

			aColliderCapSize =              SafeGet(_rightCapColliderSize, 1, aId);
			aColliderCapSize = (result.width==0?aColliderCapSize-1:aColliderCapSize) * width;
			aCapOffset       = capOffset +  SafeGet(_rightCapOffset,       0, aId) * width;
			aColliderType    = (Ferr2D_CapColliderType)SafeGet(_rightCapColliderType, (int)Ferr2D_CapColliderType.Rectangle, aId);
		}
		return result;
	}
	
	private T    SafeGet<T>(T[] aSource, T aDefault, int aIndex) {
		if (aSource == null || aSource.Length <= aIndex)
			return aDefault;
		return aSource[aIndex];
	}
	private void SafeSet<T>(ref T[] aSource, T aValue, int aIndex) {
		if (aSource == null)
			aSource = new T[1];
		if (aSource.Length <= aIndex)
			System.Array.Resize(ref aSource, aIndex + 1);
		aSource[aIndex] = aValue;
	}
	
	public float GetBodyWeight(int aBodyId) {
		if (_bodyWeights == null || aBodyId >= _bodyWeights.Length) {
			return 1;
		} else {
			float weight = _bodyWeights[aBodyId];
			if (weight == 0) weight=1;
			if (weight <  0) weight=0;
			return weight;
		}
	}
	public int   GetRandomBodyId(float aRandValue01) {
		float total = 0;
		for (int i = 0; i < _bodies.Length; i++) {
			total += GetBodyWeight(i);
		}

		float val = aRandValue01 * total;
		total = 0;
		for (int i = 0; i < _bodies.Length; i++) {
			total += GetBodyWeight(i);
			if (val <= total)
				return i;
		}
		return 0;
	}
	public Rect  GetBody(int aBodyId) {
		return _bodies[aBodyId];
	}

	public float GetYOffset(bool aInvert, Vector2 aUnitsPerUV) {
		float result = -yOffset + YOffsetPercent * GetBodyMaxHeight() * aUnitsPerUV.y;
		if (aInvert) {
			result = -result;
		}
		return result;
	}
	public float GetYOffsetCollider(bool aInvert, Vector2 aUnitsPerUV) {
		float height = GetBodyMaxHeight() * aUnitsPerUV.y;
		
		float   thickness = ColliderThickness * height;
		float   result = 0;
		if (aInvert) {
			result  = yOffset - height*.5f + thickness;
			result -= ColliderOffset*height;
		} else {
			result  = -yOffset + height*.5f;
			result += ColliderOffset*height;
		}

		return result;
	}
	public float GetBodyMaxHeight() {
		float result = 0;
		for (int i = 0; i < _bodies.Length; i++) {
			if (_bodies[i].height > result)
				result = _bodies[i].height;
		}
		return result;
	}
}
