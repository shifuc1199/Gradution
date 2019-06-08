using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonAttackObject : MoveObject
{
    
    // Start is called before the first frame update
    void Start()
    {
        Direction = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
 
        transform.Translate(Direction * Speed * Time.deltaTime, Space.World);
    }
}
