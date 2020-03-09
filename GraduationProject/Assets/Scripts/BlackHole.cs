/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Extra;
public class BlackHole : MonoBehaviour
{
    public float radius;
    public float speed;
    public float attack_timer_interval;
    public int skill_id;
    float timer;
    public List<BaseEnemyController> enemys = new List<BaseEnemyController>();
    // Start is called before the first frame update
    void OnEnable()
    {
         
    }
    private void Start()
    {
         
    }
  
    private void OnDisable()
    {
        enemys.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        var cols = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("enemy"));

        foreach (var col in cols)
        {
            var enemy_ctr = col.GetComponent<BaseEnemyController>();
            if(enemy_ctr&&!enemys.Contains(enemy_ctr))
            enemys.Add(enemy_ctr);
        }
        timer += Time.deltaTime;
        if (timer >= attack_timer_interval)
        {
            foreach (var enemy in enemys)
            {
                if (!enemy)
                {
                    timer = 0;
                    return;
                }
                enemy.GetHurt(SkillModel.Get(skill_id).GetHurtValue(), HitType.普通,transform.position);
                timer = 0;
            }
        }
        foreach (var enemy in enemys)
        {
            if(!enemy.isSuperArmor)
            enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, transform.position, speed*Time.deltaTime);
        }
       
    }
}
