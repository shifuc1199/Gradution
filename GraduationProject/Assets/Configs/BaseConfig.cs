using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class BaseConfig<T> where T : BaseConfig<T>
{
    static public Dictionary<int , T> Datas = new Dictionary<int, T>();
    public BaseConfig()
    {
    }
    static public JsonData LoadJson(string path)
    {
        JsonData json = ResManager.GetJson(path);
        if (json == null)
        {
            Debug.Log("Config Load Faild: " + path);
        }
        return json;
    }
    static public void LoadFromJson(JsonData data)
    {
        if (data == null)
        {
            Debug.Log("静态数据为空"+typeof(T).ToString());
            return;
        }
        Datas.Clear();
        IDictionary<string,JsonData> dict = data.ToObject();
        foreach (var pair in dict)
        {
            T model = JsonMapper.ToObject<T>(pair.Value.ToJson());
            model.OnLoadJsonEnded();
            Datas[pair.Key.ToInt()] = model;
        }
    }
    public virtual void OnLoadJsonEnded(){}
    static public T Get(int key)
    {
        if (Datas.Count == 0)
        {
            TextAsset ta = Resources.Load<TextAsset>("all_config");
            ConfigLoader.LoadFromJson(JsonMapper.ToObject(ta.text));
        }
        if (Datas.ContainsKey(key))
        {
            return Datas[key];
        }
        Debug.LogError(typeof(T).ToString() + "找不到ID为" + key + "的条目");
        return null;
    }
    static public List<T> ToList()
    {
        var list = new List<T>();
        foreach(var pair in Datas)
        {
            list.Add(pair.Value);
        }
        return list;
    }
    static public int Count
    {
        get
        {
            return Datas.Count;
        }
    }
    
}
