using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerAnimationEvent : MonoBehaviour
{
    public GameObject sword_slash;
    
    public void ShootBullet() //远程攻击
    {
        Instantiate(Resources.Load<GameObject>("Bullet"), GetComponent<PlayerController>()._shoot_pos.position, transform.rotation);
    }
    public void SetSlash()
    {
        sword_slash.SetActive(true);
    }
    public void OnAttackEnter()
    {

    }

    public void OnAttackExit()
    {
      
        GetComponent<PlayerState>().isInputable = true;
    }

}

