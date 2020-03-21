/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class CreateActorFaceView : MonoBehaviour
{
    public GameObject Cell;
    public Transform Root;

    private FaceUICell current_select_item;
    private List<GameObject> cell_lists = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        SetType(0);
    }

    public void Select(FaceUICell cell)
    {
        if (current_select_item != null)
        {
            current_select_item.DeSelect();
        }
        cell.Select();
        current_select_item = cell;
        ActorModel.Model.SetFace(cell._type, cell.config_id);
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
                    cell.GetComponent<FaceUICell>().SetConfig(_type, _config.Value.ID, Select);
                    if (cell_lists.Count + 1 == ActorModel.Model.GetFace(_type))
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
