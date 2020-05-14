/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DreamerTool.UI;


public class TestScene : Scene
{
    public Transform player;
    bool isRotate;
    float rotateV;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            isRotate = true;
        }
        if(isRotate)
        {
            var value = Mathf.Lerp(0, 90, Time.deltaTime);
            rotateV += value;
            if (rotateV >= 90)
            {
                rotateV = 0;
                isRotate = false;
                return;
            }
 
            transform.RotateAround(player.transform.position, transform.up, value);
        }
      
     
        
    }
 
}
