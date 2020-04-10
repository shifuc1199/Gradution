/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Singleton;
using LitJson;
using System.IO;
public class SaveManager : Singleton<SaveManager>
{
    public ActorModel GetActorModel()
    {
        if (File.Exists(GameConstData.SAVE_DATA_PATH))
        {
            using (StreamReader sr = new StreamReader(new FileStream(GameConstData.SAVE_DATA_PATH, FileMode.Open)))
            {
                var model_data = sr.ReadToEnd();
               
                return JsonMapper.ToObject<ActorModel>(model_data);
                
            }
        }
        Debug.Log("创建新model");
        return new ActorModel();

    }
    public void SaveActorModel()
    {
        var json = JsonMapper.ToJson(ActorModel.Model);
        if (!File.Exists(GameConstData.SAVE_DATA_PATH))
        {
            File.Create(GameConstData.SAVE_DATA_PATH).Dispose();
        }
         using (StreamWriter sw = new StreamWriter(new FileStream(GameConstData.SAVE_DATA_PATH, FileMode.Truncate)))
        {
            sw.Write(json);
            
        }
    }
}
