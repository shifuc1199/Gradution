/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class NPCTalk : MonoBehaviour
{
    public int id;
    public void Talk()
    {
        View.CurrentScene.OpenView<NPCView>().LoadTextAsset("npc_inactive_"+id);
    }
}
