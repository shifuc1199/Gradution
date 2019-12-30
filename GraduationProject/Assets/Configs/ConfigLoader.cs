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
	}
}
