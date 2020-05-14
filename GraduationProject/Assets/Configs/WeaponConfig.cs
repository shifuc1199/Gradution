using System.Reflection;
using UnityEngine;
using LitJson;
using System.IO;
using UnityEditor;
using System.Text;
 using Sirenix.OdinInspector;
using DreamerTool.Extra;
public   class WeaponConfig : ItemConfig<WeaponConfig>
{
    [Tip(3, GameConstData.COLOR_WHITE)]
    [BoxGroup("属性信息")]
    public WeaponType 武器种类;/*nil*/
    [BoxGroup("属性信息")]
    [Tip(4, GameConstData.COLOR_YELLOW)]
    [EquipMent(PlayerAttribute.攻击力)]
    public double 攻击力;
	[BoxGroup("属性信息")]
    [Tip(5, GameConstData.COLOR_WHITE)]
    public double 法术强度;
	[BoxGroup("属性信息")]
    [Tip(6, GameConstData.COLOR_WHITE)]
    [EquipMent(PlayerAttribute.暴击率)]
    public double 暴击率;
	[BoxGroup("属性信息")]
    [Tip(7, GameConstData.COLOR_WHITE)]
    public double 暴击伤害;
    [BoxGroup("属性信息")]
    [Tip(8, GameConstData.COLOR_WHITE)]
    public double 回复能量;
    [BoxGroup("属性信息")]
    [Tip(9, GameConstData.COLOR_WHITE)]
    public int 需要等级;
  
    public override string GetTipString()
    {
        var type = GetType();
        var fields = type.GetFields();
        StringBuilder sb = new StringBuilder();
        var color = GameStaticData.ITEM_COLOR_DICT[(ItemLevel)type.GetField("物品阶级").GetValue(this)].ToHtmlString();
        System.Collections.Generic.SortedDictionary<int, string> dict = new System.Collections.Generic.SortedDictionary<int, string>();
        foreach (var field in fields)
        {
            var tip_attribute = field.GetCustomAttribute(typeof(TipAttribute));
            if(tip_attribute!=null)
            {
                
                var attribute = (tip_attribute as TipAttribute);

                var valueStr = DreamerTool.Util.DreamerUtil.GetColorRichText(field.GetValue(this).ToString(), attribute.index==2 || attribute.index == 1 ? color : attribute.valueColor);
                dict.Add(attribute.index, field.Name + ": " + valueStr + "\n");
               
            }
        }
        foreach (var item in dict)
        {
            if(item.Key<3)
            {
                sb.Append("\t\t\t\t\t");
            }
            if (item.Key == 3)
            {
                sb.Append("\n");
            }   
            sb.Append(item.Value);
        }
        return sb.ToString();
    }
    public override void ChangePlayerAttribute()
    {
        var current = Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.武器));
        var type = GetType();
        foreach (FieldInfo f in type.GetFields())
        {
            var attribute = f.GetCustomAttribute(typeof(EquipMentAttribute));
            if (attribute != null)
            {
                var equipmentAttribute = attribute as EquipMentAttribute;
                var before = f.GetValue(current);
                var after = f.GetValue(this);
                var value = (double)after - (double)before;
                ActorModel.Model.SetPlayerAttribute(equipmentAttribute.attribute, value);
             }
        }
        
    }
    [Button("保存",50)]
	public override void Save()
	{
		TextAsset ta =Resources.Load<TextAsset>("all_config");
		JsonData jd = JsonMapper.ToObject(ta.text);
		if(jd["Weapon"]==null)
		jd["Weapon"] = new JsonData();
		JsonData data = new JsonData();
        data["物品ID"] = 物品ID;
        data["回复能量"] = 回复能量;
        data["物品名字"] =物品名字;
        data["购买价格"] = 购买价格;
        data["卖出价格"] = 卖出价格;
        data["武器种类"] = (int)武器种类;
        data["需要等级"] = 需要等级;
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
		ItemEditorWindow._window._tree.MenuItems[物品ID-1].Select();
#endif
    }

    public override Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Weapons/"+ 图标名字);
    }

 
}
 

