using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SwordAttackTrigger : BaseAttackTrigger
{

   
    public override void OnTriggerEnter2D(Collider2D collision)
    {   
        if(collision.gameObject.tag=="Enemy")
        {
            GameScene._instance.HitCount++;
            if(attack_type == HitType.击飞)
            {
                
                Camera.main.GetComponent< Cinemachine.CinemachineImpulseSource>().GenerateImpulse();
                Time.timeScale = 0.2f;
            }
            collision.gameObject.GetComponent<IHurt>().GetHurt(attack_type,()=> {
                collision.gameObject.transform.rotation = ActorController._controller.transform.rotation;
            });
        }
    }
 
    // Update is called once per frame
    void Update()
    {
        
        
    }

   
}
