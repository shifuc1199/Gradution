/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
public class InactiveButtons : SerializedMonoBehaviour
{
    public Image main_inactive_image;
    public Dictionary<InactiveType, Sprite> inactive_sprite_dic = new Dictionary<InactiveType, Sprite>();
    public InactiveType inactive_type = InactiveType.攻击;

    public void SetInactiveType(InactiveType _type)
    {
        this.inactive_type = _type;
        main_inactive_image.sprite = inactive_sprite_dic[_type];
    }
  
}
