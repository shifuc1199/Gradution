/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
 
public class AssetMenuExtra  
{
    [MenuItem("Assets/WeaponToResources")]
    public static void MoveTo()
    {
        var guids = Selection.assetGUIDs;
        var oldPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        var paths = oldPath.Split('/');
        var name = paths[paths.Length-1];
        Debug.Log(AssetDatabase.MoveAsset(oldPath, "Assets/Resources/Weapons/"+ name));
        AssetDatabase.Refresh();
    }
}
