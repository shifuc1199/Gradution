using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Extra;
public class BaseEnemyAnimationEvent : MonoBehaviour
{
    [HideInInspector]public BaseEnemyController _controller;
    [HideInInspector] public Rigidbody2D _rigi;
    private void Awake()
    {

        _controller = GetComponentInParent<BaseEnemyController>();
        _rigi = GetComponentInParent<Rigidbody2D>();
       
    }
    public void OnAttackEnter()
    {
        _controller.isMoveable = false;
    }
    public void OnAttackExit()
    {
        _controller.isMoveable = true;
    }
    public void OnStandEnter()
    {
        
        _rigi.SetGravity(_controller.start_gravity);
    }
}
