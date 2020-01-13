/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using Werewolf.StatusIndicators.Components;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Werewolf.StatusIndicators.Demo {
	public class CharacterDemo5 : MonoBehaviour {
		public SplatManager Splats { get; set; }

		void Start() {
			Splats = GetComponentInChildren<SplatManager>();
		}

		void Update() {
			if(Input.GetMouseButtonDown(0)) {
				Splats.CancelRangeIndicator();
			}
			if(Input.GetKeyDown(KeyCode.Q)) {
				Splats.SelectRangeIndicator("Range1");
				Splats.CurrentRangeIndicator.Scale = 14f;
			}
			if(Input.GetKeyDown(KeyCode.W)) {
				Splats.SelectRangeIndicator("Range2");
				Splats.CurrentRangeIndicator.Scale = 15f;
			}
			if(Input.GetKeyDown(KeyCode.E)) {
				Splats.SelectRangeIndicator("Range3");
				Splats.CurrentRangeIndicator.Scale = 16f;
			}
			if(Input.GetKeyDown(KeyCode.S)) {
				Splats.SelectRangeIndicator("RangeSmall");
				Splats.CurrentRangeIndicator.Scale = 10f;
			}
			if(Input.GetKeyDown(KeyCode.D)) {
				Splats.SelectRangeIndicator("RangeBasic");
				Splats.CurrentRangeIndicator.Scale = 12f;
			}
		}
	}
}
