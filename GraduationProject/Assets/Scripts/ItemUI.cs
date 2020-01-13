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
 
    public void SetConfig(ItemType t,int id) 
    {
        config_id = id;
        itemtype = t;
        switch (t)
        {
            case ItemType.腿部:
                break;
            case ItemType.裤子:
                break;
            case ItemType.肩膀:
                break;
            case ItemType.手腕:
                break;
            case ItemType.武器:
                GetComponent<Image>().sprite = WeaponConfig.Get(id).GetSprite();
                break;
            case ItemType.上衣:
                break;
            case ItemType.消耗品:
                break;
            default:
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
