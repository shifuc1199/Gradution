/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExpBar : MonoBehaviour
{
    public Text _text;
    public Text _level_text;
    public Image _image;
    // Start is called before the first frame update
    private void Awake()
    {
        EventHandler.OnChangeExp += SetBar;
    }
    void Start()
    {
        SetBar();
    }
    private void OnDisable()
    {
        EventHandler.OnChangeExp -= SetBar;
    }
    public void SetBar()
    {
        _level_text.text = "LV "+ ActorModel.Model.GetLevel();
        _text.text = ActorModel.Model.GetExp() + " / " + ActorModel.Model.GetMaxExp();
        _image.fillAmount = ActorModel.Model.GetExp() / (float)ActorModel.Model.GetMaxExp();
    }
}
