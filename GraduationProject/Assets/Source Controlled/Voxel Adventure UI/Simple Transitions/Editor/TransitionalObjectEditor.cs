//#define ownsEaser//Uncomment this if you own the easer framework!
#define StoreVersion
#define UsingUGUI
//#define UsingNGUI

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using TransitionalObjects;
using UnityEngine.UI;
#if (!StoreVersion)
using K2Framework;
#endif

[CanEditMultipleObjects]
[CustomEditor(typeof(TransitionalObject))]

/// <summary>
/// This editor class determines how this script is viewed in the Unity inspector
/// Author: Jason Guthrie
/// Company: K2 Games
/// Website: www.facebook.com/K2 Games
/// Email: support@k2games.co.uk
/// </summary>
public class TransitionalObjectEditor : Editor
{
    #region Variables
    delegate void BasicDelegate();
    delegate void IntInputDelegate(int value);//used for the affect images etc lists
    delegate float GetAlphaDelegate(int value);

    public enum ComponenentType { None = 0, Alpha, Colour, Movement, Scaling, Rotating, Manual }
    public ComponenentType selectedType;
    const float CurveHeight = 50;

    AnimationCurve defualtCurve;

    SerializedProperty onReset, runWhilstPaused, useLateUpdate;

    TransitionalObject current;
    SerializedObject currentObject;//used to display arrays correctly

    TransitionalObject.ResetType resetType;

    List<TransitionalObject> selectedTransitions = new List<TransitionalObject>();

    #region Affect All Section
    BaseType affectAllType;

    float affectAllFloatValue;
    bool affectAllBoolValue;
    string affectAllStringValue;
    AnimationCurve affectAllCurve;
    TransitionalObject.Operator affectAllOperation;
    #endregion

    bool reparenting;

    const float Spacing = 2.5f;

    #region Styles
    static GUIStyle centeredLabel, boldLabel;
    static Color headingsColour = new Color(0.6f, 0.918f, 1, 1);
    static Color subHeadingsColour = new Color(1, 1, 1, 1);
    static Color backingColour = new Color(0.859f, 0.855f, 0.992f, 1);

    string previousString;
    float previousFloat;
    bool previousBool;

    GUIHelper.LayoutStyle headingStyles = GUIHelper.LayoutStyle.Toolbar,
        subHeadingStyle = GUIHelper.LayoutStyle.Button,
        subContentStyle = GUIHelper.LayoutStyle.Box,
        mainContentStyle = GUIHelper.LayoutStyle.Box;

    Color previousLabelColour;//used to fix an annoying unity bug. remove this and you will see the heading colour in the project view
    #endregion

    GameObject removeFromHierarchyObject;//this object is used to search and remove renderers
    #endregion

