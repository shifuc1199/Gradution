    /*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.UI;
using Photon.Pun;
using Photon.Realtime;
using LitJson;
public class FightScene : Scene 
{

    public static Dictionary<int, NetkActorController> PlayerDict = new Dictionary<int, NetkActorController>();
  
    // Start is called before the first frame update
    void Start()
    {
      GameObject player = PhotonNetwork.Instantiate("Net/NetWorkPlayer", new Vector3(-50, 3, 0), Quaternion.identity,0,new object[] {JsonMapper.ToJson(ActorModel.Model )});
    }
    public void GameOver(int lose)
    {
        lose++;
        lose %= 2;
        PhotonNetwork.AutomaticallySyncScene = false;
        OpenView<TipView>().SetContent("比赛结束\n"+ PlayerDict[lose].GetModel().actor_name+ "获胜！",()=>{
          if (PlayerDict[lose].GetModel().actor_name == ActorModel.Model.actor_name)
                ActorModel.Model.SetPKScore(10);
            GameScene.backScene = BackScene.FightScene;
            LoadingScene.LoadScene(GameConstData.GAME_MAIN_SCENE_NAME); });
    }
    // Update is called once per frame
    void Update()
    {
        
    }

  
}
