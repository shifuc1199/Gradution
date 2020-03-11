/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DreamerTool.Singleton;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using Sirenix.OdinInspector;
 
public class BiliBiliiMessageBackEditorWindow : OdinEditorWindow
{
    public string RoomID = "3555146";
    [TextArea]
    public string Content;
     
    [MenuItem("DreamerEditor/弹幕回复")]
    private static void Open()
    {
        OdinEditorWindow _window = GetWindow<BiliBiliiMessageBackEditorWindow>();
        _window.titleContent = new GUIContent("弹幕回复工具");
        _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(400, 150);
        
    }
    [Button("发送",50)]
    public void Send()
    {
        BiliBiliMessageHelper.Instance.SendContent(Content,RoomID);
        DestroyImmediate(BiliBiliMessageHelper.Instance.gameObject);
    }

}
#endif
 
class BiliBiliMessageHelper : MonoSingleton<BiliBiliMessageHelper>
{
    
    public   void SendContent(string Content,string RoomID)
    {
        StartCoroutine(Send(Content, RoomID));
    }
    IEnumerator Send(string Content,string RoomID)
    {

        WWWForm form = new WWWForm();
        form.AddField("color", 1);
        form.AddField("fontsize", 1);
        form.AddField("mode", 1);
        form.AddField("msg", Content);
        form.AddField("rnd", 1);
        form.AddField("roomid", RoomID);
        form.AddField("bubble", 0);
        form.AddField("csrf_token", "f875ed41fdb483ebaf8f6148a02364b9");
        form.AddField("csrf", "f875ed41fdb483ebaf8f6148a02364b9");
        UnityWebRequest webRequest = UnityWebRequest.Post("api.live.bilibili.com/msg/send", form);
        webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        webRequest.SetRequestHeader("Cookie", "SESSDATA=76d951ab,1585029490,df692721");

        yield return webRequest.SendWebRequest();

        if (webRequest.isHttpError || webRequest.isNetworkError)
            Debug.Log(webRequest.error);
        else
        {
            Debug.Log(webRequest.downloadHandler.text);
        }

    }
}