/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
public class EnemyFireBall : EnemyAttackTrigger
{
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.tag == "Player")
        {
            GetComponent<ObjectRecover>().RecoverImmediately();
            GameObjectPoolManager.GetPool("fire_ball_explosion").Get(transform.position, Quaternion.Euler(-90, 0, 0),1);
        }

    }
}
