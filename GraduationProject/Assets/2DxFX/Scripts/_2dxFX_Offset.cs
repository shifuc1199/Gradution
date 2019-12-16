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
[AddComponentMenu ("2DxFX/Standard/Offset")]
[System.Serializable]
public class _2dxFX_Offset : MonoBehaviour
{
	[HideInInspector] public Material ForceMaterial;
	[HideInInspector] public bool ActiveChange=true;
	private string shader = "2DxFX/Standard/Offset";
	[HideInInspector] [Range(0, 1)] public float _Alpha = 1f;

	[HideInInspector] [Range(-1f, 1f)] public float _OffsetX = 0f;
	[HideInInspector] [Range(-1f, 1f)] public float _OffsetY = 0f;
	[HideInInspector] [Range(0.001f, 8f)] public float _ZoomX = 1f;
	[HideInInspector] [Range(0.001f, 8f)] public float _ZoomY = 1f;
    [HideInInspector] [Range(0.001f, 64f)] public float _ZoomXY = 1f;

  	[HideInInspector] public bool _AutoScrollX;
	[HideInInspector] [Range(-100,100)]public float _AutoScrollSpeedX;
	[HideInInspector] public bool _AutoScrollY;
	[HideInInspector] [Range(-100,100)] public float _AutoScrollSpeedY;
	[HideInInspector] private float _AutoScrollCountX;
	[HideInInspector] private float _AutoScrollCountY;

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
		ShaderChange = 0;
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
		}
		#endif
		if (ActiveChange)
		{
			if(this.gameObject.GetComponent<SpriteRenderer>() != null)
			{
            this.GetComponent<Renderer>().sharedMaterial.SetFloat("_Alpha", 1-_Alpha);
			
            if (_AutoScrollX == true)
			{
                _AutoScrollCountX+=_AutoScrollSpeedX*0.01f*Time.deltaTime;
                if (_AutoScrollCountX<0) _AutoScrollCountX = 1;
                this.GetComponent<Renderer>().sharedMaterial.SetFloat("_OffsetX", 1+_AutoScrollCountX);
               
			} else
            {
                this.GetComponent<Renderer>().sharedMaterial.SetFloat("_OffsetX", 1+_OffsetX);
			}
			 if (_AutoScrollY == true)
			{
                _AutoScrollCountY+=_AutoScrollSpeedY*0.01f*Time.deltaTime;
                if (_AutoScrollCountY<0) _AutoScrollCountY = 1;
                this.GetComponent<Renderer>().sharedMaterial.SetFloat("_OffsetY", 1+_AutoScrollCountY);
			} else
            {
                this.GetComponent<Renderer>().sharedMaterial.SetFloat("_OffsetY", 1+_OffsetY);
			}
			
			this.GetComponent<Renderer>().sharedMaterial.SetFloat("_ZoomX", _ZoomX*_ZoomXY);
			this.GetComponent<Renderer>().sharedMaterial.SetFloat("_ZoomY", _ZoomY*_ZoomXY);
			}
			else if(this.gameObject.GetComponent<Image>() != null)
			{

            CanvasImage.material.SetFloat("_Alpha", 1-_Alpha);
			if (_AutoScrollX == true)
			{
                _AutoScrollCountX+=_AutoScrollSpeedX*0.01f*Time.deltaTime;
                if (_AutoScrollCountX<0) _AutoScrollCountX = 1;
               CanvasImage.material.SetFloat("_OffsetX", 1+_AutoScrollCountX);
			} else
            {
                CanvasImage.material.SetFloat("_OffsetX", 1+_OffsetX);
			}
			 if (_AutoScrollY == true)
			{
                _AutoScrollCountY+=_AutoScrollSpeedY*0.01f*Time.deltaTime;
                if (_AutoScrollCountY<0) _AutoScrollCountY = 1;
                CanvasImage.material.SetFloat("_OffsetY", 1+_AutoScrollCountY);
			} else
            {
                CanvasImage.material.SetFloat("_OffsetY", 1+_OffsetY);
			}
			CanvasImage.material.SetFloat("_ZoomX", _ZoomX*_ZoomXY);
			CanvasImage.material.SetFloat("_ZoomY", _ZoomY*_ZoomXY);
		
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
		}
		
	}
}




#if UNITY_EDITOR
[CustomEditor(typeof(_2dxFX_Offset)),CanEditMultipleObjects]
public class _2dxFX_Offset_Editor : Editor
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
		
		_2dxFX_Offset _2dxScript = (_2dxFX_Offset)target;
	
		Texture2D icon = Resources.Load ("2dxfxinspector") as Texture2D;
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


			Texture2D icone = Resources.Load ("2dxfx-icon-clip_left") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_OffsetX"), new GUIContent("Offset X", icone, "Change the offset value of X"));

			icone = Resources.Load ("2dxfx-icon-clip_right") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_OffsetY"), new GUIContent("Offset Y", icone, "Change the offset value of Y"));

			icone = Resources.Load ("2dxfx-icon-clip_up") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_ZoomX"), new GUIContent("Zoom X", icone, "Change the Zoom value of X"));

			icone = Resources.Load ("2dxfx-icon-clip_down") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_ZoomY"), new GUIContent("Zoom Y", icone, "Change the Zoom value of Y"));

        	icone = Resources.Load ("2dxfx-icon-clip_down") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_ZoomXY"), new GUIContent("Zoom XY", icone, "Change the Zoom value of X and Y. Note if you want to change only the X or the Y, you must set Zoom XY to 1"));

            icone = Resources.Load ("2dxfx-icon-value") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_AutoScrollX"), new GUIContent("Auto Scroll X", icone, "Change the value of the posterize effect"));
			if (_2dxScript._AutoScrollX)
			{
				icone = Resources.Load ("2dxfx-icon-time") as Texture2D;
				EditorGUILayout.PropertyField(m_object.FindProperty("_AutoScrollSpeedX"), new GUIContent("Auto Scroll Speed X", icone, "Change the value of the posterize effect"));
			}

			icone = Resources.Load ("2dxfx-icon-value") as Texture2D;
			EditorGUILayout.PropertyField(m_object.FindProperty("_AutoScrollY"), new GUIContent("Auto Scroll Y", icone, "Change the value of the posterize effect"));

			if (_2dxScript._AutoScrollY)
			{
				icone = Resources.Load ("2dxfx-icon-time") as Texture2D;
				EditorGUILayout.PropertyField(m_object.FindProperty("_AutoScrollSpeedY"), new GUIContent("Auto Scroll Speed Y", icone, "Change the value of the posterize effect"));
			}


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