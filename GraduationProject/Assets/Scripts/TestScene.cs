/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.GameObjectPool;
public class TestScene : MonoBehaviour
{
    public Texture red_tex;
    public Texture white_tex;
    public float amount;
    private void Start()
    {
       

    }
    private void OnGUI()
    {
       
        GUI.DrawTexture(new Rect(new Vector2(0, 0), new Vector2(200, 25)), white_tex);
        GUI.DrawTexture(new Rect(new Vector2(0, 0), new Vector2(amount, 25)), red_tex);
    }
    private void Update()
    {
        
    }
}
