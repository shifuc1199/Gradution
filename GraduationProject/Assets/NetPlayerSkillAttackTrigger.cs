/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using DreamerTool.UI;
using LitJson;

public class NetPlayerSkillAttackTrigger : BaseAttackTrigger
{
    private PhotonView photonView;
    public int skill_id;
    SkillModel model;
    private void Awake()
    {
        photonView = GetComponentInParent<PhotonView>();
        Destroy(transform.parent.gameObject,(float)photonView.InstantiationData[0]);
        model = (JsonMapper.ToObject<ActorModel>(photonView.Owner.CustomProperties["model"].ToString())).GetSkillModel(skill_id);
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (photonView.Owner.ActorNumber == collision.GetComponent<NetkActorController>().photonView.Owner.ActorNumber)
                return;
            if (attack_type == HitType.击飞)
            {

                Camera.main.GetComponent<Cinemachine.CinemachineImpulseSource>().GenerateImpulse();
              //  Time.timeScale = 0.2f;
            }
            collision.gameObject.GetComponent<IHurt>().GetHurt(
                new AttackData(model.GetHurtValue(), false, transform.position, attack_type)
                );
        }
    }
}
