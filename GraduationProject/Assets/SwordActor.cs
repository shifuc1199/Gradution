using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SwordActor : BaseActorController
{
    public float attack_time;
    float timer;
    float attack_timer;
    private void Start()
    {
   
    }
   

    public override void Attack()
    {
        if(Input.GetKey(attack_key))
        {
            attack_timer += Time.deltaTime;
            if(attack_timer >= attack_time)
            {
                GetComponent<Animator>().SetTrigger("attack");
                attack_timer = 0;
            }
        }
    }
}
