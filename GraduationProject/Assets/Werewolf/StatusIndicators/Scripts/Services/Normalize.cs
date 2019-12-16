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

namespace Werewolf.StatusIndicators.Services {
  public class Normalize {
    public float Portion;
    public float Max;
    public float Factor;
    public float Value;

    public Normalize(float portion, float max) {
      this.Portion = portion;
      this.Max = max;
      this.Factor = Portion / Max;
      this.Value = Mathf.Clamp(Factor, 0, 1f);
    }

    public static float GetValue(float portion, float max) {
      return Mathf.Clamp(portion / max, 0, 1f);
    }
  }
}
