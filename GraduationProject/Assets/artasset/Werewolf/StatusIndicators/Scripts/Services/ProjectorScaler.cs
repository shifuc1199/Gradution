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
using Werewolf.StatusIndicators.Components;

namespace Werewolf.StatusIndicators.Services {
  public class ProjectorScaler {
    public static void Resize(Projector projector, float scale) {
      if (projector != null)
        projector.orthographicSize = scale / 2;
    }

    public static void Resize(Projector projector, ScalingType scaling, float scale, float width) {
      if (projector != null) {
        if (scaling != ScalingType.None) {
          if (scaling == ScalingType.LengthOnly) {
            projector.aspectRatio = width / scale;
          } else {
            projector.aspectRatio = 1f;
          }
          projector.orthographicSize = scale / 2;
        }
      }
    }

    public static void Resize(Projector[] projectors, ScalingType scaling, float scale, float width) {
      foreach (Projector p in projectors)
        Resize(p, scaling, scale, width);
    }

    public static void Resize(Projector[] projectors, float scale) {
      foreach (Projector p in projectors)
        Resize(p, scale);
    }
  }
}
