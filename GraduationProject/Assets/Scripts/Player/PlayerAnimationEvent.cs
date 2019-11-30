using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerAnimationEvent : MonoBehaviour
{
    public GameObject sword_slash_prefab;
    public GameObject attack_trigger;
    private Rigidbody2D _rigi;
    List<int> effect_rotation = new List<int>() { 60, 110, 70, 120 };
    private void Start()
    {
        _rigi = GetComponentInParent<Rigidbody2D>();
    }
    public void ShootBullet() //远程攻击
    {
        GameObject temp = Instantiate(Resources.Load<GameObject>("Bullet"), GetComponent<PlayerController>()._shoot_pos.position, transform.rotation);
    }
    public void OnDashEnter()
    {
        _rigi.velocity = transform.right * 80;
        GetComponentInParent<AfterImage>().IsUpdate = true;
    }
    public void OnDashUpdate()
    {

    }
    public void OnDashExit()
    {
        _rigi.velocity = Vector2.zero;
        GetComponentInParent<AfterImage>().IsUpdate = false;

    }
    public void ResetTrigger(string _name)
    {
        GetComponent<Animator>().ResetTrigger(_name);
    }
    public void SetSlash(int index)
    {
        
         
        GameObject temp = Instantiate(sword_slash_prefab, transform.position+new Vector3(0,2,0), Quaternion.Euler(transform.eulerAngles.y,90, transform.eulerAngles.y+ effect_rotation[index]));
       
            temp.transform.position += new Vector3(0, 0, -index);
        
       // Destroy(temp, 2);
    }
    public void SetAttackTriggerActive()
    {
        attack_trigger.SetActive(true);
    }
    public void SetAttackTriggerDeactive()
    {
        attack_trigger.SetActive(false);
    }
    public void OnAttackEnter()
    {

    }

    public void OnAttackExit()
    {
         
         
    }

}

