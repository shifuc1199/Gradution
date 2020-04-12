/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using cn.bmob.api;
using cn.bmob.tools;
using cn.bmob.io;
 
public class StartScene : Scene
{

   
    private void Start()
    {
        Application.targetFrameRate = 60;
        BmobDebug.Register(Debug.Log);
        
    }
}
