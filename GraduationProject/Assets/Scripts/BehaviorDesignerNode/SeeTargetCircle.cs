/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
public class SeeTargetCircle : Conditional
{
    public float radius;
    public Vector3 offset;
    public override TaskStatus OnUpdate()
    {
        if (!GetComponent<BaseEnemyController>().isMoveable)
            return TaskStatus.Running;

        var col = Physics2D.OverlapCircle(transform.position+ offset, radius,LayerMask.GetMask("player"));
        if(col!=null)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
