//////////////////////////////////////////////
/// 2DxFX - 2D SPRITE FX - by VETASOFT 2015 //
//////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu ("2DxFX/Standard/Waterfall")]
[System.Serializable]
public class _2dxFX_Waterfall : MonoBehaviour
{
	[HideInInspector] public Material ForceMaterial;
	[HideInInspector] public bool ActiveChange=true;
	private string shader = "2DxFX/Standard/Waterfall";
	[HideInInspector] [Range(0, 1)] public float _Alpha = 1f;

	[HideInInspector] public Texture2D __MainTex2; 
	[HideInInspector] [Range(0.0f, 2f)] public float Liquid = 1.0f;
	[HideInInspector] [Range(-2.0f, 4f)] public float Speed = 1.0f;
	[HideInInspector] [Range(-2f, 2f)] public float EValue = -0.65f;
	[HideInInspector] [Range(-2f, 2f)] public float TValue = 0.6f;
	[HideInInspector] public Color LightColor = new Color(0.2f,0.5f,1,1);
	[HideInInspector] [Range(-1f,  1f)] public float Light =  0.5f;


	[HideInInspector] public int ShaderChange=0;
	Material tempMaterial;
	Material defaultMaterial;
	Image CanvasImage;

	
	void Awake()
	{
		if (this.gameObject.GetComponent<Image> () != null) 
		{
			CanvasImage = this.gameObject.GetComponent<Image> ();
		}
	}
	void Start ()
	{  
		__MainTex2 = Resources.Load ("_2dxFX_WaterfallTXT") as Texture2D;
		ShaderChange = 0;
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
					this.GetComponent<Renderer>().sharedMaterial.SetTexture ("_MainTex2", __MainTex2);
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
					CanvasImage.material.SetTexture ("_MainTex2", __MainTex2);
			}
	}

 	public void CallUpdate()
	{
		Update ();
	}


	void Update()
	{
        
       		if (this.gameObject.GetComponent<Image> () != null) 
		{
			if (CanvasImage==null) CanvasImage = this.gameObject.GetComponent<Image> ();
		}		
		if ((ShaderChange == 0) && (ForceMaterial != null)) 
		{
			ShaderChange=1;
			if (tempMaterial!=null) DestroyImmediate(tempMaterial);
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
				this.GetComponent<Renderer>().sharedMaterial = ForceMaterial;
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
				CanvasImage.material = ForceMaterial;
			}
			ForceMaterial.hideFlags = HideFlags.None;
			ForceMaterial.shader=Shader.Find(shader);
		

		}
		if ((ForceMaterial == null) && (ShaderChange==1))
		{
			if (tempMaterial!=null) DestroyImmediate(tempMaterial);
			tempMaterial = new Material(Shader.Find(shader));
			tempMaterial.hideFlags = HideFlags.None;
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
				this.GetComponent<Renderer>().sharedMaterial = tempMaterial;
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
				CanvasImage.material = tempMaterial;
			}
			ShaderChange=0;
		}
		
		#if UNITY_EDITOR
		string dfname = "";
		if(this.gameObject.GetComponent<SpriteRenderer>() != null) dfname=this.GetComponent<Renderer>().sharedMaterial.shader.name;
		if(this.gameObject.GetComponent<Image>() != null) 
		{
			Image img = this.gameObject.GetComponent<Image>();
			if (img.material==null)	dfname="Sprites/Default";
		}
		if (dfname == "Sprites/Default")
		{
			ForceMaterial.shader=Shader.Find(shader);
			ForceMaterial.hideFlags = HideFlags.None;
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
				this.GetComponent<Renderer>().sharedMaterial = ForceMaterial;
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
				Image img = this.gameObject.GetComponent<Image>();
				if (img.material==null)
				{
				CanvasImage.material = ForceMaterial;
				}
			}
			__MainTex2 = Resources.Load ("_2dxFX_WaterfallTXT") as Texture2D;
				if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
					this.GetComponent<Renderer>().sharedMaterial.SetTexture ("_MainTex2", __MainTex2);
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
				Image img = this.gameObject.GetComponent<Image>();
				if (img.material==null)	CanvasImage.material.SetTexture ("_MainTex2", __MainTex2);
			}
		}
		#endif
		if (ActiveChange)
		{
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
			this.GetComponent<Renderer>().sharedMaterial.SetFloat("_Alpha", _Alpha);
			this.GetComponent<Renderer>().sharedMaterial.SetFloat("_Distortion", Liquid);
			this.GetComponent<Renderer>().sharedMaterial.SetFloat("_Speed", Speed);
			this.GetComponent<Renderer>().sharedMaterial.SetFloat("EValue",EValue);
			this.GetComponent<Renderer>().sharedMaterial.SetFloat("TValue",TValue);
			this.GetComponent<Renderer>().sharedMaterial.SetFloat("Light",Light);
			this.GetComponent<Renderer>().sharedMaterial.SetColor("Lightcolor",LightColor);
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
			CanvasImage.material.SetFloat("_Alpha", _Alpha);
			CanvasImage.material.SetFloat("_Distortion", Liquid);
			CanvasImage.material.SetFloat("_Speed", Speed);
			CanvasImage.material.SetFloat("EValue",EValue);
			CanvasImage.material.SetFloat("TValue",TValue);
			CanvasImage.material.SetFloat("Light",Light);
			CanvasImage.material.SetColor("Lightcolor",LightColor);
			}
			
		}
		
	}
	
	void OnDestroy()
	{
	if (this.gameObject.GetComponent<Image> () != null) 
		{
			if (CanvasImage==null) CanvasImage = this.gameObject.GetComponent<Image> ();
		}
		if ((Application.isPlaying == false) && (Application.isEditor == true)) {
			
			if (tempMaterial!=null) DestroyImmediate(tempMaterial);
			
			if (gameObject.activeSelf && defaultMaterial!=null) {
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
				this.GetComponent<Renderer>().sharedMaterial = defaultMaterial;
				this.GetComponent<Renderer>().sharedMaterial.hideFlags = HideFlags.None;
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
				CanvasImage.material = defaultMaterial;
				CanvasImage.material.hideFlags = HideFlags.None;
			}
		}	
		}
	}
	void OnDisable()
	{ 
	if (this.gameObject.GetComponent<Image> () != null) 
		{
			if (CanvasImage==null) CanvasImage = this.gameObject.GetComponent<Image> ();
		} 
		if (gameObject.activeSelf && defaultMaterial!=null) {
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
				this.GetComponent<Renderer>().sharedMaterial = defaultMaterial;
				this.GetComponent<Renderer>().sharedMaterial.hideFlags = HideFlags.None;
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
				CanvasImage.material = defaultMaterial;
				CanvasImage.material.hideFlags = HideFlags.None;
			}
		}		
	}
	
	void OnEnable()
	{
		if (this.gameObject.GetComponent<Image> () != null) 
		{
			if (CanvasImage==null) CanvasImage = this.gameObject.GetComponent<Image> ();
		} 
		if (defaultMaterial == null) {
			defaultMaterial = new Material(Shader.Find("Sprites/Default"));
			 
			
		}
		if (ForceMaterial==null)
		{
			ActiveChange=true;
			tempMaterial = new Material(Shader.Find(shader));
			tempMaterial.hideFlags = HideFlags.None;
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
				this.GetComponent<Renderer>().sharedMaterial = tempMaterial;
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
				CanvasImage.material = tempMaterial;
			}
			__MainTex2 = Resources.Load ("_2dxFX_WaterfallTXT") as Texture2D;
		}
		else
		{
			ForceMaterial.shader=Shader.Find(shader);
			ForceMaterial.hideFlags = HideFlags.None;
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
				this.GetComponent<Renderer>().sharedMaterial = ForceMaterial;
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
				CanvasImage.material = ForceMaterial;
			}
			__MainTex2 = Resources.Load ("_2dxFX_WaterfallTXT") as Texture2D;
		}
		if (__MainTex2)	
		{
			__MainTex2.wrapMode= TextureWrapMode.Repeat;
	if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
					this.GetComponent<Renderer>().sharedMaterial.SetTexture ("_MainTex2", __MainTex2);
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
					CanvasImage.material.SetTexture ("_MainTex2", __MainTex2);
			}
		}
	}
}




