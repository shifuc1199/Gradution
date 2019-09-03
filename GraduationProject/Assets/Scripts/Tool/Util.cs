using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
public static class Util 
{
    public static IEnumerator GetTime(Action<string> action)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=1");
        yield return www.SendWebRequest();
        string timeStr = www.downloadHandler.text.Substring(2);
        DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        DateTime time = startTime.AddMilliseconds(Convert.ToDouble(timeStr));
        timeStr = time.ToString();
        action(timeStr);
    }
}