    void OnEnable()
    {
        #region Colours
        if(!EditorPrefs.HasKey("HeadingR"))//if there is no data
            ResetColours();
        #endregion

        onReset = serializedObject.FindProperty("OnReset");
        runWhilstPaused = serializedObject.FindProperty("runWhilstPaused");
        useLateUpdate = serializedObject.FindProperty("useLateUpdate");

        currentObject = new SerializedObject(((TransitionalObject)target));//handled differently because its an array

        #region Defualt Animation Curve
        defualtCurve = new AnimationCurve();//make a new defualt curve when there is no existing one
        Keyframe frame = new Keyframe(0, 0);
        defualtCurve.AddKey(frame);

        frame.time = 1;//this produces a linear diagonal line from 0 to 1 but...
        frame.value = 1;//unity will smooth these automatically to give a nice fade in and out

        defualtCurve.AddKey(frame);
        #endregion

        TransitionalObject temp;

        selectedTransitions.Clear();

        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            temp = Selection.gameObjects[i].GetComponent<TransitionalObject>();

            if(temp != null)//if we found a transitional object in the players selection
                selectedTransitions.Add(temp);
        }
    }

    static void ResetColours()
    {
        if(EditorGUIUtility.isProSkin)
        {
            headingsColour = new Color(0.6f, 0.918f, 1, 1);
            subHeadingsColour = new Color(1, 1, 1, 1);
            backingColour = new Color(0.859f, 0.855f, 0.992f, 1);
        }
        else
        {
            headingsColour = new Color(0f, 0.384f, 0.831f, 1);
            subHeadingsColour = new Color(0, 0, 0, 1);
            backingColour = new Color(0.815f, 0.815f, 0.815f, 1);
        }
    }

    public override void OnInspectorGUI()
    {
        #region Initialise
        //undoManager.CheckUndo();
        serializedObject.Update();

        current = (TransitionalObject)target;

        if(current.transitions == null)
            current.transitions = new BaseTransition[0];

        previousLabelColour = EditorStyles.boldLabel.normal.textColor;

        centeredLabel = EditorStyles.boldLabel;
        centeredLabel.alignment = TextAnchor.MiddleCenter;

        boldLabel = EditorStyles.boldLabel;
        boldLabel.normal.textColor = headingsColour;

        Undo.RecordObject(current, "Transition Settings");//apparently this isn't enough and I need to add snapshots etc instead

        for(int i = 0; i < current.transitions.Length; i++)
            Undo.RecordObject(current.transitions[i], "Transition Settings");
        #endregion

        #region Settings
        GUILayout.Space(Spacing);

        if(current.transitions.Length > 0)
        {
            using(new Vertical(headingStyles))
            using(new ColourChange(headingsColour))
                DrawToggle(ref current.settingsGroup, new GUIContent(current.label.Length == 0 ? "Settings" : current.label + " (Settings)"));

            if(EditorGUILayout.BeginFadeGroup(current.settingsGroup.faded))
                using(new Vertical(mainContentStyle))
                {
                    using(new FixedWidthLabel("Label"))
                        current.label = EditorGUILayout.TextField(current.label);

                    #region Reset Types
                    resetType = current.OnReset;

                    current.OnReset = (TransitionalObject.ResetType)EditorGUILayout.EnumPopup(new GUIContent("On Reset", "Determines what method to run if the static Reset method is called for all Transitional Objects"), resetType);
                    #endregion

                    #region Triggers
                    GUILayout.Space(Spacing);

                    EditorGUILayout.PropertyField(runWhilstPaused, new GUIContent("Run Whilst Paused", "Should this run even if the game is paused"));
                    EditorGUILayout.PropertyField(useLateUpdate, new GUIContent("Use Late Update", "Should this use update or late update to run the transition. Useful for alpha components trying to alter their children in the correct order"));

                    using(new Horizontal())
                    {
                        if(GUILayout.Button("Show"))
                            ShowComponents(true);

                        if(GUILayout.Button("Hide"))
                            ShowComponents(false);
                    }
                    #endregion

                    #region Scan For Alpha or Colour
                    if(GUILayout.Button("Scan for Alpha Components"))//used during development. Basically if a component doesn't show the affected renderers etc clicking this will check if it should
                        ScanForAlphaComponents();
                    #endregion
                }

            EditorGUILayout.EndFadeGroup();
        }
        #endregion

        #region Change All Section
        if(current.transitions.Length > 1)
        {
            GUILayout.Space(Spacing);

            using(new Vertical(headingStyles))
            using(new ColourChange(headingsColour))
                DrawToggle(ref current.affectAllGroup, new GUIContent("Affect All"));

            if(EditorGUILayout.BeginFadeGroup(current.affectAllGroup.faded))
                using(new Vertical(mainContentStyle))
                {
                    EditorGUILayout.HelpBox("This section allows you to edit values of multiple components at once", MessageType.Info);

                    using(new FixedWidthLabel("Type"))
                        affectAllType = (BaseType)EditorGUILayout.EnumPopup(affectAllType);

                    using(new Horizontal())
                    {
                        switch(affectAllType)
                        {
                            #region Label
                            case BaseType.Label:
                                affectAllStringValue = EditorGUILayout.TextField(GUIContent.none, affectAllStringValue, GUILayout.MaxWidth(100));

                                if(GUILayout.Button("Set"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                                            selectedTransitions[i].transitions[ii].label = affectAllStringValue;

                                if(GUILayout.Button("Append"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                                            selectedTransitions[i].transitions[ii].label += affectAllStringValue;
                                break;
                            #endregion

                            #region Delay
                            case BaseType.Delay:
                                affectAllFloatValue = EditorGUILayout.FloatField(GUIContent.none, affectAllFloatValue, GUILayout.MaxWidth(50));
                                affectAllOperation = (TransitionalObject.Operator)EditorGUILayout.EnumPopup(affectAllOperation);

                                if(GUILayout.Button("Run"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        selectedTransitions[i].ModifyDelay(affectAllFloatValue, affectAllOperation);
                                break;
                            #endregion

                            #region Transition In
                            case BaseType.TransitionIn:
                                affectAllFloatValue = EditorGUILayout.FloatField(GUIContent.none, affectAllFloatValue, GUILayout.MaxWidth(50));
                                affectAllOperation = (TransitionalObject.Operator)EditorGUILayout.EnumPopup(affectAllOperation);

                                if(GUILayout.Button("Run"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        selectedTransitions[i].ModifyTransitionInTime(affectAllFloatValue, affectAllOperation);
                                break;
                            #endregion

                            #region Display Time
                            case BaseType.DisplayTime:
                                affectAllFloatValue = EditorGUILayout.FloatField(GUIContent.none, affectAllFloatValue, GUILayout.MaxWidth(50));
                                affectAllOperation = (TransitionalObject.Operator)EditorGUILayout.EnumPopup(affectAllOperation);

                                if(GUILayout.Button("Run"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        selectedTransitions[i].ModifyDisplayTime(affectAllFloatValue, affectAllOperation);
                                break;
                            #endregion

                            #region Fade Out Delay
                            case BaseType.FadeOutDelay:
                                affectAllFloatValue = EditorGUILayout.FloatField(GUIContent.none, affectAllFloatValue, GUILayout.MaxWidth(50));
                                affectAllOperation = (TransitionalObject.Operator)EditorGUILayout.EnumPopup(affectAllOperation);

                                if(GUILayout.Button("Run"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        selectedTransitions[i].ModifyFadeOutDelay(affectAllFloatValue, affectAllOperation);
                                break;
                            #endregion

                            #region Fade Out Time
                            case BaseType.FadeOutTime:
                                affectAllFloatValue = EditorGUILayout.FloatField(GUIContent.none, affectAllFloatValue, GUILayout.MaxWidth(50));
                                affectAllOperation = (TransitionalObject.Operator)EditorGUILayout.EnumPopup(affectAllOperation);

                                if(GUILayout.Button("Run"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        selectedTransitions[i].ModifyFadeOutTime(affectAllFloatValue, affectAllOperation);
                                break;
                            #endregion

                            #region Trigger Instantly
                            case BaseType.TriggerInstantly:
                                using(new FixedWidthLabel("Value"))
                                    affectAllBoolValue = EditorGUILayout.Toggle(GUIContent.none, affectAllBoolValue, GUILayout.MaxWidth(50));

                                if(GUILayout.Button("Set"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                                            selectedTransitions[i].transitions[ii].triggerInstantly = affectAllBoolValue;
                                break;
                            #endregion

                            #region Stay Forever
                            case BaseType.StayForever:
                                using(new FixedWidthLabel("Value"))
                                    affectAllBoolValue = EditorGUILayout.Toggle(GUIContent.none, affectAllBoolValue, GUILayout.MaxWidth(50));

                                if(GUILayout.Button("Set"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                                            selectedTransitions[i].transitions[ii].stayForever = affectAllBoolValue;
                                break;
                            #endregion

                            #region Looping
                            case BaseType.Looping:
                                using(new FixedWidthLabel("Value"))
                                    affectAllBoolValue = EditorGUILayout.Toggle(GUIContent.none, affectAllBoolValue, GUILayout.MaxWidth(50));

                                if(GUILayout.Button("Set"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                                            selectedTransitions[i].transitions[ii].looping = affectAllBoolValue;
                                break;
                            #endregion

                            #region Copy Transition In Time
                            case BaseType.CopyTransitionInTime:
                                using(new FixedWidthLabel("Value"))
                                    affectAllBoolValue = EditorGUILayout.Toggle(GUIContent.none, affectAllBoolValue, GUILayout.MaxWidth(50));

                                if(GUILayout.Button("Set"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                                            selectedTransitions[i].transitions[ii].copyTransitionInTime = affectAllBoolValue;
                                break;
                            #endregion

                            #region Transition In Curve
                            case BaseType.TransitionInCurve:
                                affectAllCurve = EditorGUILayout.CurveField(GUIContent.none, affectAllCurve, GUILayout.Height(CurveHeight));

                                if(GUILayout.Button("Set"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                                            selectedTransitions[i].transitions[ii].transitionInCurve = new AnimationCurve(affectAllCurve.keys);
                                break;
                            #endregion

                            #region Fade Out Curve
                            case BaseType.FadeOutCurve:
                                affectAllCurve = EditorGUILayout.CurveField(GUIContent.none, affectAllCurve, GUILayout.Height(CurveHeight));

                                if(GUILayout.Button("Set"))
                                    for(int i = 0; i < selectedTransitions.Count; i++)
                                        for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                                            selectedTransitions[i].transitions[ii].fadeOutCurve = new AnimationCurve(affectAllCurve.keys);
                                break;
                            #endregion
                        }

                    }
                }

            EditorGUILayout.EndFadeGroup();
        }
        #endregion

        #region Main Array
        for(int i = 0; i < current.transitions.Length; i++)
            if(current.transitions[i] != null)
            {
                GUILayout.Space(Spacing);

                if(current.transitions[i].GetType() == typeof(AlphaTransition))
                    DrawAlphaComponent(i, (AlphaTransition)current.transitions[i]);

                else if(current.transitions[i].GetType() == typeof(ColourTransition))
                    DrawColouringComponent(i, (ColourTransition)current.transitions[i]);

                else if(current.transitions[i].GetType() == typeof(MovingTransition))
                    DrawMovingComponent(i, (MovingTransition)current.transitions[i]);

                else if(current.transitions[i].GetType() == typeof(RotatingTransition))
                    DrawRotatingComponent(i, (RotatingTransition)current.transitions[i]);

                else if(current.transitions[i].GetType() == typeof(ScalingTransition))
                    DrawScalingComponent(i, (ScalingTransition)current.transitions[i]);

                else
                    DrawManualComponent(i, current.transitions[i]);
            }
        #endregion

        #region Adding New Component
        GUILayout.Space(Spacing);

        using(new Vertical(headingStyles))
        using(new FixedWidthLabel(new GUIContent("Add New:"), boldLabel))
        {
            GUI.contentColor = Color.white;
            selectedType = (ComponenentType)EditorGUILayout.EnumPopup(selectedType);
        }

        if(selectedType > 0)//if the player selected anything
        {
            for(int i = 0; i < selectedTransitions.Count; i++)
                AddNewComponent(selectedTransitions[i]);

            selectedType = 0;//show that something has been selected
        }
        #endregion

        #region Affected Renderers/Images/Widgets
        if(current.alphaOrColourComponentCount > 0)
        {
            GUILayout.Space(Spacing * 2);

            using(new Vertical(headingStyles))
            using(new ColourChange(headingsColour))
                DrawToggle(ref current.effectedRenderersGroup, new GUIContent("Affected Renderers (" + current.CountEffectedImages() + ")"));

            if(EditorGUILayout.BeginFadeGroup(current.effectedRenderersGroup.faded))
                using(new Vertical(mainContentStyle))
                {
                    if(GUILayout.Button(new GUIContent("Re-Scan", "Updates the list of renders etc")))
                        current.InitialiseAlphaTransition();

                    #region Renderers
                    DrawAffectedObjectsList("Renderers (" + current.affectedRenderers.Length + ")",
                     ref current.renderersGroup, 0,
                     AddRenderer, RemoveRenderer, RemoveAllRenderers, GetRendererAlpha,
                     current.affectedRenderers, typeof(CanvasGroup));
                    #endregion

#if(UsingUGUI)
                    #region Images
                    DrawAffectedObjectsList("Images (" + current.affectedImages.Length + ")",
                          ref current.imagesGroup, current.imageStartIndex,
                          AddImage, RemoveImage, RemoveAllImages, GetImageAlpha,
                          current.affectedImages, typeof(MaskableGraphic));
                    #endregion

                    #region Canvas Groups
                    DrawAffectedObjectsList("Canvas Groups (" + current.affectedCanvasGroups.Length + ")",
                        ref current.canvasGroups, current.canvasStartIndex,
                        AddCanvasGroup, RemoveCanvasGroup, RemoveAllCanvases, GetCanvasAlpha,
                        current.affectedCanvasGroups, typeof(CanvasGroup));
                    #endregion
#endif

#if(UsingNGUI)
                    #region Widgets
                    DrawAffectedObjectsList("Widgets (" + current.affectedWidgets.Length + ")",
                          ref current.widgetsGroup, current.widgetStartIndex,
                          AddWidget, RemoveWidget, RemoveAllWidgets, GetWidgetAlpha,
                          current.affectedWidgets, typeof(UIWidget));
                    #endregion
#endif

                    #region Tools
                    using(new ColourChange(subHeadingsColour))
                    using(new Vertical(subHeadingStyle))
                        DrawToggle(ref current.toolsGroup, new GUIContent("Tools"));

                    if(EditorGUILayout.BeginFadeGroup(current.toolsGroup.faded))
                        using(new Vertical(subContentStyle))
                        {
                            using(new Horizontal())
                            {
                                using(new FixedWidthLabel("Remove From Heirarchy"))
                                    removeFromHierarchyObject = EditorGUILayout.ObjectField(removeFromHierarchyObject, typeof(GameObject)) as GameObject;

                                if(GUILayout.Button(new GUIContent("Run", "Finds all instances of renderers, images etc from the given objects heirarchy and removes them from this transitions list of affected renderers etc")))
                                {
                                    #region Remove Renderers
                                    Renderer[] renderers = removeFromHierarchyObject.GetComponentsInChildren<Renderer>();

                                    for(int i = 0; i < renderers.Length; i++)
                                        for(int ii = 0; ii < current.affectedRenderers.Length; ii++)
                                            if(current.affectedRenderers[ii].Equals(renderers[i]))
                                                RemoveRenderer(ii);//basically check both lists and remove any matches
                                    #endregion

#if(UsingUGUI)
                                    #region Remove Images
                                    MaskableGraphic[] images = removeFromHierarchyObject.GetComponentsInChildren<MaskableGraphic>();

                                    for(int i = 0; i < images.Length; i++)
                                        for(int ii = 0; ii < current.affectedImages.Length; ii++)
                                            if(current.affectedImages[ii].Equals(images[i]))
                                                RemoveImage(ii);//basically check both lists and remove any matches
                                    #endregion

                                    #region Remove Canvases
                                    CanvasGroup[] canvases = removeFromHierarchyObject.GetComponentsInChildren<CanvasGroup>();

                                    for(int i = 0; i < canvases.Length; i++)
                                        for(int ii = 0; ii < current.affectedCanvasGroups.Length; ii++)
                                            if(current.affectedCanvasGroups[ii].Equals(canvases[i]))
                                                RemoveCanvasGroup(ii);//basically check both lists and remove any matches
                                    #endregion
#endif

#if(UsingNGUI)
                                    #region Remove Widgets
                                    UIWidget[] widgets = removeFromHierarchyObject.GetComponentsInChildren<UIWidget>();

                                    for(int i = 0; i < widgets.Length; i++)
                                        for(int ii = 0; ii < current.affectedWidgets.Length; ii++)
                                            if(current.affectedWidgets[ii].Equals(widgets[i]))
                                                RemoveWidget(ii);//basically check both lists and remove any matches
                                    #endregion
#endif

                                }
                            }

                            if(removeFromHierarchyObject != null)
                            {
                                EditorGUILayout.LabelField("Found: " + removeFromHierarchyObject.GetComponentsInChildren<Renderer>().Length + " renderers");

#if(UsingUGUI)
                                EditorGUILayout.LabelField("Found: " + removeFromHierarchyObject.GetComponentsInChildren<MaskableGraphic>().Length + " images");
                                EditorGUILayout.LabelField("Found: " + removeFromHierarchyObject.GetComponentsInChildren<CanvasGroup>().Length + " canvas groups");
#endif

#if(UsingNGUI)
                                EditorGUILayout.LabelField("Found: " + removeFromHierarchyObject.GetComponentsInChildren<UIWidget>().Length + " widgets");
#endif
                            }
                        }

                    EditorGUILayout.EndFadeGroup();
                    #endregion
                }

            EditorGUILayout.EndFadeGroup();
        }
        #endregion

        #region Finalising
        if(GUI.changed)
        {
            //Undo.RecordObject(current, "Transition Data");
            EditorUtility.SetDirty(target);
        }

        Repaint();//repaint each frame, this means the fade groups animations are far smoother

        serializedObject.ApplyModifiedProperties();// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.

        EditorStyles.boldLabel.normal.textColor = previousLabelColour;//fixes an annoying unity bug

        //undoManager.CheckDirty();
        #endregion
    }

    #region Affected Images
    void DrawAffectedObjectsList(string heading, ref AnimBool animation, int childrenStartIndex, BasicDelegate addMethod, IntInputDelegate removeMethod, BasicDelegate deleteAll, GetAlphaDelegate getAlpha, Object[] affectedObjets, System.Type affectedType)
    {
        using(new ColourChange(subHeadingsColour))
        using(new Vertical(subHeadingStyle))
            DrawToggle(ref animation, new GUIContent(heading));

        if(EditorGUILayout.BeginFadeGroup(animation.faded))
            using(new Vertical(subContentStyle))
            {
                for(int i = 0; i < affectedObjets.Length; i++)
                    using(new Horizontal())
                    {
                        affectedObjets[i] = EditorGUILayout.ObjectField(affectedObjets[i], affectedType, true);

                        if(GUI.changed)//if a new value has been set
                            if(affectedObjets[i] != null)
                                current.childrenMaxAlpha[childrenStartIndex + i] = getAlpha.Invoke(i);

                        using(new FixedWidthLabel("Max Alpha"))
                            current.childrenMaxAlpha[childrenStartIndex + i] = EditorGUILayout.FloatField(current.childrenMaxAlpha[childrenStartIndex + i], GUILayout.MaxWidth(50));

                        if(GUILayout.Button("-", GUILayout.Width(25)))
                            removeMethod.Invoke(i);//run the delegate. Pass in remove image, widget, render or canvas group
                    }

                using(new Horizontal())
                {
                    if(GUILayout.Button("Remove All"))
                        deleteAll.Invoke();

                    GUILayout.FlexibleSpace();

                    if(GUILayout.Button("+", GUILayout.Width(25)))
                        addMethod.Invoke();
                }
            }

        EditorGUILayout.EndFadeGroup();
    }

    /// <summary>
    /// Updates the start indexes of the arrays whenever an array is modified
    /// </summary>
    void UpdateStartIndexes()
    {
#if(UsingUGUI)
        current.imageStartIndex = current.affectedRenderers.Length;
        current.canvasStartIndex = current.imageStartIndex + current.affectedImages.Length;
#endif

#if(UsingNGUI)
#if(UsingUGUI)//if using both
        current.widgetStartIndex = current.canvasStartIndex + current.affectedCanvasGroups.Length;
#else
         current.widgetStartIndex = current.affectedRenderers.Length;
#endif
#endif
    }

    #region Renderers
    void AddRenderer()
    {
        System.Array.Resize<Renderer>(ref current.affectedRenderers, current.affectedRenderers.Length + 1);
        System.Array.Resize<float>(ref current.childrenMaxAlpha, current.childrenMaxAlpha.Length + 1);
    }

    void RemoveRenderer(int index)
    {
#if(StoreVersion)
        TransitionalObject.RemoveFromArray<Renderer>(index, ref current.affectedRenderers);
        TransitionalObject.RemoveFromArray<float>(index, ref current.childrenMaxAlpha);
#else
        K2Maths.RemoveFromArray<Renderer>(index, ref current.affectedRenderers);
        K2Maths.RemoveFromArray<float>(index, ref current.childrenMaxAlpha);
#endif

        UpdateStartIndexes();
    }

    void RemoveAllRenderers()
    {
        System.Array.Resize<float>(ref current.childrenMaxAlpha, current.childrenMaxAlpha.Length - current.affectedRenderers.Length);
        System.Array.Resize<Renderer>(ref current.affectedRenderers, 0);

        UpdateStartIndexes();
    }

    float GetRendererAlpha(int index)
    {
        return current.affectedRenderers[index].sharedMaterial.color.a;
    }
    #endregion

#if(UsingUGUI)
    #region Images
    void AddImage()
    {
        System.Array.Resize<MaskableGraphic>(ref current.affectedImages, current.affectedImages.Length + 1);
        System.Array.Resize<float>(ref current.childrenMaxAlpha, current.childrenMaxAlpha.Length + 1);
    }

    void RemoveImage(int index)
    {
#if(StoreVersion)
        TransitionalObject.RemoveFromArray<MaskableGraphic>(index, ref current.affectedImages);
        TransitionalObject.RemoveFromArray<float>(current.imageStartIndex + index, ref current.childrenMaxAlpha);
#else
        K2Maths.RemoveFromArray<MaskableGraphic>(index, ref current.affectedImages);
        K2Maths.RemoveFromArray<float>(current.imageStartIndex + index, ref current.childrenMaxAlpha);
#endif

        UpdateStartIndexes();
    }

    void RemoveAllImages()
    {
        System.Array.Resize<float>(ref current.childrenMaxAlpha, current.childrenMaxAlpha.Length - current.affectedImages.Length);
        System.Array.Resize<MaskableGraphic>(ref current.affectedImages, 0);

        UpdateStartIndexes();
    }

    float GetImageAlpha(int index)
    {
        return current.affectedImages[index].color.a;
    }
    #endregion

    #region Canvases
    void AddCanvasGroup()
    {
        System.Array.Resize<CanvasGroup>(ref current.affectedCanvasGroups, current.affectedCanvasGroups.Length + 1);
        System.Array.Resize<float>(ref current.childrenMaxAlpha, current.childrenMaxAlpha.Length + 1);
    }

    void RemoveCanvasGroup(int index)
    {
#if(StoreVersion)
        TransitionalObject.RemoveFromArray<CanvasGroup>(index, ref current.affectedCanvasGroups);
        TransitionalObject.RemoveFromArray<float>(current.canvasStartIndex + index, ref current.childrenMaxAlpha);
#else
        K2Maths.RemoveFromArray<CanvasGroup>(index, ref current.affectedCanvasGroups);
        K2Maths.RemoveFromArray<float>(current.canvasStartIndex + index, ref current.childrenMaxAlpha);
#endif

        UpdateStartIndexes();
    }

    void RemoveAllCanvases()
    {
        System.Array.Resize<float>(ref current.childrenMaxAlpha, current.childrenMaxAlpha.Length - current.affectedCanvasGroups.Length);
        System.Array.Resize<CanvasGroup>(ref current.affectedCanvasGroups, 0);

        UpdateStartIndexes();
    }

    float GetCanvasAlpha(int index)
    {
        return current.affectedCanvasGroups[index].alpha;
    }
    #endregion
#endif

#if(UsingNGUI)
    #region Widgets
    void AddWidget()
    {
        System.Array.Resize<UIWidget>(ref current.affectedWidgets, current.affectedWidgets.Length + 1);
        System.Array.Resize<float>(ref current.childrenMaxAlpha, current.childrenMaxAlpha.Length + 1);
    }

    void RemoveWidget(int index)
    {
        TransitionalObject.RemoveFromArray<UIWidget>(index, ref current.affectedWidgets);
        TransitionalObject.RemoveFromArray<float>(current.widgetStartIndex + index, ref current.childrenMaxAlpha);
    }

    void RemoveAllWidgets()
    {
        System.Array.Resize<float>(ref current.childrenMaxAlpha, current.childrenMaxAlpha.Length - current.affectedWidgets.Length);
        System.Array.Resize<UIWidget>(ref current.affectedWidgets, 0);
    }

    float GetWidgetAlpha(int index)
    {
        return current.affectedWidgets[index].color.a;
    }
    #endregion
#endif
    #endregion

    void ShowComponents(bool show)
    {
        BaseTransition[] transitions = current.gameObject.GetComponents<BaseTransition>();

        for(int i = 0; i < transitions.Length; i++)
            if(transitions[i] != null)
                transitions[i].hideFlags = show ? HideFlags.None : HideFlags.HideInInspector;
    }

    void AddNewComponent(TransitionalObject current)
    {
        ResizeArrays(ref current, 1);

        switch(selectedType)
        {
            case ComponenentType.Manual:
                current.transitions[current.transitions.Length - 1] = current.gameObject.AddComponent<BaseTransition>();
                break;

            case ComponenentType.Alpha:
                current.transitions[current.transitions.Length - 1] = current.gameObject.AddComponent<AlphaTransition>();// AlphaTransition.CreateInstance<AlphaTransition>();

                CheckForAlphaSetup(current);
                break;

            case ComponenentType.Colour:
                current.transitions[current.transitions.Length - 1] = current.gameObject.AddComponent<ColourTransition>();// ColourTransition.CreateInstance<ColourTransition>();

                CheckForAlphaSetup(current);
                break;

            case ComponenentType.Movement:
                current.transitions[current.transitions.Length - 1] = current.gameObject.AddComponent<MovingTransition>();// MovingTransition.CreateInstance<MovingTransition>();
                break;

            case ComponenentType.Rotating:
                current.transitions[current.transitions.Length - 1] = current.gameObject.AddComponent<RotatingTransition>();// RotatingTransition.CreateInstance<RotatingTransition>();
                break;

            case ComponenentType.Scaling:
                current.transitions[current.transitions.Length - 1] = current.gameObject.AddComponent<ScalingTransition>();// ScalingTransition.CreateInstance<ScalingTransition>();
                break;
        }

        current.transitions[current.transitions.Length - 1].EditorInitialise(current);
    }

    #region Base Components
    enum BaseType
    {
        None = 0, Label, Delay, TransitionIn, DisplayTime, FadeOutDelay, FadeOutTime, TriggerInstantly,
        StayForever, Looping, CopyTransitionInTime, TransitionInCurve, FadeOutCurve
    }

    void DrawBaseComponents(BaseTransition transition)
    {
        #region Transition In Values
        using(new Vertical(subHeadingStyle))
        using(new ColourChange(subHeadingsColour))
            DrawToggle(ref transition.transitionInDropDown, new GUIContent("Transition In"));

        if(EditorGUILayout.BeginFadeGroup(transition.transitionInDropDown.faded))
        {
            GUI.color = backingColour;

            using(new Vertical(subContentStyle))
            {
                GUI.color = Color.white;

                GUILayout.Space(Spacing);

                #region Delay
                previousFloat = transition.delay;

                using(new Horizontal())
                {
                    using(new FixedWidthLabel("Delay"))
                        transition.delay = EditorGUILayout.FloatField(GUIContent.none, transition.delay);

                    if(transition.delay != 0)//only show if there is a delay
                        using(new FixedWidthLabel(new GUIContent("Run Once", "If this is true this delay will only run once, then it will reset to 0 for any other transitions")))
                        {
                            bool changed = EditorGUILayout.Toggle(transition.delay < 0);

                            if((!changed && transition.delay < 0) || (changed && transition.delay > 0))
                                transition.delay *= -1;
                        }
                }

                if(previousFloat != transition.delay)
                    SetBaseData(transition, BaseType.Delay);
                #endregion

                #region Run Time
                previousFloat = transition.transitionInTime;
                using(new FixedWidthLabel("Run Time"))
                    transition.transitionInTime = EditorGUILayout.FloatField(GUIContent.none, transition.transitionInTime);

                if(previousFloat != transition.transitionInTime)
                    SetBaseData(transition, BaseType.TransitionIn);

                if(transition.copyTransitionInTime)
                    transition.fadeOutTime = transition.transitionInTime;//copy the value to the fade out time as well, if needed
                #endregion

                #region Curve
                transition.transitionInCurve = EditorGUILayout.CurveField(transition.transitionInCurve, GUILayout.Height(CurveHeight));

                if(!CheckCurveKeys(transition.previousTransitionInCurve, transition.transitionInCurve.keys))
                    SetBaseData(transition, BaseType.TransitionInCurve);
                #endregion
            }
        }

        EditorGUILayout.EndFadeGroup();
        #endregion

        #region Fade Out Values
        GUILayout.Space(Spacing);

        using(new Vertical(subHeadingStyle))
        using(new ColourChange(subHeadingsColour))
            DrawToggle(ref transition.fadeOutDropDown, new GUIContent("Fade Out"));

        if(EditorGUILayout.BeginFadeGroup(transition.fadeOutDropDown.faded))
        {
            GUI.color = backingColour;

            using(new Vertical(subContentStyle))
            {
                GUI.color = Color.white;

                #region Delay
                previousFloat = transition.fadeOutDelay;

                using(new Horizontal())
                {
                    using(new FixedWidthLabel("Delay"))
                        transition.fadeOutDelay = EditorGUILayout.FloatField(GUIContent.none, transition.fadeOutDelay);

                    if(transition.fadeOutDelay != 0)//only show if there is a delay
                        using(new FixedWidthLabel(new GUIContent("Run Once", "If this is true this delay will only run once, then it will reset to 0 for any other transitions")))
                        {
                            bool changed = EditorGUILayout.Toggle(transition.fadeOutDelay < 0);

                            if((!changed && transition.fadeOutDelay < 0) || (changed && transition.fadeOutDelay > 0))
                                transition.fadeOutDelay *= -1;
                        }
                }

                if(previousFloat != transition.fadeOutDelay)
                    SetBaseData(transition, BaseType.FadeOutDelay);
                #endregion

                using(new Horizontal())
                {
                    #region Fade Out Time
                    using(new FixedWidthLabel("Run Time"))
                    {
                        if(transition.copyTransitionInTime)
                            GUI.enabled = false;

                        previousFloat = transition.fadeOutTime;
                        transition.fadeOutTime = EditorGUILayout.FloatField(GUIContent.none, transition.fadeOutTime, GUILayout.Width(50));

                        if(previousFloat != transition.fadeOutTime)
                            SetBaseData(transition, BaseType.FadeOutTime);
                    }

                    GUI.enabled = true;
                    #endregion

                    #region Copy Transition In Time
                    previousBool = transition.copyTransitionInTime;

                    using(new FixedWidthLabel("Match Transition In"))
                        transition.copyTransitionInTime = EditorGUILayout.Toggle(transition.copyTransitionInTime);

                    if(previousBool != transition.copyTransitionInTime)
                        SetBaseData(transition, BaseType.CopyTransitionInTime);
                    #endregion
                }

                #region Curve
                transition.fadeOutCurve = EditorGUILayout.CurveField(transition.fadeOutCurve, GUILayout.Height(CurveHeight));

                if(!CheckCurveKeys(transition.previousFadeOutCurve, transition.fadeOutCurve.keys))
                    SetBaseData(transition, BaseType.FadeOutCurve);
                #endregion
            }
        }

        EditorGUILayout.EndFadeGroup();
        #endregion

        #region Timings
        GUILayout.Space(Spacing);

        using(new Vertical(subHeadingStyle))
        using(new ColourChange(subHeadingsColour))
            DrawToggle(ref transition.loopingDropDown, new GUIContent("Timings"));

        if(EditorGUILayout.BeginFadeGroup(transition.loopingDropDown.faded))
        {
            GUI.color = backingColour;

            using(new Vertical(subContentStyle))
            {
                GUI.color = Color.white;

                using(new Horizontal())
                {
                    #region Trigger Instantly
                    previousBool = transition.triggerInstantly;
                    using(new FixedWidthLabel(new GUIContent("Trigger Instantly", "Starts animating as soon as the object becomes active")))
                        transition.triggerInstantly = EditorGUILayout.Toggle(transition.triggerInstantly);

                    if(previousBool != transition.triggerInstantly)
                        SetBaseData(transition, BaseType.TriggerInstantly);
                    #endregion

                    #region Stay Forever
                    previousBool = transition.stayForever;
                    using(new FixedWidthLabel(new GUIContent("Stay Forever", "Once this object has faded in don't fade it out")))
                        transition.stayForever = EditorGUILayout.Toggle(transition.stayForever);

                    if(previousBool != transition.stayForever)
                        SetBaseData(transition, BaseType.StayForever);
                    #endregion
                }

                if(!transition.stayForever)
                {
                    if(transition.displayTime < 0)
                    {
                        transition.displayTime = 0;//helper to reset the value automatically
                        SetBaseData(transition, BaseType.DisplayTime);
                    }

                    using(new Horizontal())
                    {
                        #region Display Time
                        previousFloat = transition.displayTime;
                        using(new FixedWidthLabel(new GUIContent("Display Time", "How long to wait before fading out again")))
                            transition.displayTime = EditorGUILayout.FloatField(transition.displayTime);

                        if(previousFloat != transition.displayTime)
                            SetBaseData(transition, BaseType.DisplayTime);
                        #endregion

                        #region Looping
                        previousBool = transition.looping;
                        using(new FixedWidthLabel(new GUIContent("Is Looping", "If true then this transition will loop infinitely")))
                            transition.looping = EditorGUILayout.Toggle(transition.looping);

                        if(previousBool != transition.looping)
                            SetBaseData(transition, BaseType.Looping);
                        #endregion
                    }
                }
                else
                {
                    transition.displayTime = -1;
                    SetBaseData(transition, BaseType.DisplayTime);
                }
            }
        }

        EditorGUILayout.EndFadeGroup();
        #endregion

        #region Messaging
        GUILayout.Space(Spacing);

        using(new Vertical(subHeadingStyle))
        using(new ColourChange(subHeadingsColour))
            DrawToggle(ref transition.messagingDropDown, new GUIContent(selectedTransitions.Count == 1 ? "Messages" : "Messages (This object only)"));

        if(EditorGUILayout.BeginFadeGroup(transition.messagingDropDown.faded))
        {
            GUI.color = backingColour;

            using(new Vertical(mainContentStyle))
            {
                GUI.color = Color.white;

                using(new Horizontal())
                {
                    if(GUILayout.Button(new GUIContent("+", "Add a new message target"), EditorStyles.miniButton))
                        ResizeEventArrays(ref transition, 1);

                    if(GUILayout.Button(new GUIContent("-", "Remove a message target"), EditorStyles.miniButton))
                        ResizeEventArrays(ref transition, -1);
                }

                if(transition.events != null)
                {
                    const string name = "events";
                    currentObject = new SerializedObject(transition);
                    int size = currentObject.FindProperty(name + ".Array.size").intValue;
                    SerializedProperty property;

                    for(int i = 0; i < size; i++)
                    {
                        EditorGUI.indentLevel = 0;
                        transition.whenToSends[i] = (BaseTransition.TransitionState)EditorGUILayout.EnumPopup("When To Send", transition.whenToSends[i]);

                        property = currentObject.FindProperty(string.Format("{0}.Array.data[{1}]", name, i));

                        EditorGUI.indentLevel = 3;
                        EditorGUILayout.PropertyField(property, new GUIContent(transition.whenToSends[i].ToString()));

                        GUILayout.Space(Spacing * 2);
                    }

                    EditorGUI.indentLevel = 0;
                }
            }
        }

        EditorGUILayout.EndFadeGroup();
        #endregion
    }

    bool CheckCurveKeys(Keyframe[] keys, Keyframe[] compare)
    {
        if(keys == null || compare == null)
            return false;

        if(keys.Length != compare.Length)
            return false;

        for(int i = 0; i < keys.Length; i++)
        {
            if(keys[i].inTangent != compare[i].inTangent || keys[i].outTangent != compare[i].outTangent ||
                keys[i].tangentMode != compare[i].tangentMode || keys[i].time != compare[i].time || keys[i].value != compare[i].value)
                return false;
        }

        return true;
    }

    void SetBaseData(BaseTransition data, BaseType type)
    {
        System.Type transitionType = data.GetType();

        if(selectedTransitions.Count == 1)
            return;//dont run this for editing single transitions

        for(int i = 0; i < selectedTransitions.Count; i++)
            for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                if(selectedTransitions[i].transitions[ii].GetType().Equals(transitionType))//if we found an alpha transition component
                {
                    switch(type)
                    {
                        case BaseType.Label:
                            selectedTransitions[i].transitions[ii].label = data.label;
                            break;

                        case BaseType.Delay:
                            selectedTransitions[i].transitions[ii].delay = data.delay;
                            break;

                        case BaseType.DisplayTime:
                            selectedTransitions[i].transitions[ii].displayTime = data.displayTime;
                            break;

                        case BaseType.FadeOutDelay:
                            selectedTransitions[i].transitions[ii].fadeOutDelay = data.fadeOutDelay;
                            break;

                        case BaseType.FadeOutTime:
                            selectedTransitions[i].transitions[ii].fadeOutTime = data.fadeOutTime;
                            break;

                        case BaseType.Looping:
                            selectedTransitions[i].transitions[ii].looping = data.looping;
                            break;

                        case BaseType.StayForever:
                            selectedTransitions[i].transitions[ii].stayForever = data.stayForever;
                            break;

                        case BaseType.TransitionIn:
                            selectedTransitions[i].transitions[ii].transitionInTime = data.transitionInTime;

                            if(selectedTransitions[i].transitions[ii].copyTransitionInTime)//copy values to the fade out time if needed
                                selectedTransitions[i].transitions[ii].fadeOutTime = data.transitionInTime;
                            break;

                        case BaseType.TriggerInstantly:
                            selectedTransitions[i].transitions[ii].triggerInstantly = data.triggerInstantly;
                            break;

                        case BaseType.TransitionInCurve:
                            selectedTransitions[i].transitions[ii].transitionInCurve = new AnimationCurve(data.transitionInCurve.keys);
                            selectedTransitions[i].transitions[ii].previousTransitionInCurve = (Keyframe[])data.transitionInCurve.keys.Clone();
                            break;

                        case BaseType.FadeOutCurve:
                            selectedTransitions[i].transitions[ii].fadeOutCurve = new AnimationCurve(data.fadeOutCurve.keys);
                            selectedTransitions[i].transitions[ii].previousFadeOutCurve = (Keyframe[])data.fadeOutCurve.keys.Clone();
                            break;

                        case BaseType.CopyTransitionInTime:
                            selectedTransitions[i].transitions[ii].copyTransitionInTime = data.copyTransitionInTime;
                            break;
                    }
                }
    }

    void DrawLabel(BaseTransition transition)
    {
        previousString = transition.label;
        using(new FixedWidthLabel(new GUIContent("Label", "This is not compiled and is an editor only variable")))
            transition.label = EditorGUILayout.TextField(GUIContent.none, transition.label);

        if(!previousString.Equals(transition.label))
            SetBaseData(transition, BaseType.Label);
    }

    void DrawControlsPanel(int index)
    {
        GUILayout.Space(Spacing);

        using(new Vertical(subHeadingStyle))
        using(new ColourChange(subHeadingsColour))
            DrawToggle(ref current.controlsGroup, new GUIContent("Controls"));

        if(EditorGUILayout.BeginFadeGroup(current.controlsGroup.faded))
            using(new Vertical(mainContentStyle))
            {
                if(GUILayout.Button(selectedTransitions.Count > 1 ? "Delete (This Object Only)" : "Delete"))
                    RemoveTransition(index);

                if(selectedTransitions.Count == 1)
                {
                    if(!reparenting && GUILayout.Button("Reparent"))
                        reparenting = true;

                    if(reparenting)
                    {
                        TransitionalObject temp = null;

                        using(new FixedWidthLabel("New Parent"))
                            temp = EditorGUILayout.ObjectField(temp, typeof(TransitionalObject)) as TransitionalObject;

                        if(temp != null)
                        {
                            if(current.transitions[index].GetType().Equals(typeof(AlphaTransition)))//select the selected type, read by AddComponent
                                selectedType = ComponenentType.Alpha;
                            else if(current.transitions[index].GetType().Equals(typeof(ColourTransition)))
                                selectedType = ComponenentType.Colour;
                            else if(current.transitions[index].GetType().Equals(typeof(MovingTransition)))
                                selectedType = ComponenentType.Movement;
                            else if(current.transitions[index].GetType().Equals(typeof(RotatingTransition)))
                                selectedType = ComponenentType.Rotating;
                            else// if(current.transitions[index].GetType().Equals(typeof(ScalingTransition)))
                                selectedType = ComponenentType.Scaling;

                            AddNewComponent(temp);
                            selectedType = 0;//restore

                            temp.transitions[temp.transitions.Length - 1].Clone(current.transitions[index]);
                            temp.transitions[temp.transitions.Length - 1].parent = temp;//reparent after the cloning

                            RemoveTransition(index);

                            reparenting = false;
                        }
                    }
                }
            }

        EditorGUILayout.EndFadeGroup();
    }

    void RemoveTransition(int index)
    {
        BaseTransition removedObject = current.transitions[index];//store a reference to the object we wish to remove

#if(StoreVersion)
        TransitionalObject.RemoveFromArray<BaseTransition>(index, ref current.transitions);
#else
        K2Maths.RemoveFromArray<BaseTransition>(index, ref current.transitions);
#endif

        if(removedObject.GetType().Equals(typeof(AlphaTransition)) || removedObject.GetType().Equals(typeof(ColourTransition)))
            current.alphaOrColourComponentCount--;

        DestroyImmediate(removedObject);//destroy the object now that its no longer referenced
    }

    #region Specific Components
    #region Manual
    void DrawManualComponent(int index, BaseTransition transition)
    {
        GUILayout.BeginVertical("Box");

        using(new ColourChange(headingsColour))
            DrawToggle(ref transition.mainDropDown, new GUIContent("Manual " + transition.label));

        if(EditorGUILayout.BeginFadeGroup(transition.mainDropDown.faded))
        {
            DrawLabel(transition);

            DrawBaseComponents(transition);

            DrawControlsPanel(index);
        }

        EditorGUILayout.EndFadeGroup();

        GUILayout.EndVertical();
    }
    #endregion

    #region Alpha
    void DrawAlphaComponent(int index, AlphaTransition transition)
    {
        if(current.alphaOrColourComponentCount == 0)///if the alpha component count is 0 whilst displaying an alpha component something is wrong
            ScanForAlphaComponents();

        using(new Vertical(headingStyles))
        using(new ColourChange(headingsColour))
            DrawToggle(ref transition.mainDropDown, new GUIContent("Alpha " + transition.label));

        if(EditorGUILayout.BeginFadeGroup(transition.mainDropDown.faded))
            using(new Vertical(mainContentStyle))
            {
                DrawLabel(transition);

                using(new FixedWidthLabel(new GUIContent("Start Faded", "Makes the object invisible when the game starts")))
                    transition.startFaded = EditorGUILayout.Toggle(transition.startFaded);

                SetStartFadedValue(transition.startFaded);

                DrawBaseComponents(transition);

                DrawControlsPanel(index);
            }

        EditorGUILayout.EndFadeGroup();
    }

    void SetStartFadedValue(bool value)
    {
        if(selectedTransitions.Count == 1)
            return;//dont run this for editing single transitions

        for(int i = 0; i < selectedTransitions.Count; i++)
            for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                if(selectedTransitions[i].transitions[ii].GetType().Equals(typeof(AlphaTransition)))//if we found an alpha transition component
                    ((AlphaTransition)selectedTransitions[i].transitions[ii]).startFaded = value;
    }

    void CheckForAlphaSetup(TransitionalObject current)
    {
        current.alphaOrColourComponentCount++;//bit of a hack to register alpha or colour
        current.InitialiseAlphaTransition();
    }

    void ScanForAlphaComponents()
    {
        current.alphaOrColourComponentCount = 0;

        for(int i = 0; i < current.transitions.Length; i++)
            if(current.transitions[i].GetType().Equals(typeof(AlphaTransition)) || current.transitions[i].GetType().Equals(typeof(ColourTransition)))
                current.alphaOrColourComponentCount++;
    }
    #endregion

    #region Moving
    void DrawMovingComponent(int index, MovingTransition transition)
    {
        using(new Vertical(headingStyles))
        using(new ColourChange(headingsColour))
            DrawToggle(ref transition.mainDropDown, new GUIContent("Position " + transition.label));

        if(EditorGUILayout.BeginFadeGroup(transition.mainDropDown.faded))
            using(new Vertical(mainContentStyle))
            {
                DrawLabel(transition);

                using(new Vertical(subHeadingStyle))
                using(new ColourChange(subHeadingsColour))
                    DrawToggle(ref transition.dataDropDown, new GUIContent("Data"));

                if(EditorGUILayout.BeginFadeGroup(transition.dataDropDown.faded))
                    using(new Vertical(subContentStyle))
                    {
                        #region Type
                        MovingTransition.MovementType previousType = transition.type;
                        using(new FixedWidthLabel(new GUIContent("Type", "How to move between these points")))
                            transition.type = (MovingTransition.MovementType)EditorGUILayout.EnumPopup(GUIContent.none, transition.type);

                        if(previousType != transition.type)
                            SetMovingData(transition, TransitionalObject.MovingDataType.Type);
                        #endregion

                        DrawMovingData(transition, true);//draw the start point data
                        DrawMovingData(transition, false);//draw the end point data
                    }

                EditorGUILayout.EndFadeGroup();

                DrawBaseComponents(transition);

                DrawControlsPanel(index);
            }

        EditorGUILayout.EndFadeGroup();
    }

    /// <summary>
    /// used to draw both the start and end points 
    /// </summary>
    void DrawMovingData(MovingTransition transition, bool startPoint)
    {
        Vector3 previousValue;

        GUILayout.Space(Spacing);

        GUILayout.Label(startPoint ? "Start Position" : "End Position", boldLabel);

        #region General Settings
        using(new Horizontal())
        {
            using(new FixedWidthLabel(new GUIContent("Read Current", "This means that no value is set and the start point is the current position when the transition is triggered")))
            {
                if(startPoint)
                    transition.startAtCurrent = EditorGUILayout.Toggle(transition.startAtCurrent);
                else
                    transition.endAtCurrent = EditorGUILayout.Toggle(transition.endAtCurrent);
            }

            if(!transition.startAtCurrent)
                using(new FixedWidthLabel("Deviate"))
                    if(startPoint)
                        transition.deviateStart = EditorGUILayout.Toggle(transition.deviateStart);
                    else
                        transition.deviateEnd = EditorGUILayout.Toggle(transition.deviateEnd);
        }
        #endregion

        if((!transition.startAtCurrent && startPoint) || (!transition.endAtCurrent && !startPoint))
        {
            using(new Horizontal())
            {
                if((transition.deviateStart && startPoint) || (transition.deviateEnd && !startPoint))//if we are deviating then we need to show 2 values
                {
                    #region Deviation Start
                    if(startPoint)
                    {
                        previousValue = transition.startPoint;
                        transition.minStart = EditorGUILayout.Vector3Field(GUIContent.none, transition.minStart);
                    }
                    else
                    {
                        previousValue = transition.endPoint;
                        transition.minEnd = EditorGUILayout.Vector3Field(GUIContent.none, transition.minEnd);
                    }

                    if(!Application.isPlaying)
                        if(startPoint)
                            transition.startPoint = transition.minStart;
                        else
                            transition.endPoint = transition.minEnd;

                    if(startPoint)
                    {
                        if(previousValue != transition.minStart)
                            SetMovingData(transition, TransitionalObject.MovingDataType.StartPoint);
                    }
                    else
                        if(previousValue != transition.minEnd)
                            SetMovingData(transition, TransitionalObject.MovingDataType.EndPoint);
                    #endregion
                }
                else
                {
                    #region Transform Boxes
                    if(startPoint)
                    {
                        previousValue = transition.startPoint;

                        transition.startPoint = EditorGUILayout.Vector3Field(GUIContent.none, transition.startPoint);
                        transition.minStart = transition.startPoint;

                        if(previousValue != transition.startPoint)
                            SetMovingData(transition, TransitionalObject.MovingDataType.StartPoint);
                    }
                    else
                    {
                        previousValue = transition.endPoint;

                        transition.endPoint = EditorGUILayout.Vector3Field(GUIContent.none, transition.endPoint);
                        transition.minEnd = transition.endPoint;

                        if(previousValue != transition.endPoint)
                            SetMovingData(transition, TransitionalObject.MovingDataType.EndPoint);
                    }
                    #endregion
                }

                DrawTransformControls(transition, startPoint ? TransitionalObject.MovingDataType.StartPoint : TransitionalObject.MovingDataType.EndPoint);//Draw the top buttons
            }


            #region Deviation End
            if((startPoint && transition.deviateStart) || (!startPoint && transition.deviateEnd))
            {
                using(new Horizontal())
                {
                    if(startPoint)
                    {
                        previousValue = transition.maxStart;

                        transition.maxStart = EditorGUILayout.Vector3Field(GUIContent.none, transition.maxStart);

                        if(previousValue != transition.maxStart)
                            SetMovingData(transition, TransitionalObject.MovingDataType.MaxStart);

                        DrawTransformControls(transition, TransitionalObject.MovingDataType.MaxStart);//Draw the top buttons
                    }
                    else
                    {
                        previousValue = transition.maxEnd;

                        transition.maxEnd = EditorGUILayout.Vector3Field(GUIContent.none, transition.maxEnd);

                        if(previousValue != transition.maxEnd)
                            SetMovingData(transition, TransitionalObject.MovingDataType.MaxEnd);

                        DrawTransformControls(transition, TransitionalObject.MovingDataType.MaxEnd);//Draw the top buttons
                    }
                }

                EditorGUILayout.HelpBox("The start point will be a randomly selected position between these two points", MessageType.Info);
            }
            #endregion
        }
        else
        {
            if(startPoint)
                EditorGUILayout.HelpBox("Instead of moving to a start point this transition will always start from its current position", MessageType.Info);
            else
                EditorGUILayout.HelpBox("Instead of moving to an end point this transition will always start from its current position", MessageType.Info);
        }
        GUI.enabled = true;
    }

    void SetMovingData(MovingTransition data, TransitionalObject.MovingDataType type)
    {
        if(selectedTransitions.Count == 1)
            return;//dont run this for editing single transitions

        for(int i = 0; i < selectedTransitions.Count; i++)
            for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                if(selectedTransitions[i].transitions[ii].GetType().Equals(typeof(MovingTransition)))//if we found an alpha transition component
                {
                    switch(type)
                    {
                        case TransitionalObject.MovingDataType.Type:
                            ((MovingTransition)selectedTransitions[i].transitions[ii]).type = data.type;
                            break;

                        case TransitionalObject.MovingDataType.StartPoint:
                            ((MovingTransition)selectedTransitions[i].transitions[ii]).startPoint = data.startPoint;
                            ((MovingTransition)selectedTransitions[i].transitions[ii]).minStart = data.startPoint;
                            break;

                        case TransitionalObject.MovingDataType.EndPoint:
                            ((MovingTransition)selectedTransitions[i].transitions[ii]).endPoint = data.endPoint;
                            ((MovingTransition)selectedTransitions[i].transitions[ii]).minEnd = data.endPoint;
                            break;

                        case TransitionalObject.MovingDataType.MaxStart:
                            ((MovingTransition)selectedTransitions[i].transitions[ii]).maxStart = data.maxStart;
                            break;

                        case TransitionalObject.MovingDataType.MaxEnd:
                            ((MovingTransition)selectedTransitions[i].transitions[ii]).maxEnd = data.maxEnd;
                            break;
                    }
                }
    }
    #endregion

    #region Rotating
    enum RotatingDataType { Reverse = 0, StartPoint, EndPoint }

    void DrawRotatingComponent(int index, RotatingTransition transition)
    {
        using(new Vertical(headingStyles))
        using(new ColourChange(headingsColour))
            DrawToggle(ref transition.mainDropDown, new GUIContent("Rotation " + transition.label));

        if(EditorGUILayout.BeginFadeGroup(transition.mainDropDown.faded))
            using(new Vertical(mainContentStyle))
            {
                DrawLabel(transition);

                using(new Vertical(subHeadingStyle))
                using(new ColourChange(subHeadingsColour))
                    DrawToggle(ref transition.dataDropDown, new GUIContent("Data"));

                if(EditorGUILayout.BeginFadeGroup(transition.dataDropDown.faded))
                    using(new Vertical(subContentStyle))
                    {
                        #region Reverse Rotation
                        bool previousReverse = transition.reverseNegativeRotations;
                        using(new FixedWidthLabel(new GUIContent("Reverse", "Unity automatically flips rotation to be positive. This helps to reverse any animations behaving strangely")))
                            transition.reverseNegativeRotations = EditorGUILayout.Toggle(transition.reverseNegativeRotations);

                        if(previousReverse != transition.reverseNegativeRotations)
                            SetRotatingData(transition, RotatingDataType.Reverse);
                        #endregion

                        #region Start Point
                        using(new Horizontal())
                            DrawTransformControls(transition, TransitionalObject.MovingDataType.StartPoint);

                        Vector3 previous = transition.startPoint;
                        using(new FixedWidthLabel(new GUIContent("Start", "What direction to start the transition at")))
                            transition.startPoint = EditorGUILayout.Vector3Field(GUIContent.none, transition.startPoint);

                        if(previous != transition.startPoint)
                            SetRotatingData(transition, RotatingDataType.StartPoint);
                        #endregion

                        #region End Point
                        GUILayout.Space(Spacing * 2);

                        using(new Horizontal())
                            DrawTransformControls(transition, TransitionalObject.MovingDataType.EndPoint);

                        previous = transition.endPoint;
                        using(new FixedWidthLabel(new GUIContent("End", "What direction to end the transition at")))
                            transition.endPoint = EditorGUILayout.Vector3Field(GUIContent.none, transition.endPoint);

                        if(previous != transition.endPoint)
                            SetRotatingData(transition, RotatingDataType.EndPoint);
                        #endregion
                    }

                EditorGUILayout.EndFadeGroup();

                DrawBaseComponents(transition);

                DrawControlsPanel(index);
            }


        EditorGUILayout.EndFadeGroup();
    }

    void SetRotatingData(RotatingTransition data, RotatingDataType type)
    {
        if(selectedTransitions.Count == 1)
            return;//dont run this for editing single transitions

        for(int i = 0; i < selectedTransitions.Count; i++)
            for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                if(selectedTransitions[i].transitions[ii].GetType().Equals(typeof(RotatingTransition)))//if we found an alpha transition component
                {
                    switch(type)
                    {
                        case RotatingDataType.Reverse:
                            ((RotatingTransition)selectedTransitions[i].transitions[ii]).reverseNegativeRotations = data.reverseNegativeRotations;
                            break;

                        case RotatingDataType.StartPoint:
                            ((RotatingTransition)selectedTransitions[i].transitions[ii]).startPoint = data.startPoint;
                            break;

                        case RotatingDataType.EndPoint:
                            ((RotatingTransition)selectedTransitions[i].transitions[ii]).endPoint = data.endPoint;
                            break;
                    }
                }
    }
    #endregion

    #region Scaling
    void DrawScalingComponent(int index, ScalingTransition transition)
    {
        using(new Vertical(headingStyles))
        using(new ColourChange(headingsColour))
            DrawToggle(ref transition.mainDropDown, new GUIContent("Scale " + transition.label));

        if(EditorGUILayout.BeginFadeGroup(transition.mainDropDown.faded))
            using(new Vertical(mainContentStyle))
            {
                DrawLabel(transition);

                using(new Vertical(subHeadingStyle))
                using(new ColourChange(subHeadingsColour))
                    DrawToggle(ref transition.dataDropDown, new GUIContent("Data"));

                if(EditorGUILayout.BeginFadeGroup(transition.dataDropDown.faded))
                    using(new Vertical(subContentStyle))
                    {
                        #region Start Point
                        using(new Horizontal())
                            DrawTransformControls(transition, TransitionalObject.MovingDataType.StartPoint);

                        Vector3 previous = transition.startPoint;
                        using(new FixedWidthLabel(new GUIContent("Start", "What scale to start the transition at")))
                            transition.startPoint = EditorGUILayout.Vector3Field(GUIContent.none, transition.startPoint);

                        if(previous != transition.startPoint)
                            SetScalingData(transition, true);
                        #endregion

                        #region End Point
                        GUILayout.Space(Spacing * 2);

                        using(new Horizontal())
                            DrawTransformControls(transition, TransitionalObject.MovingDataType.EndPoint);

                        previous = transition.endPoint;
                        using(new FixedWidthLabel(new GUIContent("End", "What scale to end the transition at")))
                            transition.endPoint = EditorGUILayout.Vector3Field(GUIContent.none, transition.endPoint);

                        if(previous != transition.endPoint)
                            SetScalingData(transition, false);
                        #endregion
                    }

                EditorGUILayout.EndFadeGroup();

                DrawBaseComponents(transition);

                DrawControlsPanel(index);
            }

        EditorGUILayout.EndFadeGroup();
    }

    void SetScalingData(ScalingTransition data, bool startPoint)
    {
        if(selectedTransitions.Count == 1)
            return;//dont run this for editing single transitions

        for(int i = 0; i < selectedTransitions.Count; i++)
            for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                if(selectedTransitions[i].transitions[ii].GetType().Equals(typeof(ScalingTransition)))//if we found an alpha transition component
                {
                    if(startPoint)
                        ((ScalingTransition)selectedTransitions[i].transitions[ii]).startPoint = data.startPoint;
                    else
                        ((ScalingTransition)selectedTransitions[i].transitions[ii]).endPoint = data.endPoint;
                }
    }
    #endregion

    #region Colouring
    void DrawColouringComponent(int index, ColourTransition transition)
    {
        if(current.alphaOrColourComponentCount == 0)///if the alpha component count is 0 whilst displaying an alpha component something is wrong
            ScanForAlphaComponents();

        using(new Vertical(headingStyles))
        using(new ColourChange(headingsColour))
            DrawToggle(ref transition.mainDropDown, new GUIContent("Colour " + transition.label));

        if(EditorGUILayout.BeginFadeGroup(transition.mainDropDown.faded))
            using(new Vertical(mainContentStyle))
            {
                DrawLabel(transition);

                using(new Vertical(subHeadingStyle))
                using(new ColourChange(subHeadingsColour))
                    DrawToggle(ref transition.dataDropDown, new GUIContent("Data"));

                if(EditorGUILayout.BeginFadeGroup(transition.dataDropDown.faded))
                    using(new Vertical(subContentStyle))
                    {
                        #region Start Colour
                        Color previous = transition.startColour;
                        using(new FixedWidthLabel(new GUIContent("Start", "What colour to start the transition at")))
                            transition.startColour = EditorGUILayout.ColorField(GUIContent.none, transition.startColour);

                        if(previous != transition.startColour)
                            SetColouringData(transition, true);
                        #endregion

                        #region End Colour
                        previous = transition.endColour;
                        using(new FixedWidthLabel(new GUIContent("End", "What colour to end the transition at")))
                            transition.endColour = EditorGUILayout.ColorField(GUIContent.none, transition.endColour);

                        if(previous != transition.endColour)
                            SetColouringData(transition, false);
                        #endregion
                    }

                EditorGUILayout.EndFadeGroup();

                DrawBaseComponents(transition);

                DrawControlsPanel(index);
            }

        EditorGUILayout.EndFadeGroup();
    }

    void SetColouringData(ColourTransition data, bool startColour)
    {
        if(selectedTransitions.Count == 1)
            return;//dont run this for editing single transitions

        for(int i = 0; i < selectedTransitions.Count; i++)
            for(int ii = 0; ii < selectedTransitions[i].transitions.Length; ii++)
                if(selectedTransitions[i].transitions[ii].GetType().Equals(typeof(ColourTransition)))//if we found an alpha transition component
                {
                    ((ColourTransition)selectedTransitions[i].transitions[ii]).startColour = data.startColour;
                    ((ColourTransition)selectedTransitions[i].transitions[ii]).endColour = data.endColour;
                }
    }
    #endregion
    #endregion
    #endregion

    /// <summary>
    /// Draws the View and Update buttons for animation components
    /// </summary>
    /// <param name="current"></param>
    /// <param name="isStartPoint"></param>
    void DrawTransformControls(BaseTransition current, TransitionalObject.MovingDataType type)
    {
        #region Copy Current Button
        if(GUILayout.Button(new GUIContent("Update", "Updates this value with the data from current transform")))
        {
            if(Selection.gameObjects.Length > 1)//if there are multiple objects in this selection
            {
                if(type == TransitionalObject.MovingDataType.StartPoint)
                    TransitionalObjectMenu.UpdateStart();//this function will update all selected items
                else if(type == TransitionalObject.MovingDataType.MaxStart)
                    TransitionalObjectMenu.UpdateStartDeviation();
                else if(type == TransitionalObject.MovingDataType.MaxEnd)
                    TransitionalObjectMenu.UpdateEndDeviation();
                else
                    TransitionalObjectMenu.UpdateEnd();
            }
            else
                #region Single Selection
                current.UpdatePosition(type);
                #endregion
        }
        #endregion

        #region View Current Button
        if(GUILayout.Button(new GUIContent("View", "Updates the current transform to view the animation")))
        {
            if(Selection.gameObjects.Length > 1)//if there are multiple objects in this selection
            {
                if(type == TransitionalObject.MovingDataType.StartPoint)
                    TransitionalObjectMenu.SetToStart();
                else if(type == TransitionalObject.MovingDataType.MaxStart)
                    TransitionalObjectMenu.SetToStartDeviation();
                else if(type == TransitionalObject.MovingDataType.MaxEnd)
                    TransitionalObjectMenu.SetToEndDeviation();
                else
                    TransitionalObjectMenu.SetToEnd();
            }
            else
            {
                Undo.RecordObject(current, "View Transition Point");//apparently this isn't enough and I need to add snapshots etc instead
                current.ViewPosition(type);
            }
        }
        #endregion
    }

    void ResizeArrays(ref TransitionalObject current, int change)
    {
        int size = current.transitions.Length + change;

        if(size < 1)
        {
            current.transitions = new BaseTransition[0];

            currentObject = new SerializedObject(((TransitionalObject)target));//remake the object to update the values
            return;
        }

        System.Array.Resize<BaseTransition>(ref current.transitions, size);

        currentObject = new SerializedObject(((TransitionalObject)target));//remake the object to update the values
    }

    void ResizeEventArrays(ref BaseTransition current, int change)
    {
        int size = current.events.Length + change;

        if(size < 1)
        {
            current.events = new UnityEvent[0];
            current.whenToSends = new BaseTransition.TransitionState[0];

            currentObject = new SerializedObject(((TransitionalObject)target));//remake the object to update the values
            return;
        }

        System.Array.Resize<UnityEvent>(ref current.events, size);
        System.Array.Resize<BaseTransition.TransitionState>(ref current.whenToSends, size);

        if(change == 1)//if adding a new component
            current.whenToSends[current.whenToSends.Length - 1] = BaseTransition.TransitionState.Waiting;//start on waiting since its the one needed most of the time

        currentObject = new SerializedObject(((BaseTransition)current));//remake the object to update the values
    }

    void DrawToggle(ref AnimBool animation, GUIContent content)
    {
        if(animation == null)
        {
            animation = new AnimBool();
            animation.target = true;//animate the box opening for the first time to show its interactable
        }

        //GUI.contentColor = EditorGUIUtility.isProSkin ? headingsColourPro : new Color(0f, 0f, 0f, 0.85f);//change the colour of the heading to get it to stand out

        if(!GUILayout.Toggle(true, content, "PreToolbar2", GUILayout.MinWidth(20f)))
            animation.target = !animation.target;//invert

        //GUI.contentColor = Color.white;
    }

    void DrawCenteredToggle(ref AnimBool animation, GUIContent content)
    {
        using(new Horizontal())
        {
            GUILayout.FlexibleSpace();
            DrawToggle(ref animation, content);
            GUILayout.FlexibleSpace();
        }
    }

    #region Preferences Window
    static bool loaded = false;

    [PreferenceItem("K2 Games")]
    static void CustomPreferencesGUI()
    {
        #region Initialise
        GUIStyle centeredLabel = new GUIStyle(EditorStyles.boldLabel);
        centeredLabel.alignment = TextAnchor.MiddleCenter;

        if(EditorGUIUtility.isProSkin)
            centeredLabel.normal.textColor = Color.white;

        if(!loaded)
        {
            headingsColour = new Color(EditorPrefs.GetFloat("HeadingR"), EditorPrefs.GetFloat("HeadingG"), EditorPrefs.GetFloat("HeadingB"), EditorPrefs.GetFloat("HeadingA"));
            subHeadingsColour = new Color(EditorPrefs.GetFloat("SubHeadingR"), EditorPrefs.GetFloat("SubHeadingG"), EditorPrefs.GetFloat("SubHeadingB"), EditorPrefs.GetFloat("SubHeadingA"));
            backingColour = new Color(EditorPrefs.GetFloat("BackingR"), EditorPrefs.GetFloat("BackingG"), EditorPrefs.GetFloat("BackingB"), EditorPrefs.GetFloat("BackingA"));

            loaded = true;
        }
        #endregion

        EditorGUILayout.LabelField("Simple Transitions", centeredLabel);

        #region Colours
        using(new FixedWidthLabel("Headings Colour"))
            headingsColour = EditorGUILayout.ColorField(headingsColour);

        using(new FixedWidthLabel("Sub Headings Colour"))
            subHeadingsColour = EditorGUILayout.ColorField(subHeadingsColour);

        using(new FixedWidthLabel("Backing Colour"))
            backingColour = EditorGUILayout.ColorField(backingColour);
        #endregion

        GUILayout.Space(20);

        if(GUILayout.Button("Restore Defualt Values"))
        {
            EditorPrefs.DeleteKey("HeadingR");
            EditorPrefs.DeleteKey("HeadingG");
            EditorPrefs.DeleteKey("HeadingB");
            EditorPrefs.DeleteKey("HeadingA");

            EditorPrefs.DeleteKey("SubHeadingR");
            EditorPrefs.DeleteKey("SubHeadingG");
            EditorPrefs.DeleteKey("SubHeadingB");
            EditorPrefs.DeleteKey("SubHeadingA");

            EditorPrefs.DeleteKey("BackingR");
            EditorPrefs.DeleteKey("BackingG");
            EditorPrefs.DeleteKey("BackingB");
            EditorPrefs.DeleteKey("BackingA");

            ResetColours();
        }

        #region Finalise
        if(GUI.changed)
        {
            EditorPrefs.SetFloat("HeadingR", headingsColour.r);
            EditorPrefs.SetFloat("HeadingG", headingsColour.g);
            EditorPrefs.SetFloat("HeadingB", headingsColour.b);
            EditorPrefs.SetFloat("HeadingA", headingsColour.a);

            EditorPrefs.SetFloat("SubHeadingR", subHeadingsColour.r);
            EditorPrefs.SetFloat("SubHeadingG", subHeadingsColour.g);
            EditorPrefs.SetFloat("SubHeadingB", subHeadingsColour.b);
            EditorPrefs.SetFloat("SubHeadingA", subHeadingsColour.a);

            EditorPrefs.SetFloat("BackingR", backingColour.r);
            EditorPrefs.SetFloat("BackingG", backingColour.g);
            EditorPrefs.SetFloat("BackingB", backingColour.b);
            EditorPrefs.SetFloat("BackingA", backingColour.a);

        }
        #endregion
    }
    #endregion
}
