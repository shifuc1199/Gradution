/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class ButtonGroup : ToggleGroup
{
     
    public MyIntUnityEvent OnSelect;
    [System.NonSerialized]
    public List<Toggle> Toggles = new List<Toggle>();

    protected override void Start()
    {
        base.Start();
        var tbs = GetComponentsInChildren<ToggleButton>();
        foreach (var tb in tbs)
        {
            tb.index = Toggles.Count;
            tb.group = this;
            Toggles.Add(tb);
        }
    }

}
[System.Serializable]
public class MyIntUnityEvent:UnityEvent<int>
{

}
