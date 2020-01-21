using UnityEngine;
using System.Collections;
using UnityEngine.Events;

#if(UNITY_EDITOR)
using UnityEditor;
using UnityEditor.AnimatedValues;
#endif

namespace TransitionalObjects
{
    [System.Serializable]
#if(UNITY_EDITOR)
    [ExecuteInEditMode]
#endif
    public class BaseTransition : MonoBehaviour
    {
        #region Variables
#if(UNITY_EDITOR)
        public string label = "";//never call this other than from the editor!!

        public AnimBool mainDropDown = new AnimBool(), transitionInDropDown = new AnimBool(),
            fadeOutDropDown = new AnimBool(), messagingDropDown = new AnimBool(),
            loopingDropDown = new AnimBool(), dataDropDown = new AnimBool();//these are for the editor. They are stored here to help act as saving your preferences whe viewing each object

        public Keyframe[] previousTransitionInCurve, previousFadeOutCurve;

        public bool stayForever = true, copyTransitionInTime = true;//used for fadeOutTime
#endif

        public enum TransitionState { Delay = 0, TransitionIn, Waiting, FadeOutDelay, FadingOut, Finished, AnyState }

        public TransitionalObject parent;

        public TransitionState state = TransitionState.Finished;
        public float currentTime, delay;

        public float transitionInTime = 0.5f, fadeOutTime = 0.5f, displayTime = -1, fadeOutDelay;
        public AnimationCurve transitionInCurve, fadeOutCurve;

        public bool looping, triggerInstantly;

        #region Messaging
        public bool messagingEnabled = true;//used to temporary enable and disable messages
        public UnityEvent[] events;
        public TransitionState[] whenToSends;
        #endregion

        /// <summary>
        /// This returns a normalised value for the transitions animation. It completely ignores the curve values!!
        /// </summary>
        public float TransitionPercentage
        {
            get
            {
                if(state < TransitionState.Waiting)
                    return currentTime / transitionInTime;
                else if(state > TransitionState.Waiting)
                    return currentTime / fadeOutTime;
                else
                    return 1;//the waiting state
            }

            set
            {
                if(state <= TransitionState.Waiting)
                    currentTime = transitionInTime * value;
                else
                    currentTime = fadeOutTime * value;
            }
        }

        /// <summary>
        /// Returns the current time in relation to the curve
        /// </summary>
        public float CurrentValue
        {
            get
            {
                if(state < TransitionState.Waiting)
                    return transitionInCurve.Evaluate(currentTime / transitionInTime);
                else if(state == TransitionState.Waiting)
                    return 1;
                else
                    return fadeOutCurve.Evaluate(currentTime / fadeOutTime);
            }
        }
        #endregion

        public void CustomUpdate(float delta)
        {
            switch(state)
            {
                case TransitionState.Delay:
                    currentTime += delta;

                    if(delay == 0 || (currentTime > delay && delay > 0) || (delay < 0 && currentTime > delay * -1))//take into consideration negative delays..
                    {

                        if(delay < 0)
                            delay = 0;//negative delays serve as one shot delays

                        currentTime = 0;
                        state = TransitionState.TransitionIn;
                        CheckMessage();
                    }
                    break;

                case TransitionState.TransitionIn:
                    currentTime += delta;

                    Transition();

                    if(currentTime > transitionInTime)
                    {
                        currentTime -= transitionInTime;
                        state = TransitionState.Waiting;

                        LockTransition(true);
                        CheckMessage();
                    }
                    break;

                case TransitionState.Waiting:
                    if(displayTime < 0)
                        return;//this means wait indefinitely until another function is called

                    currentTime += delta;

                    if(currentTime > displayTime)
                        TriggerFadeOut();
                    break;

                case TransitionState.FadeOutDelay:
                    currentTime += delta;

                    if(fadeOutDelay == 0 || (currentTime > fadeOutDelay && fadeOutDelay > 0) || (fadeOutDelay < 0 && currentTime > fadeOutDelay * -1))//take into consideration negative delays..
                    {
                        if(fadeOutDelay < 0)
                            fadeOutDelay = 0;//negative delays serve as one shot delays

                        currentTime = fadeOutTime;
                        state = TransitionState.FadingOut;
                        CheckMessage();
                    }
                    break;

                case TransitionState.FadingOut:
                    currentTime -= delta;

                    Transition();

                    if(currentTime < 0)
                    {
                        currentTime = 0;

                        if(looping)
                            state = TransitionState.Delay;//loop infinitely
                        else
                            state = TransitionState.Finished;

                        LockTransition(false);
                        CheckMessage();
                    }
                    break;
            }
        }

