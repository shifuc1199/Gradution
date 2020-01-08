/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class ItemUI : MonoBehaviour
{
    public int config_id;
    public ItemType itemtype;
    public GameObject select_frame;
 
    public void SetConfig<T>(ItemConfig<T> _config) where T:BaseConfig<T>
    {
      
        config_id = _config.物品ID;
        GetComponent<Image>().sprite = _config.GetSprite();
        switch (typeof(T).Name)
        {
            case "WeaponConfig":
                itemtype = ItemType.武器;
                break;
        }
    }
    public void Select()
    {
        select_frame.SetActive(true);
    }
    public void UnSelect()
    {
        select_frame.SetActive(false);
    }

}
