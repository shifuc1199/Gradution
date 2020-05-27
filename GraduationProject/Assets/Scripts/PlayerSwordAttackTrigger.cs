using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using DG.Tweening;
public class PlayerSwordAttackTrigger : BaseAttackTrigger
{

   
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag=="Enemy")
        {
           
            ActorModel.Model.SetEngery(ActorModel.Model.GetCurrentWeapon().回复能量);
            
               View.CurrentScene.GetView<GameInfoView>().HitCount++;
           
            if(attack_type == HitType.击飞)
            {
                Camera.main.GetComponent< Cinemachine.CinemachineImpulseSource>().GenerateImpulse();
                if (View.CurrentScene is GameScene)
                {
                    TimeModel.SetTimeScale( 0.2f,true,0.2f);
                }
            }
            bool isCrit = false;
            var weapon = WeaponConfig.Get(ActorModel.Model.GetPlayerEquipment(EquipmentType.武器));
            var hurt_value = ActorModel.Model.GetPlayerAttribute(PlayerAttribute.攻击力);
            if (Random.value <= ActorModel.Model.GetPlayerAttribute(PlayerAttribute.暴击率)*0.01f)
            {
                isCrit = true;
                hurt_value = ActorModel.Model.GetPlayerAttribute(PlayerAttribute.暴击伤害);
            }
             collision.gameObject.GetComponent<IHurt>().GetHurt(
                  new AttackData(hurt_value, isCrit, ActorController.Controller.transform.position, attack_type)
              );
           
        }
    }
 
    // Update is called once per frame
    void Update()
    {
        
        
    }

   
}
