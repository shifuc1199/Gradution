//#define ownsEaser//Uncomment this if you own the easer framework!
#define StoreVersion
//#define UsingNGUI//Uncomment this if you use NGUI. WARNING using alpha transitions can randomly fail with NGUI. To fix this you have to disable and enable the affected gameobject at runtime (If anyone finds a better workaround than this please let me know!)
#define UsingUGUI//this is Unity's build in GUI tools

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using TransitionalObjects;
#if(UNITY_EDITOR)
using UnityEditor;
using UnityEditor.AnimatedValues;
#endif

/// <summary>
/// Class for handling simple fades and animations.
/// Author: Jason Guthrie
/// Company: K2 Games
/// Website: www.facebook.com/K2 Games
/// </summary>
[System.Serializable]
[ExecuteInEditMode]
public class TransitionalObject : MonoBehaviour
{
    #region Variables
    public enum Operator { Override = 0, Add, Subtract, Multiply, Divide, Stagger }//basically how do the values change here effect the transitions. 

    public BaseTransition[] transitions = new BaseTransition[0];

    public enum ResetType { Nothing = 0, JumpToStart, JumpToEnd }//what does this object do when it is reset
    [SerializeField]
    ResetType onReset;//the variable to control the type in the setters below

    public ResetType OnReset
    {
        get
        {
            return onReset;//just return
        }

        set
        {
            onReset = value;//first set the value

            if(onReset != ResetType.Nothing)//if this object now needs registered
            {
                if(!objectsToReset.Contains(this))//if this object hasn't been registered
                    objectsToReset.Add(this);
            }
            else
                objectsToReset.Remove(this);//if this object has been registered then it needs unregistered
        }
    }

    public bool runWhilstPaused = true;
    public bool useLateUpdate;

    #region Setters
    /// <summary>
    /// Sets the delay for ALL components to this value
    /// </summary>
    public float Delay
    {
        set
        {
            for(int i = 0; i < transitions.Length; i++)
                transitions[i].delay = value;
        }
    }

    /// <summary>
    /// Sets the fade out delay for ALL components to this value
    /// </summary>
    public float FadeOutDelay
    {
        set
        {
            for(int i = 0; i < transitions.Length; i++)
                transitions[i].fadeOutDelay = value;
        }
    }

    /// <summary>
    /// Sets the transition in time for ALL components to this value
    /// </summary>
    public float TransitionInTime
    {
        set
        {
            for(int i = 0; i < transitions.Length; i++)
                transitions[i].transitionInTime = value;
        }
    }

    /// <summary>
    /// Sets the fade out time for ALL components to this value
    /// </summary>
    public float FadeOutTime
    {
        set
        {
            for(int i = 0; i < transitions.Length; i++)
                transitions[i].fadeOutTime = value;
        }
    }

    /// <summary>
    /// Sets the display time for ALL components to this value
    /// </summary>
    public float DisplayTime
    {
        set
        {
            for(int i = 0; i < transitions.Length; i++)
                transitions[i].displayTime = value;
        }
    }

    public bool MessagingEnabled
    {
        set
        {
            for(int i = 0; i < transitions.Length; i++)
                transitions[i].messagingEnabled = value;
        }
    }
    #endregion

    #region Affected Images etc
    public Renderer[] affectedRenderers;

#if(UsingNGUI)
    public UIWidget[] affectedWidgets;
    public int widgetStartIndex;//where widgets start in the affected children list
#endif

#if(UsingUGUI)
    public CanvasGroup[] affectedCanvasGroups;
    public MaskableGraphic[] affectedImages;
    public int imageStartIndex, canvasStartIndex;//where widgets start in the affected children list
#endif

    public float[] childrenMaxAlpha;//this determines the max alpha per object. E.G some objects want to animate to 0.5 rather than 1. This is determined from the current alpha when initialise is called
    #endregion

    #region Statics and Resetting
    static List<TransitionalObject> objectsToReset = new List<TransitionalObject>();
    #endregion

    #region Editor Data
#if(UNITY_EDITOR)
    public string label = "";

    public int alphaOrColourComponentCount;
    public AnimBool affectAllGroup, effectedRenderersGroup, settingsGroup, renderersGroup,
        controlsGroup, toolsGroup;//these are all to make the editor look nicer

#if(UsingUGUI)
    public AnimBool imagesGroup, canvasGroups;
#endif

#if(UsingNGUI)
    public AnimBool widgetsGroup;
#endif

    public enum MovingDataType { Type = 0, StartPoint, MaxStart, EndPoint, MaxEnd }
#endif
    #endregion
    #endregion

