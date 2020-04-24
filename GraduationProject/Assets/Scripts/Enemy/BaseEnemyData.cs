/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class BaseEnemyData 
{
    public bool isdie;
    public double maxhealth;
    public double health;
    public Sprite head;
    public string enemy_name;
    public UnityAction die_call_back;
     public BaseEnemyData(EnemyModel model,UnityAction die_call_back=null)
    {
     
        maxhealth = model.GetMaxHealth();
        head = model.config.GetSprite();
        enemy_name = model.config.EnemyName;
        health = maxhealth;
       
        this.die_call_back = die_call_back;
    }

    public void SetHealth(double v)
    {
        
        if (isdie)
            return;

        health += v;
 
        if(health<=0)
        {
            isdie = true;
            die_call_back?.Invoke();
        }
    }
}
