/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class  ShopView : View
{
    public Transform m_root;
    public ButtonGroup m_group;
   
    public BuyGoods m_buy_goods;
    public GameObject m_cell_prefab;

    List<GameObject> cellList = new List<GameObject>();
    public override void OnShow()
    {
        base.OnShow();
        CurrentScene.GetView<GameInfoView>().HideAnim();
    }
    public override void OnHide()
    {
        base.OnHide();
        CurrentScene.GetView<GameInfoView>().ShowAnim();
    }
    // Start is called before the first frame update
    void Awake()
    {
        Update(ItemType.武器);
    }
    private void GetData<T>() where T: ItemConfig<T>
    {
        foreach (var item in cellList)
        {
            Destroy(item);
        }
        cellList.Clear();
        foreach (var data in ItemConfig<T>.Datas)
        {
            var cellObject = Instantiate(m_cell_prefab, m_root.transform);
            var cell = cellObject.GetComponent<ShopCell>();
            cellList.Add(cellObject);
            cell.SetModel(data.Value);
        }
    }
    public void Update(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.鞋子:
                GetData<FootConfig>();
                break;
            case ItemType.裤子:
                GetData<PelvisConfig>();
                break;
            case ItemType.肩膀:
                GetData<ArmConfig>();
                break;
            case ItemType.手链:
                GetData<SleeveConfig>();
                break;
            case ItemType.武器:
                GetData<WeaponConfig>();
                break;
            case ItemType.上衣:
                GetData<TorsoConfig>();
                break;
            case ItemType.盾牌:
                GetData<ShieldConfig>();
                break;
            case ItemType.消耗品:
                GetData<ConsumablesConfig>();
                break;
            default:
                break;
        }
       
    }
    public void OnGroupCellSelect(int index)
    {
        Update((ItemType)index);
    }
   public void OnCellSelect(int index)
    {
        var shopCell = m_group.Toggles[index].GetComponent<ShopCell>();
        m_buy_goods.SetModel(shopCell.m_itemType, shopCell.m_configId);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
