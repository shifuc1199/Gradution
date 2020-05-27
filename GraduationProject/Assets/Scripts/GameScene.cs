using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DreamerTool.Util;
using DreamerTool.UI;
using DreamerTool.GameObjectPool;
public class GameScene : Scene
{
  
    public static BackScene backScene = BackScene.None;

    private void OnApplicationQuit()
    {
        SaveManager.Instance.SaveActorModel();
    }
    public override void Awake()
    {
        base.Awake();
 
        GameStaticMethod.GameInit();
 
        switch (backScene)
        {
            case BackScene.None:
                break;
            case BackScene.FightScene:
                OpenView<HobbyView>();
                OpenView<RoomView>();
                break;
            default:
                break;
        }
        backScene = BackScene.None;
    }
   
    // Start is called before the first frame update
    void Start()
    {


    }
    public void LoadSceneJumpByFadeAnim(string scene_name)
    {
        GetView<GameInfoView>().FadeAnim(() => {
            LoadingScene.LoadScene(scene_name);
        });
    }
    public void ResetActorState()
    {
        ActorModel.Model.ResetState();
    }
    public void TransferPlayerByFadeAnim(Transform pos)
    {
        GetView<GameInfoView>().FadeAnim(() => { ActorController.Controller.Transfer(pos);
        Camera.main.GetComponent<Cinemachine.CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<Cinemachine.CinemachineConfiner>().InvalidatePathCache();
        Camera.main.GetComponent<Cinemachine.CinemachineBrain>().ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingShape2D = pos.gameObject.GetComponent<PolygonCollider2D>(); });
    }
    // Update is called once per frame
   void Update()
    {
         
    }
}
