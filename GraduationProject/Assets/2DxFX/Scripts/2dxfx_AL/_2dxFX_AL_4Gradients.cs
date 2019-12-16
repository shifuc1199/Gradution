//////////////////////////////////////////////
/// 2DxFX - 2D SPRITE FX - by VETASOFT 2016 //
//////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu("2DxFX/Advanced Lightning/4 Gradients")]
[System.Serializable]
public class _2dxFX_AL_4Gradients : MonoBehaviour
{
[HideInInspector] public Material ForceMaterial;
// Advanced Lightning
[HideInInspector] public bool ActiveChange = true;
[HideInInspector] public bool AddShadow = true;
[HideInInspector] public bool ReceivedShadow = false;
[HideInInspector] public int BlendMode = 0;


    private string shader = "2DxFX/AL/4Gradients";
    [HideInInspector]
    public Color _Color1 = new Color(1f, 0f, 0f, 1f);
    [HideInInspector]
    public Color _Color2 = new Color(1f, 1f, 0f, 1f);
    [HideInInspector]
    public Color _Color3 = new Color(0f, 1f, 1f, 1f);
    [HideInInspector]
    public Color _Color4 = new Color(0f, 1f, 0f, 1f);
    [Range(0, 1)]
    [HideInInspector]
    public float _Alpha = 1f;

    [HideInInspector]
    public int ShaderChange = 0;
    Material tempMaterial;
    Material defaultMaterial;
    Image CanvasImage;

    void Awake()
    {
        if (this.gameObject.GetComponent<Image>() != null)
        {
            CanvasImage = this.gameObject.GetComponent<Image>();
        }

    }
    void Start()
    {
        ShaderChange = 0;
    }

