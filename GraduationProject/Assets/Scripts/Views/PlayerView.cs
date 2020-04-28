using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DreamerTool.Util;
using DreamerTool.UI;
using DG.Tweening;
public class PlayerView : MonoBehaviour
{
    public Text player_name_text;
    public Text player_level_text;
    public Text[] player_attribute_text;
 
 
  //  public Text player_attribute_text;
    private void Awake()
    {

        Init();
        EventManager.OnChangeLevel += UpdatePlayerLevel;
        EventManager.OnChangePlayerAttribute += UpdatePlayAttributeText;
    }
 
    private void OnDestroy()
    {
        EventManager.OnChangeLevel -= UpdatePlayerLevel;
        EventManager.OnChangePlayerAttribute -= UpdatePlayAttributeText;
    }
    public void UpdatePlayerLevel()
    {
        player_level_text.text ="LV  " +ActorModel.Model.GetLevel();
    }
    public void Init()
    {
        player_name_text.text = ActorModel.Model.actor_name + " (" + ActorModel.Model.knightLevel + ")";
        var attrubutes = Enum.GetNames(typeof(PlayerAttribute));

        for (int i = 0; i < attrubutes.Length; i++)
        {
            player_attribute_text[i].transform.GetChild(0).gameObject.SetActive(false);
            player_attribute_text[i].text = attrubutes[i] + ": " + DreamerUtil.GetColorRichText(ActorModel.Model.GetPlayerAttribute((PlayerAttribute)Enum.Parse(typeof(PlayerAttribute), attrubutes[i])).ToString(),Color.yellow) ;
        }
    }
    Dictionary<PlayerAttribute, Coroutine> coroutines = new Dictionary<PlayerAttribute, Coroutine>();
    public void UpdatePlayAttributeText(PlayerAttribute attribute,double value)
    {
        
        //   StopAllCoroutines();

       var start = ActorModel.Model.GetPlayerAttribute(attribute)-value;

       var coroutine =  StartCoroutine(TextAnim(player_attribute_text[(int)attribute], start, ActorModel.Model.GetPlayerAttribute(attribute), attribute+": "));

        if (coroutines.ContainsKey(attribute))
        {
            if(coroutines[attribute]!=null)
            StopCoroutine(coroutines[attribute]);
            coroutines.Remove(attribute);
        }
        coroutines.Add(attribute, coroutine);

        player_attribute_text[(int)attribute].transform.GetChild(0).gameObject.SetActive(false);
        player_attribute_text[(int)attribute].transform.GetChild(0).gameObject.SetActive(true);
         
        player_attribute_text[(int)attribute].transform.GetChild(0).GetComponent<Text>().text =DreamerUtil.GetColorRichText("("+ (value > 0?"+":"")+value +")",(value>0?Color.green:Color.red));
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        Init();
    }
    IEnumerator TextAnim(Text t,double value,double end,string c)
    {
 
            
        while (value  != end)
        {

            if (value > end)
                value--;
            else
                value++;

            t.text = c + DreamerUtil.GetColorRichText(value.ToString(),Color.yellow);

            yield return null;
        }
   
        
    }

}
