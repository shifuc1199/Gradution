using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 1)
        {

            timer += Time.unscaledDeltaTime;

            if (timer >= 0.1f)
            {

                timer = 0;
                Time.timeScale = 1f;
            }
        }
        else
        {
            timer = 0;
        }
    }
}
