/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
public class SeeTarget : Conditional
{
    public Transform target;
    public float distance;
    public float view_angle;
    public Vector3 offset;
    public override void OnAwake()
    {
        base.OnAwake();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public override TaskStatus OnUpdate()
    {
        if (!GetComponent<BaseEnemyController>().isMoveable)
            return TaskStatus.Running;
        var newPos = transform.position + offset;
        Vector3 newVec = Quaternion.Euler(0, 0, -transform.right.x * view_angle / 2) *  transform.right;
        Vector3 newVec2 = Quaternion.Euler(0, 0, transform.right.x * view_angle / 2) * transform.right;
        Vector3 newVec3 = Quaternion.Euler(0, 0, -transform.right.x * view_angle / 2) * -transform.right;
        Vector3 newVec4 = Quaternion.Euler(0, 0, transform.right.x * view_angle / 2) * -transform.right;
        Debug.DrawLine(newPos, newPos - newVec.normalized * distance);
        
        Debug.DrawLine(newPos, newPos - newVec2.normalized * distance);
        Debug.DrawLine(newPos, newPos - newVec3.normalized * distance);

        Debug.DrawLine(newPos, newPos - newVec4.normalized * distance);
        if (Vector2.Distance(transform.position,target.position)<=distance)
        {
            var dir = newPos - target.transform.position;  
            if(Mathf.Abs( Vector3.Dot(transform.right.normalized, dir.normalized))>= Mathf.Cos(view_angle /2 * Mathf.Deg2Rad))
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
