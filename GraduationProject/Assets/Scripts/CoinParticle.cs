/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
public class CoinParticle : MonoBehaviour
{
    private ParticleSystem par;
    ParticleSystem.Particle[] arrPar;
    public RectTransform transf;
    int arrCount;
    public float speed = 1000f;
    bool isopen;
    // Start is called before the first frame update
    void Start()
    {
        par = GetComponent<ParticleSystem>();
        arrPar = new ParticleSystem.Particle[par.main.maxParticles];
    }
    private void OnEnable()
    {
        Timer.Register(1.25f, () => {
             
            par.gravityModifier = 0;
            isopen = true;
        });
    }
    private void OnDisable()
    {
        par.gravityModifier = 1;
        isopen = false;
        ActorModel.Model.SetMoney(100);
    }
 
// Update is called once per frame
void Update()
    {
        if (isopen)
        {
 
           
            arrCount = par.GetParticles(arrPar);
            for (var i = 0; i < arrCount; i++)
            {
                arrPar[i].position =Vector3.MoveTowards(arrPar[i].position, Camera.main.ScreenToWorldPoint(Scene.UICamera.WorldToScreenPoint(transf.position)), speed);//设置他们的位置
            }
            par.SetParticles(arrPar, arrCount);
        }
    }
}
