/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FileCell : MonoBehaviour
{
    private FileDataCell data_cell;
    private Button createActorBtn;
    ActorModel model;
  
    private void Awake()
    {
        data_cell = GetComponentInChildren<FileDataCell>(true);
        createActorBtn = GetComponentInChildren<Button>();
        
    }
    public void DeleteActor()
    {
        SetModel(null);
        SaveManager.Instance.DeleteActorModel(model.SaveDataID);
    }
    public void ChooseActor()
    {
        ActorModel.UpdateModel(model);
         
        LoadingScene.LoadScene(GameConstData.GAME_MAIN_SCENE_NAME);
    }
    public void SetModel(ActorModel model)
    {
        if(model == null)
        {
            data_cell.gameObject.SetActive(false);
            createActorBtn.gameObject.SetActive(true);
            return;
        }
        this.model = model;
        data_cell.SetModel(model);
        createActorBtn.gameObject.SetActive(false);
    } 

    public void CreateActorOnClick()
    {
         
        ActorModel.CreateModel();
        ActorModel.Model.SaveDataID = transform.GetSiblingIndex() + 1;
        LoadingScene.LoadScene(GameConstData.CREATE_ACTOR_SCENE_NAME);
    }
    
}
