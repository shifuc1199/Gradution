/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class RoomInfoCell : MonoBehaviour
{
    public Text room_name_text;

    private string room_name;
    public void SetModel(string room_name)
    {
        room_name_text.text = "房间名: " + room_name;
        this.room_name = room_name;
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(this.room_name);
    }
}
