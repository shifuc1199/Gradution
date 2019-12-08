using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseActorAnimationEvent : MonoBehaviour
{
    [System.NonSerialized]public Rigidbody2D _rigi;
    [System.NonSerialized]public Animator _anim;
    [System.NonSerialized]public ActorController _controller;
    private void Start()
    {
        _controller = GetComponentInParent<ActorController>();
        _rigi = GetComponentInParent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }
    public void OnDashEnter()
    {
        _rigi.velocity = transform.right * 80;
        GetComponentInParent<AfterImage>().IsUpdate = true;
    }
    public void OnDashUpdate()
    {

    }
    public void OnJumpEnter()
    {
        
    }
    public void OnDashExit()
    {
        _rigi.velocity = Vector2.zero;
        GetComponentInParent<AfterImage>().IsUpdate = false;

    }
     public void OnIdleEnter()
    {
        if (!_controller.isGround)
        {
            _controller._rigi.SetGravity(_controller.start_grivaty);
        }
        _controller.isMoveable = true;
        _controller.isInputable = true;

    }
    public void ResetTrigger(string _name)
    {
        _anim.ResetTrigger(_name);
    }
}
