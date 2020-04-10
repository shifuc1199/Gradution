/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClips")]
public class AudioClips : ScriptableObject
{
    public List<AudioClipWithID> Clips = new List<AudioClipWithID>();

    public AudioClip GetClip(string id)
    {
        return Clips.Find(a => { return a.id == id; }).clip;
    }
}
[System.Serializable]
public class AudioClipWithID
{
    public string id;
    public AudioClip clip;
}

