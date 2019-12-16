/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections;
using System.Linq;
using Werewolf.StatusIndicators.Services;

namespace Werewolf.StatusIndicators.Components {
	public abstract class SpellIndicator : Splat {

		// Fields

		/// <summary>
		/// Special indicator for displaying range, unselectable.
		/// </summary>
		public RangeIndicator RangeIndicator;

		/// <summary>
		/// Set the size of the Range Indicator and bounds of Spell Cursor.
		/// </summary>
		[SerializeField]
		protected float range = 5f;

		/// <summary>
		/// Set the size of the Range Indicator and bounds of Spell Cursor.
		/// </summary>
		public float Range {
			get { return range; }
			set { Debug.Log("123"); SetRange(value); }
		}

		public override void OnShow() {
			UpdateRangeIndicatorSize();
		}

		/// <summary>
		/// Set the size of the Range Indicator and bounds of Spell Cursor.
		/// </summary>
		/// <param name="range">Range of spell</param>
		public void SetRange(float range) {
			this.range = range;
			UpdateRangeIndicatorSize();
		}

		/// <summary>
		/// Get the vector that is on the same y position as the subject to get a more accurate angle.
		/// </summary>
		/// <param name="target">The target point which we are trying to adjust against</param>
		protected Vector3 FlattenVector(Vector3 target) {
			return new Vector3(target.x, Manager.transform.position.y, target.z);
		}

		/// <summary>
		/// Scale Range Indicator to be same as Splat Range.
		/// </summary>
		private void UpdateRangeIndicatorSize() {
            Debug.Log("123");
			if(RangeIndicator != null)
				RangeIndicator.Scale = range * 2.1f;
		}
	}
}
