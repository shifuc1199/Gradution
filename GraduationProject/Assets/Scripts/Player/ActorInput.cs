using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorInput : MonoBehaviour
{
    public KeyCode attack_key = KeyCode.Mouse0;
    public KeyCode jump_key = KeyCode.Space;
    public KeyCode dash_key = KeyCode.LeftShift;
    public KeyCode heavy_attack_key = KeyCode.Mouse1;
    
    private void Awake()
    {
    
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        ActorController.Controller.actor_state.isAttackUp = Input.GetKey(KeyCode.W);
        ActorController.Controller.actor_state.isJump = Input.GetKeyDown(KeyCode.Space);
        ActorController.Controller.actor_state.isAttackDown = Input.GetKey(KeyCode.S);
#endif
    }
}
