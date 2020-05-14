/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel 
{
   public readonly EnemyConfig config;

   private int level=1;

   public EnemyModel(int config_id,int level)
   {
        config = EnemyConfig.Get(config_id);

        this.level = level;
   }

    public double GetMaxHealth()
    {
        return level * config.MaxHealth * config.level_ratio;
    }
    public double GetDefend()
    {
        return level * config.defend * config.level_ratio;
    }
    public double GetAttack()
    {
 
        return level * config.attack * config.level_ratio;
    }
}
