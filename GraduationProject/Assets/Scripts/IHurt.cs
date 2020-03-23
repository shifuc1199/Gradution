using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public struct AttackData
{
    public AttackData(double h,bool c,Vector3 d,HitType a)
    {
        hurt_value = h;
        isCrit = c;
        attack_pos = d;
        attack_type = a;
    }
    public double hurt_value;
    public bool isCrit;
    public Vector3 attack_pos;
    public HitType attack_type;
}

public interface IHurt 
{
    void GetHurt(AttackData data, UnityAction hurt_call_back = null);
}
