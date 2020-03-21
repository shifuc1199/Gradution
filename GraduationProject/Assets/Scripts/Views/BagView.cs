using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.UI;
using DreamerTool.Extra;
public class BagView : MonoBehaviour
{
    public Text money_text;
    public Transform grid_root;
    public ItemUITip ItemUITip;
    public ItemUITip CurrentEquipmentUITip;
    public ItemUI CurretnSelect;
    
    List<ItemUI> Grids = new List<ItemUI>();
    public Button sure_button;

    int grid_index = 0;
    private void Awake()
    {
        
        SetMoney();
        var children = grid_root.GetChildren();
        foreach (var item in children)
        {
            Grids.Add(item.GetComponentInChildren<ItemUI>(true));
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
        ChooseGrid();
    }
    public void Equip()
    {
        if (CurretnSelect == null)
            return;
        switch (CurretnSelect.itemtype)
        {
            case ItemType.武器:
                var config = WeaponConfig.Get(CurretnSelect.config_id);
                if (config.需要等级 > ActorModel.Model.GetLevel())
                {
                    View.CurrentScene.OpenView<TipView>().SetContent("等级不够 需要等级: "+DreamerTool.Util.DreamerUtil.GetColorRichText( config.需要等级.ToString(),Color.red));
                    return;
                }
                     
                int temp = ActorModel.Model.GetPlayerEquipment(EquipmentType.武器);
                var a = config.攻击力;
                var b = WeaponConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.武器)).攻击力;
                ActorModel.Model.SetPlayerAttribute(PlayerAttribute.攻击力, a-b );
                ActorModel.Model.SetPlayerEquipment(EquipmentType.武器, CurretnSelect.config_id);
                CurretnSelect.SetConfig(ItemType.武器, temp);
                ItemUITip.SetConfig(ItemType.武器, temp);
                break;
            case ItemType.肩膀:
                int temp1 = ActorModel.Model.GetPlayerEquipment(EquipmentType.肩膀右);
                ActorModel.Model.SetPlayerEquipment(EquipmentType.肩膀右, CurretnSelect.config_id);
                ActorModel.Model.SetPlayerEquipment(EquipmentType.肩膀左, CurretnSelect.config_id);
                CurretnSelect.SetConfig(ItemType.肩膀, temp1);
                ItemUITip.SetConfig(ItemType.肩膀, temp1);
                break;
            case ItemType.上衣:
                int temp2 = ActorModel.Model.GetPlayerEquipment(EquipmentType.上衣);
                ActorModel.Model.SetPlayerEquipment(EquipmentType.上衣, CurretnSelect.config_id);
                CurretnSelect.SetConfig(ItemType.上衣, temp2);
                ItemUITip.SetConfig(ItemType.上衣, temp2);
                break;
            case ItemType.手链:
                int temp3= ActorModel.Model.GetPlayerEquipment(EquipmentType.手链);
                ActorModel.Model.SetPlayerEquipment(EquipmentType.手链, CurretnSelect.config_id);
                CurretnSelect.SetConfig(ItemType.手链, temp3);
                ItemUITip.SetConfig(ItemType.手链, temp3);
                break;
            case ItemType.裤子:
                int temp4 = ActorModel.Model.GetPlayerEquipment(EquipmentType.裤子);
                ActorModel.Model.SetPlayerEquipment(EquipmentType.裤子, CurretnSelect.config_id);
                CurretnSelect.SetConfig(ItemType.裤子, temp4);
                ItemUITip.SetConfig(ItemType.裤子, temp4);
                break;
            case ItemType.鞋子:
                int temp5 = ActorModel.Model.GetPlayerEquipment(EquipmentType.鞋子);
                ActorModel.Model.SetPlayerEquipment(EquipmentType.鞋子, CurretnSelect.config_id);
                CurretnSelect.SetConfig(ItemType.鞋子, temp5);
                ItemUITip.SetConfig(ItemType.鞋子, temp5);
                break;
            case ItemType.消耗品:
                GameStaticMethod.ExecuteCommond(ConsumablesConfig.Get(CurretnSelect.config_id).function);
                ActorModel.Model.bag_items.Remove(CurretnSelect);
                CurretnSelect.icon.gameObject.SetActive(false);
                ItemUITip.gameObject.SetActive(false);
                break;
            default:
                break;
        }
         
         
    }
   
    
 
    public Transform GetEmptyGrid()
    {
        for(int i =0;i<grid_root.childCount;i++)
        {
            if(!grid_root.GetChild(i).GetChild(0).gameObject.activeSelf)
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
        if (Grids[grid_index].transform.GetChild(0).gameObject.activeSelf)
        {
            ItemUITip.gameObject.SetActive(true);
            SetTipPos();
            var itemui = Grids[grid_index];
            ItemUITip.SetConfig(itemui.itemtype,itemui.config_id);

            switch (itemui.itemtype)
            {
                case ItemType.鞋子:
                    CurrentEquipmentUITip.gameObject.SetActive(true);
                    CurrentEquipmentUITip.SetConfig(ItemType.鞋子, ActorModel.Model.GetPlayerEquipment(EquipmentType.鞋子));
                    break;
                case ItemType.裤子:
                    CurrentEquipmentUITip.gameObject.SetActive(true);
                    CurrentEquipmentUITip.SetConfig(ItemType.裤子, ActorModel.Model.GetPlayerEquipment(EquipmentType.裤子));
                    break;
                case ItemType.肩膀:
                    CurrentEquipmentUITip.gameObject.SetActive(true);
                    CurrentEquipmentUITip.SetConfig(ItemType.肩膀, ActorModel.Model.GetPlayerEquipment(EquipmentType.肩膀右));
                    break;
                case ItemType.手链:
                    CurrentEquipmentUITip.gameObject.SetActive(true);
                    CurrentEquipmentUITip.SetConfig(ItemType.手链, ActorModel.Model.GetPlayerEquipment(EquipmentType.手链));
                    break;
                case ItemType.武器:
                    CurrentEquipmentUITip.gameObject.SetActive(true);
                    CurrentEquipmentUITip.SetConfig(ItemType.武器, ActorModel.Model.GetPlayerEquipment(EquipmentType.武器));
                    break;
                case ItemType.上衣:
                    CurrentEquipmentUITip.gameObject.SetActive(true);
                    CurrentEquipmentUITip.SetConfig(ItemType.上衣, ActorModel.Model.GetPlayerEquipment(EquipmentType.上衣));
                    break;
                case ItemType.盾牌:
                    CurrentEquipmentUITip.gameObject.SetActive(true);
                    CurrentEquipmentUITip.SetConfig(ItemType.盾牌, ActorModel.Model.GetPlayerEquipment(EquipmentType.盾牌));
                    break;
                case ItemType.消耗品:
                    CurrentEquipmentUITip.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        else
        {
            
            ItemUITip.gameObject.SetActive(false);
        }
        CurretnSelect = Grids[grid_index];
        Grids[grid_index].Select();

        if(CurretnSelect.itemtype == ItemType.消耗品)
            sure_button.GetComponentInChildren<Text>().text = "使用";
        else
            sure_button.GetComponentInChildren<Text>().text = "装备";
    }
    public void SetTipPos()
    {
        ItemUITip.transform.position = Grids[grid_index].transform.position;
    }
    public void AddItem(int id, ItemType type) 
    {
        var grid = GetEmptyGrid();
        if(grid != null)
        {
            grid.GetChild(0).gameObject.SetActive(true);
            if(type==ItemType.武器)
                GameStaticData.WeaponUI.Copy(grid.GetChild(0).GetComponent<RectTransform>());
            else
                GameStaticData.EquipmentUI.Copy(grid.GetChild(0).GetComponent<RectTransform>());
            grid.GetComponent<ItemUI>().SetConfig(type, id,true);
            ActorModel.Model.bag_items.Add(grid.GetChild(0).GetComponent<ItemUI>());
 
        }
        
    }
}
