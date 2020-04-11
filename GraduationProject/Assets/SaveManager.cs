/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Singleton;
using LitJson;
using DreamerTool.UI;
 

public class SaveManager : Singleton<SaveManager>
{
    public LoginType LoginType;
   
    public List<ActorModel> GetActorModels()
    {
        List<ActorModel> models = new List<ActorModel>();
        switch (LoginType)
        {
            case LoginType.游客:
                
                for (int i = 1; i < GameConstData.SAVE_DATA_COUNT + 1; i++)
                {
                    var key = GameConstData.SAVE_DATA_KEY + i.ToString();
                    if (PlayerPrefs.HasKey(key))
                    {
                        var model_data = PlayerPrefs.GetString(key);
                        models.Add(JsonMapper.ToObject<ActorModel>(model_data));
                    }
                }
                return models;
            case LoginType.短信:

                var data = JsonMapper.ToObject(BmobManager.Instance.UserBmobDao.data);
             
                for (int i = 1; i < GameConstData.SAVE_DATA_COUNT + 1; i++)
                {
                   
                    var key = i.ToString();
                    if (data.Keys.Contains(key))
                    {
                        var json = data[key].ToString();
                        models.Add(JsonMapper.ToObject<ActorModel>(json));
                    }
                }
                return models;
            default:
                return models;
        }
       
    
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
        
        switch (LoginType)
        {
            case LoginType.游客:
                var guest_data = JsonMapper.ToJson(ActorModel.Model);
                PlayerPrefs.SetString(GameConstData.SAVE_DATA_KEY + ActorModel.Model.SaveDataID, guest_data);
                break;
            case LoginType.短信:
                View.CurrentScene.OpenView<LoadView>();
                var mobile_data = JsonMapper.ToJson(ActorModel.Model);
                JsonData data = new JsonData();
                data[ActorModel.Model.SaveDataID.ToString()] = mobile_data;
                BmobManager.Instance.UserBmobDao.data = data.ToJson();
 
                BmobManager.Instance.UserBmobDao.Update(()=> {

                    View.CurrentScene.OpenView<LoadView>().SetText("保存成功!");
                });
                break;
            default:
                break;
        }
        
    }
}
