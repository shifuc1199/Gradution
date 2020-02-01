/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DreamerTool.UI;
public class LoadingScene : DreamerTool.UI.Scene
{
    private static string scene_name;
    public Text load_text;
    public Image load_imag;
    public static void LoadScene(string scene_name)
    {
        LoadingScene.scene_name = scene_name;
        SceneManager.LoadScene("Loading");
    }
    public override void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        base.OnSceneLoaded(scene, mode);
 
    }

    private void Awake()
    {
        base.Awake();
         
        StartCoroutine(Load());
    }
    IEnumerator Load()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene_name);
        operation.allowSceneActivation = false;
        while (operation.progress <= 0.9f)
        {
            load_text .text = operation.progress * 100+"%";
            load_imag.fillAmount = operation.progress;
            yield return new WaitForEndOfFrame();
        }
        load_text.text = "100%";
        load_imag.fillAmount = 1;
        yield return new WaitForEndOfFrame();
        operation.allowSceneActivation = true;
    }
}
