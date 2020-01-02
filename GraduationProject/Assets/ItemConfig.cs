/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemConfig<T> : BaseConfig<T> where T: BaseConfig<T>
{
    public abstract void Save();
}
