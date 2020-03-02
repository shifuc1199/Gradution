/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Extra;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
public class Partol : Action
{
       Animator animator;
    public float partol_speed;
    public float wait_time;
    private Transform[] way_points; // 自动赋值 waypoints 路径点
     int way_point_index;
    public override void OnAwake()
    {
        base.OnAwake();
        animator = transform.GetChild(0).GetComponent<Animator>();
        way_points = transform.parent.Find("WayPoints").GetChildren().ToArray();
    }
    public override TaskStatus OnUpdate()
    { if (GetComponent<BaseEnemyController>().isMoveable)
        {
            var target = new Vector2(way_points[way_point_index].transform.position.x, transform.position.y);

            transform.rotation = target.x > transform.position.x ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
            transform.position = Vector2.MoveTowards(transform.position, target, partol_speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, target) <= 1f)
            {
                way_point_index++;
                way_point_index %= way_points.Length;

            }
            animator.SetBool("walk", true);
        }
    else
        {
            animator.SetBool("walk", false);
        }
        return TaskStatus.Running;
    }
    public override void OnConditionalAbort()
    {
        base.OnConditionalAbort();
 
        animator.SetBool("walk", false);

    }
}
