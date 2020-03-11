using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
 
public static class EventManager   
{
    public static UnityAction OnChangeEquipment;
    public static UnityAction OnChangeFace;
    public static UnityAction OnChangeMoney;
    public static UnityAction OnChangeHealth;
    public static UnityAction OnChangeEnergy;
    public static UnityAction OnChangeExp;
    public static UnityAction OnChangeLevel;
    public static UnityAction<PlayerAttribute,double> OnChangePlayerAttribute;
    public static UnityAction<SkillModel> OnSkillLevelUp;

 

}
