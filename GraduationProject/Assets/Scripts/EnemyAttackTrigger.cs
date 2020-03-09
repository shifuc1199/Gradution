/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTrigger : BaseAttackTrigger
{
    
    
    public override void OnTriggerEnter2D(Collider2D collision)
    {
          
        
        if (collision.gameObject.tag=="Player")
        {
            
            collision.GetComponent<IHurt>().GetHurt(10, attack_type,transform.position);
        }

    }
}