    public void CallUpdate()
    {
        Update();
    }


void Update()
{


        if (this.gameObject.GetComponent<Image>() != null)
        {
            if (CanvasImage == null) CanvasImage = this.gameObject.GetComponent<Image>();
        }

        if ((ShaderChange == 0) && (ForceMaterial != null))
        {
            ShaderChange = 1;
            if (tempMaterial != null) DestroyImmediate(tempMaterial);

            if (this.gameObject.GetComponent<SpriteRenderer>() != null)
            {
                this.GetComponent<Renderer>().sharedMaterial = ForceMaterial;
            }
            else if (this.gameObject.GetComponent<Image>() != null)
            {
                CanvasImage.material = ForceMaterial;
            }

            ForceMaterial.hideFlags = HideFlags.None;
            ForceMaterial.shader = Shader.Find(shader);

        }

        if ((ForceMaterial == null) && (ShaderChange == 1))
        {
            if (tempMaterial != null) DestroyImmediate(tempMaterial);
            tempMaterial = new Material(Shader.Find(shader));
            tempMaterial.hideFlags = HideFlags.None;

            if (this.gameObject.GetComponent<SpriteRenderer>() != null)
            {
                this.GetComponent<Renderer>().sharedMaterial = tempMaterial;
            }

            else if (this.gameObject.GetComponent<Image>() != null)
            {
                CanvasImage.material = tempMaterial;
            }
            ShaderChange = 0;
        }

#if UNITY_EDITOR
        if (this.gameObject.GetComponent<SpriteRenderer>() != null)
        {
            if (this.GetComponent<Renderer>().sharedMaterial.shader.name == "Sprites/Default")
            {
                ForceMaterial.shader = Shader.Find(shader);
                ForceMaterial.hideFlags = HideFlags.None;
                this.GetComponent<Renderer>().sharedMaterial = ForceMaterial;
            }
        }
        else if (this.gameObject.GetComponent<Image>() != null)
        {
            Image img = this.gameObject.GetComponent<Image>();
            if (img.material == null)
            {
                ForceMaterial.shader = Shader.Find(shader);
                ForceMaterial.hideFlags = HideFlags.None;
                CanvasImage.material = ForceMaterial;
            }
        }
#endif
        if (ActiveChange)
        {
            if (this.gameObject.GetComponent<SpriteRenderer>() != null)
            {

                this.GetComponent<Renderer>().sharedMaterial.SetFloat("_Alpha", 1 - _Alpha);

                 if (_2DxFX.ActiveShadow && AddShadow)
                {
                    this.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    if (ReceivedShadow)
                    {
                        this.GetComponent<Renderer>().receiveShadows = true;
                        this.GetComponent<Renderer>().sharedMaterial.renderQueue = 2450;
                        this.GetComponent<Renderer>().sharedMaterial.SetInt("_Z", 1);
                    }
                    else
                    {
                        this.GetComponent<Renderer>().receiveShadows = false;
                        this.GetComponent<Renderer>().sharedMaterial.renderQueue = 3000;
                        this.GetComponent<Renderer>().sharedMaterial.SetInt("_Z", 0);
                    }
                }
                else
                {
                    this.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    this.GetComponent<Renderer>().receiveShadows = false;
                    this.GetComponent<Renderer>().sharedMaterial.renderQueue = 3000;
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_Z", 0);
                }

                if (BlendMode == 0) // Normal
                {
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                }
                if (BlendMode == 1) // Additive
                {
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                }
                if (BlendMode == 2) // Darken
                {
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.ReverseSubtract);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                }
                if (BlendMode == 3) // Lighten
                {
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Max);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                }
                if (BlendMode == 4) // Linear Burn
                {
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.ReverseSubtract);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                }
                if (BlendMode == 5) // Linear Dodge
                {
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Max);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                }
                if (BlendMode == 6) // Multiply
                {
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                }
                if (BlendMode == 7) // Soft Aditive
                {
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.Add);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                }
                if (BlendMode == 8) // 2x Multiplicative
                {
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_BlendOp", (int)UnityEngine.Rendering.BlendOp.ReverseSubtract);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstAlpha);
                    this.GetComponent<Renderer>().sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                }


                this.GetComponent<Renderer>().sharedMaterial.SetColor("_Color1", _Color1);
                this.GetComponent<Renderer>().sharedMaterial.SetColor("_Color2", _Color2);
                this.GetComponent<Renderer>().sharedMaterial.SetColor("_Color3", _Color3);
                this.GetComponent<Renderer>().sharedMaterial.SetColor("_Color4", _Color4);
            }
            else if (this.gameObject.GetComponent<Image>() != null)
            {
                CanvasImage.material.SetFloat("_Alpha", 1 - _Alpha);
                CanvasImage.material.SetColor("_Color1", _Color1);
                CanvasImage.material.SetColor("_Color2", _Color2);
                CanvasImage.material.SetColor("_Color3", _Color3);
                CanvasImage.material.SetColor("_Color4", _Color4);
            }
        }

    }

    void OnDestroy()
    {
        if ((Application.isPlaying == false) && (Application.isEditor == true))
        {

            if (tempMaterial != null) DestroyImmediate(tempMaterial);

            if (gameObject.activeSelf && defaultMaterial != null)
            {
                if (this.gameObject.GetComponent<SpriteRenderer>() != null)
                {
                    this.GetComponent<Renderer>().sharedMaterial = defaultMaterial;
                    this.GetComponent<Renderer>().sharedMaterial.hideFlags = HideFlags.None;
                }
                else if (this.gameObject.GetComponent<Image>() != null)
                {
                    CanvasImage.material = defaultMaterial;
                    CanvasImage.material.hideFlags = HideFlags.None;
                }
            }
        }
    }
    void OnDisable()
    {
        if (this.gameObject.GetComponent<Image>() != null)
        {
            if (CanvasImage == null) CanvasImage = this.gameObject.GetComponent<Image>();
        }

        if (gameObject.activeSelf && defaultMaterial != null)
        {
            if (this.gameObject.GetComponent<SpriteRenderer>() != null)
            {
                this.GetComponent<Renderer>().sharedMaterial = defaultMaterial;
                this.GetComponent<Renderer>().sharedMaterial.hideFlags = HideFlags.None;
            }
            else if (this.gameObject.GetComponent<Image>() != null)
            {
                CanvasImage.material = defaultMaterial;
                CanvasImage.material.hideFlags = HideFlags.None;
            }
        }
    }

    void OnEnable()
    {
        if (this.gameObject.GetComponent<Image>() != null)
        {
            if (CanvasImage == null) CanvasImage = this.gameObject.GetComponent<Image>();
        }


        if (defaultMaterial == null)
        {
            defaultMaterial = new Material(Shader.Find("Sprites/Default"));
        }

        if (ForceMaterial == null)
        {
            ActiveChange = true;
            tempMaterial = new Material(Shader.Find(shader));
            tempMaterial.hideFlags = HideFlags.None;

            if (this.gameObject.GetComponent<SpriteRenderer>() != null)
            {
                this.GetComponent<Renderer>().sharedMaterial = tempMaterial;
            }
            else if (this.gameObject.GetComponent<Image>() != null)
            {
                CanvasImage.material = tempMaterial;
            }
        }
        else
        {
            ForceMaterial.shader = Shader.Find(shader);
            ForceMaterial.hideFlags = HideFlags.None;
            if (this.gameObject.GetComponent<SpriteRenderer>() != null)
            {
                this.GetComponent<Renderer>().sharedMaterial = ForceMaterial;
            }
            else if (this.gameObject.GetComponent<Image>() != null)
            {
                CanvasImage.material = ForceMaterial;
            }
        }

    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(_2dxFX_AL_4Gradients)), CanEditMultipleObjects]
