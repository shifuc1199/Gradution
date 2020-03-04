/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AxisType
{
    Right,
    Left
}

public class AutoMoveObjectByAxis : AutoMoveObject 
{
    public AxisType axis;

    private void Update()
    {
        switch (axis)
        {
            case AxisType.Right:
                transform.Translate(transform.right * Speed * Time.deltaTime, space_type);
                break;
            case AxisType.Left:
                transform.Translate(-transform.right * Speed * Time.deltaTime, space_type);
                break;
            default:
                break;
        }
    }
}
