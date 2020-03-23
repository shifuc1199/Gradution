using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using DreamerTool.Extra;
using UnityEngine.Events;
using DreamerTool.GameObjectPool;
using DreamerTool.Util;
using DG.Tweening;
 
public class BaseEnemyController : MonoBehaviour,IHurt
{
    public int config_id;
    BaseEnemyData enemy_data;
    private AudioSource _audio;
    private Animator _anim;
    private Rigidbody2D _rigi;
    public GameObject Shadow;
    [System.NonSerialized] public float start_gravity;
    [System.NonSerialized] public bool isGround;
    public bool isMoveable = true;
    public bool isSuperArmor = false;
    public Transform ground_check_pos;
    public float hit_fly_distance;
    public float hit_back_distance;
  
    public EnemyModel model;
    private void Awake()
    {
        model = new EnemyModel(config_id,1);
       
        _audio = GetComponent<AudioSource>();
        _anim = GetComponentInChildren<Animator>();
        _rigi = GetComponent<Rigidbody2D>();
        start_gravity = _rigi.gravityScale;
        enemy_data = new BaseEnemyData(model, ()=> {
            if(Shadow)
            Shadow.SetActive(false);
            var coin = GameObjectPoolManager.GetPool("coin_effect").Get(transform.position + new Vector3(0, 1, 0), Quaternion.identity, 2f);
            coin.GetComponent<CoinParticle>().transf = View.CurrentScene.GetView<GameInfoView>().hud.coin_icon.rectTransform;
            var exp = GameObjectPoolManager.GetPool("exp_effect").Get(transform.position + new Vector3(0, 1, 0), Quaternion.identity, 1.85f);
            exp.GetComponent<ExpParticle>().transf = View.CurrentScene.GetView<GameInfoView>().expbar.GetComponent<RectTransform>();
            var colliders = GetComponentsInChildren<PolygonCollider2D>();
            var rigis = GetComponentsInChildren<Rigidbody2D>();
            _anim.enabled = false;
            foreach (var col in colliders)
            {
                col.enabled = true;
            }
            foreach (var rigi in rigis)
            {
                if (!rigi.simulated)
                {
                    rigi.simulated = true;
                    rigi.AddTorque(2,ForceMode2D.Impulse);
                    rigi.AddForce(new Vector3(Random.Range(-1f, 1f)*4f, Random.Range(0.5f, 1f)*5, 0) ,ForceMode2D.Impulse);
                }
            }
            GetComponent<Collider2D>().enabled = false;
            _rigi.simulated = false;
        });
    }
 
    public void GetHurt(AttackData attackData,UnityAction hurt_call_back=null)
    {
        if (enemy_data.isdie)
            return;
        
        transform.rotation = attackData.attack_pos.x > transform.position.x ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
         
        var hurt = (int)DreamerUtil.GetHurtValue(attackData.hurt_value, model.GetDefend());
        enemy_data.SetHealth(-hurt);
        var pop_text = GameObjectPoolManager.GetPool("pop_text").Get(transform.position, Quaternion.identity, 0.5f);
        pop_text.GetComponent<PopText>().SetText(hurt.ToString(), attackData.isCrit?Color.red:Color.white);

        View.CurrentScene.GetView<GameInfoView>().enemy_health.SetData(enemy_data);
 
        AudioManager.Instance.PlayOneShot("hit");
        GameObjectPoolManager.GetPool("hit_effect").Get(transform.position + new Vector3(0, 2, 0), Quaternion.identity,0.5f);
        hurt_call_back?.Invoke();

        if (isSuperArmor)
        {
            attackData.attack_type = HitType.普通;
        }

         
        GameStaticMethod.ChangeChildrenSpriteRendererColor(gameObject, Color.red);

        switch (attackData.attack_type)
        {
            case HitType.普通:
                if (!isGround)
                {
                    _rigi.ClearGravity();
                }
                _rigi.ResetVelocity();
                _anim.SetTrigger("hit");
                break;
            case HitType.击退:
  
                if (!isGround)  
                {
                    _rigi.ResetVelocity();
                    _rigi.ClearGravity();
                }
                else
                {
                    _rigi.ResetVelocity();
                    _rigi.AddForce(transform.right * hit_back_distance, ForceMode2D.Impulse);
                }
                _anim.SetTrigger("hit");
                break;
            case HitType.击飞:
                _anim.SetTrigger("hitfly");
                _rigi.ResetVelocity();
                if (isGround)
                {
                    _rigi.AddForce(new Vector2(transform.right.x*0.5f, 0.5f).normalized * hit_fly_distance, ForceMode2D.Impulse);
                }
                else
                {
                    _rigi.AddForce(new Vector2(transform.right.x*0.5f, 0).normalized * hit_fly_distance, ForceMode2D.Impulse);
                }
                break;
            case HitType.上挑:
                _rigi.ResetVelocity();
                _rigi.AddForce(Vector2.up *95, ForceMode2D.Impulse);
                break;
            default:
                break;
        }
        
    }
    GameObject temp_aim;
    public void Lock()
    {
        temp_aim = GameObjectPoolManager.GetPool("aim").Get(transform.position, Quaternion.identity, -1);
        _rigi.ResetVelocity();
        _rigi.ClearGravity();
         
    }

    public void UnLock()
    {
        temp_aim.GetComponent<ObjectRecover>().RecoverImmediately();
        _rigi.ResetVelocity();
        _rigi.SetGravity(start_gravity);

    }
    void Update()
    {
        isGround = Physics2D.OverlapCircle(ground_check_pos.position, 1, LayerMask.GetMask("Ground"));
        _anim.SetBool("isground", isGround);
    }
}
