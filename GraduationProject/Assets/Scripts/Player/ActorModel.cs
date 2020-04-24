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
    public static void CreateModel()
    {
        _model = new ActorModel();
    }
    public static void UpdateModel(ActorModel model)
    {
        _model = model;
    }
    public ActorModel()
    {
        SetPlayerAttribute(PlayerAttribute.攻击力, GetAttackFromEquipment());
    }

    public int SaveDataID ;

    public string actor_name="无名氏";

    public double money=10000;

    public double health=100;

    public double energy=100;

    public int level = 1;

    public int exp = 0;

    public KnightLevel knightLevel = KnightLevel.圣骑士;
    public List<BagItemData> bag_items = new List<BagItemData>();

    public int suit_id = 1; // 套装ID
    public Dictionary<int, SkillModel> skillmodels = new Dictionary<int, SkillModel>();
    public Dictionary<int,SkillModel> equip_skil = new Dictionary<int, SkillModel>() {
        { 0, null},
        { 1, null},
        { 2, null }
    }; //装备的技能

    public Dictionary<PlayerAttribute, double> PlayerAttributes = new Dictionary<PlayerAttribute, double>()
    {
        { PlayerAttribute.攻击力,0 },
        { PlayerAttribute.生命值,100 },
        { PlayerAttribute.物防,100 },
        { PlayerAttribute.魔抗,100 },
        { PlayerAttribute.能量值,100 },
        { PlayerAttribute.善恶值,100 },
        { PlayerAttribute.暴击率,0 },
         { PlayerAttribute.法强,0 },
        { PlayerAttribute.移速,30 },
        { PlayerAttribute.暴击伤害,0 },
    };

    public Dictionary<EquipmentType, int> Equipment = new Dictionary<EquipmentType, int>()
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

    public Dictionary<FaceType, int> Faces = new Dictionary<FaceType, int>()
    {
         { FaceType.发型,10},
         { FaceType.嘴巴,1},
         { FaceType.眼睛,2},
         { FaceType.耳朵,1},
         { FaceType.发饰,3},
    };
    private double GetAttackFromEquipment()
    {
        return WeaponConfig.Get(Equipment[EquipmentType.武器]).攻击力;
    }
    public void ResetState()
    {
        health = PlayerAttributes[PlayerAttribute.生命值];
        energy = PlayerAttributes[PlayerAttribute.能量值];
        EventManager.OnChangeHealth?.Invoke();
        EventManager.OnChangeEnergy?.Invoke();
    }
    public  SkillModel GetSkillModel(int id)
    {
        if (skillmodels.ContainsKey(id))
            return skillmodels[id];

        return null;

    }
    public void SetHealth(double v)
    {
        health += v;
        EventManager.OnChangeHealth?.Invoke();
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
        EventManager.OnChangeEnergy?.Invoke();
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
        EventManager.OnChangeEquipment?.Invoke();

        if (GetSuitAmount(id) == 5)
        {
            suit_id = id;

            var commond = SuitConfig.Get(id).suit_function.Split(',');
            foreach (var c in commond)
            {
                GameStaticMethod.ExecuteCommond(c);
            }
        }
        else
        {
           
            if (suit_id!=-1)
            {
                var commond = SuitConfig.Get(suit_id).suit_function.Split(',');
                foreach (var c in commond)
                {
                    GameStaticMethod.ExecuteBackCommond(c);
                }
                suit_id = -1;
            }

        }
        
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
        EventManager.OnChangeMoney?.Invoke();
    }
    public double GetMoney()
    {
        return money;
    }
    public void SetLevel(int l)
    {
        level += l;
        EventManager.OnChangeLevel?.Invoke();
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
        EventManager.OnChangeExp?.Invoke();
    }
    public int GetMaxExp()
    {
        return level * 100;
    }
    public int GetExp()
    {
        return exp;
    }
    public int GetSuitAmount(int id)
    {
        int amount = 0;
        if (GetPlayerEquipment(EquipmentType.上衣) == id)
            amount++;
        if (GetPlayerEquipment(EquipmentType.手链) == id)
            amount++;
        if (GetPlayerEquipment(EquipmentType.裤子) == id)
            amount++;
        if (GetPlayerEquipment(EquipmentType.鞋子) == id)
            amount++;
        if (GetPlayerEquipment(EquipmentType.肩膀左) == id)
            amount++;
        return amount;
    }

}
