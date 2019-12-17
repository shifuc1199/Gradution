using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
using DG.Tweening;
public class SwordActorAnimationEvent : BaseActorAnimationEvent
{
    [Header("------------预制体----------")]
    public GameObject pickupslash_prefab;
    public GameObject sword_slash_prefab;
    public GameObject heavy_sword_slash_prefab;
    public GameObject skill_1_prefab;
 
    
    List<int> effect_rotation = new List<int>() { 45, 130, 60,0};
    BaseGameObjectPool pick_up_slash_pool;
    BaseGameObjectPool sword_slash_pool;
    BaseGameObjectPool heavy_sword_slash_pool;
    BaseGameObjectPool skill_1_pool;
    private void Awake()
    {
        pick_up_slash_pool = new BaseGameObjectPool(pickupslash_prefab);
        sword_slash_pool = new BaseGameObjectPool(sword_slash_prefab);
        heavy_sword_slash_pool = new BaseGameObjectPool(heavy_sword_slash_prefab);
        skill_1_pool = new BaseGameObjectPool(skill_1_prefab);

        skill.ReleaseSkillEvent += SetHeavyAttackDir;
    }
    public void SetPickUpSlash()
    {
       var temp = pick_up_slash_pool.Get(transform.position + new Vector3(0, 2, -5), Quaternion.Euler(-45, 90 * transform.right.x, 180),0.5f);
        temp.GetComponentInChildren<SwordAttackTrigger>().attack_type = HitType.上挑;
    }
    public void SetSkill1()
    {
        skill_1_pool.Get(transform.position + transform.right * 2+new Vector3(0,-1.5f,0), Quaternion.Euler(transform.eulerAngles.y,90,0), 3);
    }
    public void SetSlash(int index)
    {
        if (_controller.isGround)
        {
            _rigi.ResetVelocity();
            _rigi.AddForce(transform.right * 5, ForceMode2D.Impulse);
        }
        GameObject temp;
        temp = sword_slash_pool.Get(transform.position + new Vector3(0, 2, -index), Quaternion.Euler(transform.eulerAngles.y, 90, transform.eulerAngles.y + effect_rotation[index]),0.5f);

        if (index == 3)
        {
            temp.GetComponentInChildren<SwordAttackTrigger>().attack_type = HitType.击飞;
        }
        else
        {
            temp.GetComponentInChildren<SwordAttackTrigger>().attack_type = HitType.击退;
        }

    }
    public void SetHeavySlash()
    {
        GameObject temp;
 
     
        temp = heavy_sword_slash_pool.Get(transform.position + new Vector3(0, 2, 0), Quaternion.Euler(new Vector3(180-Quaternion.FromToRotation(Vector2.right, HeavyAttackDirection).eulerAngles.z,90,0)), 0.35f);
    }
    public void OnSkill2Enter()
    {
       // GetComponentInParent<AfterImage>().IsUpdate = true;
    }
    public void OnSkill2Exit()
    {
      //  GetComponentInParent<AfterImage>().IsUpdate = false;
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
        _controller._rigi.ResetVelocity();
        _controller._rigi.ClearGravity();
        _controller.isInputable = false;
    }
    public Vector2 HeavyAttackDirection;
    public SkillJoyStick skill;
    public void SetHeavyAttackDir(Vector2 v)
    {
        HeavyAttackDirection = v;
        _anim.SetTrigger("heavyattack");
        if(HeavyAttackDirection.x>0)
        {
            _controller.transform.rotation = Quaternion.identity;
        }
        else if (HeavyAttackDirection.x < 0)
        {
            _controller.transform.rotation = Quaternion.Euler(0,180,0);
        }
    }
    public void HeavyAttackMove()
    {
        _rigi.velocity = HeavyAttackDirection * 200;
        GetComponentInParent<AfterImage>().IsUpdate = true;
    }
    public void HeavyAttackReset()
    {
        _rigi.velocity = Vector2.zero;
        GetComponentInParent<AfterImage>().IsUpdate = false;
         
    }

    public void OnAttackExit()
    {

    }

}

