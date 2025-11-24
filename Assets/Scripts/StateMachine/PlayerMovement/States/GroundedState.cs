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
        playerWalkState = RuntimeManager.CreateInstance(FMODWalkEvent);
    }

    public override void Update()
    {
        RuntimeManager.AttachInstanceToGameObject(playerWalkState, movement.gameObject);
    }

    public override void FixedUpdate()
    {
        movement.CalculateLookRotation();
        movement.NormalWalking();
    }

    public override void OnStartWalking()
    {
        playerWalkState.start();
    }
    public override void OnStopWalking()
    {
        playerWalkState.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void UpdateFMODEvents()
    {
        
    }

    public override void Jump()
    {
        rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }

    public override void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")){
            movement.SetState(movement.airborneState);
        }
    }

    public override void End(bool interrupted = false)
    {
        base.End(interrupted);
    }
}