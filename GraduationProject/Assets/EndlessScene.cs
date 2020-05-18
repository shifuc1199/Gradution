/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using DreamerTool.GameObjectPool;
public class EndlessScene : Scene
{
    public static int level;
    EnemySpawn[] enemySpawners;
    public int enemyAmount;
    private new void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        if (ActorModel.Model == null)
            ActorModel.CreateModel();
        GameStaticMethod.GameInit();
#endif

        enemySpawners = GetComponentsInChildren<EnemySpawn>();
        SpawnEnemys();

        GetView<GameInfoView>().SetTipText("第" + level + "层");
    }
    private void OnDestroy()
    {
        ActorModel.Model.SetTowerLevel(level);
    }
    public void SpawnEnemys()
    {
        
        var enemys = GameStaticData.towerEnemys[(level-1) % GameStaticData.towerEnemys .Count];
        
        for (int i = 0; i < enemys.Count; i++)
        {
            if(enemySpawners[i].SpawnEnemy(level, enemys[i],() =>
            {
                enemyAmount--;
                if (enemyAmount == 0)
                {
                    level++;
                     
                    GetView<GameInfoView>().SetTipText("第" + level + "层");
                    SpawnEnemys();
                }
            }))
            {
                enemyAmount++;
            }
        }
        
    }
}
