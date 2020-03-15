/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
public class BuyGoods : MonoBehaviour
{
    public Button m_buyButton;
    public Text m_desText;
    public Text m_amountText;
    public Text m_totalpriceText;
    public Slider m_countSlider;
    int singlePrice;
    int amount = 10;

    private ItemType m_type;
    private int m_id;
    private void Awake()
    {
        
    }
    public void SetModel(ItemType _type,int _id)
    {
        m_id = _id;
        m_type = _type;
        switch (_type)
        {
            case ItemType.鞋子:
                var config_1 = FootConfig.Get(_id);
                break;
            case ItemType.裤子:
                break;
            case ItemType.肩膀:
                break;
            case ItemType.手链:
                break;
            case ItemType.武器:
                var weapon = WeaponConfig.Get(_id);
                m_desText.text = weapon.物品描述;
                singlePrice = weapon.购买价格;
                break;
            case ItemType.上衣:
                break;
            case ItemType.盾牌:
                break;
            case ItemType.消耗品:
                break;
            default:
                break;
        }
        UpdateAmountText();
        UpdateAmountSlider();
    }
    public void Buy()
    {
        for (int i = 0; i < amount; i++)
        {
            View.CurrentScene.GetView<PlayerInfoAndBagView>().bag_view.AddItem(m_id, m_type);
        }
        ActorModel.Model.SetMoney(-singlePrice*amount);
        UpdateAmountText();
        UpdateAmountSlider();
    }
    public void Add()
    {
        if (amount == m_countSlider.maxValue)
            return;

        amount += 1;

        UpdateAmountText();
        UpdateAmountSlider();
    }
    public void Decrease()
    {
        if (amount == m_countSlider.minValue)
            return;

        amount -= 1;

        UpdateAmountText();
        UpdateAmountSlider();
    }
    public void UpdateAmountText()
    {
        m_amountText.text =   amount.ToString();
        m_buyButton.interactable = amount * singlePrice <= ActorModel.Model.GetMoney();
        m_buyButton.GetComponentInChildren<Text>().color = amount * singlePrice <= ActorModel.Model.GetMoney() ? Color.white : Color.gray;
        m_totalpriceText.text = "总价:\t  " +DreamerTool.Util.DreamerUtil.GetColorRichText( (amount * singlePrice).ToString(), amount * singlePrice<=ActorModel.Model.GetMoney()?Color.green:Color.red);
    }
    public void UpdateAmountSlider()
    {
        m_countSlider.value = amount;
    }
    public void OnAmountSliderValueChanged(float v)
    {
       
        amount = (int)v;
        UpdateAmountText();
    }
}
