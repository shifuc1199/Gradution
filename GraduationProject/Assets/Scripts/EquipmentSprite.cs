/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
public class EquipmentSprite : MonoBehaviour
{
    public EquipmentType equipment_type;
    Image _image;
    SpriteRenderer _sprite;
    private void Start()
    {
        _image = GetComponent<Image>();
        _sprite = GetComponent<SpriteRenderer>();
        if (_sprite)
        {
            UpdateSpriteRenderSprite();
            EventHandler.OnChangeEquipment += UpdateSpriteRenderSprite;
        }
        if (_image)
        {
            UpdateIamgeSprite();
            EventHandler.OnChangeEquipment += UpdateIamgeSprite;
        }

         
    }
    public void UpdateIamgeSprite()
    {
        switch (equipment_type)
        {
            case EquipmentType.鞋子:
                _image.sprite = FootConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.裤子:
                _image.sprite = PelvisConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.肩膀右:
                _image.sprite = ArmConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.肩膀左:
                _image.sprite = ArmConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetLsprite();
                break;
            case EquipmentType.手链:
                _image.sprite = SleeveConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.武器:
                _image.sprite = WeaponConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.上衣:
                _image.sprite = TorsoConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.盾牌:
                _image.sprite = ShieldConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            default:
                break;
        }
    }
    public void UpdateSpriteRenderSprite()
    {
        switch (equipment_type)
        {
            case EquipmentType.鞋子:
                _sprite.sprite = FootConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.裤子:
                _sprite.sprite = PelvisConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.肩膀右:
                _sprite.sprite = ArmConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.肩膀左:
                _sprite.sprite = ArmConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetLsprite();
                break;
            case EquipmentType.手链:
                _sprite.sprite = SleeveConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.武器:
                _sprite.sprite = WeaponConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.上衣:
                _sprite.sprite = TorsoConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            case EquipmentType.盾牌:
                _sprite.sprite = ShieldConfig.Get(ActorModel.Model.GetPlayerEquipment(equipment_type)).GetSprite();
                break;
            default:
                break;
        }
    }
}
