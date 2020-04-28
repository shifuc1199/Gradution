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
    public Button joinBtn;
    public Text room_name_text;
    public Text player_count_text;
    private string room_name;
    public void SetModel(int id,string room_name,int player_count)
    {
        joinBtn.interactable = player_count < 2;
        room_name_text.text ="房间号: "+id+ "\n房间名: " + room_name.Split(';')[1];
        this.room_name = room_name;
        player_count_text.text = player_count + "/2";
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(this.room_name);
    }
}
