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
 
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
 
}
