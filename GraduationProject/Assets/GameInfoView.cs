using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class GameInfoView : View
{
    private InactiveButtons inactrive_buttons;

    public void SetInactiveType(InactiveType _type)
    {
        inactrive_buttons.SetInactiveType(_type);
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
