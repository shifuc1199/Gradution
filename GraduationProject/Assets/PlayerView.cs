using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class PlayerView : View
{
    public override void OnShow()
    {
        base.OnShow();
        CurrentScene.GetView<GameInfoView>().GetComponent<Animator>().enabled = true;
    }
}
