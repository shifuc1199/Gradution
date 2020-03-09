/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class PlayerSkillAttackTrigger : BaseAttackTrigger
{
    public int skill_id;
    SkillModel model;
    private void Awake()
    {
        model = SkillModel.Get(skill_id);
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            (View.CurrentScene as GameScene).HitCount++;
            if (attack_type == HitType.击飞)
            {

                Camera.main.GetComponent<Cinemachine.CinemachineImpulseSource>().GenerateImpulse();
                Time.timeScale = 0.2f;
            }
            collision.gameObject.GetComponent<IHurt>().GetHurt(model.GetHurtValue(), attack_type,transform.position);
        }
    }
}
