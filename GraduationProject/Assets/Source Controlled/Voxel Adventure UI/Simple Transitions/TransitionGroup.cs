using UnityEngine;
using System.Collections;

public class TransitionGroup : MonoBehaviour
{
#if(UNITY_EDITOR)
    public string label;//this will be stripped out when you compile for any platform so don't use this variable!!
#endif

    public TransitionalObject[] transitions;
    public TransitionGroup[] transitionGroups;

    public bool triggerInstantly;

    public float transitionInTime, fadeOutTime, delay, fadeOutDelay, displayTime;//the time is an override
    public TransitionalObject.Operator transitionTimeOperator, delayOperator, fadeOutDelayOperator, fadeOutTimeOperator, displayTimeOperator;

    bool initialised;

    void Awake()
    {
        if(initialised)
            return;//only run once! Useful for groups within groups

        Initialise();

        initialised = true;

        if(triggerInstantly)
            TriggerGroupTransition();
    }

    public void Initialise()
    {
        #region Delay
        SetDelay(delay);

        for(int i = 0; i < transitionGroups.Length; i++)
            if(delayOperator == TransitionalObject.Operator.Stagger)
                transitionGroups[i].SetDelay(delay * i);
            else
                transitionGroups[i].SetDelay(delay);
        #endregion

        #region Transition In Time
        SetTransitionInTime(transitionInTime);

        for(int i = 0; i < transitionGroups.Length; i++)
            if(transitionTimeOperator == TransitionalObject.Operator.Stagger)
                transitionGroups[i].SetTransitionInTime(transitionInTime * i);
            else
                transitionGroups[i].SetTransitionInTime(transitionInTime);
        #endregion


        #region Display Time
        SetDisplayTime(displayTime);

        for(int i = 0; i < transitionGroups.Length; i++)
            if(displayTimeOperator == TransitionalObject.Operator.Stagger)
                transitionGroups[i].SetDisplayTime(displayTime * i);
            else
                transitionGroups[i].SetDisplayTime(displayTime);
        #endregion

        #region Fade Out Delay
        SetFadeOutDelay(fadeOutDelay);

        for(int i = 0; i < transitionGroups.Length; i++)
            if(transitionTimeOperator == TransitionalObject.Operator.Stagger)
                transitionGroups[i].SetFadeOutDelay(fadeOutDelay * i);
            else
                transitionGroups[i].SetFadeOutDelay(fadeOutDelay);
        #endregion

        #region Fade Out Time
        SetFadeOutTime(fadeOutTime);

        for(int i = 0; i < transitionGroups.Length; i++)
            if(transitionTimeOperator == TransitionalObject.Operator.Stagger)
                transitionGroups[i].SetFadeOutTime(fadeOutTime * i);
            else
                transitionGroups[i].SetFadeOutTime(fadeOutTime);
        #endregion
    }

    #region Set Variables
    public void SetTransitionInTime(float value)
    {
        transitionInTime = value;

        for(int i = 0; i < transitions.Length; i++)
            if(transitionInTime != 0)
                switch(transitionTimeOperator)
                {
                    case TransitionalObject.Operator.Override:
                        transitions[i].TransitionInTime = value;//direct override
                        break;

                    case TransitionalObject.Operator.Stagger://basically apply a staggering effect to the group
                        transitions[i].ModifyTransitionInTime(value * i, TransitionalObject.Operator.Add);
                        break;

                    case TransitionalObject.Operator.Add://the rest are basic math functions
                        transitions[i].ModifyTransitionInTime(value, TransitionalObject.Operator.Add);
                        break;

                    case TransitionalObject.Operator.Subtract:
                        transitions[i].ModifyTransitionInTime(value, TransitionalObject.Operator.Subtract);
                        break;

                    case TransitionalObject.Operator.Multiply:
                        transitions[i].ModifyTransitionInTime(value, TransitionalObject.Operator.Multiply);
                        break;

                    case TransitionalObject.Operator.Divide:
                        transitions[i].ModifyTransitionInTime(value, TransitionalObject.Operator.Divide);
                        break;
                }
    }

    public void SetFadeOutTime(float value)
    {
        fadeOutTime = value;

        for(int i = 0; i < transitions.Length; i++)
            if(fadeOutTime != 0)
                switch(fadeOutTimeOperator)
                {
                    case TransitionalObject.Operator.Override:
                        transitions[i].FadeOutTime = value;//direct override
                        break;

                    case TransitionalObject.Operator.Stagger://basically apply a staggering effect to the group
                        transitions[i].ModifyFadeOutTime(value * i, TransitionalObject.Operator.Add);
                        break;

                    case TransitionalObject.Operator.Add://the rest are basic math functions
                        transitions[i].ModifyFadeOutTime(value, TransitionalObject.Operator.Add);
                        break;

                    case TransitionalObject.Operator.Subtract:
                        transitions[i].ModifyFadeOutTime(value, TransitionalObject.Operator.Subtract);
                        break;

                    case TransitionalObject.Operator.Multiply:
                        transitions[i].ModifyFadeOutTime(value, TransitionalObject.Operator.Multiply);
                        break;

                    case TransitionalObject.Operator.Divide:
                        transitions[i].ModifyFadeOutTime(value, TransitionalObject.Operator.Divide);
                        break;
                }
    }
    
