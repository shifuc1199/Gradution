/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemUITip : MonoBehaviour
{
    public Text _text;
    public Image _icon;
    public void SetConfig(Sprite sp,string tip) 
    {
        _icon.sprite =sp;
        _text.text = tip;
       
    }
 
}
