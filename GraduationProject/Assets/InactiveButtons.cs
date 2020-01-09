/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DreamerTool.UI;
public class InactiveButtons : SerializedMonoBehaviour
{
    public ItemSprite current_stay_item;
    public Image main_inactive_image;
    public Dictionary<InactiveType, Sprite> inactive_sprite_dic = new Dictionary<InactiveType, Sprite>();
    public InactiveType inactive_type = InactiveType.攻击;

    private void Start()
    {
        SetInactiveType(inactive_type);
    }
    public void SetInactiveType(InactiveType _type,ItemSprite item =null)
    {
        current_stay_item = item;
        this.inactive_type = _type;
        if (_type == InactiveType.攻击)
        {
            main_inactive_image.GetComponent<RectTransform>().sizeDelta = new Vector2(110, 110);
            main_inactive_image.GetComponent<RectTransform>().anchoredPosition = new Vector2(-6.6f, -13.8f);
            main_inactive_image.GetComponent<EquipmentSprite>().UpdateIamgeSprite();
        }
        else
        {
            main_inactive_image.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            main_inactive_image.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            main_inactive_image.sprite = inactive_sprite_dic[_type];
        }

         
    }
    public void Inactive_Click()
    {
        switch (inactive_type)
        {
            case InactiveType.攻击:
                ActorController._controller.actor_state.isAttack = true;
                break;
            case InactiveType.拾取:
                 
                if(current_stay_item!=null)
                {
                    View.CurrentScene.GetView<PlayerInfoAndBagView>().bag_view.AddItem(current_stay_item.config_id,current_stay_item.item_type);
                    Destroy(current_stay_item.gameObject);
                }
                break;
            default:
                break;
        }
    }
}
