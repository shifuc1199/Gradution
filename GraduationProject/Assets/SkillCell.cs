/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillCell : MonoBehaviour
{
    public Text skill_info_text;
    public Image skill_image;
 
    public SkillModel model;
     public void SetModel(SkillModel model)
    {
        this.model = model;
        skill_info_text.text = model._config.skill_name+"\n等级:"+model.skill_level+"\n升级所需金钱: 500";
        skill_image.sprite = model._config.GetSprite();
    }
}
