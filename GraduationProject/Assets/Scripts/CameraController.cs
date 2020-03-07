using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
using DG.Tweening;
public class CameraController : MonoBehaviour
{
     
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<BaseEnemyController>().IsLocked();
             
        }
    }
    public void LockEnemy()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }
 
}
