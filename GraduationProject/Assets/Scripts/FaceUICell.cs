/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class FaceUICell : MonoBehaviour,IPointerDownHandler
{
    public Image cell_image;
    public Image choose_image;
    public FaceType _type;
    public int config_id;
    public void Select()
    {
        choose_image.gameObject.SetActive(true);
    }
    public void DeSelect()
    {
        choose_image.gameObject.SetActive(false);
    }
    public void SetConfig(FaceType _type, int config_id)
    {
        this._type = _type;
        this.config_id = config_id;

        switch (_type)
        {
            case FaceType.眼睛:
                cell_image.sprite = EyeConfig.Get(config_id).GetSprite();
                break;
            case FaceType.嘴巴:

                cell_image.sprite = MouthConfig.Get(config_id).GetSprite();
                break;
            case FaceType.发型:
                cell_image.sprite = HairConfig.Get(config_id).GetSprite();
                break;
            case FaceType.耳朵:
                cell_image.sprite = EarConfig.Get(config_id).GetSprite();
                break;
            case FaceType.发饰:
                cell_image.sprite = HairDecorateConfig.Get(config_id).GetSprite();
                break;
            default:
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        View.CurrentScene.GetView<FaceView>().Select(this);
    }
}
