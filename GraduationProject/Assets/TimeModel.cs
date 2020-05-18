/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DreamerTool.Singleton;
using DreamerTool.Util;
public class TimeModel : MonoSingleton<TimeModel>
{
    public   DateTime Now = DateTime.MinValue;

    private void Awake()
    {
         
    }
    public static void   SetTimeScale(float value)
    {
        Time.timeScale = value;
    }
    public System.DateTime GetDateTimeBySeconds(int second)
    {
        var hour = second / 3600;
        second -= hour * 3600;
        var minute = second / 60;
        second -= minute * 60;
    
        return new System.DateTime(1, 1, 1, hour, minute, second);
    }
    public IEnumerator GetTime()
    {
     yield return StartCoroutine(DreamerUtil.GetDateTimeFromURL((time) => {
            Now = time;
        }));
    }

    private void Update()
    {
        if(Now != DateTime.MinValue)
        Now = Now.AddSeconds(Time.deltaTime);
 
    }

}
