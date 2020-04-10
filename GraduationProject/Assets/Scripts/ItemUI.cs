/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DreamerTool.UI;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ItemData
{
    public int config_id;
    public ItemType itemtype;

    public ItemData(ItemType type,int id)
    {
        this.config_id = id;
        this.itemtype = type;
    }
    public ItemData()
    {

    }
}


public class ItemUI : MonoBehaviour,IPointerDownHandler
{
    public ItemData data; 
    public GameObject select_frame;
    public Image icon;

    public void SetConfig(ItemData data, bool ispickup=false)
    {
        this.data = data;
        var id = this.data.config_id;
        var typ = this.data.itemtype;
        switch (typ)
        {
           
            case ItemType.鞋子:
                icon.sprite = FootConfig.Get(id).GetSprite();
                if(ispickup)
                View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + FootConfig.Get(id).物品名字 + "*1",GameStaticData.ITEM_COLOR_DICT[FootConfig.Get(id).物品阶级]);
                break;
            case ItemType.裤子:
                icon.sprite = PelvisConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + PelvisConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[PelvisConfig.Get(id).物品阶级]);
                break;
            case ItemType.肩膀:
                icon.sprite = ArmConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + ArmConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[ArmConfig.Get(id).物品阶级]);
                break;
            case ItemType.手链:
                icon.sprite = SleeveConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + SleeveConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[SleeveConfig.Get(id).物品阶级]);
                break;
            case ItemType.武器:
                icon.sprite = WeaponConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + WeaponConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[WeaponConfig.Get(id).物品阶级]);
 
                break;
            case ItemType.上衣:
                icon.sprite = TorsoConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + TorsoConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[TorsoConfig.Get(id).物品阶级]);
                break;
            case ItemType.消耗品:
                icon.sprite = ConsumablesConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + ConsumablesConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[ConsumablesConfig.Get(id).物品阶级]);
                break;
            case ItemType.盾牌:
                icon.sprite = ShieldConfig.Get(id).GetSprite();
                if (ispickup)
                    View.CurrentScene.GetView<GameInfoView>().SetPopText("拾取 " + ShieldConfig.Get(id).物品名字 + "*1", GameStaticData.ITEM_COLOR_DICT[ShieldConfig.Get(id).物品阶级]);
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

    public void OnPointerDown(PointerEventData eventData)
    {
        View.CurrentScene.GetView<PlayerInfoAndBagView>().bag_view.SelectItem(transform.GetSiblingIndex());
    }
}
