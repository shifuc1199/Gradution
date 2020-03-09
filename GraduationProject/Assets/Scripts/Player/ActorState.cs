using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorState : MonoBehaviour
{
    public bool isMoveable { get; set; }
    public bool isInputable { get; set; }
    public bool isGround { get; set; }
    public bool isJump { get; set; }
    public bool isAttack { get; set; }
    public bool isDash { get; set; }
    public bool isMoveRight { get; set; }
    public bool isMoveLeft { get; set; }
    public bool isAttackUp { get; set; }
    public bool isShield { get; set; }
    public bool isSuperArmor { get; set; }
    // Start is called before the first frame update
}
