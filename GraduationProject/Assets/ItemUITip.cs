/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemUITip : MonoBehaviour
{
    public Text _text;
    public void SetConfig<T>(ItemConfig<T> _config) where T : BaseConfig<T>
    {
 
        switch (typeof(T).Name)
        {
            case "WeaponConfig":
                var i = _config as WeaponConfig;
                Debug.Log(i.物品名字);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
