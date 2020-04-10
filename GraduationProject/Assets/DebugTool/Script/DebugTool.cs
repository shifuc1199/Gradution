using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DebugFolder
{
    public LogType type;
    public Dictionary<string, DebugInfo> info;
    public string stackTrace;
    public string lastCondition;
    public bool showDetails=false;
    public int count;
    public Vector2 scroll;
    public DebugInfo LastInfo
    {
        get
        {
            return info[lastCondition];
        }
    }
}
public class DebugInfo
{
    public string condition;
    public int count;
    public System.DateTime time;
}

public class DebugTool : MonoBehaviour
{
  
    Dictionary<string, DebugFolder> debugInfo;
    Vector2 scrollPos;
    public GUISkin debugSkin;
    int logCount;
    int warningCount;
    int errorCount;
    bool showLog;
    bool showWarning;
    bool showError;
    float fps;
    int showFlag;
    bool showMax
    {
        get
        {
            return showFlag == 10;
        }
    }
    bool showMin
    {
        get
        {
            return showFlag == 11;
        }
    }
    Coroutine fpsCountCor;
    public void Init()
    {
        errorCount = 0;
        warningCount = 0;
        logCount = 0;
        showLog = PlayerPrefs.GetInt("DebugTool" + "showLog",1) == 1;
        showWarning = PlayerPrefs.GetInt("DebugTool" + "showLog", 1) == 1;
        showError =PlayerPrefs.GetInt("DebugTool" + "showError",1) ==1;
        showFlag = 0;
    }
    public void Save()
    {
        PlayerPrefs.SetInt("DebugTool" + "showLog", showLog ? 1 : 0);
        PlayerPrefs.SetInt("DebugTool" + "showWarning", showWarning ? 1 : 0);
        PlayerPrefs.SetInt("DebugTool" + "showError", showError ? 1 : 0);
    }
    private void Start()
    {
    }
    public void OnEnable()
    {
        Init();
        Application.logMessageReceived += ReceiveDebugInfo;
        debugInfo = new Dictionary<string, DebugFolder>();
        fpsCountCor =  StartCoroutine(FpsCount());
    }
    public void OnDisable()
    {
        Application.logMessageReceived += ReceiveDebugInfo;
        debugInfo = null;
        if (fpsCountCor != null) StopCoroutine(fpsCountCor);
        Save();
    }
    public bool BeLike(string a, string b, float rate = 0.8f)
    {
        var maxDistance =Mathf.Max(3 ,Math.Min(a.Length,b.Length)*rate);
        if (Distance(a, b) > maxDistance)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public static int Distance(string a,string b)
    {
        var x = 0;
        var y = 0;
        var aList = new List<char>();
        var bList = new List<char>();
        aList.AddRange(a);
        bList.AddRange(b);
        aList.Sort();
        bList.Sort();
        var count = 0;
        while (x < aList.Count&&y< bList.Count)
        {

            if (a[x] == b[y])
            {
                x++;
                y++;
                count++;
            }
            else
            {
                var d = a[x] - b[y];
                if (d > 0)
                {
                    y++;
                }
                else
                {
                    x++;
                }
            }    
        }
        return Mathf.Max(a.Length,b.Length)-count;
    }
    IEnumerator FpsCount()
    {
        int count = 0;
        var startTime = Time.realtimeSinceStartup;
        while (true)
        {
           
            count ++;
            yield return null;
            if (count == 20)
            {
                fps = 1f / ((Time.realtimeSinceStartup - startTime) / count);
                startTime = Time.realtimeSinceStartup;
                count = 0;
            }
           
        }
    }
    IEnumerator ShowFlagDown()
    {
        yield return new WaitForSeconds(1);
        if (showFlag > 0 && showFlag < 10)
        {
            showFlag--;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (showFlag < 5)
            {
                showFlag++;
                StartCoroutine(ShowFlagDown());
            }
            else if(showFlag==5)
            {
                showFlag = 10;
            }
            
        }
    }
    public void ReceiveDebugInfo(string condition, string stackTrace, LogType type)
    {
        switch (type)
        {
            case LogType.Error:
                errorCount++;
                break;
            case LogType.Assert:
                break;
            case LogType.Warning:
                warningCount++;
                break;
            case LogType.Log:
                logCount++;
                break;
            case LogType.Exception:
                break;
            default:
                break;
        }
      
        if (string.IsNullOrEmpty(stackTrace)){
            var initKey = false;
            foreach (var kv in debugInfo)
            {
                if (BeLike(condition, kv.Key)&&kv.Value.type==type)
                {
                    stackTrace = kv.Value.stackTrace;
                    initKey = true;
                    break;
                }
            }
            if (!initKey)
            {
                stackTrace = condition;
            }
        }
        var key =stackTrace+type;
        if (debugInfo.ContainsKey(key))
        {
            var folder = debugInfo[key];
            folder.count++;
            
            if (folder.info.ContainsKey(condition))
            {
                folder.info[condition].count++;
                folder.info[condition].time = System.DateTime.Now;
            }
            else
            {
                folder.info.Add(condition, new DebugInfo()
                {
                    condition = condition,
                    count = 1,
                    time = System.DateTime.Now,
                });
            }
            folder.lastCondition = condition;

        }
        else
        {
            var folder = new DebugFolder()
            {
                type = type,
                info = new Dictionary<string, DebugInfo>(),
                count = 1,
                stackTrace = stackTrace,
            };
            folder.info.Add(condition, new DebugInfo()
            {
                condition = condition ,
                count = 1,
                time = System.DateTime.Now,
            });
            debugInfo.Add(key, folder);
            folder.lastCondition = condition;
        }
    }
    

    bool ShowToglle(bool showFlag,int count,string styleName)
    {
        var CountStr = "";
        if (count > 9999)
        {
            CountStr = " 9999+ ";
        }
        else
        {
            CountStr = " " + count + " ";
        }
        return GUILayout.Toggle(showFlag, new GUIContent(CountStr, GUI.skin.GetStyle(styleName).normal.background),
                    GUI.skin.GetStyle("ShowToggle"), GUILayout.Width(32 + CountStr.Length * 16), GUILayout.Height(40)
                    );
    }
    void CountBox(int count,string styleName)
    {
        var CountStr = "";
        if (count > 9999)
        {
            CountStr = " 9999+ ";
        }
        else
        {
            CountStr = " " + count + " ";
        }
        GUILayout.Box(new GUIContent(CountStr, GUI.skin.GetStyle(styleName).normal.background),
             GUI.skin.GetStyle("MinBg"), GUILayout.Width(65),GUILayout.Height(25));
    }
    private void OnGUI()
    {
        GUI.skin = debugSkin;
        if (showMin)
        {
           
            {
                GUILayout.BeginArea(new Rect(0, Screen.height / 2 - 45, 100, 90), GUI.skin.GetStyle("MinBg"));
                {

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    GUILayout.Space(5);
                    if (GUILayout.Button("", GUI.skin.GetStyle("MaxButton"), GUILayout.Width(16), GUILayout.Height(79)))
                    {
                        showFlag = 10;
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    GUILayout.Space(5);
                    CountBox(logCount, "SmallLog");
                    GUILayout.Space(2);
                    CountBox(warningCount, "SmallWarning");
                    GUILayout.Space(2);
                    CountBox(errorCount, "SmallError");
                    GUILayout.EndVertical();
                 
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
        }
     
       
        if (showMax) 
        {
          
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height), debugSkin.GetStyle("Area"));
            {
                
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, 46), debugSkin.GetStyle("Area"));
                GUILayout.Space(3);
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(3);
                        if (GUILayout.Button("", GUI.skin.GetStyle("Close"), GUILayout.Width(40), GUILayout.Height(40)))
                        {
                            showFlag = 0;
                        }
                        GUILayout.Space(3);
                        if(GUILayout.Button("", GUI.skin.GetStyle("Min"), GUILayout.Width(40), GUILayout.Height(40)))
                        {
                            showFlag = 11;
                        }
                        GUILayout.Space(3);
                        var fpsStr = "FPS:" + fps.ToString("f2");
                        GUILayout.Box(fpsStr.ToString(),
                             debugSkin.GetStyle("ShowToggle"), GUILayout.Width(fpsStr.Length * 16), GUILayout.Height(40));
                        
                    }
                    GUILayout.FlexibleSpace();

                    showLog = ShowToglle(showLog, logCount, "Log");
                    showWarning = ShowToglle(showWarning, warningCount, "Warning");
                    showError = ShowToglle(showError, errorCount, "Error");
                    GUILayout.Space(3);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
            GUILayout.Space(45);
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(Screen.width));
            
            foreach (var key in debugInfo.Keys)
            {

                Texture2D typeIcon = null;
                switch (debugInfo[key].type)
                {
                    case LogType.Error:
                        if (!showError) continue;
                        typeIcon = GUI.skin.GetStyle("SmallError").normal.background;
                        break;
                    case LogType.Assert:
                        break;
                    case LogType.Warning:
                        if (!showWarning) continue;
                        typeIcon = GUI.skin.GetStyle("SmallWarning").normal.background;
                        break;
                    case LogType.Log:
                        if (!showLog) continue;
                        typeIcon = GUI.skin.GetStyle("SmallLog").normal.background;
                        break;
                    case LogType.Exception:
                        break;
                    default:
                        break;
                }
                var info = debugInfo[key].LastInfo;
                var infoStr = info.condition + "\n " + info.time + "\n【" + info.count + "】";
                debugInfo[key].showDetails = GUILayout.Toggle( debugInfo[key].showDetails,
                    new GUIContent("【"+ debugInfo[key] .count+ "】"+info.time.ToString("[ HH:mm:ss:ffff ]") 
                     +"  "+ info.condition, typeIcon
                    ),GUI.skin.button);
                if (debugInfo[key].showDetails)
                {
                    debugInfo[key].scroll=GUILayout.BeginScrollView(
                        debugInfo[key].scroll,
                        GUILayout.MinHeight(Mathf.Min(debugInfo[key].info.Count*30,150)));
                    //var down = (debugInfo[strack].scroll.y >= 30 * (debugInfo[strack].count-3));
                    if (debugInfo[key].scroll.y >= 30 *(debugInfo[key].info.Count-6))
                    {
                        debugInfo[key].scroll.y = 30 *Mathf.Max(0, (debugInfo[key].info.Count-5));

                    }
                    int count=0;
                    foreach (var kv in debugInfo[key].info)
                    {   
                        info = kv.Value;
                    
                        GUILayout.BeginHorizontal();
                        GUILayout.Box(info.count.ToString(), GUI.skin.GetStyle("Count"),
                            GUILayout.Width(info.count.ToString().Length * 16),
                            GUILayout.MinWidth(32), GUILayout.Height(30));
                        if (count % 2 == 0)
                        {
                            GUILayout.Box(
                          new GUIContent(info.time.ToString("[ HH:mm:ss:ffff ]") + "  " + info.condition
                          ), GUILayout.Height(30));    //yyyy-MM-dd HH:mm:ss:ffff

                        }
                        else
                        {
                            GUILayout.Box(
                       new GUIContent(info.time.ToString("[ HH:mm:ss:ffff ]") + "  " + info.condition
                       ), GUI.skin.GetStyle("Box2"),GUILayout.Height(30));    //yyyy-MM-dd HH:mm:ss:ffff
                        }

                        GUILayout.EndHorizontal();
                        count++;
                    }

                    GUILayout.EndScrollView();
                    GUILayout.Box(debugInfo[key].stackTrace.Trim());
                }
                GUILayout.Space(5);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
