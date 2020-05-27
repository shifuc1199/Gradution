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
using DreamerTool.Extra;
using System.Linq;
public class RoomView : View
{
    public RoomActor[] actors;

    private void Start()
    {
         
      
      
    }
    public override void OnShow()
    {
        base.OnShow();
        UpdatePlayer();

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
            actors[number].UpdateReadyState(ready_state);
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
            Debug.Log(actor.name_text.text + ": 已准备");
        }

      

        if (PhotonNetwork.IsMasterClient)
        {
          

            Debug.Log("IsMaster");
            PhotonNetwork.LoadLevel(GameConstData.FIGHT_SCENE_NAME);
        }
    }
    public void LeaveRoom()
    {

        actors[(int)PhotonNetwork.LocalPlayer.CustomProperties["number"]].gameObject.SetActive(false);
        PhotonNetwork.LeaveRoom();
       
        OnCloseClick();
    }
    public void UpdatePlayer()
    {
        var dict = (from entry in PhotonNetwork.CurrentRoom.Players
                    orderby entry.Key ascending
                    select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
        foreach (var item in actors)
        {
            item.gameObject.SetActive(false);
        }
        for (int i = 0; i < PhotonNetwork.CurrentRoom.Players.Count; i++)
        {
            var player = dict.Get(i).Value;
            Debug.Log(PhotonNetwork.CurrentRoom.Players.Get(i).Key);
            var table = new ExitGames.Client.Photon.Hashtable();
            table.Add("number" , i);
            player.SetCustomProperties(table);
            actors[i].SetModel(player.IsLocal,(JsonMapper.ToObject<ActorModel>(player.CustomProperties["model"].ToString())));
        }
    }
   
}
