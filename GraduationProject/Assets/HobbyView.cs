/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
using Photon.Realtime;
using Photon.Pun;
public class HobbyView : View
{
    public Text name_text;
    public Text lv_text;

    public Transform room_root;
    public GameObject room_cell_refab;
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    private void OnEnable()
    {
        name_text.text = ActorModel.Model.actor_name;
        lv_text.text = "LV " + ActorModel.Model.level;
    }
    public void CreateRoomCell(RoomInfo info)
    {
      var cell =  Instantiate(room_cell_refab, room_root).GetComponent<RoomInfoCell>();
        cell.SetModel(info.Name);
    }
}
