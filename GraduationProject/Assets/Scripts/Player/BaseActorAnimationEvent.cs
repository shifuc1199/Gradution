using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Extra;
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
        if (!_controller.actor_state.isGround)
        {

            _controller._rigi.ResetVelocity();
            _controller._rigi.ClearGravity();
        }
        _controller.actor_state.isInputable = false;
        _rigi.velocity = transform.right * 150;
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
        _controller.actor_state.isInputable = true;
        _rigi.velocity = Vector2.zero;
        GetComponentInParent<AfterImage>().IsUpdate = false;

    }
  
     public void OnIdleEnter()
    {

            _controller._rigi.SetGravity(_controller.start_grivaty);

        _controller.actor_state.isMoveable = true;
        _controller.actor_state.isInputable = true;

    }
    public void ResetTrigger(string _name)
    {
        _anim.ResetTrigger(_name);
 
   }
}
 