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
using UnityEditor;
using System;
using System.Linq;
using Werewolf.StatusIndicators.Components;

namespace Werewolf.StatusIndicators.Editors {
  public class SplatEditor<T> : Editor where T : Splat {
    private T instance { get { return (T)target; } }

    public override void OnInspectorGUI() {
      if (instance == null)
        return;

      EditorGUI.BeginChangeCheck();

      DrawDefaultInspector();

      if (EditorGUI.EndChangeCheck()) {
        if (instance.gameObject.scene.name != null)
          instance.Manager = instance.Manager ?? instance.transform.parent.GetComponent<SplatManager>();

        instance.OnValueChanged();
      }
    }
  }

  [CustomEditor(typeof(LineMissile))]
  public class LineMissileEditor :  SplatEditor<LineMissile> {
  }

  [CustomEditor(typeof(Cone))]
  public class ConeEditor :  SplatEditor<Cone> {
  }

  [CustomEditor(typeof(Point))]
  public class PointEditor :  SplatEditor<Point> {
  }

  [CustomEditor(typeof(AngleMissile))]
  public class AngleMissileEditor : SplatEditor<AngleMissile> {
  }

  [CustomEditor(typeof(RangeIndicator))]
  public class RangeIndicatorEditor : SplatEditor<RangeIndicator> {
  }

  [CustomEditor(typeof(StatusIndicator))]
  public class StatusIndicatorEditor : SplatEditor<StatusIndicator> {
  }
}
