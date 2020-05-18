/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
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
        EventManager.OnChangePlayerAttribute += UpdateMaxHealth;
    }
    private void Start()
    {
        UpdateMoneyText();
        UpdateEnergy();
        UpdateHealth();
    }
    private void OnDestroy()
    {
        EventManager.OnChangeMoney -= UpdateMoneyText;
        EventManager.OnChangeHealth -= UpdateHealth;
        EventManager.OnChangeEnergy -= UpdateEnergy;
        EventManager.OnChangePlayerAttribute -= UpdateMaxHealth;
    }
    public void UpdateMaxHealth (PlayerAttribute p,double v)
    {
        if(p == PlayerAttribute.生命值)
        {
            UpdateHealth();
        }
    }
    public void UpdateEnergy()
    {
        energy_text.text = (int)ActorModel.Model.GetEngery() + "/" + ActorModel.Model.GetPlayerAttribute(PlayerAttribute.能量值);
        energy_bar.fillAmount = (float)(ActorModel.Model.GetEngery() / ActorModel.Model.GetPlayerAttribute(PlayerAttribute.能量值));
    }
    public void UpdateHealth()
    {
        
        health_text .text  = (int)ActorModel.Model.GetHealth()+"/"+ ActorModel.Model.GetPlayerAttribute(PlayerAttribute.生命值);
        health_bar.fillAmount =(float)( ActorModel.Model.GetHealth()/ActorModel.Model.GetPlayerAttribute(PlayerAttribute.生命值));
        if(ActorModel.Model.GetHealth()<=0)
        {
            TimeModel.SetTimeScale(0.2f);
            View.CurrentScene.OpenView<TipView>().SetContent("你已经死了！!",()=> {
                TimeModel.SetTimeScale(1);
                ActorModel.Model.ResetState();
                LoadingScene.LoadScene(GameConstData.GAME_MAIN_SCENE_NAME);
            });
        }
    }
    public void UpdateMoneyText()
    {
        money_text.text = ActorModel.Model.GetMoney().ToString(); 
    }
    private void Update()
    {
        
    }

}
