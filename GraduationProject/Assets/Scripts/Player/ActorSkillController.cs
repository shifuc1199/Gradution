using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSkillController : MonoBehaviour
{
    public Vector2 SkillDirection;
    public Vector2 SkillPos;
    private Animator _anim;
    private void Awake() {
        _anim = GetComponentInChildren<Animator>();
    }
    public void ExecuteSkill(int skill_id,Vector2 dir,Vector2 pos)
    {
        if(dir.x>0)
        {
            transform.rotation = Quaternion.identity;
        }
        else if (dir.x < 0)
        {
            transform.rotation = Quaternion.Euler(0,180,0);
        }
       
        SkillPos = pos;
        SkillDirection = dir;

        _anim.SetTrigger("skill"+ skill_id.ToString());
         
    }
}
