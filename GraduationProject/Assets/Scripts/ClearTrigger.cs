using System.Collections;
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
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        foreach (var item in trigger_name)
        {
            if (item.type == InvokeTime.Enter)
                animator.ResetTrigger (item._trigger_name);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var item in trigger_name)
        {
            if (item.type == InvokeTime.Stay)
                animator.ResetTrigger(item._trigger_name);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var item in trigger_name)
        {
            if (item.type == InvokeTime.Exit)
                animator.ResetTrigger(item._trigger_name);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
