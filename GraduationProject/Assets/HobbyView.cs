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
using DreamerTool.Util;
public class HobbyView : View
{
    public Text name_text;
    public Text lv_text;

    public Transform room_root;
    public GameObject room_cell_refab;
    
    Dictionary<string, RoomInfoCell> room_cells = new Dictionary<string, RoomInfoCell>();
    private void Start()
    {
       
         
    }
    public void OnDisable()
    {
       
        foreach (var item in room_cells)
        {
            Destroy(item.Value.gameObject);
        }
        room_cells.Clear();
    }
    private void OnEnable()
    {
         PhotonNetwork.ConnectUsingSettings();
        HobbyManager.isConnected = true;
        name_text.text = ActorModel.Model.actor_name;
        lv_text.text = "LV " + ActorModel.Model.level;
    }
    public void CreateRoomCell(RoomInfo info)
    {
        if (room_cells.ContainsKey(info.Name))
            return;
         
         var cell =  Instantiate(room_cell_refab, room_root).GetComponent<RoomInfoCell>();
        
        cell.SetModel((int)info.CustomProperties["room_id"], info.Name, info.PlayerCount);
        room_cells.Add(info.Name,cell);
    }
    public void DeleteAllRoomCell()
    {
        foreach (var cell in room_cells)
        {
            Destroy(cell.Value.gameObject);
        }
        room_cells.Clear();
    }
    public void DeleteRoomCell(RoomInfo info)
    {
        if(!room_cells.ContainsKey(info.Name))
        {
            return;
        }
        Destroy(room_cells[info.Name].gameObject);
        room_cells.Remove(info.Name);
    }
}
