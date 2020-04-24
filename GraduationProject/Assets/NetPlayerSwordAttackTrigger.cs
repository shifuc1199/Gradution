/*****************************
Created by 师鸿博
*****************************/
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayerSwordAttackTrigger : BaseAttackTrigger
{
    private Photon.Pun.PhotonView photonView;
    ActorModel model;
    private void Awake()
    {
        Destroy(gameObject, 0.5f);
        photonView = GetComponentInParent<Photon.Pun.PhotonView>();
        this.attack_type =(HitType)photonView.InstantiationData[0];
 
        model = JsonMapper.ToObject<ActorModel>(photonView.Owner.CustomProperties["model"].ToString());
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if(collision.CompareTag("Player"))
        {
            // ActorModel.Model.SetEngery(ActorModel.Model.GetCurrentWeapon().回复能量);
            if (photonView.Owner.ActorNumber == collision.GetComponent<NetkActorController>().photonView.Owner.ActorNumber)
            {
                return;
            }
    

            if (attack_type == HitType.击飞)
            {
                Camera.main.GetComponent<Cinemachine.CinemachineImpulseSource>().GenerateImpulse();
            }
            bool isCrit = false;
            var weapon = WeaponConfig.Get(model.GetPlayerEquipment(EquipmentType.武器));
            var hurt_value = model.GetPlayerAttribute(PlayerAttribute.攻击力);
            if (Random.value <= model.GetPlayerAttribute(PlayerAttribute.暴击率))
            {
                isCrit = true;
                hurt_value = model.GetPlayerAttribute(PlayerAttribute.暴击伤害);
            }

            collision.GetComponent<NetkActorController>().GetHurt(new AttackData(hurt_value, isCrit, transform.position, this.attack_type));
        }
    }
}
