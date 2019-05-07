using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;
public class PlayerInput : MonoBehaviour
{
   public float h ;
    public float v ;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        h = ETCInput.GetAxis("Horizontal");
        v = ETCInput.GetAxis("Vertical");
    }
}
