/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using Photon.Pun;
using LitJson;
using ExitGames.Client.Photon;

public class RoomView : View
{
    public RoomActor[] actors;

    private void Start()
    {
         
        UpdatePlayer();
      
    }
    public override void OnShow()
    {
        base.OnShow();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NetworkingClient.EventReceived += NetworkingClient_RoomEventHandler;
    }
    public override void OnHide()
    {
        base.OnHide();
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkingClient_RoomEventHandler;
    }
    public void NetworkingClient_RoomEventHandler(EventData obj)
    {
        if (obj.Code == GameConstData.NETWORK_READY_STATE_CHANGE_EVENT)
        {
            object[] datas = (object[])obj.CustomData;
            var number = (int)datas[0];
            var ready_state = (bool)datas[1];
            actors[number - 1].UpdateReadyState(ready_state);
            


        }
    }
    public void CheckReady()
    {
        foreach (var actor in actors)
        {
            if (!actor.isReady)
            {
                Debug.Log(actor.name_text.text + ": 没准备");
                return;
            }
        }


        if (PhotonNetwork.IsMasterClient)
        {

            PhotonNetwork.LoadLevel(GameConstData.FIGHT_SCENE_NAME);
        }
    }
    public void UpdatePlayer()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
             
            actors[player.ActorNumber - 1].SetModel(player.IsLocal,(JsonMapper.ToObject<ActorModel>(player.CustomProperties["model"].ToString())));
        }
    }
   
}
