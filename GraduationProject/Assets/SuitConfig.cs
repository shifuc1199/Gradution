/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using LitJson;
using System.IO;
using UnityEditor;
public class SuitConfig : BaseConfig<SuitConfig>
{
    [ReadOnly]
    public int ID;
    public string suit_name;
    public string suit_des;
    public string suit_function;
    [Button("保存", 50)]
    public void Save()
    {
#if UNITY_EDITOR
        TextAsset ta = Resources.Load<TextAsset>("all_config");
        JsonData jd = JsonMapper.ToObject(ta.text);
        if (jd["Suit"] == null)
            jd["Suit"] = new JsonData();
        JsonData data = new JsonData();
        data["ID"] = ID;
        data["suit_name"] = suit_name;
        data["suit_des"] = suit_des;
        data["suit_function"] = suit_function;
        jd["Suit"][ID.ToString()] = data;

        using (StreamWriter sw = new StreamWriter(new FileStream("Assets/Resources/all_config.json", FileMode.Truncate)))
        {
            sw.Write(jd.ToJson());
        }
        AssetDatabase.Refresh();
        SuitEditorWindow._window.ForceMenuTreeRebuild();
        SuitEditorWindow._window.isCreate = false;
        if (SuitEditorWindow._window._tree.MenuItems.Count > 0)
            SuitEditorWindow._window._tree.MenuItems[ID - 1].Select();
#endif
    }
    private  string GetItemUITipSuitString(string suit_name)
    {
        return "\n\t\t\t\t" +
          DreamerTool.Util.DreamerUtil.GetColorRichText(suit_name + "战甲", ActorModel.Model.GetPlayerEquipment(EquipmentType.上衣) == ID?Color.white:Color.gray)    + "\n\t\t\t\t" +
           DreamerTool.Util.DreamerUtil.GetColorRichText(suit_name + "战裤", ActorModel.Model.GetPlayerEquipment(EquipmentType.裤子) == ID ? Color.white : Color.gray) + "\n\t\t\t\t" +
        DreamerTool.Util.DreamerUtil.GetColorRichText(suit_name + "肩甲", ActorModel.Model.GetPlayerEquipment(EquipmentType.肩膀右) == ID ? Color.white : Color.gray) + "\n\t\t\t\t" +
          DreamerTool.Util.DreamerUtil.GetColorRichText(suit_name + "手环", ActorModel.Model.GetPlayerEquipment(EquipmentType.手链) == ID ? Color.white : Color.gray) + "\n\t\t\t\t" +
         DreamerTool.Util.DreamerUtil.GetColorRichText(suit_name + "战靴", ActorModel.Model.GetPlayerEquipment(EquipmentType.鞋子) == ID ? Color.white : Color.gray);
    }
    public string   GetItemUITipStr()
    {
        int suit_amount = ActorModel.Model.GetSuitAmount(ID);
        return  suit_name + "套装" + "("+ suit_amount + "/5)" + GetItemUITipSuitString(suit_name) + "\n"+DreamerTool.Util.DreamerUtil.GetColorRichText("(5)套装: " +  suit_des, suit_amount == 5 ? Color.white:Color.gray);
    }
}
