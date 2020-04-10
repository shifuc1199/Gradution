using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResManager 
{
    public static TextAsset LoadTextAsset(string path)
    {
        return Resources.Load<TextAsset>("TextAsset/" + path);
    }
}
