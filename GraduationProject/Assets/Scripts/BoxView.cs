/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DreamerTool.UI;

public class BoxView : View
{
    public UnityAction<bool> ClickEvent;
    public Text _text;
 
    public void SetText(string t, UnityAction<bool> c= null)
    {
        ClickEvent = c;
        _text.text = t;
    }
    public void OnYesClick()
    {
        OnCloseClick();
        ClickEvent?.Invoke(true);
    }
    public void OnNoClick()
    {
        OnCloseClick();
        ClickEvent?.Invoke(false);
    }
}
