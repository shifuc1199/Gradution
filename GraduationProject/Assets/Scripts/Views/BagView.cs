using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.UI;
using DreamerTool.Extra;
using System;
[AttributeUsage(AttributeTargets.Field)]
class EquipMentAttribute:Attribute
{
    public PlayerAttribute attribute;
    public EquipMentAttribute (PlayerAttribute attr)
    {
        attribute = attr;
    }
}
[AttributeUsage(AttributeTargets.Field)]
class TipAttribute : Attribute
{
    public int index;
    public string valueColor;
    public string showName="";
    public TipAttribute(int index,string c, string n="")
    {
        valueColor = c;
        this.index = index;
        showName = n;
    }
}
public class BagView : MonoBehaviour
{
    public Text money_text;
    public Transform grid_root;
    public ItemUITip ItemUITip;
    public ItemUITip CurrentEquipmentUITip;
    public ItemUI CurretnSelect;
    
    List<ItemUI> Grids = new List<ItemUI>();
    public Button sure_button;
    public Button sell_button;
    int grid_index = 0;
    private void Awake()
    {
        SetMoney();
        var children = grid_root.GetChildren();
        foreach (var item in children)
        {
            Grids.Add(item.GetComponentInChildren<ItemUI>(true));
        }

        foreach (var item in ActorModel.Model.bag_items)
        {
            AddItem(item);
        }
    }
  
