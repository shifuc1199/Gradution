/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class MainView : View
{
    public void GuestLogin()
    {
        LoadingScene.LoadScene(GameConstData.CREATE_ACTOR_SCENE_NAME);
    }
}
