using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlayerAnimationEvent : MonoBehaviour
{
    public GameObject sword_slash_prefab;
    public GameObject attack_trigger;
    List<int> effect_rotation = new List<int>() { 60, 100, 80, 120 };
    public void ShootBullet() //远程攻击
    {
        GameObject temp = Instantiate(Resources.Load<GameObject>("Bullet"), GetComponent<PlayerController>()._shoot_pos.position, transform.rotation);
    }
    public void ResetTrigger(string _name)
    {
        GetComponent<Animator>().ResetTrigger(_name);
    }
    public void SetSlash(int index)
    {
        
         
        GameObject temp = Instantiate(sword_slash_prefab, transform.position, Quaternion.Euler(transform.eulerAngles.y,90, transform.eulerAngles.y+ effect_rotation[index]));
        if(index==3)
        {
            temp.transform.position += new Vector3(0, 0, -1);
        }
       // Destroy(temp, 2);
    }
    public void SetAttackTrigger()
    {
        attack_trigger.SetActive(true);
    }
    public void OnAttackEnter()
    {

    }

    public void OnAttackExit()
    {
        attack_trigger.SetActive(false);
        Debug.Log("exit");
    }

}

