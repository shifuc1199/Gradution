using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash_WroomWroom : MonoBehaviour
{
    public float Speed = 1;

    public GameObject Target;
	// Use this for initialization
	void OnEnable () {
		transform.localPosition = new Vector3(0,0,0);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, Speed * Time.deltaTime);
    }
}
