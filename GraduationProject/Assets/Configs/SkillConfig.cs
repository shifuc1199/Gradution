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
public class SkillConfig :  BaseConfig<SkillConfig>
{
    [ReadOnly]
    public int ID;
    [PreviewField(100)][AssetsOnly]public Sprite editor_sprite;
    public string skill_name;
    public SkillType skill_type;
    [System.NonSerialized]public string sprite_path;
    [TextArea]public string skill_des;

    [BoxGroup("技能伤害数据")] public double basic_hurt;
    [BoxGroup("技能伤害数据")] public double skill_level_ratio;
    [BoxGroup("技能伤害数据")] public double actor_attack_ratio;

    [BoxGroup("技能升级数据")] public double skill_level_up_basic_money;
    [BoxGroup("技能升级数据")] public double skill_level_up_money_ratio;

    [BoxGroup("冷却时间数据")] public double basic_skill_cool_time;
    [BoxGroup("冷却时间数据")] public double skill_cool_ratio;

    [Button("保存", 50)]
    public void Save()
    {
#if UNITY_EDITOR
        TextAsset ta = Resources.Load<TextAsset>("all_config");
        JsonData jd = JsonMapper.ToObject(ta.text);
        if (jd["Skill"] == null)
            jd["Skill"] = new JsonData();
        JsonData data = new JsonData();
        data["ID"] = ID;
        data["skill_type"] = (int)skill_type;
        data["skill_name"] = skill_name;
        data["skill_des"] = skill_des;
        data["basic_hurt"] = basic_hurt;
        data["skill_level_ratio"] = skill_level_ratio;
        data["actor_attack_ratio"] = actor_attack_ratio;
        data["basic_skill_cool_time"] = basic_skill_cool_time;
        data["skill_level_up_basic_money"] = skill_level_up_basic_money;
        data["skill_level_up_money_ratio"] = skill_level_up_money_ratio;
        data["skill_cool_ratio"] = skill_cool_ratio;
        data["sprite_path"] = editor_sprite ? AssetDatabase.GetAssetPath(editor_sprite).Substring(0, AssetDatabase.GetAssetPath(editor_sprite).Length - 4).Substring(17) : "";
        jd["Skill"][ID.ToString()] = data;
        using (StreamWriter sw = new StreamWriter(new FileStream("Assets/Resources/all_config.json", FileMode.Truncate)))
        {
            sw.Write(jd.ToJson());
            sw.Close();
        }
        AssetDatabase.Refresh();
        SkillEditorWindow._window.ForceMenuTreeRebuild();
        SkillEditorWindow._window.isCreate = false;
        SkillEditorWindow._window._tree.MenuItems[ID-1].Select();
#endif
    }
    public static void RemoveAll()
    {
#if UNITY_EDITOR
        TextAsset ta = Resources.Load<TextAsset>("all_config");
        JsonData jd = JsonMapper.ToObject(ta.text);
        if (jd["Skill"] == null)
            jd["Skill"] = new JsonData();
        jd["Skill"] = null;
        using (StreamWriter sw = new StreamWriter(new FileStream("Assets/Resources/all_config.json", FileMode.Truncate)))
        {
            sw.Write(jd.ToJson());
            sw.Close();
        }
        AssetDatabase.Refresh();
        SkillEditorWindow._window.ForceMenuTreeRebuild();
        SkillEditorWindow._window.isCreate = false;
        if (SkillEditorWindow._window._tree.MenuItems.Count > 0)
            SkillEditorWindow._window._tree.MenuItems[SkillEditorWindow._window._tree.MenuItems.Count - 1].Select();
#endif
    }
    public Sprite GetSprite()
    {
        return Resources.Load<Sprite>(sprite_path);
    }
    public void SetEditorSprite()
    {
        if (editor_sprite == null)
            editor_sprite = GetSprite();
    }
}