#if UNITY_EDITOR
[CustomEditor(typeof(_2dxFX_Waterfall)),CanEditMultipleObjects]
public class _2dxFX_Waterfall_Editor : Editor
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
		
		_2dxFX_Waterfall _2dxScript = (_2dxFX_Waterfall)target;
	
		Texture2D icon = Resources.Load ("2dxfxinspector-anim") as Texture2D;
		if (icon)
		{
			Rect r;
			float ih=icon.height;
			float iw=icon.width;
			float result=ih/iw;
			float w=Screen.width;
			result=result*w;
			r = GUILayoutUtility.GetRect(ih, result);
			EditorGUI.DrawTextureTransparent(r,icon);
		}
		EditorGUILayout.LabelField ("*This FX Could be slow on mobile devices, it should be used for desktop or consoles.");
		EditorGUILayout.PropertyField(m_object.FindProperty("ForceMaterial"), new GUIContent("Shared Material", "Use a unique material, reduce drastically the use of draw call"));
		
		if (_2dxScript.ForceMaterial == null)
		{
			_2dxScript.ActiveChange = true;
		}
		else
		{
			if(GUILayout.Button("Remove Shared Material"))
			{
				_2dxScript.ForceMaterial= null;
				_2dxScript.ShaderChange = 1;
				_2dxScript.ActiveChange = true;
				_2dxScript.CallUpdate();
			}
		
			EditorGUILayout.PropertyField (m_object.FindProperty ("ActiveChange"), new GUIContent ("Change Material Property", "Change The Material Property"));
		}

		if (_2dxScript.ActiveChange)
		{

			EditorGUILayout.BeginVertical("Box");

			Texture2D icone = Resources.Load ("2dxfx-icon-distortion") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("Liquid"), new GUIContent("Liquid Distortion", icone, "Change the distortion of the liquid"));
			icone = Resources.Load ("2dxfx-icon-time") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("Speed"), new GUIContent("Time Speed", icone, "Change the time speed"));
			icone = Resources.Load ("2dxfx-icon-brightness") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("Light"), new GUIContent("Light Intensity", icone, "Change the intensity of the light"));
			icone = Resources.Load ("2dxfx-icon-fade") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("EValue"), new GUIContent("Effect Value", icone, "Change the effect intensity"));
			icone = Resources.Load ("2dxfx-icon-fade") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("TValue"), new GUIContent("Texture Light Value", icone, "Change the Texture Light intensity"));
			icone = Resources.Load ("2dxfx-icon-color") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("LightColor"), new GUIContent("Light Color", icone, "Change the light color"));

			EditorGUILayout.BeginVertical("Box");



			icone = Resources.Load ("2dxfx-icon-fade") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_Alpha"), new GUIContent("Fading", icone, "Fade from nothing to showing"));

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndVertical();
	

		}
		
		m_object.ApplyModifiedProperties();
		
	}
}
#endif