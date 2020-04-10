/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FileDataCell : MonoBehaviour
{
    public Text name_text;
    public Text lv_text;
    public FileDataActor actor;
 
 
    private void Start()
    {
        
    }
 
    public void SetModel(ActorModel _model)
    {
 
        gameObject.SetActive(true);
        actor.SetModel(_model);
 
        name_text.text = _model.actor_name;
        lv_text.text = "LV "+_model.level;
    }
}
