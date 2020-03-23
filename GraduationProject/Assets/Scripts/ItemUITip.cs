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
              _text.text = "\t\t\t\t\t装备名字: " + DreamerTool.Util.DreamerUtil.GetColorRichText(weapon.物品名字, GameStaticData.ITEM_COLOR_DICT[weapon.物品阶级])
                    + "\n\n\t\t\t\t\t装备阶级: " + DreamerTool.Util.DreamerUtil.GetColorRichText(weapon.物品阶级.ToString(), GameStaticData.ITEM_COLOR_DICT[weapon.物品阶级])
                    + "\n\n\n\t装备部位: 武器"
                    + "\n\n\t武器类型: "+weapon.武器种类
                    + "\n\n\t所需等级: " + DreamerTool.Util.DreamerUtil.GetColorRichText(weapon.需要等级.ToString(), weapon.需要等级 > ActorModel.Model.GetLevel() ? Color.red : Color.green)
                    + "\n\n\t攻击力: " + DreamerTool.Util.DreamerUtil.GetColorRichText(weapon.攻击力.ToString(),Color.yellow)
                      + "\n\n\t法术强度: " + weapon.法术强度
                     + "\n\n\t暴击率: " + weapon.暴击率*100+"%"
                     +"\n\n\t暴击伤害: " + weapon.暴击伤害
                    + "\n\n\t装备描述: " + weapon.物品描述;
                break;
            case ItemType.上衣:
                var torso = TorsoConfig.Get(id);
                  _icon.sprite = torso.GetSprite();
                _text.text = "\t\t\t\t\t装备名字: " + DreamerTool.Util.DreamerUtil.GetColorRichText(torso.物品名字, GameStaticData.ITEM_COLOR_DICT[torso.物品阶级])
                    + "\n\n\t\t\t\t\t装备阶级: " + DreamerTool.Util.DreamerUtil.GetColorRichText(torso.物品名字, GameStaticData.ITEM_COLOR_DICT[torso.物品阶级])
                    + "\n\n\n\t物理防御力: " + torso.defend
                     + "\n\n\t魔法防御力: " + torso.magicdefend
                    + "\n\n\t生命力: " + torso.helath + SuitConfig.Get(id).GetItemUITipStr()
                    + "\n\n装备描述: \n" + torso.物品描述;
                break;
            case ItemType.手链:
                var sleeve = SleeveConfig.Get(id);
                _icon.sprite = sleeve.GetSprite();
                _text.text = "\t\t\t\t\t装备名字: " + DreamerTool.Util.DreamerUtil.GetColorRichText(sleeve.物品名字, GameStaticData.ITEM_COLOR_DICT[sleeve.物品阶级])
                    + "\n\n\t\t\t\t\t装备阶级: " + DreamerTool.Util.DreamerUtil.GetColorRichText(sleeve.物品名字, GameStaticData.ITEM_COLOR_DICT[sleeve.物品阶级])
                    + "\n\n\n\t生命力: " + sleeve.helath + SuitConfig.Get(id).GetItemUITipStr()
                    + "\n\n装备描述: \n" + sleeve.物品描述;
                break;
            case ItemType.肩膀:
                var arm = ArmConfig.Get(id);
                _icon.sprite = arm.GetSprite();
                _text.text = "\t\t\t\t\t装备名字: " + DreamerTool.Util.DreamerUtil.GetColorRichText(arm.物品名字, GameStaticData.ITEM_COLOR_DICT[arm.物品阶级])
                    + "\n\n\t\t\t\t\t装备阶级: " + DreamerTool.Util.DreamerUtil.GetColorRichText(arm.物品名字, GameStaticData.ITEM_COLOR_DICT[arm.物品阶级])
                    + "\n\n\n\t生命力: " + arm.defend + SuitConfig.Get(id).GetItemUITipStr()
                    + "\n\n装备描述: \n" + arm.物品描述;
                break;
            case ItemType.裤子:
                var pelvis = PelvisConfig.Get(id);
                _icon.sprite = pelvis.GetSprite();
                _text.text = "\t\t\t\t\t装备名字: " + DreamerTool.Util.DreamerUtil.GetColorRichText(pelvis.物品名字, GameStaticData.ITEM_COLOR_DICT[pelvis.物品阶级])
                    + "\n\n\t\t\t\t\t装备阶级: " + DreamerTool.Util.DreamerUtil.GetColorRichText(pelvis.物品名字, GameStaticData.ITEM_COLOR_DICT[pelvis.物品阶级])
                 + "\n\n\n\t物理防御力: " + pelvis.defend + SuitConfig.Get(id).GetItemUITipStr()
                    + "\n\n装备描述: \n" + pelvis.物品描述;
                break;
            case ItemType.鞋子:
                 
                var foot = FootConfig.Get(id);
                _icon.sprite = foot.GetSprite();
                _text.text = "\t\t\t\t\t装备名字: " + DreamerTool.Util.DreamerUtil.GetColorRichText(foot.物品名字, GameStaticData.ITEM_COLOR_DICT[foot.物品阶级])
                    + "\n\n\t\t\t\t\t装备阶级: " + DreamerTool.Util.DreamerUtil.GetColorRichText(foot.物品名字, GameStaticData.ITEM_COLOR_DICT[foot.物品阶级])
                     + "\n\n\n\t物理防御力: " + foot.defend +
                     SuitConfig.Get(id).GetItemUITipStr()
                + "\n\n装备描述: \n" + foot.物品描述;
                
                break;
            case ItemType.盾牌:

                var shield = ShieldConfig.Get(id);
                _icon.sprite = shield.GetSprite();
                _text.text = "\t\t\t\t\t物品名字: " + DreamerTool.Util.DreamerUtil.GetColorRichText(shield.物品名字, GameStaticData.ITEM_COLOR_DICT[shield.物品阶级])
                    + "\n\n\t\t\t\t\t物品阶级: " + DreamerTool.Util.DreamerUtil.GetColorRichText(shield.物品名字, GameStaticData.ITEM_COLOR_DICT[shield.物品阶级])
                    + "\n\n\n\t生命值: " + shield.helath
                +"\n\n\t防御力: " + shield.defend
                +"\n\n\t魔法抗性: " + shield.magic_defend
                + "\n\n\t物品描述: " + shield.物品描述;
                break;
            case ItemType.消耗品:

                var consumbale = ConsumablesConfig.Get(id);
                _icon.sprite = consumbale.GetSprite();
                _text.text = "\t\t\t\t\t物品名字: " + DreamerTool.Util.DreamerUtil.GetColorRichText(consumbale.物品名字, GameStaticData.ITEM_COLOR_DICT[consumbale.物品阶级])
                    + "\n\n\t\t\t\t\t物品阶级: " + DreamerTool.Util.DreamerUtil.GetColorRichText(consumbale.物品名字, GameStaticData.ITEM_COLOR_DICT[consumbale.物品阶级])
                    + "\n\n\n\t物品描述: " + consumbale.物品描述;
                break;
        }
    }
 
}
