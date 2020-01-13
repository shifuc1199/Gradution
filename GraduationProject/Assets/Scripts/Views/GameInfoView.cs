using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class GameInfoView : View
{
    public InactiveButtons inactrive_buttons;
    public EnemyHealthBar enemy_health;
    public void SetInactiveType(InactiveType _type,ItemSprite item = null)
    {
        inactrive_buttons.SetInactiveType(_type,item);
    }
    public void ShowAnim()
    {
        GetComponent<Animator>().SetTrigger("show");
    }
    public void HideAnim()
    {
        GetComponent<Animator>().SetTrigger("hide");
    }
}
