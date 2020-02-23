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
        ActorController._controller.actor_state.isAttackUp = false;
        ActorController._controller.actor_state.isMoveRight = false;
        ActorController._controller.actor_state.isMoveLeft = false;
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
            ActorController._controller.actor_state.isAttackUp = true;
            ActorController._controller.actor_state.isMoveRight = false;
            ActorController._controller.actor_state.isMoveLeft = false;

        }
        else if  (V.x > 0.8f && V.y< 0.8f)
        {
            ActorController._controller.actor_state.isAttackUp = false;
            ActorController._controller.actor_state.isMoveRight = true;
            ActorController._controller.actor_state.isMoveLeft = false;

        }
        else if (V.x < -0.8f && V.y < 0.8f)
        {
            ActorController._controller.actor_state.isAttackUp = false;
            ActorController._controller.actor_state.isMoveRight = false;
            ActorController._controller.actor_state.isMoveLeft = true;

        }

    }
 
}
