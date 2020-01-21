#define StoreVersion

using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// A collection of functions which are used in the menu items.
/// Author: Jason Guthrie
/// Company: K2 Games
/// Website: www.facebook.com/K2 Games
/// Email: support@k2games.co.uk
/// </summary>
public class TransitionalObjectMenu : EditorWindow
{
    #region Variables
    float startDelay, increment;
    bool reverse;
    GameObject first;

    enum StaggerType { Delay = 0, TransitionTime, FadeOutDelay, FadeOutTime, DisplayTime, Rename }
    static StaggerType staggerType;//what type of stagger is being used

#if(StoreVersion)
    const string menuName = "Edit/";//if you would rather this has its own menu then just make this variable empty and you will notice a new menu called Transitions in the editor
#else
   const string menuName = "K2 Framework/";//this simply moves the menu to a place we use. However Unity, wisely, recommend we don't do this for each published asset
#endif
    #endregion

    void OnGUI()
    {
        this.position = new Rect(Screen.currentResolution.width / 2 - 125, Screen.currentResolution.height / 2 - 75, 250, 150);

        if(staggerType == StaggerType.Rename)
        {
            first = EditorGUILayout.ObjectField("First Object", first, typeof(GameObject), true) as GameObject;

            EditorGUILayout.LabelField(new GUIContent("Help (Hover)", "Assign your first object.\nThen select every object to rename and hit run.\nObjects are renamed according to their position in relation to the first object."));
        }
        else
        {
            switch(staggerType)
            {
                case StaggerType.Delay:
                    startDelay = EditorGUILayout.FloatField(new GUIContent("Start Delay", "What is the delay of the first selected object"), startDelay);
                    break;

                case StaggerType.FadeOutDelay:
                    startDelay = EditorGUILayout.FloatField(new GUIContent("Start Fade Out Delay", "What is the fade out delay of the first selected object"), startDelay);
                    break;

                case StaggerType.DisplayTime:
                    startDelay = EditorGUILayout.FloatField(new GUIContent("Start Display Time", "What is the display time of the first selected object"), startDelay);
                    break;

                case StaggerType.TransitionTime:
                    startDelay = EditorGUILayout.FloatField(new GUIContent("Start Transition Time", "What is the transition time of the first selected object"), startDelay);
                    break;

                case StaggerType.FadeOutTime:
                    startDelay = EditorGUILayout.FloatField(new GUIContent("Start Fade Out Time", "What is the fade out time of the first selected object"), startDelay);
                    break;
            }

            increment = EditorGUILayout.FloatField(new GUIContent("Increment", "How much to add onto the delay for each object."), increment);
            reverse = EditorGUILayout.Toggle(new GUIContent("Reverse", "If the stagger should be applied in reverse order"), reverse);

            EditorGUILayout.LabelField(new GUIContent("Help (Hover)", "Selected objects will have the Start and Increment values added according to their position in the selection.\nTo ensure consistent results give each object a unqiue name and sort them alphabetically."));
        }

        if(GUILayout.Button(new GUIContent("Run", "Staggers over all selected transitions")))
        {
            if(staggerType == StaggerType.Rename)
            {
                char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

                List<GameObject> objects = new List<GameObject>();
                objects.Add(first);//add the first item

                float distance, lastDistance = 0;

                for(int i = 0; i < Selection.gameObjects.Length; i++)
                {
                    if(Selection.gameObjects[i].Equals(first))
                        continue;//ignore the first item when it appears

                    distance = Vector3.SqrMagnitude(objects[0].transform.position - Selection.gameObjects[i].transform.position);

                    if(distance > lastDistance)//if we are further away than the last object
                    {
                        objects.Add(Selection.gameObjects[i]);//just add it onto the end
                        lastDistance = distance;//and record the last distance
                    }
                    else
                    {
                        float newDistance;

                        for(int j = 1; j < objects.Count; j++)//ignore the first
                        {
                            newDistance = Vector3.SqrMagnitude(objects[j].transform.position - objects[0].transform.position);//determine the distance to every object we have added already

                            if(newDistance > distance)
                            {
                                objects.Insert(j, Selection.gameObjects[i]);
                                lastDistance = Vector3.SqrMagnitude(objects[objects.Count - 1].transform.position - objects[0].transform.position);
                                break;
                            }
                        }
                    }
                }

                for(int i = 0; i < objects.Count; i++)
                {
                    if(i < alphabet.Length)
                        objects[i].name += " " + alphabet[i];
                    else
                    {
                        objects[i].name += " ";

                        for(int j = 0; j < i / alphabet.Length; j++)
                            objects[i].name += alphabet[alphabet.Length - 1];//figure outhow many Z's to add on

                        objects[i].name += alphabet[i - alphabet.Length];
                    }
                }
            }
            else
            {
                GameObject[] selection = Selection.gameObjects;

#if(StoreVersion)
                selection = SortAlphabetically(selection, !reverse);
#else
                //selection = K2Maths.SortAlphabetically(selection, !reverse);
#endif

                for(int i = 0; i < selection.Length; i++)
                {
                    TransitionalObject temp = selection[i].GetComponent<TransitionalObject>();

                    if(temp == null)
                        temp = selection[i].GetComponentInChildren<TransitionalObject>();


                    switch(staggerType)
                    {
                        case StaggerType.Delay:
                            temp.Delay = startDelay + i * increment;
                            break;

                        case StaggerType.DisplayTime:
                            temp.DisplayTime = startDelay + i * increment;
                            break;

                        case StaggerType.TransitionTime:
                            temp.TransitionInTime = startDelay + i * increment;
                            break;
                    }

                }
            }

            this.Close();
        }

        if(GUILayout.Button(new GUIContent("Close", "Closes this window")))
            this.Close();
    }