    #region Methods
    void Awake()
    {
#if(UNITY_EDITOR)
        if(!Application.isPlaying)
            return;
#endif

        if(transitions != null)
            for(int i = 0; i < transitions.Length; i++)
                if(transitions[i] != null)
                    transitions[i].Initialise();
    }

#if(UNITY_EDITOR)
    /// <summary>
    /// Again helps situations when copy pasting components
    /// </summary>
    void Reset()
    {
        if(!Application.isPlaying)
            CheckForCopyPasting();
    }

    void CheckForCopyPasting()
    {
        if(transitions.Length > 0)
            if(transitions[0] != null && !transitions[0].parent.Equals(this))//if the transitions parent isn't this object then this object has been copy pasted
            {
                BaseTransition temp = null;
                System.Type type;

                for(int i = 0; i < transitions.Length; i++)
                {
                    type = transitions[i].GetType();

                    if(type.Equals(typeof(AlphaTransition)))
                        temp = gameObject.AddComponent<AlphaTransition>();//make a correct instance
                    else if(type.Equals(typeof(MovingTransition)))
                        temp = gameObject.AddComponent<MovingTransition>();
                    else if(type.Equals(typeof(ScalingTransition)))
                        temp = gameObject.AddComponent<ScalingTransition>();
                    else if(type.Equals(typeof(ColourTransition)))
                        temp = gameObject.AddComponent<ColourTransition>();
                    else if(type.Equals(typeof(RotatingTransition)))
                        temp = gameObject.AddComponent<RotatingTransition>();
                    else
                        temp = gameObject.AddComponent<BaseTransition>();//these are for manual transitions

                    if(temp != null)
                    {
                        temp.Clone(transitions[i]);//copy the other data
                        temp.parent = this;
                        temp.hideFlags = HideFlags.HideInInspector;
                        transitions[i] = temp;
                    }
                }
            }
    }

    /// <summary>
    /// Counts how many images etc are effected by this transition. Only applies to Colour and Alpha transitions and is used by the editor
    /// </summary>
    /// <returns></returns>
    public int CountEffectedImages()
    {
#if(UsingUGUI && UsingNGUI)
        return affectedRenderers.Length + affectedImages.Length + affectedCanvasGroups.Length + affectedWidgets.Length;
#elif(UsingUGUI)
        return affectedRenderers.Length + affectedImages.Length + affectedCanvasGroups.Length;
#elif(UsingNGUI)
        return affectedRenderers.Length + affectedWidgets.Length;
#else
        return affectedRenderers.Length;
#endif
    }
#endif

    void Start()
    {
#if(UNITY_EDITOR)
        if(!Application.isPlaying)
            return;
#endif

        if(onReset != ResetType.Nothing)//if this object is supposed to reset 
            objectsToReset.Add(this);//store the reference to this object to reset properly
    }

    /// <summary>
    /// Called if you have set the onReset to anything other than Nothing
    /// </summary>
    public void ResetInstance()
    {
        switch(onReset)
        {
            case ResetType.JumpToStart:
                JumpToStart();
                break;

            case ResetType.JumpToEnd:
                JumpToEnd();
                break;
        }
    }

    /// <summary>
    /// Needed to determine the current alpha and to find the renderers this animation should effect
    /// </summary>
    public void InitialiseAlphaTransition()
    {
        try
        {
            if(Application.isPlaying)
                JumpToEnd();//show everything at full alpha! Important for calculating the max alpha per image

            affectedRenderers = gameObject.GetComponentsInChildren<Renderer>();
            childrenMaxAlpha = new float[affectedRenderers.Length];

            for(int i = 0; i < affectedRenderers.Length; i++)
                childrenMaxAlpha[i] = affectedRenderers[i].sharedMaterial.color.a;//store the max alpha so we don't use a higher alpha than its base

#if(UsingUGUI)
            affectedImages = GetComponentsInChildren<MaskableGraphic>();//populate the list of effect images with the child images

            if(affectedImages.Length > 0)//if we also have images being effected
            {
                imageStartIndex = childrenMaxAlpha.Length;

                System.Array.Resize<float>(ref childrenMaxAlpha, childrenMaxAlpha.Length + affectedImages.Length);//make more array slots

                for(int i = 0; i < affectedImages.Length; i++)
                    childrenMaxAlpha[imageStartIndex + i] = affectedImages[i].color.a;
            }

            affectedCanvasGroups = GetComponentsInChildren<CanvasGroup>();//populate the list of effect images with the child images

            if(affectedCanvasGroups.Length > 0)//if we also have images being effected
            {
                canvasStartIndex = childrenMaxAlpha.Length;

                System.Array.Resize<float>(ref childrenMaxAlpha, childrenMaxAlpha.Length + affectedCanvasGroups.Length);//make more array slots

                for(int i = 0; i < affectedCanvasGroups.Length; i++)
                    childrenMaxAlpha[canvasStartIndex + i] = affectedCanvasGroups[i].alpha;
            }
#endif

#if(UsingNGUI)
            affectedWidgets = gameObject.GetComponentsInChildren<UIWidget>();//so look for NGUI sprites

            if(affectedWidgets.Length > 0)//if we also have images being effected
            {
                widgetStartIndex = childrenMaxAlpha.Length;

                System.Array.Resize<float>(ref childrenMaxAlpha, childrenMaxAlpha.Length + affectedWidgets.Length);//make more array slots

                for(int i = 0; i < affectedWidgets.Length; i++)
                    childrenMaxAlpha[widgetStartIndex + i] = affectedWidgets[i].color.a;
            }
#endif

            if(Application.isPlaying)
                JumpToStart();
        }
        catch(UnityException e)
        {
            Debug.LogError("Your material isn't compatible with an Alpha transition! To work with alpha a material needs a colour property." +
                           "\nNote: Unlit/Transparent does not have the colour property, try VertexLit instead.");
        }
    }

