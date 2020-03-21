/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
using DG.Tweening;
public class NPCView : View
{
    public Text _text;
    public float time_interval;
    public Button[] m_inactiveBtns;
   
    int inactiveBtn_index;
    int content_index;
    private void Awake()
    {

    }
    public override void OnShow()
    {
        base.OnShow();
        CurrentScene.GetView<GameInfoView>().HideAnim();
    }
    public override void OnHide()
    {
        base.OnHide();
        CurrentScene.GetView<GameInfoView>().ShowAnim();
    }
    public void LoadTextAsset(string path)
    {
        inactiveBtn_index = 0;
        content_index = 0;
         var textAsset = ResManager.LoadTextAsset(path).text;
        Execute(textAsset.Split('\n'));
    }
    public void Execute(string[] content)
    {
        while (content_index < content.Length)
        {
            var commond = content[content_index].Split(';');
            switch (commond[0])
            {
                case "npc_inactive":
                    _text.text = commond[1];
                    break;
                case "inactive_button":
                    var btn = m_inactiveBtns[inactiveBtn_index];
                    btn.gameObject.SetActive(true);
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(()=> { GameStaticMethod.ExecuteCommond(commond[2].Trim()); });
                    btn.GetComponentInChildren<Text>().text = commond[1];
                    inactiveBtn_index += 1;
                    break;
                default:
                    break;
            }

            content_index++;
        }
    }
    private void Update()
    {
          
    }

}
