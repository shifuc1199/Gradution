using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class BaseAttackTrigger:MonoBehaviour
{
    
    public HitType attack_type;
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {

    }
    
}
