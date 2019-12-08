using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public interface IHurt 
{
  
    // Start is called before the first frame update
    void GetHurt(HitType _type, UnityAction hurt_call_back = null);
  
}
