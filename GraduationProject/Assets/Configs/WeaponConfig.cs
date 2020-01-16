
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using UnityEditor;
 using Sirenix.OdinInspector;
public   class WeaponConfig : ItemConfig<WeaponConfig>
{
 
    [BoxGroup("属性信息")]
    public WeaponType 武器种类;/*nil*/
    [BoxGroup("属性信息")]
    public double 攻击力;
	[BoxGroup("属性信息")]
	public double 法术强度;
	[BoxGroup("属性信息")]
	public double 暴击率;
	[BoxGroup("属性信息")]
	public double 暴击伤害;
 
    [Button("保存",50)]
	public override void Save()
	{
		TextAsset ta =Resources.Load<TextAsset>("all_config");
		JsonData jd = JsonMapper.ToObject(ta.text);
		if(jd["Weapon"]==null)
		jd["Weapon"] = new JsonData();
		JsonData data = new JsonData();
        data["物品ID"] = 物品ID;
        data["物品名字"] =物品名字;
        data["武器种类"] = (int)武器种类;
        data["物品描述"] = 物品描述;
		data["图标名字"] = 编辑器图标 ? 编辑器图标.name:"";
        data["攻击力"] = 攻击力;
        data["法术强度"] = 法术强度;
        data["物品阶级"] = (int)物品阶级;
        data["暴击率"] =暴击率;
		data["暴击伤害"] = 暴击伤害;
		jd["Weapon"][物品ID.ToString()] = data;
		using(StreamWriter sw = new StreamWriter(new FileStream("Assets/Resources/all_config.json",FileMode.Truncate)))
		{
			sw.Write(jd.ToJson());
		}

#if UNITY_EDITOR
        AssetDatabase.Refresh();
        ItemEditorWindow._window.ForceMenuTreeRebuild();
		ItemEditorWindow._window.isCreate=false;
		ItemEditorWindow._window._tree.MenuItems[ItemEditorWindow._window._tree.MenuItems.Count-1].Select();
#endif
    }
    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Weapons/"+ 图标名字);
    }






}
 