    /// <summary>
    /// Only trigger this transition if doing nothing
    /// </summary>
    public void TriggerTransitionIfIdle()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].TriggerTransitionIfIdle();

        enabled = true;
    }

    public void TriggerTransition()
    {
        if(!gameObject.activeSelf)
            gameObject.SetActive(true);

        for(int i = transitions.Length - 1; i > -1; i--)
            transitions[i].TriggerTransition(true);

        enabled = true;
    }

    /// <summary>
    /// Starts the animation
    /// </summary>
    public void TriggerTransitionWithReset(bool forceReset)
    {
        if(!gameObject.activeSelf)//if this game object is not active
            gameObject.SetActive(true);//set active

        for(int i = transitions.Length - 1; i > -1; i--)
            transitions[i].TriggerTransition(forceReset);

        enabled = true;
    }

    /// <summary>
    /// If the animation has yet to play then the animation fades in, otherwise it fades out
    /// </summary>
    public void ToggleTransition()
    {
        if(!gameObject.activeSelf)//if this game object is not active
            gameObject.SetActive(true);//set active

        for(int i = 0; i < transitions.Length; i++)
            transitions[i].ToggleTransition();

        enabled = true;
    }

    void Update()
    {
        #region Editor Functions
#if(UNITY_EDITOR)
        ///This block is very important for the editor. It basically lets you copy and paste properly! It will only run in the editor
        if(!Application.isPlaying)//if in the editor
        {
            CheckForCopyPasting();
            return;//prevent any actual code from running
        }
#endif
        #endregion

        if(!useLateUpdate)//only update if in the right queue!
            UpdateTransitions();
    }

    void UpdateTransitions()
    {
        float delta = Time.smoothDeltaTime;

        #region Pausing
        if(Time.timeScale < 0.01f && runWhilstPaused)
            delta = Time.unscaledDeltaTime;//delta will be 0 at this point so set it ourselves as a baked value
        #endregion

        for(int i = 0; i < transitions.Length; i++)
            transitions[i].CustomUpdate(delta);
    }

    /// <summary>
    /// Late update runs after update. It can be useful to run parent alpha transitions on late update to override the alpha values of its children
    /// </summary>
    void LateUpdate()
    {
        if(useLateUpdate)
            UpdateTransitions();
    }

    /// <summary>
    /// Only trigger this fadeout if visible
    /// </summary>
    public void TriggerFadeOutIfActive()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].TriggerFadeOutIfActive();

        enabled = true;
    }

    /// <summary>
    /// Essentially triggers the animation to play again in reverse
    /// </summary>
    public void TriggerFadeOut()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].TriggerFadeOut();

        enabled = true;
    }

    /// <summary>
    /// Transitions this object, be it a fade in or out.
    /// </summary>
    protected virtual void Transition()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].Transition();
    }

    public void Clone(TransitionalObject other)
    {
        for(int i = 0; i < transitions.Length; i++)
        {
            transitions[i].parent = other;
            transitions[i].Clone(other.transitions[i]);
        }
    }

    /// <summary>
    /// This removes any delay for future transitions.
    /// </summary>
    public void ResetDelay()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].ResetDelay();
    }

    public void JumpToEnd()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].JumpToEnd();
    }

    public void JumpToStart()
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].JumpToStart();
    }

    public void Stop()
    {
        Stop(true);
    }

    public void Stop(bool checkMessage)
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].Stop();

        enabled = false;//stops running update methods
    }

    public void SetToPercentage(float percentage)
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].SetToPercentage(percentage);
    }
    #endregion

    #region Exposed Helpers
    public void DestroyTarget(GameObject target)
    {
        Destroy(target);
    }
    #endregion

    #region Editor Externals
