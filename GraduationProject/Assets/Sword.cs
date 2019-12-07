using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Sword : MonoBehaviour
{
    private Animator _anim;
    public HitType attack_type;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInParent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="Enemy")
        {
            Camera.main.DOShakePosition(0.1f, 1);
            collision.gameObject.GetComponent<IHurt>().GetHurt(attack_type,()=> { collision.gameObject.transform.rotation = transform.rotation; });
        }
    }
 
    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale!=1)
        {
            timer += Time.unscaledDeltaTime;
            if(timer>=0.1f)
            {
               
                timer = 0;
                Time.timeScale = 1f;
            }
        }
    }
}
