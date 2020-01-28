using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using DreamerTool.Extra;
using UnityEngine.Events;
using DreamerTool.ScriptableObject;
using DreamerTool.GameObjectPool;
public class BaseEnemyController : MonoBehaviour,IHurt
{
    public int config_id;
    BaseEnemyData enemy_data;
    private AudioSource _audio;
    private Animator _anim;
    private Rigidbody2D _rigi;
   [System.NonSerialized] public float start_gravity;
    [System.NonSerialized] public bool isGround;
    public Transform ground_check_pos;
     
    EnemyConfig _config;
    private void Awake()
    {
        _config = EnemyConfig.Get(config_id);
        _audio = GetComponent<AudioSource>();
        _anim = GetComponentInChildren<Animator>();
        _rigi = GetComponent<Rigidbody2D>();
        start_gravity = _rigi.gravityScale;
        enemy_data = new BaseEnemyData(_config,()=> {
   
            var coin = GameObjectPoolManager.GetPool("coin_effect").Get(transform.position + new Vector3(0, 1, 0), Quaternion.identity, 2f);
            coin.GetComponent<CoinParticle>().transf = View.CurrentScene.GetView<GameInfoView>().hud.coin_icon.rectTransform;
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
                    rigi.AddForce(new Vector3(Random.Range(-1f, 1f)*2, Random.Range(0f, 1f)*5, 0) ,ForceMode2D.Impulse);
                }
            }
            _rigi.simulated = false;
        });
       

    }
     
    public void GetHurt(double hurt_value,HitType _type,UnityAction hurt_call_back=null)
    {
        _audio.PlayOneShot(ScriptableObjectUtil.GetScriptableObject<AudioClips>().GetClip("hit"));
        enemy_data.SetHealth(-hurt_value);
        var pop_text = GameObjectPoolManager.GetPool("pop_text").Get(transform.position, Quaternion.identity, 0.5f);
        pop_text.GetComponent<PopText>().SetText(hurt_value.ToString(),Color.white);
        GameObjectPoolManager.GetPool("hit_effect").Get(transform.position + new Vector3(0, 2, 0), Quaternion.identity,0.5f);
        _anim.SetTrigger("Impact");
        hurt_call_back?.Invoke();
        View.CurrentScene.GetView<GameInfoView>().enemy_health.SetData(enemy_data);
        switch (_type)
        {
            case HitType.普通:
                break;
            case HitType.击退:
                if (!isGround)  
                {
                    _rigi.ResetVelocity();
                    _rigi.ClearGravity();
                    Debug.Log("受伤");
                }
                else
                {
                    _rigi.ResetVelocity();
                    _rigi.AddForce(transform.right * 10, ForceMode2D.Impulse);
                }
                break;
            case HitType.击飞:
                _rigi.ResetVelocity();
                _rigi.AddForce(new Vector2(transform.right.x, 1).normalized * 40, ForceMode2D.Impulse);
                break;
            case HitType.上挑:
                _rigi.ResetVelocity();
                _rigi.AddForce(Vector2.up *50, ForceMode2D.Impulse);
                break;
            default:
                break;
        }
        
    }
    public void IsLocked()
    {

        _rigi.ResetVelocity();
        _rigi.ClearGravity();
         
    }
    
 
    void Update()
    {
        isGround = Physics2D.OverlapCircle(ground_check_pos.position, 1, LayerMask.GetMask("Ground"));
    }
}
