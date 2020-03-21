/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using DreamerTool.Extra;
using UnityEngine.UI;
using Sirenix.OdinInspector;
public class FaceView : View 
{
    public double need_money;

    public GameObject Cell;
    public Transform Root;

    public Button sure_button;
    public GameObject no_money_tip;

    public FaceViewActor actor;
 
    private FaceUICell current_select_item;
    private List<GameObject> cell_lists = new List<GameObject>();
    private Dictionary<FaceType, int> Faces = new Dictionary<FaceType, int>();
    private void OnEnable()
    {

        Faces.Copy(ActorModel.Model.GetFaceDictionary());
        actor.UpdateFace(Faces);

        SetType(0);

        if (ActorModel.Model.GetMoney() >= need_money)
        {
            no_money_tip.SetActive(false);
            sure_button.interactable = true;
            sure_button.GetComponentInChildren<Text>().color = Color.white;
        }
        else
        {
            no_money_tip.SetActive(true);
            sure_button.interactable = false;
            sure_button.GetComponentInChildren<Text>().color = Color.gray;
        }
    }
 
    public void Save()
    {
        OnCloseClick();
        CurrentScene.OpenView<TipView>().SetContent("整容成功！共花费你"+ need_money + "金币");
        ActorModel.Model.SetMoney(-need_money);
        foreach (var face in Faces)
        {
            ActorModel.Model.SetFace(face.Key, face.Value);
        }
    }
    public void Select(FaceUICell cell)
    {
        if(current_select_item!=null)
        {
            current_select_item.DeSelect();
        }
        cell.Select();
        current_select_item = cell;
        Faces[cell._type]= cell.config_id;
        actor.UpdateFace(cell._type,cell.config_id);
    }
    public void SetType(int t)
    {
        foreach (var item in cell_lists)
        {
            Destroy(item);
        }
        cell_lists.Clear();
        var _type = (FaceType)t;
        switch (_type)
        {
            case FaceType.眼睛:
                EyeConfig.Get(1);
                foreach (var _config in EyeConfig.Datas)
                {
                    var cell = Instantiate(Cell, Root);
                    cell.GetComponent<FaceUICell>().SetConfig(_type, _config.Value.ID,Select);
                    if (cell_lists.Count+1 == ActorModel.Model.GetFace(_type))
                        Select(cell.GetComponent<FaceUICell>());
                    cell_lists.Add(cell);
                }
                break;
            case FaceType.嘴巴:
                MouthConfig.Get(1);
                foreach (var _config in MouthConfig.Datas)
                {
                    var cell = Instantiate(Cell, Root);
                    cell.GetComponent<FaceUICell>().SetConfig(_type, _config.Value.ID, Select);
                    if (cell_lists.Count + 1 == ActorModel.Model.GetFace(_type))
                        Select(cell.GetComponent<FaceUICell>());
                    cell_lists.Add(cell);
                }
                break;
            case FaceType.发型:
                HairConfig.Get(1);
                foreach (var _config in HairConfig.Datas)
                {
                    var cell = Instantiate(Cell, Root);
                    cell.GetComponent<FaceUICell>().SetConfig(_type, _config.Value.ID, Select);
                    if (cell_lists.Count + 1 == ActorModel.Model.GetFace(_type))
                        Select(cell.GetComponent<FaceUICell>());
                    cell_lists.Add(cell);
                }
                break;
            case FaceType.耳朵:
                EarConfig.Get(1);
                foreach (var _config in EarConfig.Datas)
                {
                    var cell = Instantiate(Cell, Root);
                    cell.GetComponent<FaceUICell>().SetConfig(_type, _config.Value.ID, Select);
                    if (cell_lists.Count + 1 == ActorModel.Model.GetFace(_type))
                        Select(cell.GetComponent<FaceUICell>());
                    cell_lists.Add(cell);
                }
                break;
            case FaceType.发饰:
                HairDecorateConfig.Get(1);
                foreach (var _config in HairDecorateConfig.Datas)
                {
                    var cell = Instantiate(Cell, Root);
                    cell.GetComponent<FaceUICell>().SetConfig(_type, _config.Value.ID, Select);
                    if (cell_lists.Count + 1 == ActorModel.Model.GetFace(_type))
                        Select(cell.GetComponent<FaceUICell>());
                    cell_lists.Add(cell);
                }
                break;
            default:
                break;
        }
 
    }
}
