using UnityEngine;

[System.Serializable]
public class PlayerMovementState : State
{
    public float jumpPower;
    public PlayerMovementStateMachine movement;
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
    }
    public virtual void OnStartWalking(){}
    public virtual void OnStopWalking(){}
    
}