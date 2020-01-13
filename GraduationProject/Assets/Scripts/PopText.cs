/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PopText : MonoBehaviour
{
    private Text _text;
    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponentInChildren<Text>();
    }
    public void SetText(string t)
    {
        _text.text = t;
    }
}
