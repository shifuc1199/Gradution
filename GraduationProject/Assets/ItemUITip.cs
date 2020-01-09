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
    public void SetConfig(ItemType type,int id) 
    {
        switch (type)
        {
            case ItemType.武器:
                var i = WeaponConfig.Get(id);
                _text.text = "武器名字: "+i.物品名字 + "\n武器描述: " +i.物品描述+"\n武器阶级: " + i.物品阶级;
                break;
        }
    }
 
}
