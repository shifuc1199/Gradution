/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Linq;
using Werewolf.StatusIndicators.Services;
using System.Collections;

namespace Werewolf.StatusIndicators.Components {
	public class LineMissile : SpellIndicator {

		// Fields

		private float arrowHeadScale;
		private Projector arrowHeadProjector;

		public GameObject ArrowHead;
		public float MinimumRange;

		// Properties

		public override ScalingType Scaling { get { return ScalingType.LengthOnly; } }

		// Methods

		public override void Initialize() {
			base.Initialize();
			arrowHeadProjector = ArrowHead.GetComponent<Projector>();
			arrowHeadScale = arrowHeadProjector.orthographicSize;
		}

		public override void Update() {
			 
		}

		public override void OnValueChanged() {
			base.OnValueChanged();
			arrowHeadProjector.aspectRatio = 1f;
			arrowHeadProjector.orthographicSize = arrowHeadScale;
		}

		/// <summary>
		/// Calculate distance of the Arrow Head from the centre point when scaling.
		/// </summary>
		private float ArrowHeadDistance() {
			return (float)arrowHeadProjector.orthographicSize * 0.96f;
		}
	}
}
