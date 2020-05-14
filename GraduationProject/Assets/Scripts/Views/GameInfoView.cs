using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
public class GameInfoView : View
{
    private int hit_count;
    public Text hit_count_text;
    public int HitCount
    {
        get
        {
            return hit_count;
        }
        set
        {
            hit_count = value;
            hit_count_text.GetComponent<DOTweenAnimation>().DORestart();
            hit_count_text.text = hit_count + "  Combo";
        }
    }
    public InactiveButtons inactrive_buttons;
    public EnemyHealthBar enemy_health;
    public Image fade_image;
    public ExpBar expbar;
    public ActorHUD hud;
    public GameObject pop_text;
    public Image m_screen_effect;
    public Text tipText;
    public void SetInactiveType(InactiveType _type,ItemSprite item = null)
    {
        inactrive_buttons.SetInactiveType(_type,item);
    }
    public void SetTipText(string text)
    {
        tipText.text = text;
    }
    Timer popTextTimer_1=null;
    Timer popTextTimer_2 = null;
 
    public void FadeAnim(UnityAction action = null,float t = 1)
    {
        fade_image.DOFade(1, 0.5f).SetEase(Ease.Linear).onComplete = () => { action?.Invoke(); Timer.Register(t, () => { fade_image.DOFade(0, 0.5f).SetEase(Ease.Linear); }); };
        
    }
    public void SetScreenEffect(Color c,float fade_value, float fade_time = 0.5f,float fill=0.1f)
    {
        c.a = 1-fade_value;
        m_screen_effect.pixelsPerUnitMultiplier = fill;
        m_screen_effect.color = c;
        m_screen_effect.DOFade(fade_value, fade_time).SetEase(Ease.Linear);
    }
    public void SetPopText(string v,Color c,float t=0.5f)
    {
        if (popTextTimer_1 != null)
        {
            popTextTimer_1.Cancel();
        }
        if(popTextTimer_2!=null)
        {
            popTextTimer_2.Cancel();
        }
        if(pop_text.activeSelf)
            pop_text.SetActive(false);
        pop_text.SetActive(true);
        pop_text.GetComponentInChildren<Text>().text = v;
        pop_text.GetComponentInChildren<Text>().color = c;
        pop_text.GetComponent<Animator>().SetTrigger("show");
        popTextTimer_1 = Timer.Register(t, () => { pop_text.GetComponent<Animator>().SetTrigger("hide"); popTextTimer_2= Timer.Register(0.5f, () => { pop_text.SetActive(false); }); });
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
