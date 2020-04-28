/*****************************
Created by 师鸿博
*****************************/
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Reflection;
using System.Text;

public class ShieldConfig : ItemConfig<ShieldConfig>
{
    public double helath;
    public double magic_defend;
    public double defend;
    [Button("保存", 50)]
    public override void Save()
    {
#if UNITY_EDITOR
        TextAsset ta = Resources.Load<TextAsset>("all_config");
        JsonData jd = JsonMapper.ToObject(ta.text);
        if (jd["Shield"] == null)
            jd["Shield"] = new JsonData();
        JsonData data = new JsonData();
        data["物品ID"] = 物品ID;
        data["health"] = helath;
        data["magic_defend"] = magic_defend;
        data["defend"] = defend;
        data["物品名字"] = 物品名字;
        data["物品描述"] = 物品描述;
        data["购买价格"] = 购买价格;
        data["卖出价格"] = 卖出价格;
        data["图标名字"] = 编辑器图标 ? AssetDatabase.GetAssetPath(编辑器图标).Substring(0, AssetDatabase.GetAssetPath(编辑器图标).Length-4).Substring(17) : "";
        data["物品阶级"] = (int)物品阶级;
        jd["Shield"][物品ID.ToString()] = data;
        using (StreamWriter sw = new StreamWriter(new FileStream("Assets/Resources/all_config.json", FileMode.Truncate)))
        {
            sw.Write(jd.ToJson());
        }
        AssetDatabase.Refresh();
        ItemEditorWindow._window.ForceMenuTreeRebuild();
        ItemEditorWindow._window.isCreate = false;
        if(ItemEditorWindow._window._tree.MenuItems.Count>0)
            ItemEditorWindow._window._tree.MenuItems[物品ID - 1].Select();
#endif
    }
    public override string GetTipString()
    {
        var type = GetType();
        var fields = type.GetFields();
        StringBuilder sb = new StringBuilder();
        SortedDictionary<int, string> dict = new SortedDictionary<int, string>();
        foreach (var field in fields)
        {
            var tip_attribute = field.GetCustomAttribute(typeof(TipAttribute));
            if (tip_attribute != null)
            {
                var attribute = (tip_attribute as TipAttribute);
                var valueStr = DreamerTool.Util.DreamerUtil.GetColorRichText(field.GetValue(this).ToString(), attribute.valueColor);
                dict.Add(attribute.index, field.Name + ": " + valueStr + "\n");

            }
        }
        foreach (var item in dict)
        {
            if (item.Key < 3)
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
        var current = Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.盾牌));
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
    public override Sprite GetSprite()
    {
 
        return Resources.Load<Sprite>(图标名字);
    }


}
 