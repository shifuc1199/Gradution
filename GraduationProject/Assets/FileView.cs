/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class FileView : View
{

    public List<FileCell> cells = new List<FileCell>();

    private void Start()
    {
 

        var models = SaveManager.Instance.GetActorModels();
   
        for (int i = 0; i < models.Count; i++)
        {
            cells[models[i].SaveDataID-1].SetModel(models[i]);
        }
    }
    public void ClearData()
    {
        PlayerPrefs.DeleteAll();
    }
   
}
