using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
using DG.Tweening;
using DreamerTool.Extra;
using DreamerTool.ScriptableObject;
public class SwordActorAnimationEvent : BaseActorAnimationEvent
{
 
    List<int> effect_rotation = new List<int>() { 45, 130, 60,0};
 
    private void Awake()
    {
 
    }
    public void SetPickUpSlash()
    {
        var temp = GameObjectPoolManager.GetPool("pick_up_slash").Get(transform.position + new Vector3(0, 2, -5), Quaternion.Euler(-45, 90 * transform.right.x, 180),0.5f);
        temp.GetComponentInChildren<SwordAttackTrigger>().attack_type = HitType.上挑;
        temp.transform.parent = transform.parent.parent;
        temp.transform.localScale = Vector3.one * 2f;
    }
    public void SetSkill1()
    {
        GameObjectPoolManager.GetPool("skill_1").Get(transform.position + transform.right * 2+new Vector3(0,-1.5f,0), Quaternion.Euler(new Vector3(-Quaternion.FromToRotation(Vector2.right, _controller.skill_controller.SkillDirection).eulerAngles.z,90,0)), 3);
    }
    public void SetSlash(int index)
    {
        if (_controller.actor_state.isGround)
        {
            _rigi.ResetVelocity();
            _rigi.AddForce(transform.right * 5, ForceMode2D.Impulse);
        }
        GameObject temp;
        temp = GameObjectPoolManager.GetPool("sword_slash").Get(transform.position + new Vector3(0, 2, 0), Quaternion.Euler(transform.eulerAngles.y, 90, transform.eulerAngles.y + effect_rotation[index]),0.35f);

        if (index == 3)
        {
            temp.GetComponentInChildren<SwordAttackTrigger>().attack_type = HitType.击飞;
            temp.GetComponent<AudioSource>().PlayOneShot(ScriptableObjectUtil.GetScriptableObject<AudioClips>().GetClip("player_heavy_attack"));
        }
        else
        {
            temp.GetComponent<AudioSource>().PlayOneShot(ScriptableObjectUtil.GetScriptableObject<AudioClips>().GetClip("player_common_attack"));
            temp.GetComponentInChildren<SwordAttackTrigger>().attack_type = HitType.击退;
        }

    }
    public void SetHeavySlash()
    {
        GameObject temp;
 
        temp = GameObjectPoolManager.GetPool("heavy_sword_slash").Get(transform.position + new Vector3(0, 2, 0), Quaternion.Euler(new Vector3(180-Quaternion.FromToRotation(Vector2.right, _controller.skill_controller.SkillDirection).eulerAngles.z,90,0)), 0.35f);
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
        _rigi.velocity = Vector2.up * 85; 
    }

    public void OnAttackExit()
    {

    }

}

