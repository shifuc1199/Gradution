/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealthBar : MonoBehaviour
{
    public Text health_text;
    public Text name_text;
    public Image health_bar;
    public Image head;
    BaseEnemyData data;
    // Start is called before the first frame update
   public void SetData(BaseEnemyData data)
    {
        this.data = data;
        gameObject.SetActive(true);
        head.sprite = data.head;
        name_text.text = data.enemy_name;
        health_text.text = data.health + "/" + data.maxhealth;
        health_bar.fillAmount = (float)(data.health / data.maxhealth);
    }
}
