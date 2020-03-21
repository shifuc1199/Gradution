/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScratchCard : MonoBehaviour
{
    public EraseMask erase;
    public Text m_text;

    public int money { get; private set; }
 
    // Start is called before the first frame update

    public void Init()
    {
        int a = Random.Range(0, 10);
        if(a<=2)
        {
            money = 10;
            m_text.text = "金币: x10";
        }
        else
        {
            money = 0;
            m_text.text = "谢谢惠顾";
        }
    }
}
