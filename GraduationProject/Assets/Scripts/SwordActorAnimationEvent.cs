using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
using DG.Tweening;
using DreamerTool.Extra;
public class SwordActorAnimationEvent : BaseActorAnimationEvent
{
    [Header("------------预制体----------")]
    public GameObject pickupslash_prefab;
    public GameObject sword_slash_prefab;
    public GameObject heavy_sword_slash_prefab;
    public GameObject skill_1_prefab;
    List<int> effect_rotation = new List<int>() { 45, 130, 60,0};
    GameObjectPool pick_up_slash_pool;
    GameObjectPool sword_slash_pool;
    GameObjectPool heavy_sword_slash_pool;
    GameObjectPool skill_1_pool;
    private void Awake()
    {
        pick_up_slash_pool = GameObjectPoolManager.AddPool("pick_up_slash_pool",pickupslash_prefab);
        sword_slash_pool =  GameObjectPoolManager.AddPool("sword_slash_pool", sword_slash_prefab);
        heavy_sword_slash_pool = GameObjectPoolManager.AddPool("heavy_sword_slash_pool", heavy_sword_slash_prefab);
        skill_1_pool = GameObjectPoolManager.AddPool("skill_1_pool", skill_1_prefab);
    }
    public void SetPickUpSlash()
    {
       var temp = pick_up_slash_pool.Get(transform.position + new Vector3(0, 2, -5), Quaternion.Euler(-45, 90 * transform.right.x, 180),0.5f);
        temp.GetComponentInChildren<SwordAttackTrigger>().attack_type = HitType.上挑;
        temp.transform.parent = transform.parent.parent;
        temp.transform.localScale = Vector3.one * 1.5f;
    }
    public void SetSkill1()
    {
        skill_1_pool.Get(transform.position + transform.right * 2+new Vector3(0,-1.5f,0), Quaternion.Euler(new Vector3(-Quaternion.FromToRotation(Vector2.right, _controller.skill_controller.SkillDirection).eulerAngles.z,90,0)), 3);
    }
    public void SetSlash(int index)
    {
        if (_controller.actor_state.isGround)
        {
            _rigi.ResetVelocity();
            _rigi.AddForce(transform.right * 5, ForceMode2D.Impulse);
        }
        GameObject temp;
        temp = sword_slash_pool.Get(transform.position + new Vector3(0, 2, 0), Quaternion.Euler(transform.eulerAngles.y, 90, transform.eulerAngles.y + effect_rotation[index]),0.25f);

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
 
        temp = heavy_sword_slash_pool.Get(transform.position + new Vector3(0, 2, 0), Quaternion.Euler(new Vector3(180-Quaternion.FromToRotation(Vector2.right, _controller.skill_controller.SkillDirection).eulerAngles.z,90,0)), 0.35f);
    }
    public void OnAttackEnter()
    { 
        if (!_controller.actor_state.isGround)
        {
             
            _controller._rigi.ResetVelocity();
            _controller._rigi.ClearGravity();
        }
        _controller.actor_state.isMoveable = false;

    }
    public void OnHeavyAttackEnter()
    {
        _controller._rigi.ResetVelocity();
        _controller._rigi.ClearGravity();
        _controller.actor_state.isInputable = false;
    }
 
    public void HeavyAttackMove()
    {
        _rigi.velocity = _controller.skill_controller.SkillDirection * 200;
        GetComponentInParent<AfterImage>().IsUpdate = true;
    }
    public void HeavyAttackReset()
    {
        _rigi.velocity = Vector2.zero;
        GetComponentInParent<AfterImage>().IsUpdate = false;
         
    }
    public void PickUpAttackJump()
    {
        _rigi.ResetVelocity();
        _rigi.velocity = Vector2.up * 100; 
    }

    public void OnAttackExit()
    {

    }

}

