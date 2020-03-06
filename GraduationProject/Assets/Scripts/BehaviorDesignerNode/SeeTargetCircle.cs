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
    GameObject find_tip;
    public SharedFloat radius;
    public SharedVector3 offset;
    public override void OnAwake()
    {
        base.OnAwake();
        find_tip = transform.GetChild(1).gameObject;
    }
    public override TaskStatus OnUpdate()
    {
        if (!GetComponent<BaseEnemyController>().isMoveable)
        {
            find_tip.SetActive(false);
        return TaskStatus.Running;
        }

        var col = Physics2D.OverlapCircle(transform.position+ offset.Value, radius.Value,LayerMask.GetMask("player"));
        if(col!=null)
        {
            find_tip.SetActive(true);
            return TaskStatus.Success;
        }
        find_tip.SetActive(false);
        return TaskStatus.Failure;
    }
    public override void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Color c = Color.yellow;
        UnityEditor.Handles.color =c;
        UnityEditor.Handles.CircleHandleCap(0, Owner.transform.position + offset.Value, Quaternion.identity, radius.Value, EventType.Repaint);
#endif      
    }
 
    }
