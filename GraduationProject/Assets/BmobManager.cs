 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Singleton;
using LitJson;
using cn.bmob.api;
public class BmobManager : MonoSingleton<BmobManager>
{
    public BmobUnity Bmob;
    public UserBmobDao UserBmobDao;
 
    private void Start()
    {
        Bmob = GetComponent<BmobUnity>();

        if (Bmob == null)
            Bmob = gameObject.AddComponent<BmobUnity>();

        Bmob.ApplicationId = GameConstData.BMOB_APP_ID;
        Bmob.RestKey = GameConstData.BMOB_REST_KEY;

        DontDestroyOnLoad(gameObject);
    }

}