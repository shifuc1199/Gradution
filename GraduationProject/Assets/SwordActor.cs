using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordActor : ActorBase
{
    public float attack_timer;
    float timer;
     
    private void Start()
    {
   
    }
    public override void Jump()
    {
        if(Input.GetKeyDown(jump_key))
        {

        }
    }

    public override void Attack()
    {
        if(Input.GetKey(attack_key))
        {
            timer += Time.deltaTime;
            if(timer>=attack_timer)
            {
                GetComponent<Animator>().SetTrigger("attack");
            }
        }
    }

    
}
