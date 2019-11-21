using UnityEngine;



public class Slash_Light : MonoBehaviour
{
    public AnimationCurve LightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Gradient LightColor = new Gradient();
    public float GraphTimeMultiplier = 1, GraphIntensityMultiplier = 1;
    public bool IsLoop;

    [HideInInspector] public bool canUpdate;
    private float startTime;
    Color startColor;
    private Light lightSource;


    public void SetStartColor(Color color)
    {
        startColor = color;
    }

    private void Awake()
    {
        lightSource = GetComponent<Light>();
        startColor = lightSource.color;

        lightSource.intensity = LightCurve.Evaluate(0) * GraphIntensityMultiplier;
        lightSource.color = startColor * LightColor.Evaluate(0);

        startTime = Time.time;
        canUpdate = true;
    }

    private void OnEnable()
    {
        startTime = Time.time;
        canUpdate = true;
        if (lightSource != null)
        {
            lightSource.intensity = LightCurve.Evaluate(0) * GraphIntensityMultiplier;
            lightSource.color = startColor * LightColor.Evaluate(0);
        }
    }

    private void Update()
    {
        
        var time = Time.time - startTime;
        if (canUpdate) {
            var eval = LightCurve.Evaluate(time / GraphTimeMultiplier) * GraphIntensityMultiplier;
            lightSource.intensity = eval;
            lightSource.color = startColor * LightColor.Evaluate(time / GraphTimeMultiplier);
        }
        if (time >= GraphTimeMultiplier) {
            if (IsLoop) startTime = Time.time;
            else canUpdate = false;
        }
    }
}