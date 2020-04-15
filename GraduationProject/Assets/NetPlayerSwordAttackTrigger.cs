/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayerSwordAttackTrigger : BaseAttackTrigger
{
    [HideInInspector]
    public NetWorkActorController owner;

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if(collision.CompareTag("Player"))
        {
            // ActorModel.Model.SetEngery(ActorModel.Model.GetCurrentWeapon().回复能量);
            if (owner == collision.GetComponent<NetWorkActorController>())
            {
                return;
            }
            Debug.Log(collision.gameObject.name);
            var model = owner.GetModel();

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

            collision.GetComponent<NetWorkActorController>().GetHurt(new AttackData(hurt_value, isCrit, transform.position, this.attack_type));
        }
    }
}
