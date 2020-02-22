/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton <T>:MonoBehaviour where T :MonoBehaviour   
{
    private static T instance;
    public static T Instance {
        get
        {
            if (instance == null)
            {
                GameObject temp = new GameObject();
                temp.name = typeof(T).Name;
                instance = temp.AddComponent<T>();
            }

            return instance;

        }
    }

}
