/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
public class TestScene : MonoBehaviour
{
    bool isDraw = false;
    Vector3 last_pos;
    private void Start()
    {
        GameObjectPoolManager.InitByScriptableObject();

    }
 
    private void Update()
    {
        
        if (Input.GetKey(KeyCode.Mouse0) && !isDraw)
        {
            last_pos = Input.mousePosition;
            isDraw = true;
            GameObjectPoolManager.GetPool("mask_sprite").Get(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10)), Quaternion.identity, -1);
        }
        if (Input.mousePosition != last_pos && isDraw)
        {
            isDraw = false;
        }
    }
}
