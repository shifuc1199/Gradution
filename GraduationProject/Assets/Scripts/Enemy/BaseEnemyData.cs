/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyData 
{
    public double maxhealth;
    public double health;
    public Sprite head;
    public string enemy_name;

     public BaseEnemyData(EnemyConfig _config)
    {
        maxhealth = _config.MaxHealth;
        head = _config.GetSprite();
        enemy_name = _config.EnemyName;
        health = maxhealth;
    }

    public void SetHealth(double v)
    {
        health += v;
    }
}
