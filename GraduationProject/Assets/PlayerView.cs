using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class PlayerView : View
{
    public override void OnShow()
    {
        base.OnShow();
        CurrentScene.GetView<GameInfoView>().HideAnim();
    }
    public override void OnHide()
    {
        base.OnHide();
        CurrentScene.GetView<GameInfoView>().ShowAnim();
    }
}
