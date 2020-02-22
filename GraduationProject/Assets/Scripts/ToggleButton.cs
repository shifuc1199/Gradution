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
 
  
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        (group as ButtonGroup).OnSelect.Invoke(index);
    }
    

}