    public void SetDelay(float value)
    {
        delay = value;

        for(int i = 0; i < transitions.Length; i++)
            if(delay != 0)
                switch(delayOperator)
                {
                    case TransitionalObject.Operator.Override:
                        transitions[i].Delay = value;//direct override
                        break;

                    case TransitionalObject.Operator.Stagger://basically apply a staggering effect to the group
                        transitions[i].ModifyDelay(value * i,  TransitionalObject.Operator.Add);
                        break;

                    case TransitionalObject.Operator.Add://the rest are basic math functions
                        transitions[i].ModifyDelay(value,  TransitionalObject.Operator.Add);
                        break;

                    case TransitionalObject.Operator.Subtract:
                        transitions[i].ModifyDelay(value,  TransitionalObject.Operator.Subtract);
                        break;

                    case TransitionalObject.Operator.Multiply:
                        transitions[i].ModifyDelay(value,  TransitionalObject.Operator.Multiply);
                        break;

                    case TransitionalObject.Operator.Divide:
                        transitions[i].ModifyDelay(value,  TransitionalObject.Operator.Divide);
                        break;
                }
    }

    public void SetFadeOutDelay(float value)
    {
        fadeOutDelay = value;

        for(int i = 0; i < transitions.Length; i++)
            if(fadeOutDelay != 0)
                switch(fadeOutDelayOperator)
                {
                    case TransitionalObject.Operator.Override:
                        transitions[i].FadeOutDelay = value;//direct override
                        break;

                    case TransitionalObject.Operator.Stagger://basically apply a staggering effect to the group
                        transitions[i].ModifyFadeOutDelay(value * i,  TransitionalObject.Operator.Add);
                        break;

                    case TransitionalObject.Operator.Add://the rest are basic math functions
                        transitions[i].ModifyFadeOutDelay(value,  TransitionalObject.Operator.Add);
                        break;

                    case TransitionalObject.Operator.Subtract:
                        transitions[i].ModifyFadeOutDelay(value,  TransitionalObject.Operator.Subtract);
                        break;

                    case TransitionalObject.Operator.Multiply:
                        transitions[i].ModifyFadeOutDelay(value,  TransitionalObject.Operator.Multiply);
                        break;

                    case TransitionalObject.Operator.Divide:
                        transitions[i].ModifyFadeOutDelay(value,  TransitionalObject.Operator.Divide);
                        break;
                }
    }

    public void SetDisplayTime(float value)
    {
        displayTime = value;

        for(int i = 0; i < transitions.Length; i++)
            if(displayTime != 0)
                switch(displayTimeOperator)
                {
                    case TransitionalObject.Operator.Override:
                        transitions[i].DisplayTime = value;//direct override
                        break;

                    case TransitionalObject.Operator.Stagger://basically apply a staggering effect to the group
                        transitions[i].ModifyDisplayTime(value * i,  TransitionalObject.Operator.Add);
                        break;

                    case TransitionalObject.Operator.Add://the rest are basic math functions
                        transitions[i].ModifyDisplayTime(value,  TransitionalObject.Operator.Add);
                        break;

                    case TransitionalObject.Operator.Subtract:
                        transitions[i].ModifyDisplayTime(value,  TransitionalObject.Operator.Subtract);
                        break;

                    case TransitionalObject.Operator.Multiply:
                        transitions[i].ModifyDisplayTime(value,  TransitionalObject.Operator.Multiply);
                        break;

                    case TransitionalObject.Operator.Divide:
                        transitions[i].ModifyDisplayTime(value,  TransitionalObject.Operator.Divide);
                        break;
                }
    }
    #endregion

    public void TriggerGroupTransition()
    {
        TriggerGroupTransition(true);
    }

    public void TriggerGroupTransition(bool reset)
    {
        if(!gameObject.activeSelf)
            gameObject.SetActive(true);//this also helps ensure Awake is called and thus delays etc are set at the correct time

        for(int i = 0; i < transitions.Length; i++)
            transitions[i].TriggerTransitionWithReset(reset);

        for(int i = 0; i < transitionGroups.Length; i++)
            transitionGroups[i].TriggerGroupTransition(reset);
    }

    public void TriggerGroupFadeOut()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].TriggerFadeOut();

        for(int i = 0; i < transitionGroups.Length; i++)
            transitionGroups[i].TriggerGroupFadeOut();
    }

    public void StopGroup()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].Stop();

        for(int i = 0; i < transitionGroups.Length; i++)
            transitionGroups[i].StopGroup();
    }

    public void JumpToStart()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].JumpToStart();

        for(int i = 0; i < transitionGroups.Length; i++)
            transitionGroups[i].JumpToStart();
    }

    public void JumpToEnd()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].JumpToEnd();

        for(int i = 0; i < transitionGroups.Length; i++)
            transitionGroups[i].JumpToEnd();
    }

    public void ActivateAll(bool activate)
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].gameObject.SetActive(activate);
    }

    /// <summary>
    /// This should be called when you add new objects that need either an alpha or colour transition. It adds them to our animtion queue
    /// </summary>
    public void InitialiseAlphaTransitions()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].InitialiseAlphaTransition();

        for(int i = 0; i < transitionGroups.Length; i++)
            transitionGroups[i].InitialiseAlphaTransitions();
    }
}