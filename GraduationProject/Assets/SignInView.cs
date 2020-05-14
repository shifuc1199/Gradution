/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using DreamerTool.Util;
using DreamerTool.Extra;
using UnityEngine.UI;
public class SignInView : View
{
    public Transform root;
    public GameObject cell_prefab;
    public Text timing_text;
    
    private List<SignInCell> cell_list = new List<SignInCell>();
    int second;
    public GameObject failed;
    public GameObject successful;
    public GameObject loading;
    private IEnumerator Start()
    {
        yield return StartCoroutine(TimeModel.Instance.GetTime());

        loading.SetActive(false);

        if (TimeModel.Instance.Now == System.DateTime.MinValue)
        {
            failed.SetActive(true);
            yield break ;
        }

        successful.SetActive(true);

        for (int i = 0; i < 6; i++)
        {
            var cell = Instantiate(cell_prefab, root);
            var signincell = cell.GetComponent<SignInCell>();
            signincell.SetModel(ActorModel.Model. SignInDate.Count, i+1);
            cell_list.Add(signincell);
        }
    }

    public override void OnShow()
    {
        base.OnShow();
        CurrentScene.GetView<GameInfoView>().HideAnim();
         
       
    }
    public override void OnHide()
    {
        base.OnShow();
        CurrentScene.GetView<GameInfoView>().ShowAnim();

    }
    public void UpdateCell()
    {
 
        for (int i = 0; i < cell_list.Count; i++)
        {
            cell_list[i].SetModel(ActorModel.Model.SignInDate.Count, i + 1);
        }
         
    }
    
    public bool isTiming = true;


    

    private void Update()
    {
        if (isTiming && ActorModel.Model.SignInDate.Count > 0)
        {
            second = (24 * 60 * 60 - (int)(TimeModel.Instance.Now - ActorModel.Model.SignInDate.GetLast()).TotalSeconds);
         
          
            timing_text.text = "(距离下一次签到还有: <color=green>" + TimeModel.Instance.GetDateTimeBySeconds(second).ToString("HH时:mm分:ss秒") + "</color>)";
            if ((24 * 60 * 60 - (TimeModel.Instance.Now - ActorModel.Model.SignInDate.GetLast()).Seconds) <= 0)
            {
                timing_text.text = "(可以签到啦!)";
                if (ActorModel.Model.SignInDate.Count == 6)
                {
                    ActorModel.Model.SignInDate.Clear();
                }
                isTiming = false;
                UpdateCell();
        
            }
        }
    }

}
