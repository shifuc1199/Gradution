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
using DreamerTool.Util;

public class HobbyManager : MonoBehaviourPunCallbacks
{
    NumUtil roomIdNumUtil = new NumUtil();
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public static bool isConnected = false;
    public static Dictionary<string, string> roomDict = new Dictionary<string, string>();
    public override void OnEnable()
    {
        base.OnEnable();
        View.CurrentScene.GetView<HobbyView>().tipText.text = "正在连接中......";
    }
    public override void OnConnected()
    {
        base.OnConnected();
    
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        if (isConnected)
        {
            View.CurrentScene.GetView<HobbyView>().tipText.text = "";
            Debug.Log("加入大厅");
            var table = new ExitGames.Client.Photon.Hashtable();
            table.Add("model", JsonMapper.ToJson(ActorModel.Model));
            PhotonNetwork.SetPlayerCustomProperties(table);
            isConnected = false;
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        List<RoomInfo> RemoveRoomList = new List<RoomInfo>();
 
        foreach (var room in roomList)
        {
            if (room.PlayerCount == 0)
            {
                
                View.CurrentScene.GetView<HobbyView>().DeleteRoomCell(room);
                RemoveRoomList.Add(room);
            }
            else
            {
                
                View.CurrentScene.GetView<HobbyView>().CreateRoomCell(room);
            }
        }
        foreach (var item in RemoveRoomList)
        {
            roomList.Remove(item);
        }
        if (roomList.Count == 0)
        {
            View.CurrentScene.GetView<HobbyView>().tipText.text = "暂无房间！";
        }
        else
        {
            View.CurrentScene.GetView<HobbyView>().tipText.text = "";
        }
    }
     
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        if (!newPlayer.IsLocal)
        {
            View.CurrentScene.GetView<RoomView>().UpdatePlayer();
        }
        
        Debug.Log(newPlayer.NickName+" 加入房间！");
       
    }
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        var num = roomIdNumUtil.genNum();
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOptions.CustomRoomProperties.Add("room_id", num);
        roomOptions.CustomRoomPropertiesForLobby = new string[] {"room_id" };
        PhotonNetwork.CreateRoom(System.Guid.NewGuid().ToString()+ ";"+ActorModel.Model.actor_name + ": 的房间", roomOptions);
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        View.CurrentScene.GetView<RoomView>().UpdatePlayer();
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
 
        View.CurrentScene.OpenView<RoomView>();
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        View.CurrentScene.OpenView<RoomView>();
    }
}
