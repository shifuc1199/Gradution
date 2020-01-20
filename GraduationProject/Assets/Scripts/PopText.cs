/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
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
        _text.DOFade(1, 0);
    }
    public void SetText(string t,Color c)
    {
        _text.color = c;
        _text.text = t;
        transform.DOLocalMoveY(2, 0.5f).SetEase(Ease.Linear);
        _text.DOFade(0,0.5f).SetEase(Ease.Linear);
    }
}
