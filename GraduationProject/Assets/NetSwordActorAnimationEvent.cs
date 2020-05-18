using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
using DG.Tweening;
using DreamerTool.Extra;
using DreamerTool.UI;

using Photon.Pun;
using DreamerTool.ScriptableObject;
public class NetSwordActorAnimationEvent : BaseActorAnimationEvent
{
    bool isMine;
    public float attack_forward_distance;
    List<int> effect_rotation = new List<int>() { 45, 130, 55, 0 };

    private void Start()
    {
        isMine = (_controller as NetkActorController).photonView.IsMine;
    }
    public void SetPickUpSlash()
    {
        if (isMine)
        {
            AudioManager.Instance.PlayOneShot("player_heavy_attack");
            PhotonNetwork.Instantiate("Net/net_pick_up_slash", transform.position + new Vector3(0, 2, -5), Quaternion.Euler(-45, 90 * transform.right.x, 180),0 ,new object[] { HitType.上挑  });
        }
    }
    public void SetSkill1()
    {
        if (isMine)
             PhotonNetwork.Instantiate("Net/skill_1", transform.position + transform.right * 2 + new Vector3(0, -1.5f, 0), Quaternion.Euler(new Vector3(-Quaternion.FromToRotation(Vector2.right, _controller.skill_controller.SkillDirection).eulerAngles.z, 90, 0)),0,new object[] { 2});
    }
    public void SetSlash(int index)
    {
        if (isMine)
        {
            if (_controller.actor_state.isGround)
            {
                _rigi.ResetVelocity();
                _rigi.AddForce(transform.right * attack_forward_distance, ForceMode2D.Impulse);
            }
            
            var temp = PhotonNetwork.Instantiate("Net/net_sword_slash", transform.position + new Vector3(0, 2, 0), Quaternion.Euler(transform.eulerAngles.y, 90, transform.eulerAngles.y + effect_rotation[index]), 0,new object[] { index==3? HitType.击飞: HitType.击退 });

           
        }
        if (index == 3)
        {

            AudioManager.Instance.PlayOneShot("player_heavy_attack");
        }
        else
        {
            AudioManager.Instance.PlayOneShot("player_common_attack");

        }

    }
    public void SetHeavySlash()
    {
        if (isMine)
        {
            PhotonNetwork.Instantiate("Net/net_heavy_sword_slash", transform.position + new Vector3(0, 1.5f, 0), Quaternion.Euler(180 - transform.eulerAngles.y, 90, 0), 0, new object[] { 0.5f });
        }
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
        if (isMine)
        {
            PhotonNetwork.Instantiate("Net/net_blackhole", ActorController.Controller.skill_controller.SkillPos, Quaternion.identity, 0,new object[] { 3});
        }
    }
    public void OnHeavyAttackEnter()
    {
        _controller._rigi.ResetVelocity();
        _controller._rigi.ClearGravity();
        _controller.actor_state.isInputable = false;
    }
    public void HeavyAttackUpdate()
    {
        if (isMine && GetComponentInParent<AfterImage>().IsUpdate)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 12, LayerMask.GetMask("enemy"));
            RaycastHit2D hit1 = Physics2D.Raycast(transform.position, transform.right + new Vector3(0, 0.25f, 0), 12, LayerMask.GetMask("enemy"));
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position, transform.right + new Vector3(0, -0.25f, 0), 12, LayerMask.GetMask("enemy"));
            NetkActorController enemy = null;
            NetkActorController enemy1 = null;
            NetkActorController enemy2 = null;
            if (hit.collider)
                enemy = hit.collider.GetComponent<NetkActorController>();
            if (hit1.collider)
                enemy1 = hit1.collider.GetComponent<NetkActorController>();
            if (hit2.collider)
                enemy2 = hit2.collider.GetComponent<NetkActorController>();

            if (enemy || enemy1 || enemy2)
            {
                ActorController.Controller.transform.SetPositionY((enemy ? enemy : (enemy1 ? enemy1 : enemy2)).transform.position.y);

                _rigi.velocity = Vector2.zero;
                _anim.SetTrigger("skill2_dash");
            }

        }
    }
    public void HeavyAttackMove()
    {
        if(isMine)
            _rigi.velocity = _controller.skill_controller.SkillDirection * 150;
        GetComponentInParent<AfterImage>().IsUpdate = true;
    }
    public void HeavyAttackReset()
    {
        GetComponentInParent<AfterImage>().IsUpdate = false;
        if (isMine)
            _rigi.velocity = Vector2.zero;


    }
    public void PickUpAttackJump()
    {
        _rigi.ResetVelocity();
        _rigi.AddForce(Vector2.up * 95, ForceMode2D.Impulse);
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

        if (isMine &&isMove)
        {

            ActorController.Controller.transform.position = Vector3.MoveTowards(ActorController.Controller.transform.position, Skill4_Pos, 2.5f);
            if (ActorController.Controller.actor_state.isGround)
            {
                isMove = false;
               PhotonNetwork.Instantiate("Net/net_skill4_effect", ActorController.Controller.transform.position + new Vector3(0, -4, 0), Quaternion.identity, 0,new object[] { 0.75f });
            }
        }
    }
    bool isMove;
    Vector3 Skill4_Pos;
    public void Skill4Dash()
    {
        if (isMine)
        {
            isMove = true;
        }
        GetComponentInParent<AfterImage>().IsUpdate = true;
    }
    public void Skill4Reset()
    {
        if (isMine)
        {
            _rigi.ResetVelocity();
        }
    }
    public void OnSkill4Enter()
    {
        if (isMine)
        {
            Skill4_Pos = ActorController.Controller.transform.position + transform.right * 40;
            _rigi.ResetVelocity();
            _rigi.ClearGravity();
            _rigi.velocity = Vector2.up * 100;
        }
    }
    bool isSkill5 = false;
    public GameObject separated_body;
    public void Skill5Excute()
    {
        if (isMine)
        {
            isSkill5 = true;
            Camera.main.GetComponent<CameraController>().LockEnemy();
            View.CurrentScene.GetView<GameInfoView>().SetScreenEffect(Color.black, 1);
        }
    }
    public void OnSkill5Enter()
    {
        if (isMine)
        {
            View.CurrentScene.GetView<GameInfoView>().HideAnim();
        }
    }
    public void OnSkill5Exit()
    {
        if (isMine)
        {
            View.CurrentScene.GetView<GameInfoView>().SetScreenEffect(Color.black, 0);
            Camera.main.GetComponent<CameraController>().UnLockEnemy();
            View.CurrentScene.GetView<GameInfoView>().ShowAnim();
            ActorController.Controller.gameObject.SetActive(true);
            isSkill5 = false;
            separated_body.SetActive(false);
        }
    }
    Vector2 pos = Vector2.zero;
    public void OnSkill5Stay()
    {
        if (isMine)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && isSkill5)
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100000, LayerMask.GetMask("enemy"));
                if (hit.collider)
                {
                    separated_body.SetActive(true);
                    var pos_list = new List<int>() { -1, 0, 1 };
                    var x = pos_list[Random.Range(0, pos_list.Count)];
                    var y = pos_list[Random.Range(0, pos_list.Count)];
                    while (x == y && x == 0)
                    {
                        x = pos_list[Random.Range(0, pos_list.Count)];
                    }
                    separated_body.transform.position = hit.collider.transform.position + new Vector3(x, y, 0).normalized * 18;
                    separated_body.transform.right = (hit.collider.transform.position - separated_body.transform.position).normalized;
                    pos = separated_body.transform.position + separated_body.transform.right * 36;
                    var heavy_sword_slash = PhotonNetwork.Instantiate("Net/net_heavy_sword_slash", separated_body.transform.position + separated_body.transform.right * 15, Quaternion.Euler(new Vector3(180 - Quaternion.FromToRotation(Vector2.right, separated_body.transform.right).eulerAngles.z, 90, 0)), 0, new object[] { 0.5f });
                    heavy_sword_slash.GetComponentInChildren<BaseAttackTrigger>().attack_type = HitType.普通;
                }
            }
            if ((Vector2)separated_body.transform.position == pos)
            {
                separated_body.SetActive(false);
            }
            separated_body.transform.position = Vector3.MoveTowards(separated_body.transform.position, pos, 4f);
        }
    }
    public void OnDownAttackEnter()
    {
        _rigi.ResetVelocity();
        _rigi.ClearGravity();
    }
    public void DownAttackDash()
    {
        _rigi.AddForce(Vector2.down * 100, ForceMode2D.Impulse);
    }
    public void OnDownAttackExit()
    {
        _rigi.ResetVelocity();
        _rigi.SetGravity(ActorController.Controller.start_grivaty);
        GameObjectPoolManager.GetPool("skill4_effect").Get(transform.position, Quaternion.identity, 0.5f);
    }
    public void OnSkill4Exit()
    {
        GetComponentInParent<AfterImage>().IsUpdate = false;
        isMove = false;
    }

}

