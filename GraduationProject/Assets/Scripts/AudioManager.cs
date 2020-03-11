/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.ScriptableObject;
using DreamerTool.Singleton;
public class AudioManager : MonoSingleton<AudioManager>
{
    private AudioSource _audio;
  
    private void Awake()
    {
        if (!GetComponent<AudioSource>())
            _audio = gameObject.AddComponent<AudioSource>();
    }
 


    public void PlayOneShot(string audio_name)
    {
        var clip = ScriptableObjectUtil.GetScriptableObject<AudioClips>().GetClip(audio_name);
        _audio.pitch = Random.Range(1.0f, 2.0f);
        _audio.PlayOneShot(clip);
    }
}
