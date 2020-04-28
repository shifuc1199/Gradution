/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class OnNetPlayerInstantiate : MonoBehaviour, IPunInstantiateMagicCallback
{
    private NetkActorController controller;
    private PhotonView photonView;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        controller = GetComponentInChildren<NetkActorController>();
    }
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        FightScene.PlayerDict.Add((int)photonView.Owner.CustomProperties["number"], controller);
    }
}
