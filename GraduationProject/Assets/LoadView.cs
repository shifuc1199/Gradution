/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.UI;
public class LoadView : View
{
    public GameObject loading;
    public Text _content_text;
    public override void OnShow()
    {
        base.OnShow();
        loading.SetActive(true);
        _content_text.gameObject.SetActive(false);
    }
    public void SetText(string c)
    {
        loading.SetActive(false);
        _content_text.gameObject.SetActive(true);
        _content_text.text = c;

    }
}
