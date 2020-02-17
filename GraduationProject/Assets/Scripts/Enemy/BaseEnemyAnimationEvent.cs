using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Extra;
public class BaseEnemyAnimationEvent : MonoBehaviour
{
    private BaseEnemyController _controller;
    private Rigidbody2D _rigi;
    private void Awake()
    {

        _controller = GetComponentInParent<BaseEnemyController>();
        _rigi = GetComponentInParent<Rigidbody2D>();
       
    }

    public void OnStandEnter()
    {
        
        _rigi.SetGravity(_controller.start_gravity);
    }
}
