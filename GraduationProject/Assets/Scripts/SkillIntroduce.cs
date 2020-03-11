/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.Util;
using DreamerTool.UI;
public class SkillIntroduce : MonoBehaviour
{
    public Text intro_text;
    public GifManager gif;
    public Button level_up_button;
    public Text level_up_info_text;
    SkillModel model;
    private void Awake()
    {
       
    }
    public void SetModel(SkillModel model)
    {
        this.model = model;
        UpdateModel();
        gif.SetPath(model._config.ID.ToString());
    }
    public void UpdateModel()
    {
        intro_text.text = model._config.skill_des.Replace("t", "<color=#FF9A00>" + (int)model.GetHurtValue() + "</color>");
        double needMoney=0;
        if (!model.IsLearn())
        {
            needMoney = model.GetLearnMoney();
            level_up_button.GetComponentInChildren<Text>().text = "学习";
            level_up_info_text.text = "学习需要金钱：" + ((ActorModel.Model.GetMoney() >= needMoney) ? "<color=green>" : "<color=red>") + needMoney + "</color>";
        }
        else
        {
            needMoney = model.GetLevelUpMoney();
            level_up_button.GetComponentInChildren<Text>().text = "升级";
            level_up_info_text.text = DreamerUtil.GetColorRichText("\t\t等级:" + model.GetSkillLevel(), Color.white) + "\n升级所需金钱: " + ((ActorModel.Model.GetMoney() >= needMoney) ? "<color=green>" : "<color=red>") + needMoney + " </color>";
        }
         
        level_up_button.interactable = ActorModel.Model.GetMoney() >= needMoney;
        level_up_button.GetComponentInChildren<Text>().color = ActorModel.Model.GetMoney() >= needMoney ? Color.yellow : Color.gray;
         
    }
    public void LevelUp()
    {
        if(!model.IsLearn())
        {
            View.CurrentScene.OpenView<TipView>().SetContent(DreamerUtil.GetColorRichText(model._config.skill_name,Color.yellow)+"\t学习成功！");
            model.Learn();
        }
        else
        {
            View.CurrentScene.OpenView<TipView>().SetContent(DreamerUtil.GetColorRichText(model._config.skill_name, Color.yellow) + "\t升级成功！");
        }
        model.SetSkillLevel (1);
         
        ActorModel.Model.SetMoney(-model.GetLevelUpMoney());
        EventManager.OnSkillLevelUp(model);
    }
}
