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
    public Text m_moneyText;
    public Slider m_countSlider;
    int singlePrice;
    int amount = 1;

    private ItemType m_type;
    private int m_id;
 
    private void Awake()
    {       
        EventManager.OnChangeMoney += UpdateMoneyText;
    }
    private void Start()
    {
        UpdateMoneyText();
    }
    private void OnDestroy()
    {
        EventManager.OnChangeMoney -= UpdateMoneyText;
    }
    void UpdateMoneyText()
    {
        m_moneyText.text = ActorModel.Model.GetMoney().ToString();
    }
    
    public void SetModel(ItemType _type,int _id)
    {
        m_id = _id;
        m_type = _type;
        
        switch (_type)
        {
            case ItemType.鞋子:
                var foot = FootConfig.Get(_id);
                m_desText.text = foot.物品描述;
                singlePrice = foot.购买价格;
                break;
            case ItemType.裤子:
                var sleeve = SleeveConfig.Get(_id);
                m_desText.text = sleeve.物品描述;
                singlePrice = sleeve.购买价格;
                break;
            case ItemType.肩膀:
                var arm = ArmConfig.Get(_id);
                m_desText.text = arm.物品描述;
                singlePrice = arm.购买价格;
                break;
            case ItemType.手链:
                var pelvis = PelvisConfig.Get(_id);
                m_desText.text = pelvis.物品描述;
                singlePrice = pelvis.购买价格;
                break;
            case ItemType.武器:
                var weapon = WeaponConfig.Get(_id);
                m_desText.text = weapon.物品描述;
                singlePrice = weapon.购买价格;
                break;
            case ItemType.上衣:
                var torso = TorsoConfig.Get(_id);
                m_desText.text = torso.物品描述;
                singlePrice = torso.购买价格;
                break;
            case ItemType.盾牌:
                var shield = ShieldConfig.Get(_id);
                m_desText.text = shield.物品描述;
                singlePrice = shield.购买价格;
                break;
            case ItemType.消耗品:
                var con = ConsumablesConfig.Get(_id);
                m_desText.text = con.物品描述;
                singlePrice = con.购买价格;
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
        View.CurrentScene.OpenView<TipView>().SetContent("购买成功! ");
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
