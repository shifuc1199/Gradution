/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
public class Attack : Action
{
    Animator animator;
    public float attack_interval;
    float attack_timer;
    public override void OnAwake()
    {
        base.OnAwake();
        animator = transform.GetChild(0).GetComponent<Animator>();
    }
    public override TaskStatus OnUpdate()
    {
        if (!GetComponent<BaseEnemyController>().isMoveable)
            return TaskStatus.Running;

        attack_timer += Time.deltaTime;
        if (attack_timer >= attack_interval)
        {
            animator.SetTrigger("attack");
            attack_timer = 0;
        }

        return TaskStatus.Running;


    }
    public override void OnEnd()
    {
        base.OnEnd();
        attack_timer = 0;
    }
}
