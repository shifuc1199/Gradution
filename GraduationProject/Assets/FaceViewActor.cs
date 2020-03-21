/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
public class FaceViewActor : SerializedMonoBehaviour
{
    public Dictionary<FaceType, SpriteRenderer> faces = new Dictionary<FaceType, SpriteRenderer>();

    public void UpdateFace(FaceType _type, int _id)
    {
        switch (_type)
        {
            case FaceType.眼睛:
                faces[_type].sprite = EyeConfig.Get(_id).GetSprite();
                break;
            case FaceType.嘴巴:
                faces[_type].sprite = MouthConfig.Get(_id).GetSprite();
                break;
            case FaceType.发型:
                faces[_type].sprite = HairConfig.Get(_id).GetSprite();
                break;
            case FaceType.耳朵:
                faces[_type].sprite = EarConfig.Get(_id).GetSprite();
                break;
            case FaceType.发饰:
                faces[_type].sprite = HairDecorateConfig.Get(_id).GetSprite();
                break;
            default:
                break;
        }
    }
    public void UpdateFace(Dictionary<FaceType, int> dict)
    {
        foreach (var item in dict)
        {
            var _type = item.Key;
            var _id = item.Value;
            switch (_type)
            {
                case FaceType.眼睛:
                    faces[_type].sprite = EyeConfig.Get(_id).GetSprite();
                    break;
                case FaceType.嘴巴:
                    faces[_type].sprite = MouthConfig.Get(_id).GetSprite();
                    break;
                case FaceType.发型:
                    faces[_type].sprite = HairConfig.Get(_id).GetSprite();
                    break;
                case FaceType.耳朵:
                    faces[_type].sprite = EarConfig.Get(_id).GetSprite();
                    break;
                case FaceType.发饰:
                    faces[_type].sprite = HairDecorateConfig.Get(_id).GetSprite();
                    break;
                default:
                    break;
            }
        }
        
    }
}
