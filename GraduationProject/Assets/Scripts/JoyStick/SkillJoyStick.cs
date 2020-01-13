using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Werewolf.StatusIndicators.Components;
public class SkillJoyStick : JoyStick
{
    public int Skill_ID;
    public SplatManager splat_manager;

    private void Awake()
    {
      
    }
    public override void onJoystickDown(Vector2 V)
    {
        base.onJoystickDown(V);
        splat_manager.SelectSpellIndicator("Skill1_Indicator");
        splat_manager.CurrentSpellIndicator.transform.rotation = Quaternion.FromToRotation(Vector2.up, V);
    }
    public override void onJoystickUp(Vector2 V)
    {
        base.onJoystickUp(V);
        splat_manager.CancelSpellIndicator();
        ActorController._controller.skill_controller.ExecuteSkill(Skill_ID,V,V);
    }
    public override void onJoystickMove(Vector2 V)
    {
        base.onJoystickMove(V);
        splat_manager.CurrentSpellIndicator.transform.rotation = Quaternion.FromToRotation(Vector2.up, V);
    }
}
