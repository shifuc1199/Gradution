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
    public GameObject find_tip;
    public SharedGameObject animator;
    public SharedTransform target;
    public float chase_speed;
    public float attack_distance;
    public float attack_interval;
    float attack_timer ;
    public override void OnAwake()
    {
        base.OnAwake();
         
    }

    public override void OnStart()
    {
        base.OnStart();
         
        find_tip.SetActive(true);
        Debug.Log("Start");
    }
     
    
    public override TaskStatus OnUpdate()
    {
        if(Vector3.Distance(transform.position, new Vector2(target.Value.position.x, transform.position.y))<= attack_distance)
        {
            animator.Value.GetComponent<Animator>().SetBool("run", false);
            
            attack_timer += Time.deltaTime;
            if(attack_timer>=attack_interval)
            {
                animator.Value.GetComponent<Animator>().SetTrigger("attack");
                attack_timer = 0;
            }
            
        }
        else
        {
            attack_timer = attack_interval;
            if (GetComponent<BaseEnemyController>().isMoveable)
            transform.position = Vector3.MoveTowards(transform.position, new Vector2(target.Value.position.x, transform.position.y), Time.deltaTime * chase_speed);
            animator.Value.GetComponent<Animator>().SetBool("run", true);
        }
       
        return TaskStatus.Running;
    }
    
    public override void OnEnd()
    {
        base.OnConditionalAbort();
         
        find_tip.SetActive(false);
        Debug.Log("退出");
        animator.Value.GetComponent<Animator>().SetBool("run", false);
    }
     
    
 
}
