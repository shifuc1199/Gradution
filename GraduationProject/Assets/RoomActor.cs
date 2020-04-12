/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoomActor : MonoBehaviour
{
    public Text lv_text;
    public Text name_text;
    public Text ready_text;
    public Button ready_btn;
    public void SetModel(bool islocal,ActorModel model)
    {
        ready_text.gameObject.SetActive(!islocal);
        ready_btn.gameObject.SetActive(islocal);
        gameObject.SetActive(true);
        name_text.text = model.actor_name;
        lv_text.text ="LV: "+ model.level;
    }
}
