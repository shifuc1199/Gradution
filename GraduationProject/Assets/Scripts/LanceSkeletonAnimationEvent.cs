/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Extra;
public class LanceSkeletonAnimationEvent : BaseEnemyAnimationEvent
{
    public float dash_speed;
    bool isDash = false;
    public void LanceAttackStart()
    {
        isDash = true;
    }
    public void OnLanceAttackStay()
    {
        if(isDash)
        _rigi.position  -=  (Vector2)_controller.transform.right * Time.deltaTime * dash_speed;
       
    }
    
    public void LanceAttackDashEnd()
    {
        isDash = false;
        
    }

}
