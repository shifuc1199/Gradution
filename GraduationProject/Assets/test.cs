using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class test : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public void StartMatching()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("连接到主机");
    }
    public override void OnConnected()
    {
        Debug.Log("连接成功");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(System.Guid.NewGuid().ToString());
        Debug.Log("加入失败");
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("创建房间");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("加入成功");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
