/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Singleton;
using LitJson;
 
public class SaveManager : Singleton<SaveManager>
{
   
    public List<ActorModel> GetActorModels()
    {
        List<ActorModel> models = new List<ActorModel>();
        for (int i = 1; i < GameConstData.SAVE_DATA_COUNT+1; i++)
        {
            var key = GameConstData.SAVE_DATA_KEY + i.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                var model_data = PlayerPrefs.GetString(key);
                models.Add(JsonMapper.ToObject<ActorModel>(model_data));
            }
        }
        return models;
    }
    public void DeleteActorModel(int id)
    {
        var key = GameConstData.SAVE_DATA_KEY + id.ToString();
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
    public void SaveActorModel()
    {
        var json = JsonMapper.ToJson(ActorModel.Model);
        PlayerPrefs.SetString(GameConstData.SAVE_DATA_KEY + ActorModel.Model.SaveDataID, json);
    }
}
