using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DreamerTool.UI;
public class PlayerView : MonoBehaviour
{
    public Text player_name_text;
    public Text player_level_text;
  //  public Text player_attribute_text;
    private void Awake()
    {
        SetPlayAttributeText();
        player_name_text.text = ActorModel.Model.actor_name ;
        SetPlayerLevel();

        EventHandler.OnChangeLevel += SetPlayerLevel;
    }
   public void SetPlayerLevel()
    {
        player_level_text.text ="LV  " +ActorModel.Model.GetLevel();
    }
    public void SetPlayAttributeText()
    {
      /*  foreach (var temp in Enum.GetNames(typeof(PlayerAttribute)))
        {
          
            player_attribute_text.text += temp+": "+ ActorModel.Model.GetPlayerAttribute((PlayerAttribute)Enum.Parse(typeof(PlayerAttribute), temp))+"\n";
        }*/
       
    }


}
