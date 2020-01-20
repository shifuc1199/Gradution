/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class FaceConfig<T> : BaseConfig<T> where T:BaseConfig<T>
{
    public int ID;
    public string sprite_path;
    [PreviewField(100)][AssetsOnly]
    public Sprite editor_sprite;
}
