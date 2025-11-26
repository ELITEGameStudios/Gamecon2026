using UnityEngine;

[System.Serializable]
public class PlayerMovementState : State
{
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

    public virtual void Jump(){}
    public virtual void OnStartWalking(){}
    public virtual void OnStopWalking(){}
    
}