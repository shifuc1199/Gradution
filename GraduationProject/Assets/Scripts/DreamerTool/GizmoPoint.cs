/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoPoint : MonoBehaviour
{
    public float radius;
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, radius);

    }
}
