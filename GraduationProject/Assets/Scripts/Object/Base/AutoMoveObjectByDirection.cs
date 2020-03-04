using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
 
 
public class AutoMoveObjectByDirection : AutoMoveObject
{
 
    public Vector3 Direction;
     
    void   Update()
    {
 
     transform.Translate(Direction.normalized * Speed * Time.deltaTime, space_type);
      
    }
}
