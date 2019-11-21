using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash_UIEffectList : MonoBehaviour {

    public GameObject[] Prefabs;
    private int currentNomber;
    private GameObject currentInstance;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeCurrent(int delta)
    {
        currentNomber += delta;
        if (currentNomber > Prefabs.Length - 1)
            currentNomber = 0;
        else if (currentNomber < 0)
            currentNomber = Prefabs.Length - 1;

        if (currentInstance != null)
        {
            Destroy(currentInstance);
            RemoveClones();
        }
        currentInstance = Instantiate(Prefabs[currentNomber]);
    }


    void RemoveClones()
    {
        var allGO = FindObjectsOfType<GameObject>();
        foreach (var go in allGO)
        {
            if (go.name.Contains("(Clone)")) Destroy(go);
        }
    }

}
