/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopCell : MonoBehaviour
{
    public Text m_nameText;
    public Image m_icon;
    public Text m_singlepriceText;
    [HideInInspector]
    public ItemType m_itemType;
    [HideInInspector]
    public int m_configId;
    
    public void SetModel<T>(ItemConfig<T> config) where T:BaseConfig<T>
    {

        m_itemType = GameStaticData.ITEM_CONFIG[typeof(T).Name];
    
        m_configId = config.物品ID;

        m_nameText.text = DreamerTool.Util.DreamerUtil.GetColorRichText(config.物品名字,GameStaticData.ITEM_COLOR_DICT[config.物品阶级]);
        m_icon.sprite = config.GetSprite();
        m_singlepriceText.text = config.购买价格.ToString();
    }

     
 }
