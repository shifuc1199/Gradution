/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemUITip : MonoBehaviour
{
    public Text _text;
    public Image _icon;
    public void SetConfig(ItemType type,int id) 
    {
       
        switch (type)
        {
            case ItemType.武器:
                var weapon = WeaponConfig.Get(id);
                _icon.sprite = weapon.GetSprite();
              _text.text = "\t\t\t\t\t武器名字: " + weapon.物品名字
                    + "\n\n\t\t\t\t\t武器阶级: " + DreamerTool.Util.DreamerUtil.GetColorRichText(weapon.物品阶级.ToString(), GameStaticData.ITEM_COLOR_DICT[weapon.物品阶级])
                    + "\n\n\n\t所需等级: " + DreamerTool.Util.DreamerUtil.GetColorRichText(weapon.需要等级.ToString(), weapon.需要等级 > ActorModel.Model.GetLevel() ? Color.red : Color.green)
                    + "\n\n\t攻击力: " + DreamerTool.Util.DreamerUtil.GetColorRichText(weapon.攻击力.ToString(),Color.yellow)
                    + "\n\n\t武器描述: " + weapon.物品描述;
                break;
            case ItemType.上衣:
                var torso = TorsoConfig.Get(id);
                  _icon.sprite = torso.GetSprite();
                _text.text = "武器名字: " + torso.物品名字 + "\n\n武器描述: " + torso.物品描述 + "\n\n武器阶级: " + torso.物品阶级;
                break;
            case ItemType.手链:
                var sleeve = SleeveConfig.Get(id);
                _icon.sprite = sleeve.GetSprite();
                _text.text = "武器名字: " + sleeve.物品名字 + "\n\n武器描述: " + sleeve.物品描述 + "\n\n武器阶级: " + sleeve.物品阶级;
                break;
            case ItemType.肩膀:
                var arm = ArmConfig.Get(id);
                _icon.sprite = arm.GetSprite();
                _text.text = "武器名字: " + arm.物品名字 + "\n\n武器描述: " + arm.物品描述 + "\n\n武器阶级: " + arm.物品阶级;
                break;
            case ItemType.裤子:
                var pelvis = PelvisConfig.Get(id);
                _icon.sprite = pelvis.GetSprite();
                _text.text = "武器名字: " + pelvis.物品名字 + "\n\n武器描述: " + pelvis.物品描述 + "\n\n武器阶级: " + pelvis.物品阶级;
                break;
            case ItemType.鞋子:
                 
                var foot = FootConfig.Get(id);
                _icon.sprite = foot.GetSprite();
                _text.text = "武器名字: " + foot.物品名字 + "\n\n武器描述: " + foot.物品描述 + "\n\n武器阶级: " + foot.物品阶级;
                break;
        }
    }
 
}