    public void SelectItem(int index)
    {
        grid_index = index;
        ChooseGrid();
    }
    public void SetMoney()
    {
        money_text.text = ActorModel.Model.GetMoney().ToString();
    }
    public void SelectUp()
    {
        if (grid_index-5 <0)
            return;
        grid_index-=5;
        ChooseGrid();
    }
    public void SelectDown()
    {
        if (grid_index+5 > Grids.Count - 1)
            return;
        grid_index+=5;
        ChooseGrid();
    }
    public void SelectRight()
    {
        if (grid_index >= Grids.Count-1)
            return;
        grid_index++;
        ChooseGrid();
    }
    public void SelectLeft()
    {
        if (grid_index == 0)
            return;
        grid_index--;
        ChooseGrid();
    }
    private void OnEnable()
    {
        foreach (var item in ActorModel.Model.bag_items)
        {
            
            ShowItem(item);
        }
        ChooseGrid();

    }
    public void Sell()
    {
      
        if(!CurretnSelect.icon.gameObject.activeSelf)
        {
            return;
        }
        View.CurrentScene.OpenView<BoxView>().SetText("您确定要出售此物品吗",(v)=> {
            if(v)
            {
                ItemUITip.gameObject.SetActive(false);
                ActorModel.Model.bag_items.Remove(CurretnSelect.data);
                CurretnSelect.data = null;
                CurretnSelect.icon.gameObject.SetActive(false);
            }

        });
    }
    public void ChangeEquipMent(EquipmentType equipType)
    {
        int temp = ActorModel.Model.GetPlayerEquipment(equipType);
        ActorModel.Model.SetPlayerEquipment(equipType, CurretnSelect.data.config_id);
        ActorModel.Model.bag_items.Remove(CurretnSelect.data);
        CurretnSelect.SetConfig(new BagItemData(CurretnSelect.data.itemtype, temp, CurretnSelect.transform.GetSiblingIndex()));
        ActorModel.Model.bag_items.Add(CurretnSelect.data);
    }
    public void Equip<T>(ItemConfig<T> config, ItemConfig<T> config_2,EquipmentType equipType) where T:BaseConfig<T>
    {
        config.ChangePlayerAttribute();
        ChangeEquipMent(equipType);
        SetTip(config, config_2);
    }
    public void SetTip<T>(ItemConfig<T> config, ItemConfig<T> config_2) where T : BaseConfig<T>
    {
        ItemUITip.gameObject.SetActive(config!=null);

        CurrentEquipmentUITip.gameObject.SetActive(config_2!=null);

        if(config!=null)
            ItemUITip.SetConfig(config.GetSprite(), config.GetTipString());
        if (config_2 != null)
            CurrentEquipmentUITip.SetConfig(config_2.GetSprite(), config_2.GetTipString());
    }
    public void Equip()
    {
        if (!CurretnSelect.icon.gameObject.activeSelf)
            return;
        switch (CurretnSelect.data.itemtype)
        {
            case ItemType.武器:
                var config = WeaponConfig.Get(CurretnSelect.data.config_id);
                if (config.需要等级 > ActorModel.Model.GetLevel())
                {
                    View.CurrentScene.OpenView<TipView>().SetContent("等级不够 需要等级: "+DreamerTool.Util.DreamerUtil.GetColorRichText( config.需要等级.ToString(),Color.red));
                    return;
                }
                Equip(config, WeaponConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.武器)), EquipmentType.武器);
                break;
            case ItemType.肩膀:
                var arm_config = ArmConfig.Get(CurretnSelect.data.config_id);
                Equip(arm_config, ArmConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.肩膀右)), EquipmentType.肩膀右);
                break;
            case ItemType.上衣:
                var torso_config = TorsoConfig.Get(CurretnSelect.data.config_id);
                Equip(torso_config, TorsoConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.上衣)), EquipmentType.上衣);
                break;
            case ItemType.手链:
                var sleeve_config = SleeveConfig.Get(CurretnSelect.data.config_id);
                Equip(sleeve_config, SleeveConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.手链)), EquipmentType.手链);
                break;
            case ItemType.裤子:
                var pelvis_config = PelvisConfig.Get(CurretnSelect.data.config_id);
                Equip(pelvis_config, PelvisConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.裤子)), EquipmentType.裤子);
                break;
            case ItemType.鞋子:
                var foot_config = FootConfig.Get(CurretnSelect.data.config_id);
                Equip(foot_config, FootConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.鞋子)), EquipmentType.鞋子);
                break;
            case ItemType.消耗品:
                GameStaticMethod.ExecuteCommond(ConsumablesConfig.Get(CurretnSelect.data.config_id).function);
                ActorModel.Model.bag_items.Remove(CurretnSelect.data);
                CurretnSelect.icon.gameObject.SetActive(false);
                ItemUITip.gameObject.SetActive(false);
                break;
            case ItemType.盾牌:
                var shield_config = ShieldConfig.Get(CurretnSelect.data.config_id);
                Equip(shield_config, ShieldConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.盾牌)), EquipmentType.盾牌);
                break;
            default:
                break;
        }
    }
   
    
 
    public Transform GetEmptyGrid()
    {
        for(int i =0;i<grid_root.childCount;i++)
        {
            if(grid_root.GetChild(i).GetComponent<ItemUI>().data==null)
            {
                return grid_root.GetChild(i);
            }
        }
        Debug.LogWarning("背包已满！！");
        return null;
    }
    public void ChooseGrid()
    { 
        if(CurretnSelect!=null)
        {
            CurretnSelect.UnSelect();
        }
        CurretnSelect = Grids[grid_index];
        CurretnSelect.Select();

        if (CurretnSelect.data != null)
        {
            
            SetTipPos();
            switch (CurretnSelect.data.itemtype)
            {
                case ItemType.鞋子:
                    var foot = FootConfig.Get(CurretnSelect.data.config_id);
                    SetTip(foot, FootConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.鞋子)));
                    break;
                case ItemType.裤子:
                    var pelvis = PelvisConfig.Get(CurretnSelect.data.config_id);
                    SetTip(pelvis, PelvisConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.裤子)));
                    break;
                case ItemType.肩膀:
                    var arm = ArmConfig.Get(CurretnSelect.data.config_id);
                    SetTip(arm, ArmConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.肩膀右)));
                    break;
                case ItemType.手链:
                    var sleeve = SleeveConfig.Get(CurretnSelect.data.config_id);
                    SetTip(sleeve, SleeveConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.手链)));
                    break;
                case ItemType.武器:
                    var weapon = WeaponConfig.Get(CurretnSelect.data.config_id);
                    SetTip(weapon, WeaponConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.武器)));
                    break;
                case ItemType.上衣:
                    var torso = TorsoConfig.Get(CurretnSelect.data.config_id);
                    SetTip(torso, TorsoConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.上衣)));
                    break;
                case ItemType.盾牌:
                    var shield = ShieldConfig.Get(CurretnSelect.data.config_id);
                    SetTip(shield, ShieldConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.盾牌)));
                    break;
                case ItemType.消耗品:
                    var consum = ConsumablesConfig.Get(CurretnSelect.data.config_id);
                    SetTip(consum, null);
                    break;
                default:
                    break;
            }

            sell_button.gameObject.SetActive(true);
            sure_button.gameObject.SetActive(true);
            if (CurretnSelect.data.itemtype == ItemType.消耗品)
                sure_button.GetComponentInChildren<Text>().text = "使用";
            else
                sure_button.GetComponentInChildren<Text>().text = "装备";
        }
        else
        {
            sell_button.gameObject.SetActive(false);
            sure_button.gameObject.SetActive(false);
            ItemUITip.gameObject.SetActive(false);
        }
 
     
    }
    public void SetTipPos()
    {
        ItemUITip.transform.position = Grids[grid_index].transform.position;
    }
    public void ShowItem(BagItemData data)
    {
        Grids[data.bag_index].transform.GetChild(0).gameObject.SetActive(true);
    }  
    public void AddItem(BagItemData data)
    {
        var grid = Grids[data.bag_index].transform;
        if (data.itemtype == ItemType.武器)
            GameStaticData.WeaponUI.Copy(grid.GetChild(0).GetComponent<RectTransform>());
        else
            GameStaticData.EquipmentUI.Copy(grid.GetChild(0).GetComponent<RectTransform>());
        grid.GetComponent<ItemUI>().SetConfig(data);
    }
    public void AddItem(int id, ItemType type) 
    {
        var grid = GetEmptyGrid();
        if(grid != null)
        {
            if(type==ItemType.武器)
                GameStaticData.WeaponUI.Copy(grid.GetChild(0).GetComponent<RectTransform>());
            else
                GameStaticData.EquipmentUI.Copy(grid.GetChild(0).GetComponent<RectTransform>());
            
            grid.GetComponent<ItemUI>().SetConfig(new BagItemData(type, id, grid.GetSiblingIndex()),true);
             
            ActorModel.Model.bag_items.Add(grid.GetComponent<ItemUI>().data);
            
        }
        
    }
}
