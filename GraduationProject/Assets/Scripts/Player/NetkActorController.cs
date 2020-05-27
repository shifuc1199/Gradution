/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using LitJson;
using DreamerTool.GameObjectPool;
using DreamerTool.Extra;
using UnityEngine.Events;

public class NetkActorController : ActorController 
{
    private FileDataActor _actor;
    public GameObject[] hide_gameObject;
    [HideInInspector]
    public PhotonView photonView;
    public TextMesh nameText;
    public GameObject circle;
    private ActorModel _model;

    public GameObject shieldEffect;
    public ActorModel GetModel()
    {
        return _model;
    }
 
    public new void Start()
    {
        _actor = GetComponent<FileDataActor>();
        photonView = GetComponent<PhotonView>();
        
        if (photonView.IsMine)
        {
            Controller = this;
            _rigi.bodyType = RigidbodyType2D.Dynamic;

        }
        else
        {
   
            foreach (var gam in hide_gameObject)
            {
                gam.SetActive(false);
            }
            tag = "Enemy";
            gameObject.layer = LayerMask.NameToLayer("enemy");
        }
        SetModel( JsonMapper.ToObject<ActorModel>(photonView.InstantiationData[0].ToString()));
    }
    public void SetModel(ActorModel model)
    {
        _model = model;
        _actor.SetModel(model);
       
        DreamerTool.UI.View.CurrentScene.GetView<NetWorkGameInfoView>().huds[(int)photonView.Owner.CustomProperties["number"]].SetModel(model);
        nameText.text =DreamerTool.Util.DreamerUtil.GetColorRichText( model.actor_name, photonView.IsMine?Color.white:Color.red);
        circle.SetActive(photonView.IsMine);
    }
    [PunRPC]
    private void RPCGetHurt(string dataJson)
    {
        var attackData = JsonMapper.ToObject<AttackData>(dataJson);
        GetHurt(attackData);
    }
 
    public override void GetHurt(AttackData attackData)
    {
        if(!photonView.IsMine)
            photonView.RPC("RPCGetHurt", RpcTarget.Others,JsonMapper.ToJson(attackData));

        if (shieldEffect.activeSelf && (attackData.attack_pos - transform.position).normalized.x * transform.right.x > 0)
        {
            _rigi.ResetVelocity();
            _rigi.AddForce(-transform.right * 20, ForceMode2D.Impulse);
            GameObjectPoolManager.GetPool("metal_hit").Get(transform.position + transform.right * 2, Quaternion.identity, 1.5f);
            return;
        }

        _anim.SetTrigger("hit");
        GameStaticMethod.ChangeChildrenSpriteRendererColor(gameObject, Color.red);
        transform.rotation = attackData.attack_pos.x > transform.position.x ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        GameObjectPoolManager.GetPool("hit_effect").Get(transform.position + new Vector3(0, 2, 0), Quaternion.identity, 0.5f);

 
       _model.SetHealth(-DreamerTool.Util.DreamerUtil.GetHurtValue(attackData.hurt_value, _model.GetPlayerAttribute(PlayerAttribute.物防)));
 

        switch (attackData.attack_type)
        {
            case HitType.普通:
                break;
            case HitType.击退:
                if (!actor_state.isGround)
                {
                    _rigi.ResetVelocity();
                    _rigi.ClearGravity();
                }
                else
                {
                    _rigi.ResetVelocity();
                    _rigi.AddForce(-transform.right * 20, ForceMode2D.Impulse);
                }
                break;
            case HitType.击飞:
                _rigi.ResetVelocity();
                _rigi.AddForce(new Vector2(-transform.right.x * 0.5f, 0.5f).normalized * 50, ForceMode2D.Impulse);
                break;
            case HitType.上挑:
                _rigi.ResetVelocity();
                _rigi.AddForce(transform.up * 95, ForceMode2D.Impulse);
                break;
            default:
                break;
        }
  
}
    [PunRPC]
    public void PlayAttackAnim()
    {
        _anim.SetTrigger("attack");
    }
    public override void Attack()
    {
        if (actor_state.isAttack)
        {
            if (actor_state.isAttackUp && actor_state.isGround)
            {
                _anim.SetTrigger("pickupattack");
            }
            else if (actor_state.isAttackDown && !actor_state.isGround)
            {
                _anim.SetTrigger("downattack");
            }
            else
                _anim.SetTrigger("attack");

            photonView.RPC("PlayAttackAnim", RpcTarget.Others);
            actor_state.isAttack = false;

        }
        
    }
    GameObject temp_aim;
    public void Lock()
    {
        temp_aim = GameObjectPoolManager.GetPool("aim").Get(transform.position, Quaternion.identity, -1);

    }

    public void UnLock()
    {
        temp_aim.GetComponent<ObjectRecover>().RecoverImmediately();

    }
    public new void Update()
    {
        if (!photonView.IsMine)
        {
            StateCheck();
            return;
        }
        base.Update();
    }
    public new void FixedUpdate()
    {
       
        if (!photonView.IsMine)
        {
            
            return;
        }
            
        base.FixedUpdate();
    }
 
}
