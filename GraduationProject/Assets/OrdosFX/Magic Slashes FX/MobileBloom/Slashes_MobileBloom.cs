using UnityEngine;

public class Slashes_MobileBloom : MonoBehaviour
{
    [Range(0.2f, 1)]
    [Tooltip("Camera render texture resolution")]
    public float RenderTextureResolutoinFactor = 0.5f;

    [Range(0.05f, 2)]
    [Tooltip("Blend factor of the result image.")]
    public float Intensity = 0.5f;

    static float Threshold = 1.3f;

    const string shaderName = "Hidden/KriptoFX/PostEffects/Slashes_Bloom";

    private const int kMaxIterations = 16;
    private readonly RenderTexture[] m_blurBuffer1 = new RenderTexture[kMaxIterations];
    private readonly RenderTexture[] m_blurBuffer2 = new RenderTexture[kMaxIterations];

    RenderTexture Source;

    private Material _bloomMaterial;
    private Material bloomMaterial
    {
        get
        {
            if (_bloomMaterial == null)
            {
                var shader = Shader.Find(shaderName);
                if (shader == null) Debug.LogError("Can't find shader " + shaderName);
                _bloomMaterial = new Material(shader);
            }

            return _bloomMaterial;
        }
    }

    void Start()
    {

    }

    //void OnRenderImage(RenderTexture Source, RenderTexture Dest)
    //{
    //    UpdateBloom(Source, Dest);
    //}

    void OnPreRender()
    {
        Source = RenderTexture.GetTemporary(Screen.width, Screen.height, 24, SupportedHdrFormat());
        Camera.main.targetTexture = Source;
    }

    void OnPostRender()
    {
        Camera.main.targetTexture = null;
        UpdateBloom(Source, null as RenderTexture);
        RenderTexture.ReleaseTemporary(Source);
    }

    RenderTextureFormat SupportedHdrFormat()
    {
        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB111110Float))
            return RenderTextureFormat.RGB111110Float;
        else return RenderTextureFormat.DefaultHDR;
    }

  

    private void UpdateBloom(RenderTexture source, RenderTexture dest)
    {
        // source texture size
        var tw = Screen.width / 2;
        var th = Screen.height / 2;

        var rtFormat = RenderTextureFormat.Default;

        
        tw  = (int) (tw * RenderTextureResolutoinFactor);
        th = (int) (th * RenderTextureResolutoinFactor);
          
        // determine the iteration count
        var logh = Mathf.Log(th, 2) - 1;
        var logh_i = (int)logh;
        var iterations = Mathf.Clamp(logh_i, 1, kMaxIterations);

        //// update the shader properties
        var threshold = Mathf.GammaToLinearSpace(Threshold);

        bloomMaterial.SetFloat("_Threshold", threshold);
      
        var sampleScale = 0.5f + logh - logh_i;
     
        bloomMaterial.SetFloat("_SampleScale",  sampleScale * 0.5f);
        bloomMaterial.SetFloat("_Intensity", Mathf.Max(0.0f, Intensity));

        var prefiltered = RenderTexture.GetTemporary(tw, th, 0, rtFormat);
 
        Graphics.Blit(source, prefiltered, bloomMaterial, 0);

        //02457
        // construct A mip pyramid
        var last = prefiltered;
        for (var level = 0; level < iterations; level++)
        {
            m_blurBuffer1[level] = RenderTexture.GetTemporary(last.width / 2, last.height / 2, 0, rtFormat);
            Graphics.Blit(last, m_blurBuffer1[level], bloomMaterial, 1);
            last = m_blurBuffer1[level];
        }

        // upsample and combine loop
        for (var level = iterations - 2; level >= 0; level--)
        {
            var basetex = m_blurBuffer1[level];
            bloomMaterial.SetTexture("_BaseTex", basetex);
            m_blurBuffer2[level] = RenderTexture.GetTemporary(basetex.width, basetex.height, 0, rtFormat);
            Graphics.Blit(last, m_blurBuffer2[level], bloomMaterial, 2);
            last = m_blurBuffer2[level];
        }
        var finalBloom = RenderTexture.GetTemporary(last.width, last.height, 0, last.format);
        //bloomMaterial.SetTexture("_BaseTex", Source);

        Graphics.Blit(last, finalBloom, bloomMaterial, 3);
        bloomMaterial.SetTexture("_BaseTex", source);
        Graphics.Blit(finalBloom, dest, bloomMaterial, 4);

        for (var i = 0; i < kMaxIterations; i++)
        {
            if (m_blurBuffer1[i] != null) RenderTexture.ReleaseTemporary(m_blurBuffer1[i]);
            if (m_blurBuffer2[i] != null) RenderTexture.ReleaseTemporary(m_blurBuffer2[i]);
            m_blurBuffer1[i] = null;
            m_blurBuffer2[i] = null;
        }
        RenderTexture.ReleaseTemporary(finalBloom);
        RenderTexture.ReleaseTemporary(prefiltered);
    }
}
