using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DreamerTool.UI;
public class PlayerView : MonoBehaviour
{
    public Text player_attribute_text;
    private void Awake()
    {
        SetPlayAttributeText();
    }
    public void SetPlayAttributeText()
    {
        foreach (var temp in Enum.GetNames(typeof(PlayerAttribute)))
        {
          
            player_attribute_text.text += temp+": "+ ActorModel.Model.GetPlayerAttribute((PlayerAttribute)Enum.Parse(typeof(PlayerAttribute), temp))+"\n";
        }

    }


}
