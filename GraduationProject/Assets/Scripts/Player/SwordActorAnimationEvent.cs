using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SwordActorAnimationEvent : BaseActorAnimationEvent
{
    [Header("------------预制体----------")]
    public GameObject pickupslash_prefab;
    public GameObject sword_slash_prefab;
    public GameObject heavy_sword_slash_prefab;

    public GameObject attack_trigger;
    public GameObject heavy_attack_trigger;
    
    List<int> effect_rotation = new List<int>() { 45, 130, 60,0};

    private void Awake()
    {
         
    }
    public void SetPickUpSlash()
    {
      var temp =  Instantiate(pickupslash_prefab, transform.position + new Vector3(0, 2, 0), Quaternion.Euler(-45, 90, 180));
        temp.transform.position += new Vector3(0, 0, -5);
        attack_trigger.GetComponent<Sword>().attack_type = HitType.上挑;
    }
    public void SetSlash(int index)
    {
        if (_controller.isGround)
        {
            _rigi.ResetVelocity();
            _rigi.AddForce(transform.right * 6, ForceMode2D.Impulse);
        }
        GameObject temp;
 
        temp = Instantiate(sword_slash_prefab, transform.position+new Vector3(0,2,0), Quaternion.Euler(transform.eulerAngles.y,90, transform.eulerAngles.y+ effect_rotation[index]));
       
        temp.transform.position += new Vector3(0, 0, -index);

        if (index == 3)
        {
            attack_trigger.GetComponent<Sword>().attack_type = HitType.击飞;
        }
       

        // Destroy(temp, 2);
    }
    public void SetHeavySlash(int index)
    {
        GameObject temp;

        temp = Instantiate(heavy_sword_slash_prefab, transform.position + new Vector3(0, 2, 0), Quaternion.Euler(180-transform.eulerAngles.y, 90, 0));

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
    public void SetHeavyAttackTriggerActive()
    {
        heavy_attack_trigger.SetActive(true);
    }
    public void SetHeavyAttackTriggerDeActive()
    {
        heavy_attack_trigger.SetActive(false);
    }
     
    public void OnAttackEnter()
    { 
        if (!_controller.isGround)
        {
             
            _controller._rigi.ResetVelocity();
            _controller._rigi.ClearGravity();
        }
        _controller.isMoveable = false;

    }
    
    public void OnHeavyAttackEnter()
    {
        _rigi.velocity = transform.right * 200;
        GetComponentInParent<AfterImage>().IsUpdate = true;
    }
    public void OnHeavyAttackExit()
    {
        _rigi.velocity = Vector2.zero;
        GetComponentInParent<AfterImage>().IsUpdate = false;
    }

    public void OnAttackExit()
    {

        attack_trigger.GetComponent<Sword>().attack_type = HitType.击退;
    }

}

