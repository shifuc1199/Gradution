using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class BagView : MonoBehaviour
{
    public Transform grid_root;
    public Transform GetEmptyGrid()
    {
        for(int i =0;i<grid_root.childCount;i++)
        {
            if(!grid_root.GetChild(i).GetChild(0).gameObject.activeSelf)
            {
                return grid_root.GetChild(i);
            }
        }
        Debug.LogWarning("背包已满！！");
        return null;
    }
    public void AddItem<T>(ItemConfig<T> _config) where T:BaseConfig<T>
    {
        var grid = GetEmptyGrid();
        if(grid != null)
        {
            grid.GetChild(0).gameObject.SetActive(true);
            //grid.GetChild(0).GetComponent<Image>().sprite = _config.sprite;
        }
        Debug.Log(typeof(T).Name);
    }
}
