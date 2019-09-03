using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using XLua;
[Hotfix]
public class test : MonoBehaviour
{
    public Text text;
    DateTime time;
    double time_str = -1;
    LuaEnv _env;
    // Use this for initialization
    void Start()
    {
        _env = new LuaEnv();
        _env.DoString("require 'test'");
    }
    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       StartCoroutine(Util.GetTime((str)=> {
           time = Convert.ToDateTime(str);
           if (time_str == -1)
           {
               time_str = (time - Convert.ToDateTime(PlayerPrefs.GetString("offline_time"))).TotalSeconds;
               Debug.Log(time_str);
           }
       }
       ));//时刻更新时间
        if(time!=null)
        text.text = time.ToString();
    }
    private void OnDisable()
    {
      Debug.Log("下线：" + time.ToString());
      PlayerPrefs.SetString("offline_time", time.ToString());
         
    }

}
