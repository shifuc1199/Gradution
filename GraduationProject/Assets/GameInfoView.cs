using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class GameInfoView : View
{
 
    public void ShowAnim()
    {
        GetComponent<Animator>().SetTrigger("show");
    }
    public void HideAnim()
    {
        GetComponent<Animator>().SetTrigger("hide");
    }
}
