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
    public float radius;
 
    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 dir = eventData.position - (Vector2)bound.position;
        float r = dir.magnitude;
        r = Mathf.Clamp(r, 0, radius);
      
        center.localPosition = dir.normalized * r ;
        onJoystickDown(dir.normalized);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 dir = eventData.position - (Vector2)bound.position;
        float r = dir.magnitude;
        r = Mathf.Clamp(r, 0, radius);
        center.localPosition = Vector2.zero;
        onJoystickUp(dir.normalized);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dir = eventData.position - (Vector2)bound.position;
     
        float r = dir.magnitude;
        r = Mathf.Clamp(r, 0, radius);
        center.localPosition = dir.normalized * r ;

        onJoystickMove(dir.normalized);
    }

    public virtual void onJoystickDown(Vector2 V)
    {

    }
    public virtual void onJoystickUp(Vector2 V)
    {

    }
    public virtual void onJoystickMove(Vector2 V)
    {

    }
}
