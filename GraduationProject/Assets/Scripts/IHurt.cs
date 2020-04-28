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
        posx = d.x;
        posy = d.y;
        attack_type = a;
    }
    public double hurt_value;
    public bool isCrit;
    public double posx;
    public double posy;
    [JsonNonField]
    public Vector3 attack_pos {
        get {
            return new Vector3((float)posx, (float)posy);
        }
    }

    public HitType attack_type;
}

public interface IHurt 
{
    void GetHurt(AttackData data);
}
