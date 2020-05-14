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
    public int level;
    public EnemySpawn[] enemySpawners;
    public int enemyAmount;
    private new void Awake()
    {
        base.Awake();
        if (ActorModel.Model == null)
            ActorModel.CreateModel();
        GameStaticMethod.GameInit();

        SpawnEnemys();

        GetView<GameInfoView>().SetTipText("第" + level + "层");
    }
    public void SpawnEnemys()
    {
        enemyAmount = enemySpawners.Length;
        foreach (var enemySpawn in enemySpawners)
        {
            enemySpawn.SpawnEnemy(level, () => {
                enemyAmount--;
                if(enemyAmount == 0)
                {
                    level++;
                    GetView<GameInfoView>().SetTipText("第" + level + "层");
                    SpawnEnemys();
                }
            });
        }
    }
}