#if(UNITY_EDITOR)
    /// <summary>
    /// Called by the editor to view either the start of end point
    /// </summary>
    /// <param name="position">the given start or end point</param>
    public void ViewPosition(TransitionalObject.MovingDataType movingType)
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].ViewPosition(movingType);
    }

    /// <summary>
    /// Called by the editor to update the start or end point based on the current transform
    /// </summary>
    /// <param name="isStartPoint"></param>
    public void UpdatePosition(TransitionalObject.MovingDataType movingType)
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].UpdatePosition(movingType);
    }
#endif

    public void ModifyDelay(float value, Operator operation)
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].delay = PerformArithmetic(transitions[i].delay, value, operation);
    }

    public void ModifyTransitionInTime(float value, Operator operation)
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].transitionInTime = PerformArithmetic(transitions[i].transitionInTime, value, operation);
    }

    public void ModifyDisplayTime(float value, Operator operation)
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].displayTime = PerformArithmetic(transitions[i].displayTime, value, operation);
    }

    public void ModifyFadeOutDelay(float value, Operator operation)
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].fadeOutDelay = PerformArithmetic(transitions[i].fadeOutDelay, value, operation);
    }

    public void ModifyFadeOutTime(float value, Operator operation)
    {
        for(int i = 0; i < transitions.Length; i++)
            transitions[i].fadeOutTime = PerformArithmetic(transitions[i].fadeOutTime, value, operation);
    }
    #endregion

    #region Static Methods
    public static float PerformArithmetic(float original, float value, Operator operation)
    {
        switch(operation)
        {
            case Operator.Add:
                return original += value;

            case Operator.Subtract:
                return original -= value;

            case Operator.Multiply:
                return original *= value;

            case Operator.Divide:
                return original /= value;

            default:
                return original;
        }
    }

    /// <summary>
    /// This method you will have to call yourself when you do any sort of game resets. If you reset your game by reloading the scene you can ignore this
    /// </summary>
    public static void ResetAll()
    {
        int count = objectsToReset.Count;

        for(int i = 0; i < count; i++)
            objectsToReset[i].ResetInstance();
    }

#if StoreVersion
    public static Vector3 FixRotations(Vector3 eulerInput)
    {
        //Debug.Log("Starting: " + eulerInput);

        if(eulerInput.x < 0)// || eulerInput.x > 180)
            eulerInput.x = 360 + eulerInput.x;

        if(eulerInput.y < 0)// || eulerInput.y > 180)
            eulerInput.y = 360 + eulerInput.y;

        if(eulerInput.z < 0)// || eulerInput.z > 180)
            eulerInput.z = 360 + eulerInput.z;

        //Debug.Log("Output: " + eulerInput);

        return eulerInput;
    }

    /// <summary>
    /// Returns the value that is the defined percentage between both values
    /// </summary>
    public static Color Lerp(Color min, Color max, float percentageBetween)
    {
        return new Color(Lerp(min.r, max.r, percentageBetween), Lerp(min.g, max.g, percentageBetween), Lerp(min.b, max.b, percentageBetween), Lerp(min.a, max.a, percentageBetween));
    }

    /// <summary>
    /// Returns the value that is the defined percentage between both values
    /// </summary>
    public static float Lerp(float min, float max, float percentageBetween)
    {
        float differenceSign = (min - max > 0 ? -1 : 1);//if the difference is negative then subtract the value at the end!

        return min + Mathf.Abs(min - max) * percentageBetween * differenceSign;
    }

    public static void RemoveFromArray<Type>(int index, ref Type[] array)
    {
        List<Type> list = new List<Type>();//so create a list
        list.AddRange(array);//fill it with our existing array

        list.RemoveAt(index);//remove the element from the list, it will plug the gap
        array = list.ToArray();//then convert back
    }
#endif

    /// <summary>
    /// Called to create a blank transform to act as an animation placeholder
    /// </summary>
    /// <param name="current">Which object to copy the name and current position etc from</param>
    /// <param name="label">A label to add onto the. Ususally this is start or end point</param>
    /// <returns></returns>
    public static Transform CreateTransform(GameObject current, string label)
    {
        GameObject temp = new GameObject(current.name + " Transition" + label);
        temp.transform.position = current.transform.position;
        temp.transform.localScale = current.transform.lossyScale;
        temp.transform.rotation = current.transform.rotation;

        GameObject holder = GameObject.Find("Transitions");

        if(holder != null)
            temp.transform.parent = holder.transform;
        else if(current.transform.parent != null)
            temp.transform.parent = current.transform.parent;

        return temp.transform;
    }
    #endregion
}