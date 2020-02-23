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
 
    float timer = 0;
    UnityAction move_action;
    public override void onJoystickDown(Vector2 V,float R)
    {
        base.onJoystickDown(V,R);
        ChangeButton(V);
        isDown = true;
    }
    public override void onJoystickUp(Vector2 V, float R)
    {
        base.onJoystickUp(V,R);
       

        if(timer<0.15f&&timer>0)
        {
            if (move_action != null)
                move_action.Invoke();
        }
        timer = 0;
        isDown = false;

    }
    public override void onJoystickMove(Vector2 V, float R)
    {
        base.onJoystickMove(V,R);
        ChangeButton(V);
    }
    bool isDown = false;
    private void Update()
    {
        if (isDown)
        {
            timer += Time.deltaTime;
            if(timer>=0.15f)
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
            move_action = View.CurrentScene.GetView<PlayerInfoAndBagView>().bag_view.SelectUp;
          
        }
        else if (V.y <= -0.9f && Mathf.Abs(V.x) <= 0.8f)
        {
            move_action = View.CurrentScene.GetView<PlayerInfoAndBagView>().bag_view.SelectDown;
           
        }
        else if (V.x > 0.8f && V.y < 0.8f)
        {
            move_action = View.CurrentScene.GetView<PlayerInfoAndBagView>().bag_view.SelectRight;
            
        }
        else if (V.x < -0.8f && V.y < 0.8f)
        {
            move_action = View.CurrentScene.GetView<PlayerInfoAndBagView>().bag_view.SelectLeft;
           
        }

    }
}
