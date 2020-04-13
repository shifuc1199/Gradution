using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MoveJoyStick : JoyStick
{

    public override void onJoystickDown(Vector2 V,float R)
    {
        base.onJoystickDown(V,R);
        ChangeButton(V);
    }
    public override void onJoystickUp(Vector2 V, float R)
    {
        base.onJoystickUp(V,R);
        ActorController.Controller.actor_state.isAttackDown = false;
        ActorController.Controller.actor_state.isAttackUp = false;
        ActorController.Controller.actor_state.isMoveRight = false;
        ActorController.Controller.actor_state.isMoveLeft = false;
    }
    public override void onJoystickMove(Vector2 V, float R)
    {
        base.onJoystickMove(V,R);
        ChangeButton(V);
    }

    public void ChangeButton(Vector2 V)
    {
        if (V.y >= 0.9f && Mathf.Abs(V.x) <= 0.8f)
        {
            ActorController.Controller.actor_state.isAttackUp = true;
            ActorController.Controller.actor_state.isMoveRight = false;
            ActorController.Controller.actor_state.isMoveLeft = false;
            ActorController.Controller.actor_state.isAttackDown = false;

        }
        else if (V.y <= -0.9f && Mathf.Abs(V.x) <= 0.8f)
        {
            ActorController.Controller.actor_state.isAttackDown = true;
            ActorController.Controller.actor_state.isAttackUp = false;
            ActorController.Controller.actor_state.isMoveRight = false;
            ActorController.Controller.actor_state.isMoveLeft = false;
        }
        else if  (V.x > 0.8f && V.y< 0.8f)
        {
            ActorController.Controller.actor_state.isAttackUp = false;
            ActorController.Controller.actor_state.isMoveRight = true;
            ActorController.Controller.actor_state.isMoveLeft = false;
            ActorController.Controller.actor_state.isAttackDown = false;

        }
        else if (V.x < -0.8f && V.y < 0.8f)
        {
            ActorController.Controller.actor_state.isAttackUp = false;
            ActorController.Controller.actor_state.isMoveRight = false;
            ActorController.Controller.actor_state.isMoveLeft = true;
            ActorController.Controller.actor_state.isAttackDown = false;

        }

    }
 
}
