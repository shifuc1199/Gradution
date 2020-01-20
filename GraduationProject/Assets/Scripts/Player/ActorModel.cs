using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public   class ActorModel
{
    [JsonNonField]
    private static ActorModel _model = null;

    [JsonNonField]
    static public ActorModel Model
    {
        get
        {
            return _model;
        }
    }
    public ActorModel()
    {
        _model = this;
    }
    private Dictionary<PlayerAttribute, double> PlayerAttributes = new Dictionary<PlayerAttribute, double>()
    {
        { PlayerAttribute.攻击力,1},
        { PlayerAttribute.生命值,100 },
    };
    private Dictionary<EquipmentType, int> Equipment = new Dictionary<EquipmentType, int>()
    {
        { EquipmentType.武器,1},
         { EquipmentType.上衣,1},
         { EquipmentType.肩膀右,1},
         { EquipmentType.肩膀左,1},
          { EquipmentType.手链,1},
         { EquipmentType.裤子,1},
         { EquipmentType.鞋子,1}
    };
    public void SetPlayerAttribute(PlayerAttribute attribute, double value)
    {
        PlayerAttributes[attribute] += value;
    }
    public double GetPlayerAttribute(PlayerAttribute attribute)
    {
        return PlayerAttributes[attribute];
    }
    public void SetPlayerEquipment(EquipmentType equip, int id)
    {
        Equipment[equip] = id;
    }
    public int GetPlayerEquipment(EquipmentType equip)
    {
        return Equipment[equip];
    }
    public double GetAttack()
    {
        return WeaponConfig.Get(Equipment[EquipmentType.武器]).攻击力;
    }

}