        #region Virtual Methods
        public virtual void Initialise()
        {
            #region Send Instantly
            TransitionState temp = state;

            state = TransitionState.Delay;
            CheckMessage();//check to see if a message needs sent instantly

            state = temp;
            #endregion

#if (UNITY_EDITOR)
            //if(transitionInTime <= 0 || fadeOutTime <= 0)
            //    Debug.LogError("Transition and Fade time has to be greater than 0!!");

            if(transitionInCurve.length == 0 || fadeOutCurve.length == 0)
                Debug.LogError("No animation curve set for: " + parent.gameObject +
                    ".\nIf you have upgraded this framework then selecting the straight diagonal line will reproduce the exact result from previous versions. However you can now get nicer results using more natural curves intstead of lines" +
                    "\nTo set these click the empty box labelled 'Animation Curve'");
#endif

            state = BaseTransition.TransitionState.Finished;

            if(triggerInstantly)
                TriggerTransition(false);
        }

        public virtual void Transition()
        {
#if(ownsEaser)
        float transitionPercentage = Easer.Ease(easeType, 0, 1, timer / transitionTime);
#else
            float transitionPercentage;

            if(state < TransitionState.Waiting)//if transitioning in
            {
                if(transitionInTime == 0)
                    transitionPercentage = 1;//instantly skip transitions with no run time
                else
                    transitionPercentage = transitionInCurve.Evaluate(currentTime / transitionInTime);
            }
            else//fade outs
            {
                if(fadeOutTime == 0)
                    transitionPercentage = 0;
                else
                    transitionPercentage = fadeOutCurve.Evaluate(currentTime / fadeOutTime);
            }
#endif

            Transition(transitionPercentage);
        }

        protected virtual void Transition(float transitionPercentage)
        {
        }

        public virtual void Clone(BaseTransition other)
        {
#if(UNITY_EDITOR)
            label = other.label;

            mainDropDown = new AnimBool();
            transitionInDropDown = new AnimBool();
            fadeOutDropDown = new AnimBool();
            messagingDropDown = new AnimBool();
            loopingDropDown = new AnimBool();
            dataDropDown = new AnimBool();//these are for the editor. They are stored here to help act as saving your preferences whe viewing each object

            stayForever = other.stayForever;
#endif

            parent = other.parent;

            state = other.state;
            currentTime = other.currentTime;
            delay = other.delay;

            transitionInTime = other.transitionInTime;
            fadeOutTime = other.fadeOutTime;
            displayTime = other.displayTime;
            fadeOutDelay = other.fadeOutDelay;

            transitionInCurve = new AnimationCurve(other.transitionInCurve.keys);
            fadeOutCurve = new AnimationCurve(other.fadeOutCurve.keys);

            looping = other.looping;
            triggerInstantly = other.triggerInstantly;

            #region Messaging
            messagingEnabled = other.messagingEnabled;//used to temporary enable and disable messages
            events = other.events;
            whenToSends = (TransitionState[])other.whenToSends.Clone();
            #endregion
        }
        #endregion

        /// <summary>
        /// This removes any delay for future transitions.
        /// </summary>
        public void ResetDelay()
        {
            delay = 0;
        }

        public void JumpToEnd()
        {
            currentTime = transitionInTime;
            Transition();
            state = BaseTransition.TransitionState.Waiting;
            CheckMessage();
        }

        public void JumpToStart()
        {
            currentTime = 0;
            Transition();
        }

        /// <summary>
        /// Essentially triggers the animation to play again in reverse
        /// </summary>
        public virtual void TriggerFadeOut()
        {
            if(fadeOutDelay != 0)
            {
                state = TransitionState.FadeOutDelay;
                currentTime = 0;
            }
            else
            {
                state = TransitionState.FadingOut;
                currentTime = fadeOutTime;
            }

            CheckMessage();
        }

        /// <summary>
        /// Only trigger this fadeout if visible
        /// </summary>
        public void TriggerFadeOutIfActive()
        {
            if(state == BaseTransition.TransitionState.Waiting)
                TriggerFadeOut();
        }

