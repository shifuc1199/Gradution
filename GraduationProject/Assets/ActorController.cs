using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.FSM;
public class ActorController : MonoBehaviour
{
    public static ActorController _controller;
    public Transform ground_check_pos;
    public bool isMoveable = true;
    public bool isInputable = true;
    public bool isGround = true;
    public KeyCode attack_key = KeyCode.Mouse0;
    public KeyCode jump_key = KeyCode.Space;
    public KeyCode dash_key = KeyCode.LeftShift;
    public KeyCode heavy_attack_key = KeyCode.Mouse1;
    
    public float move_speed;
    [System.NonSerialized] public Animator _anim;
    [System.NonSerialized] public Rigidbody2D _rigi;
    [System.NonSerialized] public float start_grivaty;
    private void Awake()
    {
        _controller = this;
        _rigi = GetComponent<Rigidbody2D>();
        start_grivaty = _rigi.gravityScale;
        _anim = GetComponentInChildren<Animator>();
  
    }
    public void Move()
    {
        if (!isMoveable)
        {
            _anim.SetBool("run", false);
            return;
        }
        var h = Input.GetAxisRaw("Horizontal");
        if (h != 0)
        {
            transform.rotation = Quaternion.Euler(0, h > 0 ? 0 : 180, 0);
        }
        _anim.SetBool("run", h != 0);
        transform.Translate(new Vector2(h,0) * move_speed * Time.deltaTime, Space.World);
         
    }
    public void Attack()
    {
        var v = Input.GetAxisRaw("Vertical");
        if (Input.GetKeyDown(attack_key))
        {
            if(v>0)
             _anim.SetTrigger("pickupattack");
            else
            _anim.SetTrigger("attack");
             
        }
        if(Input.GetKeyDown(heavy_attack_key))
        {
            _anim.SetTrigger("heavyattack");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            _anim.SetTrigger("skill1");
        }
    }
    
    public  void Jump()
    {
        if(Input.GetKeyDown(jump_key)&&isGround)
        {
            _rigi.ResetVelocity();
            _rigi.velocity = Vector2.up * 100;
            _anim.SetTrigger("jump");

        }
    }
    public void Dash()
    {
        if(Input.GetKeyDown(dash_key))
        {
            _anim.SetTrigger("dash");
        }
    }
    public void StateCheck()
    {
         
        isGround = Physics2D.OverlapCircle(ground_check_pos.position, 1,LayerMask.GetMask("Ground"));
       
    }
    public   void Update()
    {
        if (!isInputable)
            return;
        Attack();
        StateCheck();
     
        Move();
         
        Jump();
        Dash();
    }

}
