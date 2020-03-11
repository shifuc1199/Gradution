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


    public SkillJoyStickConfigPage config;
    public SkillIntroduce introduce;

    List<SkillCell> skill_cell_list = new List<SkillCell>();
    private void Awake()
    {
        foreach (var item in ActorModel.Model.skillmodels)
        {
            GameObject temp = Instantiate(cell_prefab, root);
            var skillcell = temp.GetComponent<SkillCell>();
            skillcell.SetModel(item.Value);
            skill_cell_list.Add(skillcell);
        }
      

        EventManager.OnSkillLevelUp += UpdateSkillViewBySkillLevelUp;
    }
    private void OnDestroy()
    {
        EventManager.OnSkillLevelUp -= UpdateSkillViewBySkillLevelUp;
    }
    private void Start()
    {
        SelecetCell(0);
    }
    public void UpdateSkillViewBySkillLevelUp(SkillModel model)
    {
        foreach (var cell in skill_cell_list)
        {
            cell.UpdateModel();
        }
        config.UpdateModel();
        introduce.UpdateModel();
    }
    public override void OnShow()
    {
        base.OnShow();
        introduce.UpdateModel();
        CurrentScene.GetView<GameInfoView>().HideAnim();
    }
    public void SelecetCell(int index)
    {
        config.SetModel(root.GetComponent<ButtonGroup>().Toggles[index].GetComponent<SkillCell>().model);
        introduce.SetModel( root.GetComponent<ButtonGroup>().Toggles[index].GetComponent<SkillCell>().model);
    }
  
    public override void OnHide()
    {
        base.OnHide();
        CurrentScene.GetView<GameInfoView>().ShowAnim();
    }
}
