using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    public void ShootBullet() //远程攻击
    {
        Instantiate(Resources.Load<GameObject>("Bullet"), GetComponent<PlayerController>()._shoot_pos.position, transform.rotation);
    }

    public void OnAttackEnter()
    {

    }

    public void OnAttackExit()
    {
      
        GetComponent<PlayerState>().isInputable = true;
    }

}

