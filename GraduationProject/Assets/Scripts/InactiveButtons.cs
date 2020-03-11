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
    public InactiveTrigger curretn_stay_trigger;
    public SkillJoyStick[] skill_joys;
    public Image main_inactive_image;
    public Dictionary<InactiveType, Sprite> inactive_sprite_dic = new Dictionary<InactiveType, Sprite>();
    public InactiveType inactive_type = InactiveType.攻击;

    private void Start()
    {
         
    }
    public void UpdateAllJoySticks()
    {
        foreach (var item in skill_joys)
        {
            item.UpdateModel();
        }
      
    }
    public void SetInactiveType(InactiveType _type,ItemSprite item =null,InactiveTrigger trigger=null)
    {
        current_stay_item = item;
        curretn_stay_trigger = trigger;
        this.inactive_type = _type;
        switch (_type)
        {
            case InactiveType.攻击:
                main_inactive_image.GetComponent<RectTransform>().sizeDelta = new Vector2(110, 110);
                main_inactive_image.GetComponent<RectTransform>().anchoredPosition = new Vector2(-6.6f, -13.8f);
                main_inactive_image.GetComponent<EquipmentSprite>().UpdateIamgeSprite();
                break;
            case InactiveType.拾取:
                main_inactive_image.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                main_inactive_image.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                main_inactive_image.sprite = inactive_sprite_dic[_type];
                break;
            case InactiveType.交互:
                main_inactive_image.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
                main_inactive_image.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                main_inactive_image.sprite = inactive_sprite_dic[_type];
                break;
            default:
                break;
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
                    Destroy(current_stay_item.transform.parent.gameObject);
                }
                break;
            case InactiveType.交互:
                    curretn_stay_trigger.inactive_event?.Invoke();
                break;
            default:
                break;
        }
    }
}
