/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScratchCard : MonoBehaviour
{
    private  Text m_text;
    private void Awake()
    {
        m_text = GetComponentInChildren<Text>();
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        Init();
    }
    public void Init()
    {
        int a = Random.Range(0, 10);
        if(a<=2)
        {
            m_text.text = "金币: x10";
        }
        else
        {
            m_text.text = "谢谢惠顾";
        }
    }
}