        /// <summary>
        /// If the animation has yet to play then the animation fades in, otherwise it fades out
        /// </summary>
        public void ToggleTransition()
        {
            if(state == BaseTransition.TransitionState.Finished)
                TriggerTransition(true);
            else if(state == BaseTransition.TransitionState.Waiting)
                TriggerFadeOut();
        }

        /// <summary>
        /// Starts the animation
        /// </summary>
        public virtual void TriggerTransition(bool forceReset)
        {
            if(!forceReset)
                if(state != BaseTransition.TransitionState.Finished && state != BaseTransition.TransitionState.Waiting)
                    return;

            if(delay != 0)
                state = BaseTransition.TransitionState.Delay;
            else
                state = BaseTransition.TransitionState.TransitionIn;

            currentTime = 0;

            CheckMessage();

            Transition();//basically reset the animation to the start
        }

        /// <summary>
        /// Only trigger this transition if doing nothing
        /// </summary>
        public void TriggerTransitionIfIdle()
        {
            if(state == BaseTransition.TransitionState.Finished)
                TriggerTransition(false);
        }

        /// <summary>
        /// Ignores any delay this transition has and runs it instantly
        /// </summary>
        public virtual void TriggerTransitionWithoutDelay()
        {
            if(!gameObject.activeSelf)//if this game objec is not active
                gameObject.SetActive(true);//set active

            state = TransitionState.TransitionIn;

            currentTime = 0;

            CheckMessage();
        }

        public void Stop()
        {
            Stop(true);
        }

        public void Stop(bool checkMessage)
        {
            if(state == BaseTransition.TransitionState.TransitionIn)
                state = BaseTransition.TransitionState.Waiting;
            else
                state = BaseTransition.TransitionState.Finished;

            if(checkMessage)
                CheckMessage();
        }

        public void SetToPercentage(float percentage)
        {
            if(state <= TransitionState.Waiting)
                currentTime = transitionInTime * percentage;
            else
                currentTime = fadeOutTime * percentage;

            Transition();
        }

        /// <summary>
        /// Called to lock objects to the exact value expected after a transition
        /// </summary>
        /// <param name="fadeInLock">True if lock to fade in values, false for out</param>
        void LockTransition(bool fadeInLock)
        {
            Transition(transitionInCurve.Evaluate(fadeInLock ? 1 : 0));
        }

        /// <summary>
        /// Checks if the state has changed to the one that sends a message
        /// </summary>
        void CheckMessage()
        {
            if(!messagingEnabled)
                return;

            if(events != null)
                for(int i = 0; i < events.Length; i++)
                    if((state == whenToSends[i] || whenToSends[i] == TransitionState.AnyState) && events[i] != null)
                        events[i].Invoke();
        }

        #region Editor Externals
#if(UNITY_EDITOR)
        /// <summary>
        /// This is only used to help clear up components in the scene
        /// </summary>
        void Update()
        {
            if(!Application.isEditor)
                return;

            if(parent == null)//if the parent has been destroyed
                DestroyImmediate(this);
        }

        public void EditorInitialise(TransitionalObject parent)
        {
            this.parent = parent;
            hideFlags = HideFlags.HideInInspector;//don't show these components in the inspector
            mainDropDown.target = true;//start by showing this frame can move

            events = new UnityEvent[0];
            whenToSends = new TransitionState[0];

            AnimationCurve defualtCurve = new AnimationCurve();//make a new defualt curve when there is no existing one
            Keyframe frame = new Keyframe(0, 0);
            defualtCurve.AddKey(frame);

            frame.time = 1;//this produces a linear diagonal line from 0 to 1 but...
            frame.value = 1;//unity will smooth these automatically to give a nice fade in and out

            defualtCurve.AddKey(frame);

            transitionInCurve = new AnimationCurve(defualtCurve.keys);
            fadeOutCurve = new AnimationCurve(defualtCurve.keys);
            previousTransitionInCurve = defualtCurve.keys;
            previousFadeOutCurve = defualtCurve.keys;
        }

        public virtual void ViewPosition(TransitionalObject.MovingDataType movingType)
        {
        }

        public virtual void UpdatePosition(TransitionalObject.MovingDataType movingType)
        {
        }
#endif
        #endregion
    }
}