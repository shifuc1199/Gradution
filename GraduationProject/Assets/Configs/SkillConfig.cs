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
    public string SkillName;
    [System.NonSerialized]
    public string sprite_path;
    [PreviewField(100)]
    [AssetsOnly]
    [HideLabel]
    public Sprite editor_sprite;
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
        data["sprite_path"] = editor_sprite ? AssetDatabase.GetAssetPath(editor_sprite).Substring(0, AssetDatabase.GetAssetPath(editor_sprite).Length - 4).Substring(17) : "";
        jd["Skill"] = data;
        using (StreamWriter sw = new StreamWriter(new FileStream("Assets/Resources/all_config.json", FileMode.Truncate)))
        {
            sw.Write(jd.ToJson());
            sw.Close();
        }
        AssetDatabase.Refresh();
        SkillEditorWindow._window.ForceMenuTreeRebuild();
        SkillEditorWindow._window.isCreate = false;
        SkillEditorWindow._window._tree.MenuItems[SkillEditorWindow._window._tree.MenuItems.Count - 1].Select();
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
