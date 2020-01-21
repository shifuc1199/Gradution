using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;

public class GUIHelper
{
    public const float ScrollBarWidth = 25f;//since these can be annoying when calculating placment

    public enum LayoutStyle
    {
        Defualt = 0, Box, Button, TextArea, Window, Textfield,
        HorizontalScrollbar,//Fixed height
        Label,//No Style
        Toggle, //Just puts a non usable CB to the left 
        Toolbar,//Fixed height
        PreToolbar2,
        scrollView
    }

    public static bool DrawButtonWithImage(Sprite sprite, Vector2 imageSize, Vector2 buttonSize, GUIContent buttonContent,
        bool drawBox = true)
    {
        bool returnVal = false;

        using(new Horizontal())
        {
            if(GUILayout.Button(buttonContent, GUILayout.Width(buttonSize.x), GUILayout.Height(buttonSize.y)))
                returnVal = true;

            GUI.color = Color.white;

            DrawSprite(sprite, imageSize.x, imageSize.y, new Vector2(buttonSize.x - (buttonSize.x - imageSize.x) / 2 + 4, (imageSize.y - buttonSize.y) / 2 - 4), drawBox);
            GUILayout.Space(-Mathf.Max(imageSize.x, buttonSize.x));
        }

        return returnVal;
    }

    public static void DrawSprite(Sprite sprite, float width, float height, bool drawBox = true)
    {
        DrawTexture(sprite.texture, GetSpriteTextureRect(sprite.texture, sprite.textureRect), width, height, Vector2.zero, drawBox);
    }

    public static void DrawSprite(Sprite sprite, float width, float height, Vector2 offset, bool drawBox = true)
    {

        DrawTexture(sprite.texture, GetSpriteTextureRect(sprite.texture, sprite.textureRect), width, height, offset, drawBox);
    }

    /// <summary>
    /// Converts a sprites given texture rect to more relatable co-ords
    /// </summary>
    static Rect GetSpriteTextureRect(Texture2D texture, Rect textureRect)
    {
        return new Rect(textureRect.x / texture.width, textureRect.y / texture.height, textureRect.width / texture.width, textureRect.height / texture.height);
    }

    /// <summary>
    /// Draws a sprite in line with the current position of the editor layout
    /// </summary>
    public static Rect DrawTexture(Texture2D texture, Rect textureRect, float width, float height, Vector2 offset, bool drawBox = true)
    {
        Rect position = GUILayoutUtility.GetRect(width, height);
        position.x -= offset.x;
        position.y -= offset.y;

        if(width > 0)//if there is no width set then just stretch the entire space
            position.width = width;//overrides a weird bug where the image is stretched to fill the space

        if(drawBox)
            GUI.Box(position, "");

        if(texture != null)
        {
            position.x += 2;
            position.y += 2;
            position.width -= 4;
            position.height -= 4;
            GUI.DrawTextureWithTexCoords(position, texture, textureRect);
        }

        return position;
    }

    public static void ArrayGUI(SerializedObject instance, string name)
    {
        ArrayGUI(instance, instance.FindProperty(name));
    }

    public static void ArrayGUI(SerializedObject instance, SerializedProperty array)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(array, true);

        if(EditorGUI.EndChangeCheck())
            instance.ApplyModifiedProperties();

        EditorGUIUtility.LookLikeControls();
    }

    public static void DrawToggle(ref AnimBool animation, GUIContent content)
    {
        if(animation == null)
            animation = new AnimBool();

        GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.85f);//change the colour of the heading to get it to stand out

        if(!GUILayout.Toggle(true, content, "PreToolbar2", GUILayout.MinWidth(20f)))
            animation.target = !animation.target;//invert

        GUI.contentColor = Color.white;
    }

    public static void DrawCenteredToggle(ref AnimBool animation, GUIContent content)
    {
        using(new Horizontal())
        using(new Centered())
            DrawToggle(ref animation, content);
    }
}

/// <summary>
/// A helper to make colour changes easier
/// </summary>
public class ColourChange : IDisposable
{
    public ColourChange(Color colour)
    {
        GUI.color = colour;
    }

    public void Dispose()
    {
        GUI.color = Color.white;
    }
}

/// <summary>
/// A helper to make horizontal groups easier
/// </summary>
public class Horizontal : IDisposable
{
    public Horizontal()
    {
        EditorGUILayout.BeginHorizontal();
    }

    public Horizontal(params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal(options);
    }

    public Horizontal(GUIHelper.LayoutStyle style)
    {
        EditorGUILayout.BeginHorizontal(style.ToString());
    }

    public void Dispose()
    {
        EditorGUILayout.EndHorizontal();
    }
}

/// <summary>
/// A helper to make vertical groups easier
/// </summary>
public class Vertical : IDisposable
{
    public Vertical()
    {
        EditorGUILayout.BeginVertical();
    }

    public Vertical(params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginVertical(options);
    }

    public Vertical(GUIHelper.LayoutStyle style)
    {
        EditorGUILayout.BeginVertical(style.ToString());
    }

    public Vertical(GUIHelper.LayoutStyle style, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginVertical(style.ToString(), options);
    }

    public void Dispose()
    {
        EditorGUILayout.EndVertical();
    }
}

public class Centered : IDisposable
{
    public Centered()
    {
        GUILayout.FlexibleSpace();
    }

    public void Dispose()
    {
        GUILayout.FlexibleSpace();
    }
}


//FixedWidthLabel class. Extends IDisposable, so that it can be used with the "using" keyword.
public class FixedWidthLabel : IDisposable
{
    private readonly ZeroIndent indentReset; //helper class to reset and restore indentation

    public FixedWidthLabel(GUIContent label, GUIStyle style, params GUILayoutOption[] options)//	constructor.
    {//						state changes are applied here.
        EditorGUILayout.BeginHorizontal(options);// create a new horizontal group

        EditorGUILayout.LabelField(label, style,
            GUILayout.Width(GUI.skin.label.CalcSize(label).x +// actual label width
                9 * EditorGUI.indentLevel));//indentation from the left side. It's 9 pixels per indent level

        indentReset = new ZeroIndent();//helper class to have no indentation after the label
    }

    public FixedWidthLabel(GUIContent label, params GUILayoutOption[] options)//	constructor.
    {//						state changes are applied here.
        EditorGUILayout.BeginHorizontal(options);// create a new horizontal group

        EditorGUILayout.LabelField(label,
            GUILayout.Width(GUI.skin.label.CalcSize(label).x +// actual label width
                9 * EditorGUI.indentLevel));//indentation from the left side. It's 9 pixels per indent level

        indentReset = new ZeroIndent();//helper class to have no indentation after the label
    }

    public FixedWidthLabel(string label)
        : this(new GUIContent(label))//alternative constructor, if we don't want to deal with GUIContents
    {
    }

    public void Dispose() //restore GUI state
    {
        indentReset.Dispose();//restore indentation
        EditorGUILayout.EndHorizontal();//finish horizontal group
    }
}

public class ZeroIndent : IDisposable //helper class to clear indentation
{
    private readonly int originalIndent;//the original indentation value before we change the GUI state
    public ZeroIndent()
    {
        originalIndent = EditorGUI.indentLevel;//save original indentation
        EditorGUI.indentLevel = 0;//clear indentation
    }

    public void Dispose()
    {
        EditorGUI.indentLevel = originalIndent;//restore original indentation
    }
}