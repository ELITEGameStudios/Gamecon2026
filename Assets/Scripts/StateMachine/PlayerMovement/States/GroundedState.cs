using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;

[System.Serializable]
public class GroundedState : PlayerMovementState
{
    public bool walking => movement.movementInput.magnitude > 0.1f && rigidbody.linearVelocity.magnitude > 0.1f;
    public float overshootKp;
    public float rampupKp;
    public float desiredSpeed;
    public float turnThresholdAngle;
    public float bigFallThreshold;
    public Vector3 desiredVelocity;
    
    // [FMODUnity.EventRef(MigrateTo ="EventReference")]
    public string FMODWalkEvent = "";
    EventInstance playerWalkState;
    [SerializeField] InGameMusicManager musicManager;


    public GroundedState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        name = "Grounded State";
    }

    public override void Start()
    {
        movement.hasDash = true;
        movement.lastGroundedPos = transform.position;
        if(movement.isConsideredMoving){OnStartWalking();}
    }

    public override void Update()
    {
    //    UnityEngine.Debug.Log(rigidbody.linearVelocity.y);
        RuntimeManager.AttachInstanceToGameObject(playerWalkState, movement.gameObject);
    }

    public override void FixedUpdate()
    {
        if (!movement.CheckGrounded()){ movement.SetState(movement.airborneState); }
        
        movement.CalculateLookRotation();
        if(movement.movementInput.magnitude > 1){movement.movementInput.Normalize();}


        desiredSpeed = Mathf.Lerp(movement.current2DVelocity, movement.liveMaxSpeed * Time.fixedDeltaTime, movement.currentVelocity > movement.liveMaxSpeed * Time.fixedDeltaTime ? overshootKp : rampupKp);
        desiredVelocity =             
            (
                (transform.right * movement.movementInput.x) +
                (transform.forward * movement.movementInput.y)
            ) * desiredSpeed;

        if(Vector3.Angle(desiredVelocity, rigidbody.linearVelocity) > turnThresholdAngle)
        {
            desiredVelocity /= Vector3.Angle(desiredVelocity, rigidbody.linearVelocity) * desiredSpeed / 180;
        }

        // UnityEngine.Debug.Log("Current: " + movement.current2DVelocity + " | Intended:" + desiredSpeed);
        rigidbody.linearVelocity =
            desiredVelocity + (transform.up * rigidbody.linearVelocity.y);
    }

    public override void OnStartWalking()
    {

        PLAYBACK_STATE playback;
        RESULT result = playerWalkState.getPlaybackState(out playback);
        if(result == RESULT.OK){
            if(playback != PLAYBACK_STATE.STOPPED){return;}
        }
                
        playerWalkState = RuntimeManager.CreateInstance(FMODWalkEvent);
        musicManager.SetFootstepEmitter(playerWalkState);
        playerWalkState.start();


    }
    public override void OnStopWalking()
    {
        playerWalkState.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    // public void UpdateFMODEvents()
    // {
        
    // }


    public override void OnCollisionEnter(Collision collision)
    {

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
        playerWalkState.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        base.End(interrupted);
    }
}