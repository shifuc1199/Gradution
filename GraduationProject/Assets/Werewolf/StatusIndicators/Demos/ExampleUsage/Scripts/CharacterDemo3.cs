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
  public class CharacterDemo3 : MonoBehaviour {
    public SplatManager Splats { get; set; }

    void Start() {
      Splats = GetComponentInChildren<SplatManager>();
    }

    void Update() {
      if (Input.GetMouseButtonDown(0)) {
        Splats.CancelSpellIndicator();
        Splats.CancelRangeIndicator();
        Splats.CancelStatusIndicator();
      }
      if (Input.GetKeyDown(KeyCode.Q)) {
        Splats.SelectSpellIndicator("Point");
      }
      if (Input.GetKeyDown(KeyCode.W)) {
        Splats.SelectSpellIndicator("Cone");
      }
      if (Input.GetKeyDown(KeyCode.E)) {
        Splats.SelectSpellIndicator("Direction");
      }
      if (Input.GetKeyDown(KeyCode.R)) {
        Splats.SelectSpellIndicator("Line");
      }
      if (Input.GetKeyDown(KeyCode.S)) {
        Splats.SelectStatusIndicator("Status");
      }
      if (Input.GetKeyDown(KeyCode.D)) {
        Splats.SelectRangeIndicator("Range");
      }
    }
  }
}
