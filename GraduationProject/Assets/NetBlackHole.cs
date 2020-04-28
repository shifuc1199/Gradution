/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.Extra;
using LitJson;
using Photon.Pun;

public class NetBlackHole : MonoBehaviour
{
    private PhotonView photonView;
    SkillModel model;
    public float radius;
    public float speed;
    public float attack_timer_interval;
    public int skill_id;
    float timer;
    public List<NetkActorController> enemys = new List<NetkActorController>();
    // Start is called before the first frame update
    void OnEnable()
    {

    }
    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        Destroy(gameObject, (float)photonView.InstantiationData[0]);
        model = (JsonMapper.ToObject<ActorModel>(photonView.Owner.CustomProperties["model"].ToString())).GetSkillModel(skill_id);
    }

    private void OnDisable()
    {
        enemys.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        var cols = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("player"));

        foreach (var col in cols)
        {
            var enemy_ctr = col.GetComponent<NetkActorController>();
            if (enemy_ctr && !enemys.Contains(enemy_ctr) &&! enemy_ctr.photonView.IsMine)
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
                enemy.GetHurt(new AttackData(
                    model.GetHurtValue(),
                    false,
                    transform.position,
                    HitType.普通
                    ));
                timer = 0;
            }
        }
        foreach (var enemy in enemys)
        {
            if (!enemy.actor_state.isSuperArmor)
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, transform.position, speed * Time.deltaTime);
        }

    }
}
