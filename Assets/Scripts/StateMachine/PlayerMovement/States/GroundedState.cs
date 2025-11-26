using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;

[System.Serializable]
public class GroundedState : PlayerMovementState
{
    public float jumpPower;
    public bool walking => movement.movementInput.magnitude > 0.1f && rigidbody.linearVelocity.magnitude > 0.1f;
    
    // [FMODUnity.EventRef(MigrateTo ="EventReference")]
    public string FMODWalkEvent = "";
    EventInstance playerWalkState;


    public GroundedState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        name = "Grounded State";
    }

    public override void Start()
    {
    }

    public override void Update()
    {
        RuntimeManager.AttachInstanceToGameObject(playerWalkState, movement.gameObject);
    }

    public override void FixedUpdate()
    {
        if (!movement.CheckGrounded())
        {
            movement.SetState(movement.airborneState);
        }
        
        movement.CalculateLookRotation();
        movement.NormalWalking();
    }

    public override void OnStartWalking()
    {
        playerWalkState = RuntimeManager.CreateInstance(FMODWalkEvent);
        playerWalkState.start();
    }
    public override void OnStopWalking()
    {
        playerWalkState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    // public void UpdateFMODEvents()
    // {
        
    // }

    public override void Jump()
    {
        rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
        movement.playerJump.start();
    }

    public override void OnCollisionExit(Collision collision)
    {
        // if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")){
        //     if(Physics.Raycast(movement.feetTf.position, Vector3.down, 0.2f, LayerMask.GetMask("Ground"))){
        //         return;

        //     }
        //     movement.SetState(movement.airborneState);
        // }
    }

    public override void End(bool interrupted = false)
    {
        base.End(interrupted);
    }
}