/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.UI;
using DreamerTool.Util;
public class TowerView : View
{
    public ButtonGroup group;
    public Text highestScoreText;
    public Text nameText;
    public Text levelText;
    public GameObject cellPrefab;
    int levelIndex=1;
    public override void OnShow()
    {
        base.OnShow();
        highestScoreText.text = "历史最高：" + ActorModel.Model.towerLevel+"层";
        nameText.text = ActorModel.Model.actor_name;
        levelText.text ="LV "+ ActorModel.Model.level;
    }
    private void Awake()
    {
        ShowTowerCell();
    }
    public void Select(int index)
    {
        levelIndex = group.Toggles.Count - index;
    }
    public void ShowTowerCell()
    {
        for (int i = ActorModel.Model.towerLevel; i >=1 ; i--)
        {
            var cell = Instantiate(cellPrefab, group.transform);
            cell.GetComponentInChildren<Text>().text = "第"+i+"层";
        }
    }
    public void OnChallengeBtnClick()
    {
        if(ActorModel.Model.GetMoney()>=1000)
        {
            CurrentScene.OpenView<BoxView>().SetText("你确定要进行挑战吗?\n需要支付"+DreamerUtil.GetColorRichText("1000金币",Color.yellow)+"门票费用哦",(v)=> {
                if(v)
                {
                    EndlessScene.level = levelIndex;
                    ActorModel.Model.SetMoney(-1000);
                    LoadingScene.LoadScene(GameConstData.ENDLESSS_SCENE_NAME);
                }
            });
            
        }
        else
        {
            CurrentScene.OpenView<TipView>().SetContent("没钱来干什么，挑战一次需要1000金币！");
        }
    }
}
