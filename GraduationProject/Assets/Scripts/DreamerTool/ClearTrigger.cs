using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Trigger_Event_Type
{
    public InvokeTime type;
    public string _trigger_name;
}
public class ClearTrigger : StateMachineBehaviour
{
    public List<Trigger_Event_Type> trigger_name;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        foreach (var item in trigger_name)
        {
            if (item.type == InvokeTime.Enter)
                animator.ResetTrigger (item._trigger_name);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var item in trigger_name)
        {
            if (item.type == InvokeTime.Stay)
                animator.ResetTrigger(item._trigger_name);
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var item in trigger_name)
        {
            if (item.type == InvokeTime.Exit)
                animator.ResetTrigger(item._trigger_name);
        }
    }
}
