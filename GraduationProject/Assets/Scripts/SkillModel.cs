using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillModel 
{
    public SkillConfig _config;
    public int skill_level=1;
    public bool is_learn = false;
    public SkillModel(int config_id)
    {
        this._config = SkillConfig.Get(config_id);
    }
    public double GetHurtValue()
    {
        return _config.basic_hurt + skill_level * _config.skill_level_ratio + _config.actor_attack_ratio * ActorModel.Model.GetAttack();
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
 
