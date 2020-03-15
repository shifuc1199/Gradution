using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
using DG.Tweening;
public class CameraController : MonoBehaviour
{
    List<BaseEnemyController> Enemys = new List<BaseEnemyController>();
     
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy"))
        {
            var component = other.GetComponent<BaseEnemyController>();
            component.Lock();
            Enemys.Add(component);
        }
    }
    public void LockEnemy()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }
    public void UnLockEnemy()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        foreach (var item in Enemys)
        {
            item.UnLock();
        }
        Enemys.Clear();
    }

}
