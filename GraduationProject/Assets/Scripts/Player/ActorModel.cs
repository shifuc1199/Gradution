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

    public string actor_name;

    private double money;
    private double health;
    private double energy;
    private int level = 1;
    private int exp;
    private int max_exp=100;
    public Dictionary<int, SkillModel> skillmodels = new Dictionary<int, SkillModel>();
    
    public double GetAttack()
    {
        return WeaponConfig.Get(Equipment[EquipmentType.武器]).攻击力;
    }
    private Dictionary<PlayerAttribute, double> PlayerAttributes = new Dictionary<PlayerAttribute, double>()
    {
        { PlayerAttribute.攻击力,1 },
        { PlayerAttribute.生命值,100 },
        { PlayerAttribute.防御力,100 },
        { PlayerAttribute.能量值,100 },
         { PlayerAttribute.善恶值,100 },
    };
    private Dictionary<EquipmentType, int> Equipment = new Dictionary<EquipmentType, int>()
    {
         { EquipmentType.武器,1},
          { EquipmentType.盾牌,1},
         { EquipmentType.上衣,1},
         { EquipmentType.肩膀右,1},
         { EquipmentType.肩膀左,1},
         { EquipmentType.手链,1},
         { EquipmentType.裤子,1},
         { EquipmentType.鞋子,1}
    };
    private Dictionary<FaceType, int> Faces = new Dictionary<FaceType, int>()
    {
         { FaceType.发型,1},
         { FaceType.嘴巴,1},
         { FaceType.眼睛,1},
         { FaceType.耳朵,1},
         { FaceType.发饰,1},
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
        EventHandler.OnChangeEquipment();
    }
    public int GetPlayerEquipment(EquipmentType equip)
    {
        return Equipment[equip];
    }
    public void SetFace(FaceType face, int id)
    {
        Faces[face] = id;
        EventHandler.OnChangeFace?.Invoke();
    }
    public int GetFace(FaceType face)
    {
        return Faces[face];
    }
    public void SetMoney(double m)
    {
        money += m;
        EventHandler.OnChangeMoney();
    }
    public double GetMoney()
    {
        return money;
    }
    public void SetLevel(int l)
    {
        level += l;
        EventHandler.OnChangeLevel();
    }
    public int GetLevel()
    {
        return level;
    }
    public void SetExp(int exp)
    {
        this.exp += exp;
        if(this.exp>=max_exp)
        {
            SetLevel(this.exp / max_exp);
            this.exp = this.exp % max_exp;
             
        }
        EventHandler.OnChangeExp();
    }
    public int GetMaxExp()
    {
        return max_exp;
    }
    public int GetExp()
    {
        return exp;
    }


}
