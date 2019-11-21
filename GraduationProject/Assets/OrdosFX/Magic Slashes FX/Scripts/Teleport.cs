using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour {

    public Vector3 Range = new Vector3();
    Vector3 startPos;

    // Use this for initialization
    void Awake () {
        startPos = transform.position;

	}
	
	// Update is called once per frame
	void OnEnable() {
        transform.position = startPos;
	}

    public void CustomTeleport(){
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var rend in renderers) {
            rend.enabled = false;
        }
        transform.position += Range;

        foreach (var rend in renderers)
        {
            rend.enabled = true;
        }
    }
}
