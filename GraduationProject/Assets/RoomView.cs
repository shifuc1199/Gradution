/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using Photon.Pun;
using LitJson;
public class RoomView : View
{
    public RoomActor[] actors;

    private void Start()
    {
         
        UpdatePlayer();
    }
    public void UpdatePlayer()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
             
            actors[player.ActorNumber - 1].SetModel(player.IsLocal,(JsonMapper.ToObject<ActorModel>(player.CustomProperties["model"].ToString())));
        }
    }
   
}
