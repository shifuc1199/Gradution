/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
public class NPCView : View
{
    public Text _text;
    public float time_interval;
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
    private void Update()
    {
          
    }

}
