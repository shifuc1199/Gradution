/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FaceSprite : MonoBehaviour
{
    public FaceType facetype;
    Image _image;
    SpriteRenderer _sprite;
    private void Start()
    {
        _image = GetComponent<Image>();
        _sprite = GetComponent<SpriteRenderer>();
        if (_sprite)
        {
            UpdateSpriteRenderSprite();

            EventManager.OnChangeFace += UpdateSpriteRenderSprite;
        }
        if (_image)
        {
            UpdateIamgeSprite();
            EventManager.OnChangeFace += UpdateSpriteRenderSprite;
        }
    }
    private void OnDestroy()
    {
        if (_sprite)
        {

            EventManager.OnChangeFace -= UpdateSpriteRenderSprite;
        }
        if (_image)
        {
            EventManager.OnChangeFace -= UpdateSpriteRenderSprite;
        }
    }
    public void UpdateIamgeSprite()
    {
        switch (facetype)
        {
            case FaceType.眼睛:
                _image.sprite = EyeConfig.Get(ActorModel.Model.GetFace(facetype)).GetSprite();
                break;
            case FaceType.嘴巴:
                _image.sprite = MouthConfig.Get(ActorModel.Model.GetFace(facetype)).GetSprite();
                break;
            case FaceType.发型:
                _image.sprite = HairConfig.Get(ActorModel.Model.GetFace(facetype)).GetSprite();
                break;
            case FaceType.耳朵:
                _image.sprite = EarConfig.Get(ActorModel.Model.GetFace(facetype)).GetSprite();
                break;
            case FaceType.发饰:
                _image.sprite = HairDecorateConfig.Get(ActorModel.Model.GetFace(facetype)).GetSprite();
                break;
            default:
                break;
        }
    }
    public void UpdateSpriteRenderSprite()
    {
        switch (facetype)
        {
            case FaceType.眼睛:
                _sprite.sprite = EyeConfig.Get(ActorModel.Model.GetFace(facetype)).GetSprite();
                break;
            case FaceType.嘴巴:
                _sprite.sprite = MouthConfig.Get(ActorModel.Model.GetFace(facetype)).GetSprite();
                break;
            case FaceType.发型:
                _sprite.sprite = HairConfig.Get(ActorModel.Model.GetFace(facetype)).GetSprite();
                break;
            case FaceType.耳朵:
                _sprite.sprite = EarConfig.Get(ActorModel.Model.GetFace(facetype)).GetSprite();
                break;
            case FaceType.发饰:
                _sprite.sprite = HairDecorateConfig.Get(ActorModel.Model.GetFace(facetype)).GetSprite();
                break;
            default:
                break;
        }
    }
}
