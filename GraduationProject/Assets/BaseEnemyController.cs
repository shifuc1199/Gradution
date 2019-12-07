using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class BaseEnemyController : MonoBehaviour,IHurt
{
    public GameObject hit_prefab;
    private Animator _anim;
    private Rigidbody2D _rigi;
    public float start_gravity;
    public bool isGround;
    public Transform ground_check_pos;
    private void Awake()
    {

        _anim = GetComponentInChildren<Animator>();
        _rigi = GetComponent<Rigidbody2D>();
        start_gravity = _rigi.gravityScale;
    }
    public void GetHurt(HitType _type,UnityAction hurt_call_back=null)
    {
        Instantiate(hit_prefab, transform.position+new Vector3(0,2,0), Quaternion.identity);
        _anim.SetTrigger("Impact");
        hurt_call_back?.Invoke();
        switch (_type)
        {
            case HitType.普通:
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
    
 
    void Update()
    {
        isGround = Physics2D.OverlapCircle(ground_check_pos.position, 1, LayerMask.GetMask("Ground"));
    }
}
