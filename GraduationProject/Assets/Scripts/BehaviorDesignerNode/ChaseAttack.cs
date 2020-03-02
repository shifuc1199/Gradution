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
      GameObject find_tip;
    Animator animator;
    public SharedTransform target;
    public float chase_speed;
    public float attack_distance;
    public float attack_interval;
    float attack_timer ;
    public override void OnAwake()
    {
        base.OnAwake();
        animator = transform.GetChild(0).GetComponent<Animator>();
        find_tip = transform.GetChild(1).gameObject;
    }

    public override void OnStart()
    {
        base.OnStart();
         
        find_tip.SetActive(true);
        Debug.Log("Start");
    }
     
    
    public override TaskStatus OnUpdate()
    {
        if (GetComponent<BaseEnemyController>().isMoveable)
        {
            transform.rotation = target.Value.position.x > transform.position.x ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
            if (Vector3.Distance(transform.position, new Vector2(target.Value.position.x, transform.position.y)) <= attack_distance)
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
                 
                transform.position = Vector3.MoveTowards(transform.position, new Vector2(target.Value.position.x, transform.position.y), Time.deltaTime * chase_speed);
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
         
        find_tip.SetActive(false);
        Debug.Log("退出");
        animator.SetBool("run", false);
    }
     
    
 
}
