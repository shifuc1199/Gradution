/*****************************
Created by 师鸿博
*****************************/
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using DreamerTool.Extra;
using DreamerTool.UI;
public class SignInCell : MonoBehaviour
{
    public Button SignInButton;
    public GameObject SelectFrame;
    public GameObject YesTip;
    public Text DayCountText;
    public Sprite SignInButtonSignInSprite;
    public Sprite SignInButtonCommonSprite;
    private void Start()
    {
         
    }
    public void SetSignInButton(bool value)
    {
        SignInButton.interactable = value;
       
        if (value)
        {
            SelectFrame.SetActive(true);
            SignInButton.gameObject.SetActive(true);
            SignInButton.image.sprite = SignInButtonCommonSprite;
            SignInButton.GetComponentInChildren<Text>().color = Color.white;
            SignInButton.GetComponentInChildren<Text>().text = "签到";
        }
        else
        {
            SignInButton.GetComponentInChildren<Text>().text = "已签到";
            SignInButton.image.sprite = SignInButtonSignInSprite;
            SignInButton.GetComponentInChildren<Text>().color = Color.gray;
        }
    }
    public void SetModel(int daycount,int index)
    {
        DayCountText.text = "第" + index.ToString() + "天";
        int value = daycount - (index - 1);
        //value>0 代表签到过的
        //value == 0 代表当天正在需要签到的
        // value < 0 代表还没签到的
        if (value == 0)
        {
            if (daycount > 0)
            {
                if ((TimeModel.Instance.Now - SignInView.SignInDate.GetLast()).Seconds >= 24*60*60)
                {
                    SetSignInButton(true);
                }
            }
            else
            {
                SetSignInButton(true);
            }
        }
        else
        {
            if (value > 0)
            {
                SetSignInButton(false);
                
            }
            else
            {
                SignInButton.gameObject.SetActive(false);
                SelectFrame.SetActive(false);
            }
        }


        YesTip.SetActive(value > 0);
         
    }
    public void SignIn()
    {
        SignInView.SignInDate.Add(TimeModel.Instance.Now);
        View.CurrentScene.GetView<SignInView>().UpdateCell();
        View.CurrentScene.GetView<SignInView>().isTiming = true;
    }
    private void Update()
    {
         
    }
}
