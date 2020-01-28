using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
public class GameInfoView : View
{
    public InactiveButtons inactrive_buttons;
    public EnemyHealthBar enemy_health;
    public Image fade_image;
    public ActorHUD hud;
    public GameObject pop_text;
    public void SetInactiveType(InactiveType _type,ItemSprite item = null)
    {
        inactrive_buttons.SetInactiveType(_type,item);
    }
    Timer timer1=null;
    Timer timer2 = null;
 
    public void FadeAnim(UnityAction action = null,float t = 1)
    {
        fade_image.DOFade(1, 0.5f).SetEase(Ease.Linear).onComplete = () => { action?.Invoke(); Timer.Register(t, () => { fade_image.DOFade(0, 0.5f).SetEase(Ease.Linear); }); };
        
    }
    public void SetPopText(string v,Color c,float t=0.5f)
    {
        if (timer1 != null)
        {
            timer1.Cancel();
        }
        if(timer2!=null)
        {
            timer2.Cancel();
        }
        if(pop_text.activeSelf)
            pop_text.SetActive(false);
        pop_text.SetActive(true);
        pop_text.GetComponentInChildren<Text>().text = v;
        pop_text.GetComponentInChildren<Text>().color = c;
        pop_text.GetComponent<Animator>().SetTrigger("show");
        timer1 = Timer.Register(t, () => { pop_text.GetComponent<Animator>().SetTrigger("hide"); timer2= Timer.Register(0.5f, () => { pop_text.SetActive(false); }); });
    }
    public void ShowAnim()
    {
        GetComponent<Animator>().SetTrigger("show");
    }
    public void HideAnim()
    {
        GetComponent<Animator>().SetTrigger("hide");
    }
}
