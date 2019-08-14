using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Touch : MonoBehaviour
{
    public Image center;
    public Image bound;
    public float radius;

    Vector3 center_start_pos;

    public Vector3 dir;

    private void Awake()
    {
        EventTrigger trigger = center.gameObject.AddComponent<EventTrigger>();
        UnityAction<BaseEventData> _movestart = new UnityAction<BaseEventData>(OnMoveStart);
        UnityAction<BaseEventData> _move = new UnityAction<BaseEventData>(OnMove);
        UnityAction<BaseEventData> _move_end = new UnityAction<BaseEventData>(OnMoveEnd);

        EventTrigger.Entry entry_move_start = new EventTrigger.Entry();
        entry_move_start.eventID = EventTriggerType.BeginDrag;
        entry_move_start.callback.AddListener(_movestart);

        EventTrigger.Entry entry_move = new EventTrigger.Entry();
        entry_move.eventID = EventTriggerType.Drag;
        entry_move.callback.AddListener(_move);

        EventTrigger.Entry entry_move_end = new EventTrigger.Entry();
        entry_move_end.eventID = EventTriggerType.EndDrag;
        entry_move_end.callback.AddListener(_move_end);


        trigger.triggers.Add(entry_move_start);
        trigger.triggers.Add(entry_move);
        trigger.triggers.Add(entry_move_end);

        center_start_pos = center.transform.position;
    }

    public virtual void OnMoveStart(BaseEventData data)
    {
        Debug.Log("start");
    }

    public virtual void OnMove(BaseEventData data)
    {
        Vector3 dir = Input.mousePosition - center_start_pos;
        this.dir = dir.normalized;
        float r = dir.magnitude;
        r = Mathf.Clamp(r,0, radius);
        center.transform.position = this.dir * r + center_start_pos;
        Debug.Log(center.transform.position);
        Debug.Log("move");
    }


    public virtual void OnMoveEnd(BaseEventData data)
    {
         
        center.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
         
    }



}
