using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class BagView : MonoBehaviour
{
    public Transform grid_root;
    public ItemUITip ItemUITip;
    public ItemUI CurretnSelect;
    public List<ItemUI> Items = new List<ItemUI>();
    int grid_index = 0;
 
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
         
        switch (CurretnSelect.itemtype)
        {
            case ItemType.武器:
                int temp = ActorController._controller.model.Equipment[EquipmentType.武器];
                ActorController._controller.model.Equipment[EquipmentType.武器] = CurretnSelect.config_id;
                CurretnSelect.SetConfig(ItemType.武器, temp);
                ItemUITip.SetConfig(ItemType.武器, temp);
                break;
            default:
                break;
        }
         
        EventHandler.OnChangeEquipment();
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
            ItemUITip.transform.position = Items[grid_index].transform.position;
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
    public void AddItem(int id, ItemType type) 
    {
        var grid = GetEmptyGrid();
        if(grid != null)
        {
            grid.GetChild(0).gameObject.SetActive(true);
            grid.GetChild(0).GetComponent<ItemUI>().SetConfig(type, id);
            Items.Add(grid.GetChild(0).GetComponent<ItemUI>());
        }
        
    }
}
