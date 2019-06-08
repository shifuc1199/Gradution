using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerinput;
    private PlayerState _playerstate;
    private Rigidbody _rigi;
    private Animator _anim;

    public Transform _shoot_pos;
    public float _attack_range;
    // Start is called before the first frame update
    void Awake()
    {
        _playerstate = GetComponent<PlayerState>();
        _anim = GetComponent<Animator>();
        _rigi = GetComponent<Rigidbody>();
        _playerinput = GetComponent<PlayerInput>();
    }
    private void Start()
    {
       
    }
   
    
    public void Common_Attack()
    {
        if (!_playerstate.isInputable)
            return;

       
      
        Collider[] colliders= Physics.OverlapSphere(transform.position, _attack_range,LayerMask.GetMask("enemy"));
       
         if(colliders.Length>0)
        {
            _playerstate.isInputable = false;
            _anim.SetTrigger("attack");
            transform.forward = (colliders[0].transform.position - transform.position).normalized;
        }

        
    }

    void MoveControl()
    {
        
        if (Mathf.Abs(_playerinput.h) > 0.05f || (Mathf.Abs(_playerinput.v) > 0.05f))
        {
            if (!_playerstate.isInputable)
                return;

            Quaternion rota = transform.rotation;
            Quaternion finl = Quaternion.LookRotation(new Vector3(_playerinput.h, 0, _playerinput.v));
            transform.rotation = Quaternion.LerpUnclamped(rota, finl, 0.5f);
            transform.Translate(transform.forward *5 * Time.deltaTime,Space.World);
            _anim.SetBool("run",true);
        }
        else
        {
            _anim.SetBool("run", false);
        }

         
    }
    // Update is called once per frame
    void  Update()
    {
        MoveControl();


    }

    
     
}
