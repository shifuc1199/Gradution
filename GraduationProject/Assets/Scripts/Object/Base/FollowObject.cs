using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public float _followspeed;
    public Transform _target;
    private  Vector3 offset;
  
    void Start()
    {
        offset = _target.transform.position - transform.position;
    }

 
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _target.transform.position - offset, _followspeed * Time.deltaTime);
    }
}
