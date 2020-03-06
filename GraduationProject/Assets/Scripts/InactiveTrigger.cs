 /*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.Events;
public class InactiveTrigger : MonoBehaviour
{
    public UnityEvent enter_event;
    public UnityEvent exit_event;
    public UnityEvent inactive_event;
    public string collision_tag;

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(collision_tag))
        {
            if (enter_event != null)
                enter_event.Invoke();

            View.CurrentScene.GetView<GameInfoView>().inactrive_buttons.SetInactiveType(InactiveType.交互, null, this);
        }
    }
    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(collision_tag))
        {
            if (exit_event != null)
                exit_event.Invoke();

            View.CurrentScene.GetView<GameInfoView>().inactrive_buttons.SetInactiveType(InactiveType.攻击);
        }
    }
}
