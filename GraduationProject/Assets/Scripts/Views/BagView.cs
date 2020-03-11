using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.UI;
public class BagView : MonoBehaviour
{
    public Text money_text;
    public Transform grid_root;
    public ItemUITip ItemUITip;
    public ItemUI CurretnSelect;
    public List<ItemUI> Items = new List<ItemUI>();
    int grid_index = 0;
    private void Awake()
    {
        SetMoney();
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

        if (grid_index+5 > Items.Count - 1)
            return;
        grid_index+=5;
        ChooseGrid();
    }
    public void SelectRight()
    {
         
        if (grid_index >= Items.Count-1)
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
            default:
                break;
        }
         
         
    }
    private void OnEnable()
    {
        ChooseGrid();
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
        if (Items.Count == 0)
            return;
       
        if(CurretnSelect!=null)
        {
            CurretnSelect.UnSelect();
        }
        if (Items[grid_index].gameObject.activeSelf)
        {
            ItemUITip.gameObject.SetActive(true);
            var itemui = Items[grid_index];
            ItemUITip.SetConfig(itemui.itemtype,itemui.config_id);
        }
        else
        {
            ItemUITip.gameObject.SetActive(false);
        }
        CurretnSelect = Items[grid_index];
        Items[grid_index].Select();
        

    }
    private void Update()
    {
         if (Items.Count > 0 && Vector2.Distance(ItemUITip.transform.position, Items[grid_index].transform.position)>=0.5f)
         {
             ItemUITip.transform.position = Items[grid_index].transform.position;
         }
    }
    public void AddItem(int id, ItemType type) 
    {
        var grid = GetEmptyGrid();
        if(grid != null)
        {
            grid.GetChild(0).gameObject.SetActive(true);
            grid.GetChild(0).GetComponent<ItemUI>().SetConfig(type, id,true);
            Items.Add(grid.GetChild(0).GetComponent<ItemUI>());
 
        }
        
    }
}
