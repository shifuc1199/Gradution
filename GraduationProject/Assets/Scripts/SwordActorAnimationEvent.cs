using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
using DG.Tweening;
using DreamerTool.Extra;
using DreamerTool.ScriptableObject;
public class SwordActorAnimationEvent : BaseActorAnimationEvent
{
 
    List<int> effect_rotation = new List<int>() { 45, 130, 55,0};
 
    private void Awake()
    {
 
    }
    public void SetPickUpSlash()
    {
        AudioManager.Instance.PlayOneShot("player_heavy_attack");
        var temp = GameObjectPoolManager.GetPool("pick_up_slash").Get(transform.position + new Vector3(0, 2, -5), Quaternion.Euler(-45, 90 * transform.right.x, 180),0.5f);
        temp.GetComponentInChildren<PlayerSwordAttackTrigger>().attack_type = HitType.上挑;
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
        
        GameObject temp = GameObjectPoolManager.GetPool("sword_slash").Get(transform.position + new Vector3(0, 2, 0), Quaternion.Euler(transform.eulerAngles.y, 90, transform.eulerAngles.y + effect_rotation[index]),0.2f);

        if (index == 3)
        {
            temp.GetComponentInChildren<PlayerSwordAttackTrigger>().attack_type = HitType.击飞;
           AudioManager.Instance.PlayOneShot("player_heavy_attack");
        }
        else
        {
            AudioManager.Instance.PlayOneShot("player_common_attack");
            temp.GetComponentInChildren<PlayerSwordAttackTrigger>().attack_type = HitType.击退;
        }

    }
    public void SetHeavySlash()
    {
        GameObject temp;
 
        temp = GameObjectPoolManager.GetPool("heavy_sword_slash").Get(transform.position + new Vector3(0, 1.5f, 0), Quaternion.Euler(180-transform.eulerAngles.y,90,0), 0.5f);
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
    public void SetBlackHole()
    {
        GameObjectPoolManager.GetPool("blackhole").Get(ActorController._controller.skill_controller.SkillPos, Quaternion.identity, 3);
    }
    public void OnHeavyAttackEnter()
    {
        _controller._rigi.ResetVelocity();
        _controller._rigi.ClearGravity();
        _controller.actor_state.isInputable = false;
    }
    public void HeavyAttackUpdate()
    {
        if (GetComponentInParent<AfterImage>().IsUpdate)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 12, LayerMask.GetMask("enemy"));
            RaycastHit2D hit1 = Physics2D.Raycast(transform.position, transform.right+new Vector3(0,0.25f,0), 12, LayerMask.GetMask("enemy"));
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position, transform.right + new Vector3(0, -0.25f, 0), 12, LayerMask.GetMask("enemy"));
            BaseEnemyController enemy=null;
            BaseEnemyController enemy1= null;
            BaseEnemyController enemy2= null;
            if (hit.collider)
                  enemy = hit.collider.GetComponent<BaseEnemyController>();
            if (hit1.collider)
                  enemy1 = hit1.collider.GetComponent<BaseEnemyController>();
            if (hit2.collider)
                  enemy2 = hit2.collider.GetComponent<BaseEnemyController>();
          
            if (enemy || enemy1 || enemy2)
            {
                ActorController._controller.transform.SetPositionY((enemy ? enemy : (enemy1 ? enemy1 : enemy2)).transform.position.y);

                _rigi.velocity = Vector2.zero;
                _anim.SetTrigger("skill2_dash");
            }
           
        }
    }
    public void HeavyAttackMove()
    {
        _rigi.velocity = _controller.skill_controller.SkillDirection * 200;
        GetComponentInParent<AfterImage>().IsUpdate = true;
    }
    public void HeavyAttackReset()
    {
        GetComponentInParent<AfterImage>().IsUpdate = false;
        _rigi.velocity = Vector2.zero;
        
         
    }
    public void PickUpAttackJump()
    {
        _rigi.ResetVelocity();
        _rigi.AddForce(Vector2.up*ActorController._controller.jump_speed,ForceMode2D.Impulse);
        _anim.ResetTrigger("pickupattack");
    }
    public void OnSkill2Exit()
    {
        GetComponentInParent<AfterImage>().IsUpdate = false;
        
    }
    public void OnAttackExit()
    {

    }
    public void OnSkill4Stay()
    {
        
        if (isMove)
        {
             
            ActorController._controller.transform.position  = Vector3.MoveTowards(ActorController._controller.transform.position , Skill4_Pos, 2.5f);
            if(ActorController._controller.actor_state.isGround)
            {
                isMove = false;
                GameObjectPoolManager.GetPool("skill4_effect").Get(ActorController._controller.transform.position+new Vector3(0,-4,0), Quaternion.Euler(-90,0,0), 0.75f);
            }
        }
    }
    bool isMove;
    Vector3 Skill4_Pos;
    public void Skill4Dash()
    {
        isMove = true;
        GetComponentInParent<AfterImage>().IsUpdate = true;
    }
    public void Skill4Reset()
    {
        _rigi.ResetVelocity();
    }
    public void OnSkill4Enter()
    {
        Skill4_Pos = ActorController._controller.transform.position + transform.right *40;
        _rigi.ResetVelocity();
        _rigi.ClearGravity();
        _rigi.velocity = Vector2.up * 100;
    }
    public void OnSkill4Exit()
    {
        GetComponentInParent<AfterImage>().IsUpdate = false;
        isMove = false;
    }

}

