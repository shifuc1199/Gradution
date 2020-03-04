/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
public class ChaseAttack : Action
{
     
    Animator animator;
     Transform target;
    public float chase_speed;
    public SharedFloat attack_distance;
    public float attack_interval;
    float attack_timer ;
    public override void OnAwake()
    {
        base.OnAwake();
        animator = transform.GetChild(0).GetComponent<Animator>();

        
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void OnStart()
    {
        base.OnStart();
         
        
        Debug.Log("Start");
    }
     
    
    public override TaskStatus OnUpdate()
    {
        if (GetComponent<BaseEnemyController>().isMoveable)
        {
            transform.rotation = target.position.x > transform.position.x ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
            if (Vector3.Distance(transform.position, new Vector2(target.position.x, transform.position.y)) <= attack_distance.Value)
            {
                animator.SetBool("run", false);

                attack_timer += Time.deltaTime;
                if (attack_timer >= attack_interval)
                {
                    animator.SetTrigger("attack");
                    attack_timer = 0;
                }

            }
            else
            {
                 
                transform.position = Vector3.MoveTowards(transform.position, new Vector2(target.position.x, transform.position.y), Time.deltaTime * chase_speed);
                animator.SetBool("run", true);
            }
        }
        else
        {
            animator.SetBool("run", false);
            attack_timer = 0;
        }
       
        return TaskStatus.Running;
    }
    
    public override void OnEnd()
    {
        base.OnConditionalAbort();
         
       
        Debug.Log("退出");
        animator.SetBool("run", false);
    }
     
    
 
}
