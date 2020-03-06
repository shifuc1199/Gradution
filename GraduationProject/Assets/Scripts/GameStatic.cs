/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Extra;
using DreamerTool.GameObjectPool;

public static class GameStaticData  
{
    /*白色 绿色 蓝色 黄色 紫色 橙色 红色 黑色*/
    public static Dictionary<ItemLevel, Color> ITEM_COLOR_DICT = new Dictionary<ItemLevel, Color>()
    {
        { ItemLevel.谦卑,Color.white},
        { ItemLevel.诚实,Color.green },
        { ItemLevel.怜悯,Color.blue },
        { ItemLevel.英勇,Color.yellow },
        { ItemLevel.公正,"#BF2CAF".GetColorByString() },
        { ItemLevel.牺牲,"#E77E11".GetColorByString() },
        { ItemLevel.荣誉,Color.red },
        { ItemLevel.灵魂,Color.black },
    };

   

}
public  class GameStaticMethod
{
    public static void GameInit()
    {
        GameObjectPoolManager.InitByScriptableObject();
        ActorModel model = new ActorModel();
        SkillModel.Init();
    }
}