public class _2dxFX_AL_4Gradients_Editor : Editor
{
    private SerializedObject m_object;

    public void OnEnable()
    {
        m_object = new SerializedObject(targets);
    }

    public override void OnInspectorGUI()
    {
        m_object.Update();

        DrawDefaultInspector();

        _2dxFX_AL_4Gradients _2dxScript = (_2dxFX_AL_4Gradients)target;

        Texture2D icon = Resources.Load("2dxfxinspector-al") as Texture2D;
        if (icon)
        {
            Rect r;
            float ih = icon.height;
            float iw = icon.width;
            float result = ih / iw;
            float w = Screen.width;
            result = result * w;
            r = GUILayoutUtility.GetRect(ih, result);
            EditorGUI.DrawTextureTransparent(r, icon);
        }
        EditorGUILayout.LabelField("Advanced Lightning may work on mobile high-end devices and may be slower than the Standard 2DxFX effects due to is lightning system. Use it only if you need it.", EditorStyles.helpBox);
        EditorGUILayout.PropertyField(m_object.FindProperty("AddShadow"), new GUIContent("Add Shadow", "Use a unique material, reduce drastically the use of draw call"));
        if (_2dxScript.AddShadow)
        {
            EditorGUILayout.PropertyField(m_object.FindProperty("ReceivedShadow"), new GUIContent("Received Shadow : No Transparency and Use Z Buffering instead of Sprite Order Layers", "Received Shadow, No Transparency and Use Z Buffering instead of Sprite Order Layers"));
            if (_2dxScript.ReceivedShadow)
            {
                EditorGUILayout.LabelField("Note 1: Blend Fusion work but without transparency\n", EditorStyles.helpBox);
            }
        }

        // Mode Blend
         string BlendMethode = "Normal";

        if (_2dxScript.BlendMode == 0) BlendMethode = "Normal";
        if (_2dxScript.BlendMode == 1) BlendMethode = "Additive";
        if (_2dxScript.BlendMode == 2) BlendMethode = "Darken";
        if (_2dxScript.BlendMode == 3) BlendMethode = "Lighten";
        if (_2dxScript.BlendMode == 4) BlendMethode = "Linear Burn";
        if (_2dxScript.BlendMode == 5) BlendMethode = "Linear Dodge";
        if (_2dxScript.BlendMode == 6) BlendMethode = "Multiply";
        if (_2dxScript.BlendMode == 7) BlendMethode = "Soft Aditive";
        if (_2dxScript.BlendMode == 8) BlendMethode = "2x Multiplicative";



        EditorGUILayout.PropertyField(m_object.FindProperty("ForceMaterial"), new GUIContent("Shared Material", "Use a unique material, reduce drastically the use of draw call"));
        if (_2dxScript.ForceMaterial == null)
        {
            _2dxScript.ActiveChange = true;
        }
        else
        {
            if (GUILayout.Button("Remove Shared Material"))
            {
                _2dxScript.ForceMaterial = null;
                _2dxScript.ShaderChange = 1;
                _2dxScript._Color1 = new Color(1, 0, 0, 1);
                _2dxScript._Color2 = new Color(0, 0, 1, 1);
                _2dxScript._Color3 = new Color(0, 1, 0, 1);
                _2dxScript._Color4 = new Color(0, 1, 1, 1);
                _2dxScript.ActiveChange = true;
                _2dxScript.CallUpdate();
            }
            EditorGUILayout.PropertyField(m_object.FindProperty("ActiveChange"), new GUIContent("Change Material Property", "Change The Material Property"));
        }

        if (_2dxScript.ActiveChange)
        {
            EditorGUILayout.BeginVertical("Box");
            Texture2D icone = Resources.Load("2dxfx-icon-corner-1") as Texture2D;

            EditorGUILayout.PropertyField(m_object.FindProperty("_Color1"), new GUIContent("Upper Left Color", icone, "Select the color from upper left"));
            icone = Resources.Load("2dxfx-icon-corner-2") as Texture2D;
            EditorGUILayout.PropertyField(m_object.FindProperty("_Color2"), new GUIContent("Upper Right Color", icone, "Select the color from upper right"));
            icone = Resources.Load("2dxfx-icon-corner-3") as Texture2D;
            EditorGUILayout.PropertyField(m_object.FindProperty("_Color3"), new GUIContent("Bottom Left Color", icone, "Select the color from Bottom left"));
            icone = Resources.Load("2dxfx-icon-corner-4") as Texture2D;
            EditorGUILayout.PropertyField(m_object.FindProperty("_Color4"), new GUIContent("Bottom Right Color", icone, "Select the color from Bottom right"));
            EditorGUILayout.BeginVertical("Box");
            icone = Resources.Load("2dxfx-icon-fade") as Texture2D;
            EditorGUILayout.PropertyField(m_object.FindProperty("_Alpha"), new GUIContent("Fading", icone, "Fade from nothing to showing"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();



            // PRESET FX
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField(new GUIContent("PRESET FX", "PRESET FX"));
            EditorGUILayout.BeginHorizontal("Box");
            Texture2D preview = Resources.Load("2dxfx-p-4g1") as Texture2D;
            if (GUILayout.Button(preview))
            {
                _2dxScript._Color1 = new Color(1, 0, 0, 1);
                _2dxScript._Color2 = new Color(0, 0, 1, 1);
                _2dxScript._Color3 = new Color(0, 1, 0, 1);
                _2dxScript._Color4 = new Color(0, 1, 1, 1);
                _2dxScript._Alpha = 1;
                m_object.ApplyModifiedProperties();
                _2dxScript.CallUpdate();
            }

            preview = Resources.Load("2dxfx-p-4g2") as Texture2D;
            if (GUILayout.Button(preview))
            {
                _2dxScript._Color1 = new Color(0, 1, 0.7f, 0);
                _2dxScript._Color2 = new Color(0, 1, 0.7f, 0);
                _2dxScript._Color3 = new Color(0, 1, 0.7f, 1);
                _2dxScript._Color4 = new Color(0, 1, 0.7f, 1);
                _2dxScript._Alpha = 1;
                m_object.ApplyModifiedProperties();
                _2dxScript.CallUpdate();
            }

            preview = Resources.Load("2dxfx-p-4g3") as Texture2D;
            if (GUILayout.Button(preview))
            {
                _2dxScript._Color1 = new Color(1, 1, 0, 1);
                _2dxScript._Color2 = new Color(1, 0.8f, 0, 0);
                _2dxScript._Color3 = new Color(1, 0.6f, 0, 0);
                _2dxScript._Color4 = new Color(1, 0.6f, 0, 0);
                _2dxScript._Alpha = 1;
                m_object.ApplyModifiedProperties();
                _2dxScript.CallUpdate();
            }

            preview = Resources.Load("2dxfx-p-4g4") as Texture2D;
            if (GUILayout.Button(preview))
            {
                _2dxScript._Color1 = new Color(1, 0, 0, 1);
                _2dxScript._Color2 = new Color(1, 0, 0, 1);
                _2dxScript._Color3 = new Color(1, 0, 0, 0);
                _2dxScript._Color4 = new Color(1, 0, 0, 0);
                _2dxScript._Alpha = 1;
                m_object.ApplyModifiedProperties();
                _2dxScript.CallUpdate();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Change Blend Fusion = " + BlendMethode, EditorStyles.whiteLargeLabel);
            if (_2dxScript.ReceivedShadow)
            {
                EditorGUILayout.LabelField("Note: Blend Fusion is not working correctly with Received Shadow", EditorStyles.helpBox);
            }

            EditorGUILayout.BeginHorizontal("Box");

            if (GUILayout.Button("Normal", EditorStyles.toolbarButton))
            {
                _2dxScript.BlendMode = 0;
                _2dxScript.CallUpdate();
            }
            if (GUILayout.Button("Additive", EditorStyles.toolbarButton))
            {
                _2dxScript.BlendMode = 1;
                _2dxScript.CallUpdate();
            }
            if (GUILayout.Button("Darken", EditorStyles.toolbarButton))
            {
                _2dxScript.BlendMode = 2;
                _2dxScript.CallUpdate();
            }
            if (GUILayout.Button("Lighten", EditorStyles.toolbarButton))
            {
                _2dxScript.BlendMode = 3;
                _2dxScript.CallUpdate();
            }
            if (GUILayout.Button("Linear Burn", EditorStyles.toolbarButton))
            {
                _2dxScript.BlendMode = 4;
                _2dxScript.CallUpdate();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal("Box");

            if (GUILayout.Button("Linear Dodge", EditorStyles.toolbarButton))
            {
                _2dxScript.BlendMode = 5;
                _2dxScript.CallUpdate();
            }
            if (GUILayout.Button("Multiply", EditorStyles.toolbarButton))
            {
                _2dxScript.BlendMode = 6;
                _2dxScript.CallUpdate();
            }
            if (GUILayout.Button("Soft Aditive", EditorStyles.toolbarButton))
            {
                _2dxScript.BlendMode = 7;
                _2dxScript.CallUpdate();
            }
            if (GUILayout.Button("2x Multiplicative", EditorStyles.toolbarButton))
            {
                _2dxScript.BlendMode = 8;
                _2dxScript.CallUpdate();

            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        m_object.ApplyModifiedProperties();

    }
}
#endif