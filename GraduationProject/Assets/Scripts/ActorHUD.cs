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

    private void Start()
    {
        SetMoneyText();
        EventHandler.OnChangeMoney += SetMoneyText;
    }
    private void OnDisable()
    {
        EventHandler.OnChangeMoney -= SetMoneyText;
    }
    public void SetMoneyText()
    {
        money_text.text = ActorModel.Model.GetMoney().ToString(); 
    }
     
}
