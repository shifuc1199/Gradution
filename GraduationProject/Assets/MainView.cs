/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
public class MainView : View
{
    public Text number_text;
    public GameObject no_login;
    public GameObject logined;
    private void Start()
    {
        if(PlayerPrefs.HasKey(GameConstData.LOGINING_PHONE_NUMBER))
        {
            number_text.text = PlayerPrefs.GetString(GameConstData.LOGINING_PHONE_NUMBER);
            no_login.SetActive(false);
            logined.SetActive(true);
        }
        else
        {
            no_login.SetActive(true);
            logined.SetActive(false);

        }
    }
    public void ContinoueLogin()
    {
        CurrentScene.OpenView<LoadView>();
        UserBmobDao user_dao = new UserBmobDao(PlayerPrefs.GetString(GameConstData.LOGINING_PHONE_NUMBER), "");
        user_dao.FindByID((user) => {
            if (user != null)
            {
                user_dao = user;
                BmobManager.Instance.UserBmobDao = user_dao;
                SaveManager.Instance.LoginType = LoginType.短信;
                LoadingScene.LoadScene(GameConstData.CHOOSE_ACTOR_SCENE_NAME);
            }
            else
            {
                CurrentScene.OpenView<LoadView>().SetText("登录失败！ 没找到此用户");
                return;
            }
            CurrentScene.CloseView<LoadView>();
        },()=> { CurrentScene.OpenView<LoadView>().SetText("登录失败！请检查网络设置"); });
    }
    public void GuestLogin()
    {
        SaveManager.Instance.LoginType = LoginType.游客;
        LoadingScene.LoadScene(GameConstData.CHOOSE_ACTOR_SCENE_NAME);
    }
}
