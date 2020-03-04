/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
public class WicherSkeletonAnimationEvent : BaseEnemyAnimationEvent
{
    public void FireAttack() //发射出6个不同方向的火球不追踪玩家
    {
        for (int i = 0; i < 6; i++)
        {
            var frie_ball = GameObjectPoolManager.GetPool("fire_ball_move").Get(transform.position+new Vector3(0,5,0),Quaternion.Euler(0,0, i * 60),3);
            
        }
      
    }

    public void FireAttackOnce() // 发射出一个火球 追踪玩家
    {
       var fireball =  GameObjectPoolManager.GetPool("fire_ball_follow").Get(transform.position + new Vector3(0, 11, 0), Quaternion.identity, 3);
        fireball.GetComponent<AutoMoveObjectByDirection>().Direction = ActorController._controller.transform.position-fireball.transform.position;
    }
}
