/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DreamerTool.UI;
using UnityEngine.Events;
public class BagJoyStick : JoyStick
{
    public Image up_button;
    public Image right_button;
    public Image left_button;
    float timer = 0;
    UnityAction move_action;
    public override void onJoystickDown(Vector2 V)
    {
        base.onJoystickDown(V);
        ChangeButton(V);
        isDown = true;
    }
    public override void onJoystickUp(Vector2 V)
    {
        base.onJoystickUp(V);
        up_button.CrossFadeColor(Color.white, 0.1f, true, true);
        left_button.CrossFadeColor(Color.white, 0.1f, true, true);
        right_button.CrossFadeColor(Color.white, 0.1f, true, true);
        timer = 0;
        isDown = false;

    }
    public override void onJoystickMove(Vector2 V)
    {
        base.onJoystickMove(V);
        ChangeButton(V);
    }
    bool isDown = false;
    private void Update()
    {
        if (isDown)
        {
            timer += Time.deltaTime;
            if(timer>=0.08f)
            {
                timer = 0;
                if(move_action!=null)
                move_action.Invoke();
            }
        }
    }

    public void ChangeButton(Vector2 V)
    {
        if (V.y >= 0.9f && Mathf.Abs(V.x) <= 0.8f)
        {
            up_button.CrossFadeColor(Color.red, 0.1f, true, true);
            right_button.CrossFadeColor(Color.white, 0.1f, true, true);
            left_button.CrossFadeColor(Color.white, 0.1f, true, true);
        }
        else if (V.x > 0.8f && V.y < 0.8f)
        {
            move_action = View.CurrentScene.GetView<PlayerInfoAndBagView>().bag_view.SelectRight;
            right_button.CrossFadeColor(Color.red, 0.1f, true, true);
            left_button.CrossFadeColor(Color.white, 0.1f, true, true);
            up_button.CrossFadeColor(Color.white, 0.1f, true, true);
        }
        else if (V.x < -0.8f && V.y < 0.8f)
        {
            move_action = View.CurrentScene.GetView<PlayerInfoAndBagView>().bag_view.SelectLeft;
            left_button.CrossFadeColor(Color.red, 0.1f, true, true);
            right_button.CrossFadeColor(Color.white, 0.1f, true, true);
            up_button.CrossFadeColor(Color.white, 0.1f, true, true);
        }

    }
}
