using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
// using Pathfinding;

public class PlayerMovementStateMachine : StateMachine
{
    public float baseSpeed = 5f;
    public float clampedRotationY = 85;
    public float movingConsiderationDeadzone = 0.1f;
    public float maxGroundedSlope = 35;
    public float maxWallSlope = 25;
    [SerializeField] private float speedMultiplier = 1f;
    public float liveMaxSpeed {get { return baseSpeed * speedMultiplier; }}
    public string stateName;
    public PlayerMovementState currentState => base.currentState as PlayerMovementState;
    public GroundedState groundedState;
    public WallRunState wallRunState;
    public AirborneState airborneState;

    public Transform feetTf;


    public Vector2 movementInput;
    public Vector2 lookInput;
    public Vector2 rotSensitivity;
    // public Vector2 targetVelocity;


    public bool hasMovementInput => movementInput.magnitude > movingConsiderationDeadzone;
    public bool hadMovementInputLastFrame;


    public bool isConsideredMoving => hasMovementInput && rigidbody.linearVelocity.magnitude > movingConsiderationDeadzone;
    public bool wasMovingLastFrame;

    

    public Transform headTf;
    public InputActionReference move, jump, look;

    [Header("FMOD events")]
    public string FMODJumpEvent = "";
    public string FMODLandEvent = "";
    public FMOD.Studio.EventInstance playerJump, playerLand;



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

    protected override void OnUpdate()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(playerJump, transform);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(playerLand, transform);
    }

    protected override void OnFixedUpdate()
    {
        movementInput = move.action.ReadValue<Vector2>();
        if(!wasMovingLastFrame && isConsideredMoving)
        {
            OnStartWalking();
        }
        if(wasMovingLastFrame && !isConsideredMoving)
        {
            OnStopWalking();
        }

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

    public void CalculateLookRotation()
    {
        Quaternion rotation = headTf.rotation;
        
        lookInput = look.action.ReadValue<Vector2>();
        headTf.Rotate(new Vector3(-lookInput.y * rotSensitivity.y, 0, 0));
        // Debug.Log(Vector3.SignedAngle(headTf.forward, transform.forward, transform.right));
        if(Mathf.Abs(Vector3.SignedAngle(headTf.forward, transform.forward, transform.right)) > 85){
            headTf.rotation = rotation;
        }


        transform.Rotate(new Vector3(0, lookInput.x * rotSensitivity.x, 0));
    }

    public void NormalWalking()
    {
        if(movementInput.magnitude > 1){movementInput.Normalize();}
        Debug.Log("Moving");

        // rigidbody.AddForce(
        //     (
        //         (transform.right * movementInput.x) +
        //         (transform.forward * movementInput.y)
        //     ) * Time.fixedDeltaTime * liveMaxSpeed
        //     + (transform.up * rigidbody.linearVelocity.y)
        // , ForceMode.Force);
        
        rigidbody.linearVelocity =
            (
                (transform.right * movementInput.x) +
                (transform.forward * movementInput.y)
            ) * Time.fixedDeltaTime * liveMaxSpeed
            + (transform.up * rigidbody.linearVelocity.y);
    }


    public virtual void DeathEvent(bool to_player = false)
    {
        Debug.Log(name + " Has Died");
    }

    void OnCollisionEnter(Collision collision)
    {
        if(currentState != null){currentState.OnCollisionEnter(collision);};

        CheckWallCriteria(collision);
    }

    void OnCollisionExit(Collision collision){
        currentState.OnCollisionExit(collision);
    }
    
    void OnCollisionStay(Collision collision){
        currentState.OnCollisionStay(collision);


        // if (collision.gameObject.layer ==LayerMask.NameToLayer("Ground") && currentState != groundedState){
        //     // SetState(groundedState);
        //     CheckGrounded(collision);
        // }
    }

    public bool CheckGrounded()
    {
        if (Physics.Raycast(feetTf.position, Vector3.down, out RaycastHit hit, 0.4f))
        {
            // if(Vector3.Angle(Vector3.up, hit.normal) <= maxGroundedSlope)
            // {
            return true;
            // }

            Debug.DrawRay(hit.point, hit.normal);
        }
        return false;
    }

    void CheckWallCriteria(Collision collision)
    {
        // if (collision.gameObject.layer != LayerMask.NameToLayer("Wall"))
        // {   
        //     return;
        // }
        Vector3 closestPoint = collision.collider.ClosestPoint(transform.position);
        Vector3 raycastDir = (closestPoint - transform.position).normalized;

        if (Physics.Raycast(transform.position, raycastDir, out RaycastHit hit, Mathf.Infinity))//, LayerMask.GetMask("Wall")))
        {
            Vector3 comparisonVector = new Vector3(
                hit.normal.x,
                0,
                hit.normal.z
            );

            if(Vector3.Angle(comparisonVector, hit.normal) <= maxWallSlope)
            {
                wallRunState.storedCollision = collision;
                SetState(wallRunState);
            }

            Debug.DrawRay(hit.point, hit.normal);
        }
    }
    
}   
    