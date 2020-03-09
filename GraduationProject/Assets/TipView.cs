/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.UI;
public class TipView : View
{
    public Text Content;

    public void SetContent(string c)
    {
        Content.text = c;
    }
}
