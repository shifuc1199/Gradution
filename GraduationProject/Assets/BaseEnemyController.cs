using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    private Animator _anim;
    private void Awake()
    {

        _anim = GetComponentInChildren<Animator>();
    }
    public void GetHurt()
    {
        _anim.SetTrigger("Impact");
    }
}
