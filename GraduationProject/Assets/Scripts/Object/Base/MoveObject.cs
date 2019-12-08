using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour   
{

    public float Speed;
    public Vector3 Direction;
    
    void   Update()
    {
        
        transform.Translate(Direction.normalized * Speed * Time.deltaTime);
    }
}
