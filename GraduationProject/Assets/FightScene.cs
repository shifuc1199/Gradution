/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using Photon.Pun;
using LitJson;
public class FightScene : Scene
{
     
    // Start is called before the first frame update
    void Start()
    {
       
      GameObject player =      PhotonNetwork.Instantiate("NetWorkPlayer", new Vector3(-50, 0, 0), Quaternion.identity,0,new object[] {JsonMapper.ToJson(ActorModel.Model )});
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
