/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
public class ScratchCardView : View
{
    public Button sure_button;
    public List<ScratchCard> cards = new List<ScratchCard>();
    int index = 0;

    private void Start()
    {
        foreach (var item in cards)
        {
            item.erase.eraserEndEvent.AddListener(CompleteOnce);
        }
    }
    public override void OnShow()
    {
        base.OnShow();

        index = 0;
        sure_button.GetComponentInChildren<Text>().text = "请刮开所有卡";
        sure_button.GetComponentInChildren<Text>().color = Color.gray;
        sure_button.interactable = false;

        foreach (var item in cards)
        {
            item.Init();
            item.erase.SetTexture();
        }
    }
 
    public void CompleteOnce()
    {
        index++;

        if(index>=cards.Count)
        {
            sure_button.GetComponentInChildren<Text>().text = "兑换奖励";
            sure_button.GetComponentInChildren<Text>().color = Color.yellow;
            sure_button.interactable = true;
        }
    }
    public void CashPrize()
    {
        int money=0;
        foreach (var item in cards)
        {
            money += item.money;
        }
      
        CurrentScene.OpenView<TipView>().SetContent("兑换成功，总共获得金币: " + money);
        OnCloseClick();
    }
}
