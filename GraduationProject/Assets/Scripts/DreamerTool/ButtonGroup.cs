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
 
    public List<Toggle> Toggles = new List<Toggle>();
 
    public void ClearToggles()
    {
        foreach (var item in Toggles)
        {
            Destroy(item.gameObject);
        }
        Toggles.Clear();
    }
    public void AddToggle(ToggleButton toggleBtn)
    {
        
        toggleBtn.index = Toggles.Count;
        Toggles.Add(toggleBtn);
    }
    public void UpdateToggle()
    {
 
      ClearToggles();
       
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
