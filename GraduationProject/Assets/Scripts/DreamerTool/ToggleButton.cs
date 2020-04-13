/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
public class ToggleButton : Toggle
{
    public int index;
    public UnityEvent YesEvent;
    public UnityEvent NoEvent;
    protected override void Awake()
    {
        base.Awake();
        if(group == null)
        {
            group = GetComponentInParent<ButtonGroup>();
        }
    
        onValueChanged.AddListener(OnValueChanged);
        OnValueChanged(isOn);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
       
        (group as ButtonGroup).OnSelect?.Invoke(index);
    }
  public void OnValueChanged(bool v)
    {
        if(v)
        {
            YesEvent?.Invoke();
        }
        else
        {
            NoEvent?.Invoke();
        }
    }
 
    

}
