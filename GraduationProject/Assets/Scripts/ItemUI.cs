/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DreamerTool.UI;
using UnityEngine.UI;
public class ItemUI : MonoBehaviour
{
    public int config_id;
    public ItemType itemtype;
    public GameObject select_frame;
 
    public void SetConfig(ItemType t,int id,bool ispickup=false) 
    {
        config_id = id;
        itemtype = t;
 
        switch (t)
        {
           
            case ItemType.鞋子:
                GetComponent<Image>().sprite = FootConfig.Get(id).GetSprite();
                if(ispickup)
                View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + FootConfig.Get(id).物品名字 + "*1",GameStaticData.ITEM_COLOR_DICT[FootConfig.Get(id).物品阶级]);
                break;
            case ItemType.裤子:
                GetComponent<Image>().sprite = PelvisConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + PelvisConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[PelvisConfig.Get(id).物品阶级]);
                break;
            case ItemType.肩膀:
                GetComponent<Image>().sprite = ArmConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + ArmConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[ArmConfig.Get(id).物品阶级]);
                break;
            case ItemType.手链:
                GetComponent<Image>().sprite = SleeveConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + SleeveConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[SleeveConfig.Get(id).物品阶级]);
                break;
            case ItemType.武器:
                GetComponent<Image>().sprite = WeaponConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + WeaponConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[WeaponConfig.Get(id).物品阶级]);

                GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -20);
                break;
            case ItemType.上衣:
                GetComponent<Image>().sprite = TorsoConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + TorsoConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[TorsoConfig.Get(id).物品阶级]);
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
