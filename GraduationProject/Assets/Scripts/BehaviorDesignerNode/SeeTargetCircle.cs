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
    public SharedFloat radius;
    public SharedVector3 offset;
    public override TaskStatus OnUpdate()
    {
        if (!GetComponent<BaseEnemyController>().isMoveable)
            return TaskStatus.Running;

        var col = Physics2D.OverlapCircle(transform.position+ offset.Value, radius.Value,LayerMask.GetMask("player"));
        if(col!=null)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
    public override void OnDrawGizmos()
    {

        Color c = Color.yellow;
        UnityEditor.Handles.color =c;
        UnityEditor.Handles.CircleHandleCap(0, Owner.transform.position + offset.Value, Quaternion.identity, radius.Value, EventType.Repaint);
        
    }
 
    }
