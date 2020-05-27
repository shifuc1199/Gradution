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
 
    public Dictionary<int,Toggle> Toggles = new Dictionary<int,Toggle>();
 
    public void ClearToggles()
    {
        foreach (var item in Toggles.Values)
        {
            DestroyImmediate(item.gameObject);
        }
        Toggles.Clear();
    }
    public void AddToggle(ToggleButton toggleBtn,int index)
    {
        
        toggleBtn.index = index;
        Toggles.Add(index,toggleBtn);
    }
    

}
[System.Serializable]
public class MyIntUnityEvent:UnityEvent<int>
{

}
