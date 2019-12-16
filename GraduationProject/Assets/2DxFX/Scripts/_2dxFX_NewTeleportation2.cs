//////////////////////////////////////////////
/// 2DxFX - 2D SPRITE FX - by VETASOFT 2017 //
//////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu ("2DxFX/Standard/NewTeleportation2")]
[System.Serializable]
public class _2dxFX_NewTeleportation2 : MonoBehaviour
{
	[HideInInspector] public Material ForceMaterial;
	[HideInInspector] public bool ActiveChange=true;
	private string shader = "2DxFX/Standard/NewTeleportation2";
	[HideInInspector] [Range(0, 1)] public float _Alpha = 1f;

	[HideInInspector] public Texture2D __MainTex2;
    [HideInInspector] [Range(0, 1)] public float _Fade = 0.5f;
    [HideInInspector] [Range(1, 16)] public float _HDR_Intensity = 1f;

    [HideInInspector] public Color TeleportationColor = new Color(0,0.5f,1);
	[HideInInspector] [Range(0.1f, 2)] public float _Distortion=1;
 

    [HideInInspector] public float _Value4;

	[HideInInspector] public bool _AutoScrollX;
	[HideInInspector] [Range(0, 10)] public float _AutoScrollSpeedX;
	[HideInInspector] public bool _AutoScrollY;
	[HideInInspector] [Range(0, 10)] public float _AutoScrollSpeedY;
	[HideInInspector]  private float _AutoScrollCountX;
	[HideInInspector]  private float _AutoScrollCountY;

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
		__MainTex2 = Resources.Load ("_2dxFX_NewTeleportation2TXT") as Texture2D;
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
			__MainTex2 = Resources.Load ("_2dxFX_NewTeleportation2TXT") as Texture2D;
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
				this.GetComponent<Renderer>().sharedMaterial = ForceMaterial;
				this.GetComponent<Renderer>().sharedMaterial.SetTexture ("_MainTex2", __MainTex2);
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{
			Image img = this.gameObject.GetComponent<Image>();
				if (img.material==null)
				{
				CanvasImage.material = ForceMaterial;
				CanvasImage.material.SetTexture ("_MainTex2", __MainTex2);
				}
			}
		
		}
		#endif
		if (ActiveChange)
		{
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
			this.GetComponent<Renderer>().sharedMaterial.SetFloat("_Alpha", 1-_Alpha);
			this.GetComponent<Renderer>().sharedMaterial.SetColor("TeleportationColor", TeleportationColor);
			this.GetComponent<Renderer>().sharedMaterial.SetFloat("_Distortion", _Distortion);
			this.GetComponent<Renderer>().sharedMaterial.SetFloat("_Fade", _Fade);
            this.GetComponent<Renderer>().sharedMaterial.SetFloat("_HDR_Intensity", _HDR_Intensity);
                

            }
			else if(this.gameObject.GetComponent<Image>() != null)
			{
				CanvasImage.material.SetFloat("_Alpha", 1-_Alpha);
				CanvasImage.material.SetColor("TeleportationColor", TeleportationColor);
				CanvasImage.material.SetFloat("_Distortion", _Distortion);
				CanvasImage.material.SetFloat("_Fade", _Fade);
                CanvasImage.material.SetFloat("_HDR_Intensity", _HDR_Intensity);
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
			__MainTex2 = Resources.Load ("_2dxFX_NewTeleportation2TXT") as Texture2D;
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
			__MainTex2 = Resources.Load ("_2dxFX_NewTeleportation2TXT") as Texture2D;
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
[CustomEditor(typeof(_2dxFX_NewTeleportation2)),CanEditMultipleObjects]
public class _2dxFX_NewTeleportation2_Editor : Editor
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

        _2dxFX_NewTeleportation2 _2dxScript = (_2dxFX_NewTeleportation2)target;
	
		Texture2D icon = Resources.Load ("2dxfxinspector-hdr") as Texture2D;
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



            Texture2D icone = Resources.Load("2dxfx-icon-fade") as Texture2D;
            EditorGUILayout.PropertyField(m_object.FindProperty("_Fade"), new GUIContent("Fade of the FX", icone, "Change the Fade of the FX"));
            icone = Resources.Load ("2dxfx-icon-color") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("TeleportationColor"), new GUIContent("Teleportation Color", icone, "Color"));

			icone = Resources.Load ("2dxfx-icon-fade") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_Distortion"), new GUIContent("Distortion Value", icone, ""));

            icone = Resources.Load("2dxfx-icon-fade") as Texture2D;
            EditorGUILayout.PropertyField(m_object.FindProperty("_HDR_Intensity"), new GUIContent("HDR Value", icone, ""));

       





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