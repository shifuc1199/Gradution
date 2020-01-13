/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
public static class GameObjectPoolManager  
{
    private static Dictionary<string, GameObjectPool> pools = new Dictionary<string, GameObjectPool>();

    public static GameObjectPool AddPool(string pool_id,GameObject prefab)
    {
        if(pools.ContainsKey(pool_id))
        {
            return null;
        }
        var pool = new GameObjectPool(prefab);
        pools.Add(pool_id, pool);
        return pool;
    }

    public static GameObjectPool GetPool(string pool_id)
    {
        return pools[pool_id];
    }
}
