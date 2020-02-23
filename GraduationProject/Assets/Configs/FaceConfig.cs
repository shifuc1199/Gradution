/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using LitJson;
using UnityEditor;
using System.IO;
public class FaceConfig<T> : BaseConfig<T> where T:BaseConfig<T>
{
    [ReadOnly]
    public int ID;
    [System.NonSerialized]
    public string sprite_path;
    [PreviewField(100)][AssetsOnly][HideLabel]
    public Sprite editor_sprite;
    [Button("保存",50)]
    public void Save()
    {
#if UNITY_EDITOR
        TextAsset ta = Resources.Load<TextAsset>("all_config");
        JsonData jd = JsonMapper.ToObject(ta.text);
        if (jd[typeof(T).Name] == null)
            jd[typeof(T).Name] = new JsonData();
        JsonData data = new JsonData();
        data["ID"] = ID;
        data["sprite_path"] = editor_sprite ? AssetDatabase.GetAssetPath(editor_sprite).Substring(0, AssetDatabase.GetAssetPath(editor_sprite).Length - 4).Substring(17) : "";
        jd[typeof(T).Name][ID.ToString()] = data;
        using (StreamWriter sw = new StreamWriter(new FileStream("Assets/Resources/all_config.json", FileMode.Truncate)))
        {
            sw.Write(jd.ToJson());
            sw.Close();
        }
        AssetDatabase.Refresh();
        FaceEditorWindow._window.ForceMenuTreeRebuild();
        FaceEditorWindow._window.isCreate = false;
        FaceEditorWindow._window._tree.MenuItems[ID - 1].Select();
#endif
    }
    public static void RemoveAll()
    {
#if UNITY_EDITOR
        TextAsset ta = Resources.Load<TextAsset>("all_config");
        JsonData jd = JsonMapper.ToObject(ta.text);
        if (jd[typeof(T).Name] == null)
            jd[typeof(T).Name] = new JsonData();
        jd[typeof(T).Name]= null;
        using (StreamWriter sw = new StreamWriter(new FileStream("Assets/Resources/all_config.json", FileMode.Truncate)))
        {
            sw.Write(jd.ToJson());
            sw.Close();
        }
        AssetDatabase.Refresh();
        FaceEditorWindow._window.ForceMenuTreeRebuild();
#endif
    }
    public Sprite GetSprite()
    {
        return Resources.Load<Sprite>(sprite_path);
    }
    public void SetEditorSprite()
    {
        if(editor_sprite==null)
        editor_sprite = GetSprite();
    }
    
}

public class EyeConfig:FaceConfig<EyeConfig>{
    public static void LoadAll()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Eye");
        foreach (var item in sprites)
        {
            EyeConfig e = new EyeConfig();
            e.ID = Datas.Count+1;
            e.editor_sprite = item;
            e.Save();
        }
      
    }


}
public class MouthConfig : FaceConfig<MouthConfig> {
    public static void LoadAll()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Mouth");
        foreach (var item in sprites)
        {
            MouthConfig e = new MouthConfig();
            e.ID = Datas.Count + 1;
            e.editor_sprite = item;
            e.Save();
        }
    }
}
public class HairConfig : FaceConfig<HairConfig> {
    public static void LoadAll()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Hair");
        foreach (var item in sprites)
        {
              HairConfig e = new HairConfig();
               e.ID = Datas.Count + 1;
               e.editor_sprite = item;
               e.Save();
          
        }
    }
}
public class EarConfig : FaceConfig<EarConfig> {
    public static void LoadAll()
    {
       
        Sprite[] sprites = Resources.LoadAll<Sprite>("Ear");
        foreach (var item in sprites)
        {
            EarConfig e = new EarConfig();
            e.ID = Datas.Count + 1;
            e.editor_sprite = item;
            e.Save();

        }
    }

}
public class HairDecorateConfig : FaceConfig<HairDecorateConfig>
{
    public static void LoadAll()
    {
       
        Sprite[] sprites = Resources.LoadAll<Sprite>("HairDecorate");
        foreach (var item in sprites)
        {
            HairDecorateConfig e = new HairDecorateConfig();
            e.ID = Datas.Count + 1;
            e.editor_sprite = item;
            e.Save();

        }
    }
}