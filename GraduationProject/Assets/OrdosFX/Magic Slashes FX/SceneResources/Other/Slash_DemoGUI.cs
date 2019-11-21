using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters;

public class Slash_DemoGUI : MonoBehaviour
{
	public GameObject[] Prefabs;
    public float[] ReactivationTimes;
    public Light Sun;
    public ReflectionProbe ReflectionProbe;
    public Light[] NightLights = new Light[0];
    public Texture HUETexture;
    public bool isDay;
    //public RFX4_DistortionAndBloom RFX4_DistortionAndBloom;

    private int currentNomber;
	private GameObject currentInstance;
	 GUIStyle guiStyleHeader = new GUIStyle();
    GUIStyle guiStyleHeaderMobile = new GUIStyle();
    float dpiScale;
   
    private float colorHUE;
    private float startSunIntensity;
    private Quaternion startSunRotation;
    private Color startAmbientLight;
    private float startAmbientIntencity;
    private float startReflectionIntencity;
    private LightShadows startLightShadows;

	void Start () {
        if (Screen.dpi < 1) dpiScale = 1;
        if (Screen.dpi < 200) dpiScale = 1;
        else dpiScale = Screen.dpi / 200f;
        guiStyleHeader.fontSize = (int)(15f * dpiScale);
		    //guiStyleHeader.normal.textColor = new Color(0.15f,0.15f,0.15f);
        guiStyleHeaderMobile.fontSize = (int)(17f * dpiScale);

        ChangeCurrent(0);
     
        startSunIntensity = Sun.intensity;
	    startSunRotation = Sun.transform.rotation;
	    startAmbientLight = RenderSettings.ambientLight;
	    startAmbientIntencity = RenderSettings.ambientIntensity;
	    startReflectionIntencity = RenderSettings.reflectionIntensity;
	    startLightShadows = Sun.shadows;

	    ChangeDayNight();

	}

    bool isButtonPressed;

    private void OnGUI()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.DownArrow))
            isButtonPressed = false;

        if (GUI.Button(new Rect(10*dpiScale, 15*dpiScale, 135*dpiScale, 37*dpiScale), "PREVIOUS EFFECT") || (!isButtonPressed && Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            isButtonPressed = true;
            ChangeCurrent(-1);
        }
        if (GUI.Button(new Rect(160*dpiScale, 15*dpiScale, 135*dpiScale, 37*dpiScale), "NEXT EFFECT") || (!isButtonPressed && Input.GetKeyDown(KeyCode.RightArrow)))
        {
            isButtonPressed = true;
            ChangeCurrent(+1);
        }
        var offset = 0f;
      
        if (GUI.Button(new Rect(10*dpiScale, 63*dpiScale + offset, 285*dpiScale, 37*dpiScale), "Day / Night") || (!isButtonPressed && Input.GetKeyDown(KeyCode.DownArrow)))
        {
            ChangeDayNight();
        }

        GUI.Label(new Rect(350 * dpiScale, 15 * dpiScale + offset / 2, 500 * dpiScale, 20 * dpiScale),
            "press left mouse button for the camera rotating and scroll wheel for zooming", guiStyleHeader);
        GUI.Label(new Rect(350*dpiScale, 35*dpiScale + offset / 2, 160*dpiScale, 20*dpiScale),
            "prefab name is: " + Prefabs[currentNomber].name, guiStyleHeader);
        

       // GUI.DrawTexture(new Rect(12*dpiScale, 120*dpiScale + offset, 285*dpiScale, 15*dpiScale), HUETexture, ScaleMode.StretchToFill, false, 0);
       

        //        float oldColorHUE = colorHUE;
        //colorHUE = GUI.HorizontalSlider(new Rect(12*dpiScale, 140*dpiScale + offset, 285*dpiScale, 15*dpiScale), colorHUE, 0, 360);
        //if (Mathf.Abs(oldColorHUE - colorHUE) > 0.001)
        //{
        //    RFX4_ColorHelper.ChangeObjectColorByHUE(currentInstance, colorHUE / 360f);
        //    var transformMotion = currentInstance.GetComponentInChildren<RFX4_PhysicsMotion>(true);
        //    if (transformMotion != null)
        //    {
        //        transformMotion.HUE = colorHUE / 360f;
        //        //foreach (var collidedInstance in transformMotion.CollidedInstances)
        //        //{
        //        //    if(collidedInstance!=null) RFX4_ColorHelper.ChangeObjectColorByHUE(collidedInstance, colorHUE / 360f);
        //        //}
        //    }

        //    var rayCastCollision = currentInstance.GetComponentInChildren<RFX4_RaycastCollision>(true);
        //    if (rayCastCollision != null)
        //    {
        //        rayCastCollision.HUE = colorHUE / 360f;
        //        foreach (var collidedInstance in rayCastCollision.CollidedInstances)
        //        {
        //            if (collidedInstance != null) RFX4_ColorHelper.ChangeObjectColorByHUE(collidedInstance, colorHUE / 360f);
        //        }
        //    }
        //}
    }

    private void ChangeDayNight()
    {
        isButtonPressed = true;
        if (ReflectionProbe != null) ReflectionProbe.RenderProbe();
        Sun.intensity = !isDay ? 0.05f : startSunIntensity;
        Sun.shadows = isDay ? startLightShadows : LightShadows.None;
        foreach (var nightLight in NightLights)
        {
            if (nightLight != null) nightLight.shadows = !isDay ? startLightShadows : LightShadows.None;
        }

        Sun.transform.rotation = isDay ? startSunRotation : Quaternion.Euler(350, 30, 90);
        RenderSettings.ambientLight = !isDay ? new Color(0.1f, 0.1f, 0.1f) : startAmbientLight;
        RenderSettings.ambientIntensity = isDay ? startAmbientIntencity : 1;
        RenderSettings.reflectionIntensity = isDay ? startReflectionIntencity : 0.2f;
        isDay = !isDay;
    }


    void ChangeCurrent(int delta) {
		currentNomber+=delta;
		if (currentNomber> Prefabs.Length - 1)
			currentNomber = 0;
		else if (currentNomber < 0)
			currentNomber = Prefabs.Length - 1;

        if (currentInstance != null)
        {
            Destroy(currentInstance);
            RemoveClones();
        }

        currentInstance = Instantiate(Prefabs[currentNomber]);
        
       // if (!UsePCVersion)
       // {
       //     currentInstance.transform.rotation = transform.rotation;
       //     currentInstance.transform.position = transform.position;
       // }
        if (ReactivationTimes.Length == Prefabs.Length)
        {
            CancelInvoke();
            if (ReactivationTimes[currentNomber] > 0.1f) InvokeRepeating("Reactivate", ReactivationTimes[currentNomber], ReactivationTimes[currentNomber]);
        }
    }


    void RemoveClones()
    {
        var allGO = FindObjectsOfType<GameObject>();
        foreach (var go in allGO)
        {
            if(go.name.Contains("(Clone)")) Destroy(go);
        }
    }

    void Reactivate()
    {
        currentInstance.SetActive(false);
        currentInstance.SetActive(true);
    }
   
}
