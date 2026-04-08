using UnityEngine;

[System.Serializable]
public class PlayerMovementState : State
{
    public float jumpPower;
    public PlayerMovementStateMachine movement;
    [SerializeField] protected InputManager inputManager;
    public PlayerMovementState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        movement = stateMachine;
    }

    public override void OnReset()
    {
        host = movement;
        base.OnReset();
    }

    public override void Start()
    {
        
    }

    public override void Update()
    {
        
    }

    public virtual void Jump()
    {
        rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
        movement.playerJump.start();
        inputManager.OnJumpPerformed();
    }
    public virtual void OnStartWalking(){}
    public virtual void OnStopWalking(){}
    
}