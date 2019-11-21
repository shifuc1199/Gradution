using System;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class Slash_AxisRotateByTime : MonoBehaviour {

    public Vector3 RotateAxis = new Vector3(0, 0, 0);

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
        transform.Rotate(RotateAxis * Time.deltaTime);
	}
}
