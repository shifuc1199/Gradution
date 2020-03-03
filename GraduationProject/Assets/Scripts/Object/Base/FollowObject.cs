using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public bool is_follow_rotation;
    public float _followspeed;
    public Transform _target;
    private  Vector3 offset;
  
    void Start()
    {
        offset = _target.transform.position - transform.position;
    }

 
    void Update()
    {
        if (is_follow_rotation)
            transform.rotation =Quaternion.Euler(0,  _target.transform.eulerAngles.y,0);
        transform.position = Vector3.Lerp(transform.position, _target.transform.position - offset, _followspeed * Time.deltaTime);
    }
}
