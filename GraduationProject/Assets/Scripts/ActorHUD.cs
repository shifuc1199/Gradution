/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ActorHUD : MonoBehaviour
{
    public Image coin_icon;
    public Text health_text;
    public Text energy_text;
    public Image health_bar;
    public Image energy_bar;
    public Text money_text;
    private void Awake()
    {
        EventManager.OnChangeMoney += UpdateMoneyText;
        EventManager.OnChangeHealth += UpdateHealth;
        EventManager.OnChangeEnergy += UpdateEnergy;
    }
    private void Start()
    {
        UpdateMoneyText();
        UpdateEnergy();
        UpdateMoneyText();
    }
    private void OnDestroy()
    {
        EventManager.OnChangeMoney -= UpdateMoneyText;
        EventManager.OnChangeHealth -= UpdateHealth;
        EventManager.OnChangeEnergy -= UpdateEnergy;
    }
    public void UpdateEnergy()
    {
         
        energy_text.text = ActorModel.Model.GetEngery() + "/" + ActorModel.Model.GetPlayerAttribute(PlayerAttribute.能量值);
        energy_bar.fillAmount = (float)(ActorModel.Model.GetEngery() / ActorModel.Model.GetPlayerAttribute(PlayerAttribute.能量值));
    }
    public void UpdateHealth()
    {
        
        health_text .text  = ActorModel.Model.GetHealth()+"/"+ ActorModel.Model.GetPlayerAttribute(PlayerAttribute.生命值);
        health_bar.fillAmount =(float)( ActorModel.Model.GetHealth()/ActorModel.Model.GetPlayerAttribute(PlayerAttribute.生命值));
    }
    public void UpdateMoneyText()
    {
        money_text.text = ActorModel.Model.GetMoney().ToString(); 
    }
    private void Update()
    {
        
    }

}
