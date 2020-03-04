using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
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
    public void OnFallExit()
    {
        GameObjectPoolManager.GetPool("dust_ground").Get(transform.position+new Vector3(0,-3,0), Quaternion.identity, 1);
    }
    public void SetInputableFalse()
    {
        _controller.actor_state.isInputable = false;
        
    }
    public void OnDashEnter()
    {
        AudioManager.Instance.PlayOneShot("dash");
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
 