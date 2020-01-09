using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extra
{ 
    public static void ResetVelocity(this Rigidbody2D _rigi)
    {
        _rigi.velocity = Vector2.zero;
    }
    public static void ClearGravity(this Rigidbody2D _rigi)
    {
        _rigi.gravityScale = 0; 
    }
    public static Color GetColorByString(this string color_s)
    {
        Color result;
        ColorUtility.TryParseHtmlString(color_s, out result);
        return result;
    }
    public static void SetGravity(this Rigidbody2D _rigi,float v)
    {
        _rigi.gravityScale = v;
    }
    public static bool IsAnim(this Animator animator,string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }
}

