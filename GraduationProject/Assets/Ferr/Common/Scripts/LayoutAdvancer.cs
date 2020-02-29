using UnityEngine;
using System.Collections;

namespace Ferr {
	[System.Serializable]
	public class LayoutAdvancer {
		public enum Direction {
			Vertical,
			Horizontal
		}
		
		[SerializeField]
		private Vector2   mPos;
		[SerializeField]
		private Direction mDirection;
		[SerializeField]
		private Vector2   mPrevious;
		[SerializeField]
		private Vector2   mPrevPos;
		
		public float X { get {return mPos.x;} }
		public float Y { get {return mPos.y;} }
		public Direction Dir {get {return mDirection;}}
		
		public LayoutAdvancer(Vector2 aStartLocation, Direction aDirection) {
			mPos       = aStartLocation;
			mDirection = aDirection;
			mPrevPos   = aStartLocation;
			mPrevious  = Vector2.zero;
		}
		
		public void Step(Vector2 aSize) {
			Step(aSize.x, aSize.y);
		}
		public void Step(float aX, float aY) {
			mPrevPos.x = mPos.x;
			mPrevPos.y = mPos.y;
			if (mDirection == Direction.Horizontal) mPos.x += aX;
			if (mDirection == Direction.Vertical  ) mPos.y += aY;
			mPrevious.x = aX;
			mPrevious.y = aY;
		}
		
		public Rect GetRect() {
			return new Rect(mPrevPos.x, mPrevPos.y, mPrevious.x, mPrevious.y);
		}
		public Rect GetRect(float aOverrideDir) {
			if      (mDirection == Direction.Vertical  ) return new Rect(mPrevPos.x, mPrevPos.y, mPrevious.x, aOverrideDir);
			else                                         return new Rect(mPrevPos.x, mPrevPos.y, aOverrideDir, mPrevious.y);
		}
		public Rect GetRect(float aOverrideWidth, float aOverrideHeight) {
			return new Rect(mPos.x, mPos.y, aOverrideWidth, aOverrideHeight);
		}
		
		public Rect GetRectPad(float aPaddingX, float aPaddingY) {
			return new Rect(mPrevPos.x + aPaddingX, mPrevPos.y + aPaddingY, mPrevious.x - aPaddingX*2, mPrevious.y - aPaddingY*2);
		}
		public Rect GetRectPad(float aPadding) {
			return GetRectPad(aPadding, aPadding);
		}
	}
}