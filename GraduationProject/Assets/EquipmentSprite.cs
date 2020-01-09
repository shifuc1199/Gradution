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
            case EquipmentType.腿部:
                break;
            case EquipmentType.裤子:
                break;
            case EquipmentType.肩膀:
                break;
            case EquipmentType.手腕:
                break;
            case EquipmentType.武器:
                _image.sprite = WeaponConfig.Get(ActorController._controller.model.Equipment[equipment_type]).GetSprite();
                break;
            case EquipmentType.上衣:
                break;
            default:
                break;
        }
    }
    public void UpdateSpriteRenderSprite()
    {
        switch (equipment_type)
        {
            case EquipmentType.腿部:
                break;
            case EquipmentType.裤子:
                break;
            case EquipmentType.肩膀:
                break;
            case EquipmentType.手腕:
                break;
            case EquipmentType.武器:
                _sprite.sprite = WeaponConfig.Get(ActorController._controller.model.Equipment[equipment_type]).GetSprite();
                break;
            case EquipmentType.上衣:
                break;
            default:
                break;
        }
    }
}
