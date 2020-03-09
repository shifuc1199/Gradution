/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ferr.Extensions;
public class TestScene : MonoBehaviour
{
    private void Start()
    {
        
    }
    [ContextMenu("sb")]
    public void Test()
    {
        GetComponent<Ferr2DT_PathTerrain>().PathData.Clear();

        GetComponent<Ferr2DT_PathTerrain>().PathData.Add(new Vector2(0, 0), new Ferr2D_PointData(1), Ferr.PointType.Sharp);
        GetComponent<Ferr2DT_PathTerrain>().PathData.Add(new Vector2(1, 1), new Ferr2D_PointData(1), Ferr.PointType.Sharp);
        GetComponent<Ferr2DT_PathTerrain>().PathData.Add(new Vector2(0, 1), new Ferr2D_PointData(1), Ferr.PointType.Sharp);
        
        GetComponent<Ferr2DT_PathTerrain>().Build();
    }
}
