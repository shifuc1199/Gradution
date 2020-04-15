/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.UI;
using DG.Tweening;
public class NetWorkGameInfoView : View
{
    public NetWorkActorHUD[] huds;
    public Image m_screen_effect;
    public void SetScreenEffect(Color c, float fade_value)
    {
        c.a = 1 - fade_value;
        m_screen_effect.color = c;
        m_screen_effect.DOFade(fade_value, 0.5f).SetEase(Ease.Linear);
    }
}
