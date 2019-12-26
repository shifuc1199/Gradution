using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MoveJoyStick : JoyStick
{
    public Image[] buttons;
    public override void onJoystickDown(Vector2 V)
    {
        base.onJoystickDown(V);
        ChangeButton(V);
    }
    public override void onJoystickUp(Vector2 V)
    {
        base.onJoystickUp(V);
        buttons[2].CrossFadeColor(Color.white, 0.1f, true, true);
        buttons[0].CrossFadeColor(Color.white, 0.1f, true, true);
        buttons[1].CrossFadeColor(Color.white, 0.1f, true, true);
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
            buttons[2].CrossFadeColor(Color.red, 0.1f, true, true);
            buttons[0].CrossFadeColor(Color.white, 0.1f, true, true);
            buttons[1].CrossFadeColor(Color.white, 0.1f, true, true);
        }
        else if  (V.x > 0.8f && V.y< 0.8f)
        {
            buttons[0].CrossFadeColor(Color.red, 0.1f, true, true);
            buttons[1].CrossFadeColor(Color.white, 0.1f, true, true);
            buttons[2].CrossFadeColor(Color.white, 0.1f, true, true);
        }
        else if (V.x < -0.8f && V.y < 0.8f)
        {
            buttons[1].CrossFadeColor(Color.red, 0.1f, true, true);
            buttons[2].CrossFadeColor(Color.white, 0.1f, true, true);
            buttons[0].CrossFadeColor(Color.white, 0.1f, true, true);
        }

    }
 
}
