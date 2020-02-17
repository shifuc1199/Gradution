using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class SkillJoyStick : JoyStick
{
    public int Skill_ID;
    public SplatManager splat_manager;
    SkillConfig _config;
    private void Awake()
    {
        _config = SkillConfig.Get(Skill_ID);
    }
    public override void onJoystickDown(Vector2 V,float R)
    {
        base. onJoystickDown(V,R);
        splat_manager.SelectSpellIndicator("skill" + _config.ID + "_indicator");
        switch (_config.skill_type)
        {
            case SkillType.点:
                splat_manager.CurrentSpellIndicator.transform.position = (Vector3)V * R + ActorController._controller.transform.position;
                 
                break;
            case SkillType.线:
                splat_manager.CurrentSpellIndicator.transform.rotation = Quaternion.FromToRotation(Vector2.up, V);
                 
                break;
            default:
                break;
        }
        
    }
    public override void onJoystickUp(Vector2 V, float R)
    {
        base.onJoystickUp(V,R);
        splat_manager.CancelSpellIndicator();
        ActorController._controller.skill_controller.ExecuteSkill(Skill_ID,V, (Vector3)V * R + ActorController._controller.transform.position);
    }
    public override void onJoystickMove(Vector2 V, float R)
    {
        base.onJoystickMove(V,R);
        switch (_config.skill_type)
        {
            case SkillType.点:
                splat_manager.CurrentSpellIndicator.transform.position =  (Vector3)V*R+ActorController._controller.transform.position;
                break;
            case SkillType.线:
                splat_manager.CurrentSpellIndicator.transform.rotation = Quaternion.FromToRotation(Vector2.up, V);
                break;
            default:
                break;
        }
       
    }
}
