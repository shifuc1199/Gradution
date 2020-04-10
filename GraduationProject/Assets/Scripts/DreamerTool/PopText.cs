/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
[RequireComponent(typeof(TextMeshPro))]
public class PopText : MonoBehaviour
{
    private TextMeshPro _text;
    // Start is called before the first frame update
    void Awake()
    {
        _text = GetComponentInChildren<TextMeshPro>();
        
    }
    private void OnEnable()
    {
        transform.GetChild(0).DOLocalMoveY(0, 0);
       
    }
    public void SetText(string t,Color c)
    {
        _text.color = c;
        _text.text = t;
        transform.GetChild(0).DOLocalMoveY(5, 0.5f).SetEase(Ease.Linear);
      
    }
}
