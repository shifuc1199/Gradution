using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class ResManager
{
    public static TextAsset LoadTextAsset(string path)
    {
        return Resources.Load<TextAsset>("TextAsset/" + path);
    }
    public static GameObject LoadViewPrefab<T>() where T : View
    {
        var viewName = typeof(T).Name;
        var viewPrefab = Resources.Load<GameObject>("Views/" + viewName);
        if (viewPrefab)
            Debug.LogError("没有" + viewName + "的预制体！");
        return viewPrefab;
    }
}
