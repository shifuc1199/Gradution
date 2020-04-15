/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class FileDataActor : SerializedMonoBehaviour
{
    public Dictionary<EquipmentType, SpriteRenderer> equipment = new Dictionary<EquipmentType, SpriteRenderer>();
    public Dictionary<FaceType, SpriteRenderer> face = new Dictionary<FaceType, SpriteRenderer>();
    
    public void SetModel(ActorModel _model)
    {
     
            foreach (var item in _model.Equipment)
            {
                Sprite sp = null;
                switch (item.Key)
                {
                    case EquipmentType.鞋子:
                        sp = FootConfig.Get(item.Value).GetSprite();
                        break;
                    case EquipmentType.裤子:
                        sp = PelvisConfig.Get(item.Value).GetSprite();
                        break;
                    case EquipmentType.肩膀左:
                        sp = ArmConfig.Get(item.Value).GetLsprite();
                        break;
                    case EquipmentType.肩膀右:
                        sp = ArmConfig.Get(item.Value).GetSprite();
                        break;
                    case EquipmentType.手链:
                        sp = SleeveConfig.Get(item.Value).GetSprite();
                        break;
                    case EquipmentType.武器:
                        sp = WeaponConfig.Get(item.Value).GetSprite();
                        break;
                    case EquipmentType.上衣:
                        sp = TorsoConfig.Get(item.Value).GetSprite();
                        break;
                    case EquipmentType.盾牌:
                        sp = ShieldConfig.Get(item.Value).GetSprite();
                        break;
                    default:
                        break;
                }
                if(equipment.ContainsKey( item.Key) )
                equipment[item.Key].sprite = sp;
            }
      
        foreach (var item in _model.Faces)
        {
            Sprite sp = null;
            switch (item.Key)
            {
                case FaceType.发型:
                    sp = HairConfig.Get(item.Value).GetSprite();
                    break;
                case FaceType.眼睛:
                    sp = EyeConfig.Get(item.Value).GetSprite();
                    break;
                case FaceType.嘴巴:
                    sp = MouthConfig.Get(item.Value).GetSprite();
                    break;
                case FaceType.耳朵:
                    sp = EarConfig.Get(item.Value).GetSprite();
                    break;
                case FaceType.发饰:
                    sp = HairDecorateConfig.Get(item.Value).GetSprite();
                    break;
                default:
                    break;
            }
            face[item.Key].sprite = sp;
        }
    }
}
