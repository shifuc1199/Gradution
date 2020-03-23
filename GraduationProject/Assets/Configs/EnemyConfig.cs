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
public class EnemyConfig : BaseConfig<EnemyConfig>  
{
    [ReadOnly]
    [BoxGroup("敌人基础信息")]
    [VerticalGroup("敌人基础信息/p/o")]
    public int EnemyID;
    [BoxGroup("敌人基础信息")]
    [VerticalGroup("敌人基础信息/p/o")]
    public string EnemyName;
    [HideLabel]
    [PreviewField(100)]
    [BoxGroup("敌人基础信息")]
    [HorizontalGroup("敌人基础信息/p",100)]
    public GameObject EnemyPrefab;
    [JsonNonField]
    [HideLabel, PreviewField(100)]
    [AssetsOnly]
    [BoxGroup("敌人基础信息")]
    [VerticalGroup("敌人基础信息/o", 100)]
    public Sprite 编辑器图标 = null;
    [System.NonSerialized]
    public string 图标名字;
    [System.NonSerialized]
    public string EnemyPrefabName;
    [BoxGroup("敌人属性")]
    public double MaxHealth;
    public double attack;
    public double defend;
    public double level_ratio;
    [Button("保存", 50)]
    public void Save()
    {
        TextAsset ta = Resources.Load<TextAsset>("all_config");
        JsonData jd = JsonMapper.ToObject(ta.text);
        if (jd["Enemy"] == null)
            jd["Enemy"] = new JsonData();
        JsonData data = new JsonData();
        data["EnemyID"] = EnemyID;
        data["图标名字"] = 编辑器图标? 编辑器图标.name:"";
        data["EnemyPrefabName"] = EnemyPrefab ? EnemyPrefab.name : ""; 
        data["EnemyName"] = EnemyName;
        data["attack"] = attack;
        data["defend"] = defend;
        data["level_ratio"] = level_ratio;
        data["MaxHealth"] = MaxHealth;
        jd["Enemy"][EnemyID.ToString()] = data;
 
        using (StreamWriter sw = new StreamWriter(new FileStream("Assets/Resources/all_config.json", FileMode.Truncate)))
        {
            sw.Write(jd.ToJson());
        }

#if UNITY_EDITOR
        AssetDatabase.Refresh();
        EnemyEditorWindow._window.ForceMenuTreeRebuild();
        EnemyEditorWindow._window.isCreate = false;
        EnemyEditorWindow._window._tree.MenuItems[EnemyID-1].Select();
#endif
    }
    public void SetEditorPrefab()
    {
        if (EnemyPrefab == null)
            EnemyPrefab = GetGameObjectPrefab();
    }
    public void SetEditorSprite()
    {
        if (编辑器图标 == null)
            编辑器图标 = GetSprite();
    }
    public Sprite GetSprite()
    {
        return Resources.Load<Sprite>(图标名字);
    }
    public GameObject GetGameObjectPrefab()
    {
        return Resources.Load<GameObject>(EnemyPrefabName);
    }
}
