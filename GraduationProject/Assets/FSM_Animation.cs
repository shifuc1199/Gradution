using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public enum InvokeTime
{
    Enter,
    Stay,
    Exit
}
[System.Serializable]
public class Animation_Event_Type
{
    public InvokeTime type;
    public string _eventname;
}

public class FSM_Animation: StateMachineBehaviour
{
    
    public List<Animation_Event_Type> _eventname;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
         
            foreach (var item in _eventname)
            {
               if(item.type== InvokeTime.Enter)
                animator.SendMessage(item._eventname);
            }
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var item in _eventname)
        {
            if (item.type == InvokeTime.Stay)
                animator.SendMessage(item._eventname);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var item in _eventname)
        {
            if (item.type == InvokeTime.Exit)
                animator.SendMessage(item._eventname);
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