    #region Menu Items
    [MenuItem(menuName + "Transitions/Staggering/Renaming")]//Changing this alters the menu at the top of Unity. Removing the word 'K2 Framework/' will make a new drop down menu as seen in the video
    public static void StaggeringRename()
    {
        TransitionalObjectMenu window = new TransitionalObjectMenu();
        window.ShowPopup();

        staggerType = StaggerType.Rename;
    }

    [MenuItem(menuName + "Transitions/Staggering/Delay")]
    public static void StaggeringDelay()
    {
        TransitionalObjectMenu window = new TransitionalObjectMenu();
        window.ShowPopup();

        staggerType = StaggerType.Delay;
    }

    [MenuItem(menuName + "Transitions/Staggering/Transition Time")]
    public static void StaggeringTransitionTime()
    {
        TransitionalObjectMenu window = new TransitionalObjectMenu();
        window.ShowPopup();

        staggerType = StaggerType.TransitionTime;
    }

    [MenuItem(menuName + "Transitions/Staggering/Display Time")]
    public static void StaggeringDisplayTime()
    {
        TransitionalObjectMenu window = new TransitionalObjectMenu();
        window.ShowPopup();

        staggerType = StaggerType.DisplayTime;
    }

    [MenuItem(menuName + "Transitions/View Start")]
    public static void SetToStart()
    {
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            TransitionalObject temp = Selection.gameObjects[i].GetComponent<TransitionalObject>();

            if(temp == null)
                if(Selection.gameObjects.Length == 1)//if we have only selected one object
                {
                    TransitionalObject[] children = Selection.gameObjects[0].GetComponentsInChildren<TransitionalObject>();//find in children instead

                    for(int j = 0; j < children.Length; j++)
                        children[j].ViewPosition(TransitionalObject.MovingDataType.StartPoint);

                    return;
                }
                else
                    temp = Selection.gameObjects[i].GetComponentInChildren<TransitionalObject>();

            temp.ViewPosition(TransitionalObject.MovingDataType.StartPoint);
        }
    }

    [MenuItem(menuName + "Transitions/View Start Devaitions")]
    public static void SetToStartDeviation()
    {
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            TransitionalObject temp = Selection.gameObjects[i].GetComponent<TransitionalObject>();

            if(temp == null)
                if(Selection.gameObjects.Length == 1)//if we have only selected one object
                {
                    TransitionalObject[] children = Selection.gameObjects[0].GetComponentsInChildren<TransitionalObject>();//find in children instead

                    for(int j = 0; j < children.Length; j++)
                        children[j].ViewPosition(TransitionalObject.MovingDataType.MaxStart);

                    return;
                }
                else
                    temp = Selection.gameObjects[i].GetComponentInChildren<TransitionalObject>();

            temp.ViewPosition(TransitionalObject.MovingDataType.MaxStart);
        }
    }

    [MenuItem(menuName + "Transitions/Update Start Points")]
    public static void UpdateStart()
    {
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            TransitionalObject temp = Selection.gameObjects[i].GetComponent<TransitionalObject>();

            if(temp == null)
                temp = Selection.gameObjects[i].GetComponentInChildren<TransitionalObject>();

            temp.UpdatePosition(TransitionalObject.MovingDataType.StartPoint);
        }
    }

    [MenuItem(menuName + "Transitions/Update Start Deviations")]
    public static void UpdateStartDeviation()
    {
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            TransitionalObject temp = Selection.gameObjects[i].GetComponent<TransitionalObject>();

            if(temp == null)
                temp = Selection.gameObjects[i].GetComponentInChildren<TransitionalObject>();

            temp.UpdatePosition(TransitionalObject.MovingDataType.MaxStart);
        }
    }

    [MenuItem(menuName + "Transitions/View End")]
    public static void SetToEnd()
    {
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            TransitionalObject temp = Selection.gameObjects[i].GetComponent<TransitionalObject>();

            if(temp == null)
                if(Selection.gameObjects.Length == 1)//if we have only selected one object
                {
                    TransitionalObject[] children = Selection.gameObjects[0].GetComponentsInChildren<TransitionalObject>();//find in children instead

                    for(int j = 0; j < children.Length; j++)
                        children[j].ViewPosition(TransitionalObject.MovingDataType.EndPoint);

                    return;
                }
                else
                    temp = Selection.gameObjects[i].GetComponentInChildren<TransitionalObject>();

            temp.ViewPosition(TransitionalObject.MovingDataType.EndPoint);
        }
    }

    [MenuItem(menuName + "Transitions/View End Deviations")]
    public static void SetToEndDeviation()
    {
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            TransitionalObject temp = Selection.gameObjects[i].GetComponent<TransitionalObject>();

            if(temp == null)
                if(Selection.gameObjects.Length == 1)//if we have only selected one object
                {
                    TransitionalObject[] children = Selection.gameObjects[0].GetComponentsInChildren<TransitionalObject>();//find in children instead

                    for(int j = 0; j < children.Length; j++)
                        children[j].ViewPosition(TransitionalObject.MovingDataType.MaxEnd);

                    return;
                }
                else
                    temp = Selection.gameObjects[i].GetComponentInChildren<TransitionalObject>();

            temp.ViewPosition(TransitionalObject.MovingDataType.MaxEnd);
        }
    }

    [MenuItem(menuName + "Transitions/Update End")]
    public static void UpdateEnd()
    {
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            TransitionalObject temp = Selection.gameObjects[i].GetComponent<TransitionalObject>();

            if(temp == null)
                temp = Selection.gameObjects[i].GetComponentInChildren<TransitionalObject>();

            temp.UpdatePosition(TransitionalObject.MovingDataType.EndPoint);
        }
    }

    [MenuItem(menuName + "Transitions/Update End Deviation")]
    public static void UpdateEndDeviation()
    {
        for(int i = 0; i < Selection.gameObjects.Length; i++)
        {
            TransitionalObject temp = Selection.gameObjects[i].GetComponent<TransitionalObject>();

            if(temp == null)
                temp = Selection.gameObjects[i].GetComponentInChildren<TransitionalObject>();

            temp.UpdatePosition(TransitionalObject.MovingDataType.MaxEnd);
        }
    }
    #endregion

    /// <summary>
    /// Takes a list of gameobjects and sorts them alphabetically. Sorts by ascending as a defualt
    /// </summary>
    public static GameObject[] SortAlphabetically(GameObject[] input)
    {
        return SortAlphabetically(input, true);
    }

    /// <summary>
    /// Takes a list of gameobjects and sorts them alphabetically.
    /// </summary>
    /// <param name="input">Your gameobject array</param>
    /// <param name="sortAscending">If the list should be A>Z or Z>A</param>
    /// <returns></returns>
    public static GameObject[] SortAlphabetically(GameObject[] input, bool sortAscending)
    {
        string[] names = new string[input.Length];
        GameObject[] output = new GameObject[input.Length];

        for(int i = 0; i < input.Length; i++)
            names[i] = input[i].name;

        Array.Sort(names);

        if(!sortAscending)
            Array.Reverse(names);

        for(int i = 0; i < input.Length; i++)
            for(int j = 0; j < input.Length; j++)
                if(names[i].Equals(input[j].name))
                {
                    output[i] = input[j];
                    break;
                }

        return output;
    }
}