/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class ShopView : View
{
    public Transform m_root;
    public BuyGoods m_buy_goods;
    public GameObject m_cell_prefab;
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
    void Start()
    {
       var cell = Instantiate(m_cell_prefab, m_root.transform).GetComponent<ShopCell>();
        cell.SetModel(WeaponConfig.Get(1));
        m_buy_goods.SetModel(cell.m_itemType, cell.m_configId);
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
}
