/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CoolButton : MonoBehaviour
{
    bool is_cool_down = false;
    public float cool_down_time;
    public Image mask_image;

    float cool_timer;
    // Start is called before the first frame update
    void Start()
    {
        cool_timer = cool_down_time;
        GetComponent<Button>().onClick.AddListener(CoolDown);
    }
    public void CoolDown()
    {
        is_cool_down = true;
        GetComponent<Button>().interactable = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(is_cool_down)
        {
            cool_timer -= Time.fixedDeltaTime;
            mask_image.fillAmount = cool_timer / cool_down_time;
            mask_image.GetComponentInChildren<Text>().text = cool_timer.ToString("f1") + "s";
            if (cool_timer<=0)
            {
                mask_image.GetComponentInChildren<Text>().text = "";
                cool_timer = cool_down_time;
                is_cool_down = false;
                GetComponent<Button>().interactable = true;
            }
        }
    }
}
