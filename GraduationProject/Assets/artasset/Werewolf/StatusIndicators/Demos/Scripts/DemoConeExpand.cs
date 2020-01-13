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

namespace Werewolf.StatusIndicators.Demo {

  public class DemoConeExpand : MonoBehaviour {
    private Cone spellIndicator;

    void Start() {
      spellIndicator = GetComponent<Cone>();
    }

    void Update() {
      spellIndicator.Angle = Mathf.PingPong(Time.time * 100f, 320f) + 40f;
    }
  }
}
