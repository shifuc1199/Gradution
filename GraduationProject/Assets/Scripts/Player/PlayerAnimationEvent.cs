using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerAnimationEvent : MonoBehaviour
{
    public GameObject sword_slash;
    public GameObject Huan_Ying;
    public void ShootBullet() //远程攻击
    {
        Instantiate(Resources.Load<GameObject>("Bullet"), GetComponent<PlayerController>()._shoot_pos.position, transform.rotation);
    }
    public void SetSlash()
    {
        transform.DOMoveX(transform.right.x * 8 + transform.position.x, 0.1f).SetEase(Ease.Linear);
        sword_slash.SetActive(true);
        Huan_Ying.SetActive(true);
        Timer.Register(0.6f, () => { Huan_Ying.SetActive(false); sword_slash.SetActive(false); });
    }
    public void OnAttackEnter()
    {

    }

    public void OnAttackExit()
    {
      
        GetComponent<PlayerState>().isInputable = true;
    }

}

