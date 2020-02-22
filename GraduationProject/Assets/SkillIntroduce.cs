/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillIntroduce : MonoBehaviour
{
    public Text intro_text;
    public GifManager gif;
    public void SetModel(SkillModel model)
    {
        intro_text.text = model._config.skill_des.Replace("t", "<color=#FF9A00>" + model.GetHurtValue() + "</color>");
        gif.SetPath(model._config.ID.ToString());
    }
}
