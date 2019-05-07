using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{

    public void OnAttackEnter()
    {

    }

    public void OnAttackExit()
    {
      
        GetComponent<PlayerState>().isInputable = true;
    }

}

