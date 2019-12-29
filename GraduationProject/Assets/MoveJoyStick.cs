using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MoveJoyStick : JoyStick
{
    
    public Image up_button;
    public Image right_button;
    public Image left_button;
    public override void onJoystickDown(Vector2 V)
    {
        base.onJoystickDown(V);
        ChangeButton(V);
    }
    public override void onJoystickUp(Vector2 V)
    {
        base.onJoystickUp(V);
        up_button.CrossFadeColor(Color.white, 0.1f, true, true);
        left_button.CrossFadeColor(Color.white, 0.1f, true, true);
        right_button.CrossFadeColor(Color.white, 0.1f, true, true);
        ActorController._controller.actor_state.isAttackUp = false;
        ActorController._controller.actor_state.isMoveRight = false;
        ActorController._controller.actor_state.isMoveLeft = false;
    }
    public override void onJoystickMove(Vector2 V)
    {
        base.onJoystickMove(V);
        ChangeButton(V);

    }

    public void ChangeButton(Vector2 V)
    {
        if (V.y >= 0.9f && Mathf.Abs(V.x) <= 0.8f)
        {
            ActorController._controller.actor_state.isAttackUp = true;
            ActorController._controller.actor_state.isMoveRight = false;
            ActorController._controller.actor_state.isMoveLeft = false;
            up_button.CrossFadeColor(Color.red, 0.1f, true, true);
            right_button.CrossFadeColor(Color.white, 0.1f, true, true);
            left_button.CrossFadeColor(Color.white, 0.1f, true, true);
        }
        else if  (V.x > 0.8f && V.y< 0.8f)
        {
            ActorController._controller.actor_state.isAttackUp = false;
            ActorController._controller.actor_state.isMoveRight = true;
            ActorController._controller.actor_state.isMoveLeft = false;
            right_button.CrossFadeColor(Color.red, 0.1f, true, true);
            left_button.CrossFadeColor(Color.white, 0.1f, true, true);
            up_button.CrossFadeColor(Color.white, 0.1f, true, true);
        }
        else if (V.x < -0.8f && V.y < 0.8f)
        {
            ActorController._controller.actor_state.isAttackUp = false;
            ActorController._controller.actor_state.isMoveRight = false;
            ActorController._controller.actor_state.isMoveLeft = true;
            left_button.CrossFadeColor(Color.red, 0.1f, true, true);
            right_button.CrossFadeColor(Color.white, 0.1f, true, true);
            up_button.CrossFadeColor(Color.white, 0.1f, true, true);
        }

    }
 
}
