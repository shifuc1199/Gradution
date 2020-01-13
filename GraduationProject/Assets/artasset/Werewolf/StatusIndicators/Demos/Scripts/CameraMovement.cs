/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;

namespace Werewolf.StatusIndicators.Demo {
  public class CameraMovement : MonoBehaviour {
    public GameObject player;
    public float offsetX = -5;
    public float offsetZ = 0;
    public float maximumDistance = 2;
    public float playerVelocity = 10;
 
    private float movementX;
    private float movementZ;

    void Update() {
      movementX = (player.transform.position.x + offsetX - transform.position.x) / maximumDistance; 
      movementZ = (player.transform.position.z + offsetZ - transform.position.z) / maximumDistance; 
      transform.position += new Vector3((movementX * playerVelocity * Time.deltaTime), 0, (movementZ * playerVelocity * Time.deltaTime)); 
    }
  }
}
