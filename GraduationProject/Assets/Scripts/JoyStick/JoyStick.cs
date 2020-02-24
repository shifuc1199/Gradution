using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Transform bound;
    public Transform center;
    public bool isDisable = false;
    public float radius;
 
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDisable)
            return;

        Vector2 dir = eventData.position - (Vector2)bound.position;
        float r = dir.magnitude;
        r = Mathf.Clamp(r, 0, radius);
      
        center.localPosition = dir.normalized * r ;
        onJoystickDown(dir.normalized,r);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDisable)
            return;

        Vector2 dir = eventData.position - (Vector2)bound.position;
        float r = dir.magnitude;
        r = Mathf.Clamp(r, 0, radius);
        center.localPosition = Vector2.zero;
        onJoystickUp(dir.normalized,r);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDisable)
            return;

        Vector2 dir = eventData.position - (Vector2)bound.position;
     
        float r = dir.magnitude;
        r = Mathf.Clamp(r, 0, radius);
        center.localPosition = dir.normalized * r ;

        onJoystickMove(dir.normalized,r);
    }

    public virtual void onJoystickDown(Vector2 V,float R)
    {

    }
    public virtual void onJoystickUp(Vector2 V, float R)
    {

    }
    public virtual void onJoystickMove(Vector2 V, float R)
    {

    }
}
