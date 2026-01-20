using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
// using Pathfinding;

public class PlayerMovementStateMachine : StateMachine
{
    [Header("Base Properties")]
    public float baseSpeed = 5f;
    public float clampedRotationY = 85;
    public float movingConsiderationDeadzone = 0.1f;
    public float minWallTangentSlope = 80;
    public float maxWallCheckDist = 5;
    public float groundedCheckDist = 0.15f;
    public float speedMultiplier = 1f;
    public float bodyRadius = 0.5f;
    public float liveMaxSpeed {get { return baseSpeed * speedMultiplier; }}
    
    public PlayerMovementState currentState => base.currentState as PlayerMovementState;
    
    [Header("States")]
    public GroundedState groundedState;
    public WallRunState wallRunState;
    public AirborneState airborneState;
    public string stateName;
    
    [Header("Other Transforms")]
    public Transform feetTf;
    public Transform headTf;


    [Header("Movement Input")]
    public Vector2 movementInput;
    public Vector2 lookInput;
    public Vector2 rotSensitivity;

    public bool hasMovementInput => movementInput.magnitude > movingConsiderationDeadzone;
    public bool hadMovementInputLastFrame;

    public bool isConsideredMoving => hasMovementInput && rigidbody.linearVelocity.magnitude > movingConsiderationDeadzone;
    public bool wasMovingLastFrame;

    public InputActionReference move, jump, look, dash;
    

    [Header("FMOD events")]
    public string FMODJumpEvent = "";
    public string FMODLandEvent = "";
    public FMOD.Studio.EventInstance playerJump, playerLand;

    [Header("External References")]
    public Projectile featherKnife;


    void Awake(){ 
        defaultState = airborneState; 
        
        airborneState.OnReset();
        groundedState.OnReset();
        wallRunState.OnReset();

        playerJump = FMODUnity.RuntimeManager.CreateInstance(FMODJumpEvent);
        playerLand = FMODUnity.RuntimeManager.CreateInstance(FMODLandEvent);
    }

    protected override void OnStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnUnityEnable()
    {
        jump.action.started += Jump;
    }

    protected override void OnUnityDisable()
    {
        jump.action.started -= Jump;
    }

    protected override void OnUpdate()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(playerJump, transform);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(playerLand, transform);
    }

    protected override void OnFixedUpdate()
    {
        movementInput = move.action.ReadValue<Vector2>();

        if(!wasMovingLastFrame && isConsideredMoving) { OnStartWalking(); }
        if(wasMovingLastFrame && !isConsideredMoving) { OnStopWalking(); }

        if(currentState != groundedState){
            if (CheckGrounded())
            {
                SetState(groundedState);
                playerLand.start();
            } 
        }

        wasMovingLastFrame = isConsideredMoving;
        hadMovementInputLastFrame = hasMovementInput; 
    }

    public void OnStartWalking()
    {
        currentState.OnStartWalking();
    }

    public void OnStopWalking()
    {
        try
        {
            currentState.OnStopWalking();
        }
        catch
        {
            Debug.Log("Caught an error");
        }
    }

    protected override void OnSetState()
    {
        stateName = currentState.name;
    }

    public void Jump(InputAction.CallbackContext ctx){
        currentState.Jump();
    }

    public void Blink(){
        SetState(airborneState);
        transform.position = featherKnife.transform.position;
        CheckWallViaRay(ignoreWallRunTimer: true);
    }

    public void CalculateLookRotation()
    {
        Quaternion rotation = headTf.rotation;
        
        lookInput = look.action.ReadValue<Vector2>();
        headTf.Rotate(new Vector3(-lookInput.y * rotSensitivity.y, 0, 0));

        if(Mathf.Abs(Vector3.SignedAngle(headTf.forward, transform.forward, transform.right)) > 85){
            headTf.rotation = rotation;
        }


        transform.Rotate(new Vector3(0, lookInput.x * rotSensitivity.x, 0));
    }

    public virtual void DeathEvent(bool to_player = false)
    {
        Debug.Log(name + " Has Died");
    }

    public bool CheckGrounded()
    {
        if (Physics.Raycast(feetTf.position, Vector3.down, out RaycastHit hit, groundedCheckDist))
        {
            return true;
        }
        return false;
    }

    public Collider GetGroundedCollider()
    {
        if (Physics.Raycast(feetTf.position, Vector3.down, out RaycastHit hit, groundedCheckDist))
        {
            return hit.collider;
        }
        return null;
    }

    void CheckWallViaRay(Collision collision = null, bool ignoreWallRunTimer = false)
    {
        if(!wallRunState.canWallRun && !ignoreWallRunTimer){return;}

        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.right * -1, out hit, maxWallCheckDist)){ wallRunState.isRight = false;  }
        else if(Physics.Raycast(transform.position, transform.right, out hit, maxWallCheckDist)){ wallRunState.isRight = true;  }
        else{return;}
        
        Collider groundedCol = GetGroundedCollider();
        if(groundedCol == hit.collider){return;}
        
        float upDiff = Vector3.Angle(Vector3.up, hit.normal);
        float downDiff = Vector3.Angle(Vector3.down, hit.normal);
        float angleRoll = upDiff < downDiff ? upDiff : downDiff;

        // if(collision == null || hit.collider == collision.collider){ 
        if(angleRoll >= minWallTangentSlope && (collision == null || hit.collider == collision.collider)){ 

            wallRunState.storedCollision = hit; 
            SetState(wallRunState);

            Debug.DrawRay(hit.point, hit.normal);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if(currentState != null){
            currentState.OnCollisionEnter(collision);

            if(currentState != wallRunState && currentState != groundedState){
                CheckWallViaRay(collision);
            }
        };

    }

    void OnCollisionExit(Collision collision){
        currentState.OnCollisionExit(collision);
    }
    
    void OnCollisionStay(Collision collision){
        currentState.OnCollisionStay(collision);

        if(currentState != wallRunState && currentState != groundedState){
            CheckWallViaRay(collision);
        }
    }

}   
    