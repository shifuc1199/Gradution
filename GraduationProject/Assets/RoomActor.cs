/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Util;
using UnityEngine.UI;
 
public class RoomActor : MonoBehaviour
{
   
    public Text lv_text;
    public Text name_text;
    public Text ready_text;
    public Button ready_btn;
    public bool isReady;
    public FileDataActor actor;

    public Sprite ready_btn_cancel_sp;

    public Sprite start_sp;
    private bool isLocal;
 
    public void SetModel(bool islocal,ActorModel model)
    {
        isLocal = islocal;
        actor.SetModel(model);
       
        ready_text.gameObject.SetActive(!islocal);
        ready_btn.gameObject.SetActive(islocal);
        gameObject.SetActive(true);
        name_text.text = model.actor_name;
        lv_text.text ="LV: "+ model.level;

         
    }
    public void UpdateReadyState(bool ready)
    {
       
        if (isLocal)
            return;

        this.isReady = ready;
        DreamerTool.UI.View.CurrentScene.GetView<RoomView>().CheckReady();
        ready_text.text = ready ? DreamerUtil.GetColorRichText( "已准备",Color.green) : DreamerUtil.GetColorRichText("未准备",Color.red);
    }
    public void ReadyOnClick()
    {
       
       
        if (isReady)
        {
            ready_btn.GetComponentInChildren<Text>().text = "准备";
            ready_btn.image.sprite = start_sp;
        }
        else
        {
            ready_btn.GetComponentInChildren<Text>().text = "取消准备";
            ready_btn.image.sprite = ready_btn_cancel_sp;
        }
        isReady =!isReady;
        DreamerTool.UI.View.CurrentScene.GetView<RoomView>().CheckReady();
        object[] datas = new object[] { Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber, ready_btn.image.sprite == ready_btn_cancel_sp };
        Photon.Pun.PhotonNetwork.RaiseEvent(GameConstData.NETWORK_READY_STATE_CHANGE_EVENT,datas , Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendUnreliable);
         
    }
}
