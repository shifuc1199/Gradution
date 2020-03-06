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
    public static List<System.DateTime> SignInDate = new List<System.DateTime>();
    private List<SignInCell> cell_list = new List<SignInCell>();

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
            signincell.SetModel(SignInDate.Count, i+1);
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
            cell_list[i].SetModel(SignInDate.Count, i + 1);
        }
         
    }
    
    public bool isTiming = true;


    System.DateTime test = new System.DateTime(1,1,1,23,59,59);

    private void Update()
    {
        if (isTiming && SignInDate.Count > 0)
        {
            
            timing_text.text = "(距离下一次签到还有: <color=green>" + test.AddSeconds( (24 * 60 * 60 - (TimeModel.Instance.Now - SignInDate.GetLast()).Seconds)).ToString("HH时:mm分:ss秒") + "</color>)";
            if ((24 * 60 * 60 - (TimeModel.Instance.Now - SignInDate.GetLast()).Seconds) <= 0)
            {
                timing_text.text = "(可以签到啦!)";
                if (SignInDate.Count == 6)
                {
                    SignInDate.Clear();
                }
                isTiming = false;
                UpdateCell();
        
            }
        }
    }

}
