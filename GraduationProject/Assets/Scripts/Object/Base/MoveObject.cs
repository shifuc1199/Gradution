using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour   
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Vector3 _direction;

    public float Speed { get { return _speed; } set { _speed = value; }}
    public Vector3 Direction { get { return _direction; } set { _direction = value; } }

  
    
    void   Update()
    {
        
        transform.Translate(Direction * Speed * Time.deltaTime);
    }
}
