using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorBase :MonoBehaviour
{
    public KeyCode attack_key = KeyCode.Mouse0;
    public KeyCode right_move_key = KeyCode.D;
    public KeyCode left_move_key = KeyCode.A;
    public KeyCode jump_key = KeyCode.Space;

    public float move_speed;
    public virtual void Move()
    {
        if (Input.GetKey(right_move_key))
        {
            transform.rotation = Quaternion.identity;
            transform.Translate(Vector2.right * move_speed * Time.deltaTime,Space.World);
        }
        if (Input.GetKey(left_move_key))
        {
            transform.rotation = Quaternion.Euler(0,180,0);
            transform.Translate(Vector2.left * move_speed * Time.deltaTime,Space.World);
        }
    }
    public abstract void Attack();
    public abstract void Jump();
    public virtual void Update()
    {
        Move();
        Attack();
        Jump();
    }

}
