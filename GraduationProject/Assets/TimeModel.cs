/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DreamerTool.Util;
public class TimeModel : Singleton<TimeModel>
{
    public   DateTime Now = DateTime.MinValue;

    private void Awake()
    {
         
    }

    public IEnumerator GetTime()
    {
     yield return StartCoroutine(Util.GetDateTimeFromURL((time) => {
            Now = time;
        }));
    }

    private void Update()
    {
        if(Now != DateTime.MinValue)
        Now = Now.AddSeconds(Time.deltaTime);
    }

}
