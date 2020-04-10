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
                sendCodeBtn.GetComponentInChildren<Text>().text = "重新发送("+(int)sendCodeTimer+")";
                break;
            case ActionType.CommitCode:
                break;
            default:
                break;
        }
    }
    private void Update()
    {
        if (isSend)
        {
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
                break;
            case ActionType.CommitCode:
                break;
            default:
                break;
        }
    }
}
