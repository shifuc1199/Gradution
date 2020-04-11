/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using TMPro;
using cn.SMSSDK.Unity;
using UnityEngine.UI;
using cn.bmob.api;
using LitJson;
public class LoginView : View, SMSSDKHandler
{
    public Button sendCodeBtn;

    public TMP_InputField PhoneNumberInput;
    public TMP_InputField VerificationInput;

    
    private SMSSDK smssdk;

    public string zone = "86";

    public string appKey = "2ebdcb72b263b";
    public string appSerect = "0bc042a3e491f49baba1ed1cdd780b6b";

    public float sendCodeTime;

    float sendCodeTimer;

    bool isSend;

    private void Start()
    {
       
        smssdk = GetComponent<SMSSDK>();
        smssdk.init(appKey, appSerect, false);//初始化SDK
        smssdk.setHandler(this);//设置回调，用户处理从客户端返回的信息
      
        sendCodeTimer = sendCodeTime;
    }
    public void OnBtnVerification()
    {
      
        smssdk.getCode(CodeType.TextCode, PhoneNumberInput.text, zone, null);
    }

    public void OnBtnOK()
    {
        CurrentScene.OpenView<LoadView>();
        smssdk.commitCode(PhoneNumberInput.text, zone, VerificationInput.text);
    }
    public void onComplete(int action, object resp)
    {
        var act = (ActionType)action;
        switch (act)
        {
            case ActionType.GetCode:
                isSend = true;
                sendCodeBtn.interactable = false;
                 
                break;
            case ActionType.CommitCode:
             
                var resp_data = JsonMapper.ToObject(resp.ToString());
                if (resp_data["phone"].ToString() != PhoneNumberInput.text)
                {
                    CurrentScene.OpenView<LoadView>().SetText("手机号与验证码不匹配！");
                    return;
                }
                UserBmobDao user_dao = new UserBmobDao(resp_data["phone"].ToString(), "");
                  user_dao.FindByID((user)=> {
                      if (user != null)
                      {
                          user_dao = user;
                          PlayerPrefs.SetString(GameConstData.LOGINING_PHONE_NUMBER,user_dao.id);
                          BmobManager.Instance.UserBmobDao = user_dao;
                          SaveManager.Instance.LoginType = LoginType.短信;
                          LoadingScene.LoadScene(GameConstData.CHOOSE_ACTOR_SCENE_NAME);
                      }
                      else
                      {
                          user_dao.Insert(()=> {
                              PlayerPrefs.SetString(GameConstData.LOGINING_PHONE_NUMBER, user_dao.id);
                              BmobManager.Instance.UserBmobDao = user_dao;
                              SaveManager.Instance.LoginType = LoginType.短信;
                              LoadingScene.LoadScene(GameConstData.CHOOSE_ACTOR_SCENE_NAME);

                          });
                      }

                      CurrentScene.CloseView<LoadView>();
                  });
                
                break;
            default:
                break;
        }
    }
    private void Update()
    {
        if (isSend)
        {
            sendCodeBtn.GetComponentInChildren<Text>().text = "已发送(" + (int)sendCodeTimer + ")";
            sendCodeTimer -= Time.deltaTime;
            if(sendCodeTimer <=0 )
            {
                sendCodeBtn.interactable = true;
                sendCodeBtn.GetComponentInChildren<Text>().text = "发送";
                isSend = false;
            }
        }
    }
    public void onError(int action, object resp)
    {
        
        var act = (ActionType)action;
        Debug.LogError(resp.ToString());
      
        switch (act)
        {
            case ActionType.GetCode:
                CurrentScene.OpenView<LoadView>().SetText("验证码发送失败!");
                break;
            case ActionType.CommitCode:
                    CurrentScene.OpenView<LoadView>().SetText("验证码错误!");
        
                break;
            default:
                break;
        }
    }
}
