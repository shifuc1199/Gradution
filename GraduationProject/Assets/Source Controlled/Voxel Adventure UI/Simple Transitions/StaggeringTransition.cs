////#define ownsEaser//Uncomment this if you own the easer framework!
//#define StoreVersion

//using UnityEngine;
//using System.Collections;
//using UnityEngine.Events;
//using System.Collections.Generic;
//#if (!StoreVersion)
//using K2Framework;
//#endif

//public class StaggeringTransition : MonoBehaviour
//{
//    public bool triggerInstantly;
//    public bool looping;//does this animation start again from the beginning once it finishes
//    public bool reverseOnFinish;//if looping instead of starting from the beginning this reverses direction instead

//    public float delay;
//    public List<int> indexesToDelay;//these are the indexes of when to delay. E.G when current point equals any of these values then run the delay

//    public Transform[] points;
//    public TransitionalObject transition;//this transition acts as the total time to complete the animation

//#if(ownsEaser)
//    public EaserEase transitionInType, mainType, fadeOutType;//these are the transition types during the animation, helps ease
//#else
//    public AnimationCurve transitionInCurve, mainCurve, fadeOutCurve;
//#endif

//    public float totalTime;
//    int currentPoint;

//    public UnityEvent onFinished;

//    void Awake()
//    {
//        if(triggerInstantly)
//            TriggerStaggeredTransition();
//    }

//    public void Reset()
//    {
//        TriggerStaggeredTransition();
//        transition.Stop();
//    }

//    public void TriggerStaggeredTransition()
//    {
//#if(ownsEaser)
//        transition.easeType = transitionInType;
//#else
//        transition.curve = transitionInCurve;
//#endif

//        transition.startPoint = points[0];
//        transition.endPoint = points[1];
//        transition.transitionTime = totalTime / (points.Length - 1);

//        currentPoint = 0;

//        transition.TriggerTransition();
//    }

//    public void TriggerStaggeredFadeOut()
//    {
//#if(ownsEaser)
//        transition.easeType = fadeOutType;
//#else
//        transition.curve = fadeOutCurve;
//#endif

//        transition.startPoint = points[points.Length - 1];
//        transition.endPoint = points[points.Length - 2];
//        transition.transitionTime = totalTime / (points.Length - 1);

//        currentPoint = points.Length - 1;

//        transition.TriggerFadeOut();
//    }

//    /// <summary>
//    /// Called whenever the state changes for a transitional object
//    /// </summary>
//    public void UpdateState()
//    {
//        switch(transition.state)
//        {
//            case TransitionalObject.TransitionState.Finished:
//                LoadPreviousPoint();
//                break;

//            case TransitionalObject.TransitionState.Waiting:
//                LoadNextPoint();
//                break;
//        }
//    }

//    /// <summary>
//    /// Called when a transition has hit the waiting state, when transitioning forward
//    /// </summary>
//    public void LoadNextPoint()
//    {
//        if(delay == 0 || !indexesToDelay.Contains(currentPoint + 1))//if there is no delay or the current index shouldnt be delayed
//            ActuallyLoadNextPoint();
//        else
//            StartCoroutine(DelayLoadNextPoint());
//    }

//    /// <summary>
//    /// Delays loading a point until the delay has passed
//    /// </summary>
//    /// <returns></returns>
//    IEnumerator DelayLoadNextPoint()
//    {
//        yield return new WaitForSeconds(delay);
//        ActuallyLoadNextPoint();
//    }

//    void ActuallyLoadNextPoint()
//    {
//        currentPoint++;

//        if(currentPoint == points.Length - 1)//if finished
//        {
//            if(onFinished != null)
//                onFinished.Invoke();

//            #region Looping
//            if(looping)
//            {
//                currentPoint++;

//                if(reverseOnFinish)
//                    LoadPreviousPoint();
//                else
//                {
//                    currentPoint = -2;//keep looping forward, -2 means the animation uses the last point and the first
//                    LoadNextPoint();
//                }
//            }
//            #endregion

