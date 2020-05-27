/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DreamerTool.GameObjectPool;
public class EnemySpawn : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public bool SpawnEnemy(int level, int spawnEnemyID,UnityAction dieCallBack = null)
    {
        var config = EnemyConfig.Get(spawnEnemyID);
        if (config == null)
            return false;
        GameObjectPoolManager.GetPool("dust").Get(transform.position, Quaternion.identity, 1);
        Timer.Register(0.25f, () => {
            var enemy = Instantiate(config.GetGameObjectPrefab(), transform.position, Quaternion.identity);
            enemy.GetComponentInChildren<BaseEnemyController>().SetLevel(level);
            enemy.GetComponentInChildren<BaseEnemyController>().dieCallBack = dieCallBack;
        });
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
