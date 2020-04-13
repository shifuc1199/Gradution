/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using LitJson;
public class NetWorkActorController : ActorController
{
    private FileDataActor _actor;
    public GameObject[] hide_gameObject;
    private PhotonView photonView;
    public new void Start()
    {
        _actor = GetComponent<FileDataActor>();
        photonView = GetComponent<PhotonView>();
        
        if (photonView.IsMine)
        {
            Controller = this;
        }
        else
        {
            foreach (var gam in hide_gameObject)
            {
                gam.SetActive(false);
            }
            _rigi.simulated = false;
        }
        SetModel( JsonMapper.ToObject<ActorModel>(photonView.InstantiationData[0].ToString()));
    }
    public void SetModel(ActorModel model)
    {
        _actor.SetModel(model);
    }
    public new void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;
        base.FixedUpdate();
    }
}
