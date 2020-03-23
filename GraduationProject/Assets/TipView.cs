/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.Events;
using UnityEngine.UI;
public class TipView : View
{
    public Text Content;
    UnityAction call_back;
    public void SetContent(string c)
    {
        Content.text = c;
    }
    public void SetContent(string c,UnityAction _action=null)
    {
        call_back = _action;
        Content.text = c;
    }
    private void OnDisable()
    {
        call_back?.Invoke();
    }
}
