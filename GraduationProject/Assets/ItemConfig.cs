/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
public abstract class ItemConfig<T> : BaseConfig<T> where T: BaseConfig<T>
{
    [ReadOnly][BoxGroup("基础信息")][VerticalGroup("基础信息/p/o")]
    public int 物品ID;/*nil*/
    [BoxGroup("基础信息")][VerticalGroup("基础信息/p/o")]
    public string 物品名字;/*nil*/
    [JsonNonField][HideLabel, PreviewField(100)][AssetsOnly][BoxGroup("基础信息")][HorizontalGroup("基础信息/p", 100)]
    public Sprite 编辑器图标 = null;
    [BoxGroup("基础信息")][VerticalGroup("基础信息/p/o")][TextArea()]
    public string 物品描述;/*nil*/
    [NonSerialized]
    public string 图标名字;
    public abstract void Save();
    public Sprite GetSprite()
    {
 
        return Resources.Load<Sprite>(图标名字);
    }
}
 

