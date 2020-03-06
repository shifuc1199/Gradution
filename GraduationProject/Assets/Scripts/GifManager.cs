
using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
public class GifManager : MonoBehaviour
{
    //帧数(数值越大播放速度越快)
    private const float Fps = 24;
    private float _time;

    Image image;
    public List<Sprite> sprites = new List<Sprite>();
    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void SetPath(string path)
    {
        sprites.Clear();
        sprites .AddRange( Resources.LoadAll<Sprite>("SkillShow/"+path));
    }
    private void Update()
    {
        if (sprites.Count <= 0) return;
   
        _time += Time.deltaTime;
        var index = (int)(_time * Fps) % sprites.Count;
        if (image != null)
        {
            image.sprite = sprites[index];
        }
    }

}
 