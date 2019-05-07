using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float _followspeed;
    public Transform _target;
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset =  _target.transform.position- transform.position ;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _target.transform.position - offset, _followspeed * Time.deltaTime);
    }
}
