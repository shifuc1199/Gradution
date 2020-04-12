/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using DreamerTool.UI;
using LitJson;
public class HobbyManager : MonoBehaviourPunCallbacks
{
    public static Dictionary<string, string> roomDict = new Dictionary<string, string>();
    public override void OnConnected()
    {
        base.OnConnected();

        Debug.Log("连接成功!");
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
        Debug.Log("加入大厅");
        var table = new ExitGames.Client.Photon.Hashtable();
        table.Add("model", JsonMapper.ToJson(ActorModel.Model));
        PhotonNetwork.SetPlayerCustomProperties(table);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        foreach (var room in roomList)
        {
          
           View.CurrentScene.GetView<HobbyView>().CreateRoomCell(room);
        }
        Debug.Log("UpdateRoomList");
    }
     
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if(!newPlayer.IsLocal)
        {
            View.CurrentScene.GetView<RoomView>().UpdatePlayer();
        }
        
        Debug.Log(newPlayer.NickName+" 加入房间！");
       
    }
    public void CreateRoom()
    {
         
        PhotonNetwork.CreateRoom(System.Guid.NewGuid().ToString());
    }
 
    public override void OnCreatedRoom()
    {
        View.CurrentScene.OpenView<RoomView>();
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        View.CurrentScene.OpenView<RoomView>();
    }
}
