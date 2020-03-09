using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillModel 
{
    public SkillConfig _config;
    private int skill_level=0;
    private bool is_learn = false;
    public void SetSkillLevel(int v)
    {
        skill_level += v;
    }
    public double GetLearnMoney()
    {
        return _config.skill_level_up_basic_money;
    }
    public double GetLevelUpMoney()
    {
        return _config.skill_level_up_basic_money + _config.skill_level_up_basic_money*_config.skill_level_up_money_ratio*skill_level;
    }
    public bool IsLearn()
    {
        return is_learn;
    }
    public int GetSkillLevel()
    {
        return skill_level;
    }
    public void Learn()
    {
        is_learn = true;
    }
    public SkillModel(int config_id)
    {
        this._config = SkillConfig.Get(config_id);
    }
 
    public double GetCoolTime()
    {
        return _config.basic_skill_cool_time+_config.skill_cool_ratio * skill_level;
    }
    public double GetHurtValue()
    {
        return _config.basic_hurt + skill_level * _config.skill_level_ratio + _config.actor_attack_ratio * ActorModel.Model.GetPlayerAttribute(PlayerAttribute.攻击力);
    }
    public static SkillModel Get(int id)
    {
        if(ActorModel.Model.skillmodels.ContainsKey(id))
        return ActorModel.Model.skillmodels[id];

        return null;
        
    }
    public static void Init()
    {
        foreach(var item in SkillConfig.Datas)
        {
            if (!ActorModel.Model.skillmodels.ContainsKey(item.Key))
            ActorModel.Model.skillmodels.Add(item.Key, new SkillModel(item.Key));
        }
        
    }
}
 
