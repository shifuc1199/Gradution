using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using DreamerTool.GameObjectPool;

using DG.Tweening;
/// <summary>
/// 基于SpriteRenderer的幻影插件，可以用于2D幻影
/// </summary>
public class AfterImage : MonoBehaviour
{
    public bool IsUpdate;
    public float timer_interval;
    public float live_time;
    public Color color;
    float timer;
    SpriteRenderer[] renderers;
    private void Start()
    {
          renderers = GetComponentsInChildren<SpriteRenderer>();
    }
    private void Update()
    {
       if(IsUpdate)
        {
            timer += Time.deltaTime;
            if (timer >= timer_interval)
            {
                 
                var player_sprite = GameObjectPoolManager.GetPool("shadow").Get(transform.position, transform.rotation, live_time);
                var aim = player_sprite.GetComponentsInChildren<SpriteRenderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    aim[i].transform.position = renderers[i].transform.position;
                    aim[i].transform.rotation = renderers[i].transform.rotation;
                    aim[i].transform.localScale = renderers[i].transform.localScale;
                    aim[i].color = color;
                    aim[i].DOFade(0.8f, 0);
                    aim[i].sortingOrder = 0;
                    aim[i].DOFade(0, live_time).SetEase(Ease.Linear);
                }
                timer = 0;
            }
        }
    }
}
 