//            return;
//        }
//#if(ownsEaser)
//        else if(looping && !reverseOnFinish)//if we are looping and dont reverse then keep a smooth animation
//            transition.easeType = mainType;//e.g always use the main animation
//        else if(currentPoint == points.Length - 2)//if on the last phase
//            transition.easeType = fadeOutType;
//        else if(currentPoint == 0)
//            transition.easeType = transitionInType;
//        else
//            transition.easeType = mainType;
//#else
//        else if(looping && !reverseOnFinish)//if we are looping and dont reverse then keep a smooth animation
//            transition.curve = mainCurve;//e.g always use the main animation
//        else if(currentPoint == points.Length - 2)//if on the last phase
//            transition.curve = fadeOutCurve;
//        else if(currentPoint == 0)
//            transition.curve = transitionInCurve;
//        else
//            transition.curve = mainCurve;
//#endif

//        if(currentPoint < 0)//if we are finishing a loop
//        {
//            transition.startPoint = points[points.Length - 1];//use the last
//            transition.endPoint = points[0];//and first point
//        }
//        else
//        {
//            transition.startPoint = points[currentPoint];//animate as normal
//            transition.endPoint = points[currentPoint + 1];
//        }

//        transition.TriggerTransition();
//    }

//    /// <summary>
//    /// Called when the transition has hit the delay state, when fading out/running backwards
//    /// </summary>
//    public void LoadPreviousPoint()
//    {
//        if(delay == 0 || !indexesToDelay.Contains(currentPoint - 1))//if there is no delay or the current index shouldnt be delayed
//            ActuallyLoadPreviousPoint();
//        else
//            StartCoroutine(DelayLoadPreviousPoint());
//    }

//    /// <summary>
//    /// Delays loading a point until the delay has passed
//    /// </summary>
//    /// <returns></returns>
//    IEnumerator DelayLoadPreviousPoint()
//    {
//        yield return new WaitForSeconds(delay);
//        ActuallyLoadPreviousPoint();
//    }

//    void ActuallyLoadPreviousPoint()
//    {
//        currentPoint--;

//        if(currentPoint == 0)//if finished
//        {
//            if(onFinished != null)
//                onFinished.Invoke();

//            #region Looping
//            if(looping)
//            {
//                if(reverseOnFinish)
//                {
//                    currentPoint--;
//                    LoadNextPoint();//begins the loop again going forward
//                }
//                else
//                {
//                    currentPoint = points.Length;
//                    LoadPreviousPoint();//keep going in reverse
//                }
//            }
//            #endregion

//            return;
//        }
//#if(ownsEaser)
//        else if(looping && !reverseOnFinish)//if we are looping and dont reverse then keep a smooth animation
//            transition.easeType = mainType;//e.g always use the main animation
//        else if(currentPoint == 1)//if on the last phase
//            transition.easeType = transitionInType;
//        else if(currentPoint == points.Length - 1)
//            transition.easeType = fadeOutType;
//        else
//            transition.easeType = mainType;
//#else
//        else if(looping && !reverseOnFinish)//if we are looping and dont reverse then keep a smooth animation
//            transition.curve = mainCurve;//e.g always use the main animation
//        else if(currentPoint == 1)//if on the last phase
//            transition.curve = transitionInCurve;
//        else if(currentPoint == points.Length - 1)
//            transition.curve = fadeOutCurve;
//        else
//            transition.curve = mainCurve;
//#endif
//        try
//        {
//            transition.endPoint = points[currentPoint];
//            transition.startPoint = points[currentPoint - 1];
//        }
//        catch(System.Exception e)
//        {
//            Debug.LogError(currentPoint + ", " + gameObject + " ," + points.Length);
//        }
//        transition.TriggerFadeOut();
//    }
//}
