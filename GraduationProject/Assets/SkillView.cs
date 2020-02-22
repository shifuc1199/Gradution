/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
public class SkillView : View
{
    public Transform root;
    public GameObject cell_prefab;

    public SkillIntroduce introduce;
    public override void OnShow()
    {
        base.OnShow();
        CurrentScene.GetView<GameInfoView>().HideAnim();

        foreach(var item in ActorModel.Model.skillmodels)
        {
           GameObject temp = Instantiate(cell_prefab, root);
            temp.GetComponent<SkillCell>().SetModel(item.Value);
        }
    }
    public void SelecetCell(int index)
    {
       introduce.SetModel( root.GetComponent<ButtonGroup>().Toggles[index].GetComponent<SkillCell>().model);
       
    }
    public override void OnHide()
    {
        base.OnHide();
        CurrentScene.GetView<GameInfoView>().ShowAnim();
    }
}
