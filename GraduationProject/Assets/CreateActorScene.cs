/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
using DreamerTool.GameObjectPool;
public class CreateActorScene : Scene
{
    public InputField name_field;
    ActorModel actor = new ActorModel();
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        
    }
    public void LoadSceneJump(string scene_name)
    {
        LoadingScene.LoadScene(scene_name);
        
         
       
    }
    public void SetName()
    {
        ActorModel.Model.actor_name = name_field.text;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
