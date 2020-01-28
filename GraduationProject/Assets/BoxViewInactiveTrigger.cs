/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DreamerTool.UI;
public class BoxViewInactiveTrigger : InactiveTrigger
{
    public string _text;
    public UnityEvent YesClickEvent;
    public UnityEvent NoClickEvent;

    public void Awake()
    {
        inactive_event =  () => {
            View.CurrentScene.OpenView<BoxView>().SetText(_text);
            View.CurrentScene.GetView<BoxView>().ClickEvent = (v) =>
            {
                if (v)
                {
                    YesClickEvent.Invoke();
                }
                else
                {
                    NoClickEvent.Invoke();
                }
            };
        };
    }

}
