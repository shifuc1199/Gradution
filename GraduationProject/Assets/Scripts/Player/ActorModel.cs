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

        SetPlayerAttribute(PlayerAttribute.攻击力, GetAttackFromEquipment());
    }

    public string actor_name;

    private double money=10000;
    private double health=100;
    private double energy=100;

    private int level = 1;
    private int exp;
    public List<ItemUI> bag_items = new List<ItemUI>();
    public Dictionary<int, SkillModel> skillmodels = new Dictionary<int, SkillModel>();
    public Dictionary<int,SkillModel> equip_skil = new Dictionary<int, SkillModel>() {
        { 0, null},
        { 1, null},
        { 2, null }
    }; //装备的技能
    private Dictionary<PlayerAttribute, double> PlayerAttributes = new Dictionary<PlayerAttribute, double>()
    {
        { PlayerAttribute.攻击力,0 },
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
    private double GetAttackFromEquipment()
    {
        return WeaponConfig.Get(Equipment[EquipmentType.武器]).攻击力;
    }
    public void SetHealth(double v)
    {
        health += v;
        EventManager.OnChangeHealth();
    }
    public WeaponConfig GetCurrentWeapon()
    {
        return WeaponConfig.Get(GetPlayerEquipment(EquipmentType.武器));
    }
    public double GetHealth()
    {
        return health;
    }
    public void SetEngery(double e)
    {
        if (e == 0)
            return;

        if (e > 0)
        {
            if (energy == GetPlayerAttribute(PlayerAttribute.能量值))
                return;
            if (energy + e >= GetPlayerAttribute(PlayerAttribute.能量值))
            {
                energy = GetPlayerAttribute(PlayerAttribute.能量值);
            }
            else
            {
                energy += e;
            }
        }
        else if (e<0)
        {
            if (energy == 0)
                return;

            energy += e;
        }
        EventManager.OnChangeEnergy();
    }
    public double GetEngery()
    {
        return energy;
    }
    public void SetPlayerAttribute(PlayerAttribute attribute, double value)
    {
        PlayerAttributes[attribute] += value;
        EventManager.OnChangePlayerAttribute?.Invoke(attribute,value);
        
    }
    public double GetPlayerAttribute(PlayerAttribute attribute)
    {
        return PlayerAttributes[attribute];
    }
    public void SetPlayerEquipment(EquipmentType equip, int id)
    {
        Equipment[equip] = id;
        EventManager.OnChangeEquipment();
    }
    public int GetPlayerEquipment(EquipmentType equip)
    {
        return Equipment[equip];
    }
    public void SetFace(FaceType face, int id)
    {
        Faces[face] = id;
        EventManager.OnChangeFace?.Invoke();
    }
    public int GetFace(FaceType face)
    {
        return Faces[face];
    }
    public Dictionary<FaceType, int> GetFaceDictionary()
    {
        return Faces;
    }
    public void SetMoney(double m)
    {
        money += m;
        EventManager.OnChangeMoney();
    }
    public double GetMoney()
    {
        return money;
    }
    public void SetLevel(int l)
    {
        level += l;
        EventManager.OnChangeLevel();
    }
    public int GetLevel()
    {
        return level;
    }
    public void SetExp(int exp)
    {
        this.exp += exp;
        if(this.exp>= GetMaxExp())
        {
            SetLevel(this.exp / GetMaxExp());
            this.exp = this.exp % GetMaxExp();
             
        }
        EventManager.OnChangeExp();
    }
    public int GetMaxExp()
    {
        return level * 100;
    }
    public int GetExp()
    {
        return exp;
    }


}
