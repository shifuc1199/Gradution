using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowActor : BaseActorController
{
    public float attack_timer;
    float timer;
    public Transform bow;
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
        var dir = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)) - transform.position;
        dir.z = 0;
        bow.right = dir;
      
        if (Input.GetKey(attack_key))
        {
            timer += Time.deltaTime;
            if(timer>=attack_timer)
            {
                GetComponent<Animator>().SetTrigger("attack");
            }
        }
    }

    
}
