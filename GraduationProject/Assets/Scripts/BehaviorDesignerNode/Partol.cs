/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
public class Partol : Action
{
    public SharedGameObject animator;
    public float partol_speed;
    public float wait_time;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("目标点")]public Transform[] way_points;
     int way_point_index;
    public override TaskStatus OnUpdate()
    {
        var target = new Vector2(way_points[way_point_index].transform.position.x, transform.position.y);
        transform.rotation = target.x > transform.position.x ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        transform.position = Vector2.MoveTowards(transform.position, target, partol_speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, target) <= 1f)
        {
            way_point_index++;
            way_point_index %= way_points.Length;

        }
        animator.Value.GetComponent<Animator>().SetBool("walk", true);
        return TaskStatus.Running;
    }
    public override void OnConditionalAbort()
    {
        base.OnConditionalAbort();
 
        animator.Value.GetComponent<Animator>().SetBool("walk", false);

    }
}
