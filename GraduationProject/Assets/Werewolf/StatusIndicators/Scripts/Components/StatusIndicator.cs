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
	public class StatusIndicator : Splat {
		public override ScalingType Scaling { get { return ScalingType.LengthAndHeight; } }

		public int ProgressSteps;

		public override void OnValueChanged() {
			if(ProgressSteps == 0) {
				UpdateProgress(progress);
			} else {
				UpdateProgress(StepProgress());
			}
		}

		/// <summary>
		/// For a staggered fill, such as dotted circles.
		/// </summary>
		private float StepProgress() {
			float stepSize = 1.0f / ProgressSteps;
			int currentStep = Mathf.RoundToInt(progress / stepSize);
			return (currentStep * stepSize) - (stepSize / 15);
		}
	}
}
