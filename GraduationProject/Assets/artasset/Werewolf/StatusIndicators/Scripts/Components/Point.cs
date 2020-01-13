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
	public class Point : SpellIndicator {

		public override ScalingType Scaling { get { return ScalingType.LengthAndHeight; } }

		/// <summary>
		/// Determine if you want the Splat to be restricted to the Range Indicator bounds. Applies to "Point" Splats only.
		/// </summary>
		[SerializeField]
		protected bool restrictCursorToRange = false;

		/// <summary>
		/// Restrict splat position bound to range from player
		/// </summary>
		private void RestrictCursorToRange() {
			if(Manager != null) {
				if(Vector3.Distance(Manager.transform.position, transform.position) > range)
					transform.position = Manager.transform.position + Vector3.ClampMagnitude(transform.position - Manager.transform.position, range);
			}
		}

		public override void Update() {
	 
			if(restrictCursorToRange)
				RestrictCursorToRange();
		}

		void LateUpdate() {
			// Prevent Splat from spinning due to player rotation
			transform.eulerAngles = new Vector3(90, 0, 0);
		}
	}
}
