/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
public class SkillJoyStickConfigPage : MonoBehaviour
{
    private SkillModel m_model;
    public Text skill_info;
    public Image[] m_skill_image;
    public Button[] m_skill_button;
    public GameObject no_learn_tip;
    public GameObject skill_buttons;
    private void Start()
    {
        UpdateEquipSkill();
    }
    public void SetModel(SkillModel model)
    {
        m_model = model;
        UpdateModel();
    }
    public void UpdateModel()
    {
        skill_info.text = m_model._config.skill_des.Replace("t", "<color=#FF9A00>" + (int)m_model.GetHurtValue() + "</color>"); ;
        if (m_model.IsLearn())
        {
            SetButtons(true);
           
         
            for (int i = 0; i < m_skill_button.Length; i++)
            {
                if ( ActorModel.Model.equip_skil[i] == m_model)
                {
                    m_skill_button[i].gameObject.SetActive(false);
                }
            }

        }
        else
        {
            SetButtons(false);
        }
    }
    public void SetButtons(bool v)
    {
        skill_buttons.SetActive(v);
        no_learn_tip.SetActive(!v);
        foreach (var item in m_skill_button)
        {
            item.gameObject.SetActive(v);

        }
    }
    public void UpdateEquipSkill()
    {
        foreach (var keyValue in ActorModel.Model.equip_skil)
        {
            if (keyValue.Value != null)
            {
                m_skill_image[keyValue.Key].gameObject.SetActive(true);
                m_skill_image[keyValue.Key].sprite = keyValue.Value._config.GetSprite();
            }
        }

    }
    public void SetSkill(int button_index)
    {
        int key=-1;
        foreach (var item in ActorModel.Model.equip_skil)
        {
            if (item.Value != null)
            {
                if (item.Value.config_id == m_model.config_id)
                {
                    key = item.Key;
                    m_skill_image[item.Key].gameObject.SetActive(false);
                    break;
                }
            }
        }
        
        if (key!= -1)
        ActorModel.Model.equip_skil[key] = null;
 
         ActorModel.Model.equip_skil[button_index] = m_model;
       

        m_skill_image[button_index].gameObject.SetActive(true);
        m_skill_image[button_index].sprite = m_model._config.GetSprite();
        View.CurrentScene.GetView<GameInfoView>().inactrive_buttons.UpdateAllJoySticks();
        UpdateModel();
    }
}
