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
        TorsoConfig.LoadFromJson(json["Torso"]);
    }
}
