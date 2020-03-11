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
    public void SetConfig(ItemType type,int id) 
    {
        switch (type)
        {
            case ItemType.武器:
                var i1 = WeaponConfig.Get(id);
                _text.text = "武器名字: "+i1.物品名字 +"\n\n武器阶级: " + i1.物品阶级+"\n\n所需等级: "+DreamerTool.Util.DreamerUtil.GetColorRichText(i1.需要等级.ToString(), i1.需要等级>ActorModel.Model.GetLevel()?Color.red:Color.green)+ "\n\n武器描述: " + i1.物品描述;
                break;
            case ItemType.上衣:
                var i2 = TorsoConfig.Get(id);
                _text.text = "武器名字: " + i2.物品名字 + "\n\n武器描述: " + i2.物品描述 + "\n\n武器阶级: " + i2.物品阶级;
                break;
            case ItemType.手链:
                var i3 = SleeveConfig.Get(id);
                _text.text = "武器名字: " + i3.物品名字 + "\n\n武器描述: " + i3.物品描述 + "\n\n武器阶级: " + i3.物品阶级;
                break;
            case ItemType.肩膀:
                var i4 = ArmConfig.Get(id);
                _text.text = "武器名字: " + i4.物品名字 + "\n\n武器描述: " + i4.物品描述 + "\n\n武器阶级: " + i4.物品阶级;
                break;
            case ItemType.裤子:
                var i5 = PelvisConfig.Get(id);
                _text.text = "武器名字: " + i5.物品名字 + "\n\n武器描述: " + i5.物品描述 + "\n\n武器阶级: " + i5.物品阶级;
                break;
            case ItemType.鞋子:
                var i6 = FootConfig.Get(id);
                _text.text = "武器名字: " + i6.物品名字 + "\n\n武器描述: " + i6.物品描述 + "\n\n武器阶级: " + i6.物品阶级;
                break;
        }
    }
 
}
