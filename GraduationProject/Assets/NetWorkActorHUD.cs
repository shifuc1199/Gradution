/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
public class NetWorkActorHUD : MonoBehaviour
{

    public Text health_text;
    public Text energy_text;
    public Image health_bar;
    public Image energy_bar;


    private ActorModel model;
    public FileDataActor head;
    private void Awake()
    {
        
    }
    public void SetModel(ActorModel model)
    {
        this.model = model;
        head.SetModel(model);
       
    }
     
    public void UpdateEnergy()
    {
        energy_text.text = (int)model.GetEngery() + "/" + model.GetPlayerAttribute(PlayerAttribute.能量值);
        energy_bar.fillAmount = (float)(model.GetEngery() / model.GetPlayerAttribute(PlayerAttribute.能量值));
    }
    public void UpdateHealth()
    {
        if(model.GetHealth()<=0)
        {
            (View.CurrentScene as FightScene).GameOver(transform.GetSiblingIndex());
        }
        health_text.text = (int)model.GetHealth() + "/" + model.GetPlayerAttribute(PlayerAttribute.生命值);
        health_bar.fillAmount = (float)(model.GetHealth() / model.GetPlayerAttribute(PlayerAttribute.生命值));
    }
 
    private void Update()
    {
        if (model == null)
            return;
        UpdateHealth();
        UpdateEnergy();
    }
}
