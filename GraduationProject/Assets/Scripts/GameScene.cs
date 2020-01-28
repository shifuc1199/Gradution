using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DreamerTool.Util;
using DreamerTool.UI;
public class GameScene : Scene
{
    public static GameScene _instance;
    float timer;
    private int hit_count;
    public Text hit_count_text;
    public int HitCount {
        get
        {
            return hit_count;
        }
        set
        {
            hit_count = value;
            hit_count_text.GetComponent<DOTweenAnimation>().DORestart();
            hit_count_text.text = hit_count + "  Combo";
        }
    }
    // Start is called before the first frame update
    void Start()
    {
         
        ActorModel model = new ActorModel();
        _instance = this;
        Application.targetFrameRate = 60;
        GameObjectPoolManager.InitByScriptableObject();  
    }
    public void TransferPlayerByFadeAnim(Transform pos)
    {
        GetView<GameInfoView>().FadeAnim(() => { ActorController._controller.Transfer(pos);
        Camera.main.GetComponent<Cinemachine.CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingShape2D = pos.gameObject.GetComponent<PolygonCollider2D>(); });
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 1)
        {

            timer += Time.unscaledDeltaTime;

            if (timer >= 0.15f)
            {

                timer = 0;
                Time.timeScale = 1f;
            }
        }
        else
        {
            timer = 0;
        }
    }
}
