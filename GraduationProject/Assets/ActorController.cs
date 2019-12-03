using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.FSM;
public class ActorController : MonoBehaviour
{
    public KeyCode attack_key = KeyCode.Mouse0;
    public KeyCode right_move_key = KeyCode.D;
    public KeyCode left_move_key = KeyCode.A;
    public KeyCode jump_key = KeyCode.Space;
    public KeyCode dash_key = KeyCode.LeftShift;
    public KeyCode heavy_attack_key = KeyCode.Mouse1;
    public float move_speed;
    private Animator _anim;
    private Rigidbody2D _rigi;
    private void Awake()
    {
        _rigi = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
    }
    public virtual void Move()
    {
         
        if (Input.GetKey(right_move_key))
        {
            
            transform.rotation = Quaternion.identity;
            transform.Translate(Vector2.right * move_speed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(left_move_key))
        {
            
            transform.rotation = Quaternion.Euler(0, 180, 0);
            transform.Translate(Vector2.left * move_speed * Time.deltaTime, Space.World);
        }   
    }
    public  void Attack()
    {
        if (Input.GetKeyDown(attack_key))
        {
        _anim.SetTrigger ("attack");
        }
        if(Input.GetKeyDown(heavy_attack_key))
        {
            _anim.SetTrigger("heavyattack");
        }
    }
    
    public  void Jump()
    {
        if(Input.GetKeyDown(jump_key))
        {
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
    public   void Update()
    {
        Move();
        Attack();
        Jump();
        Dash();
    }

}
