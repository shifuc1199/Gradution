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
using System.Collections;

namespace Werewolf.StatusIndicators.Demo {
	public class CharacterDemo1 : MonoBehaviour {
		public SplatManager Splats { get; set; }

		private int index;

		void Start() {
			Splats = GetComponentInChildren<SplatManager>();
		}

		void Update() {
			if(Input.GetMouseButtonDown(0)) {
				Splats.CancelSpellIndicator();
			}
			if(Input.GetKeyDown(KeyCode.LeftArrow)) {
				index = (int)Mathf.Repeat(index - 1, Splats.SpellIndicators.Length);
				UpdateSelection();
			}
			if(Input.GetKeyDown(KeyCode.RightArrow)) {
				index = (int)Mathf.Repeat(index + 1, Splats.SpellIndicators.Length);
				UpdateSelection();
			}
		}

		private void UpdateSelection() {
			Splats.SelectSpellIndicator(Splats.SpellIndicators[index].name);
			GameObject.FindObjectOfType<SplatName>().GetComponent<Text>().text = index + ": " + Splats.CurrentSpellIndicator.name;
		}
	}
}
