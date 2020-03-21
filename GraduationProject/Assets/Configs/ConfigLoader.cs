using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class ConfigLoader
{
	static public void LoadFromJson(JsonData json)
	{
        WeaponConfig.LoadFromJson(json["Weapon"]);
        EnemyConfig.LoadFromJson(json["Enemy"]);
        FootConfig.LoadFromJson(json["Foot"]);
        SleeveConfig.LoadFromJson(json["Sleeve"]);
        ArmConfig.LoadFromJson(json["Arm"]);
        PelvisConfig.LoadFromJson(json["Pelvis"]);
        EyeConfig.LoadFromJson(json["EyeConfig"]);
        MouthConfig.LoadFromJson(json["MouthConfig"]);
        HairConfig.LoadFromJson(json["HairConfig"]);
        EarConfig.LoadFromJson(json["EarConfig"]);
        SkillConfig.LoadFromJson(json["Skill"]);
        TorsoConfig.LoadFromJson(json["Torso"]);
        ShieldConfig.LoadFromJson(json["Shield"]);
        ConsumablesConfig.LoadFromJson(json["Consumables"]);
        HairDecorateConfig.LoadFromJson(json["HairDecorateConfig"]);
    }
}
