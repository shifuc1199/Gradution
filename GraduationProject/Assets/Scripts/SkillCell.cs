/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.Util;
public class SkillCell : MonoBehaviour
{
    public Text skill_info_text;
    public Text skill_level_text;
    public Image skill_image;
    public SkillModel model;
    public GameObject mask;
    private void Awake()
    {
        
    }
    public void SetModel(SkillModel model)
    {
        if (this.model != null)
            return;

        this.model = model;

        UpdateModel();
    }
    private void OnEnable()
    {
        UpdateModel();
    }
    public void UpdateModel()
    {
        if (this.model == null)
            return;
        if(!this.model.IsLearn())
        {
            skill_level_text.color = Color.gray;
            skill_level_text.text = "未学习";
            skill_image.color = Color.gray;
            skill_info_text.color = Color.gray;

            mask.SetActive(true);
        }
        else
        {
            skill_info_text.color = Color.white;
            skill_image.color = Color.white;
            skill_level_text.color = Color.white;
            skill_level_text.text = model.GetSkillLevel() + "/" + "5";

            mask.SetActive(false);
        }
        skill_info_text.text = model._config.skill_name;   
        skill_image.sprite = model._config.GetSprite();
   
    }
    
}
