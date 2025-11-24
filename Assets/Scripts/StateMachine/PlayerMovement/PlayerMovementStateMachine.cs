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
    [SerializeField] private float speedMultiplier = 1f;
    public float liveMaxSpeed {get { return baseSpeed * speedMultiplier; }}
    public string stateName;
    public PlayerMovementState currentState => base.currentState as PlayerMovementState;
    public GroundedState groundedState;
    public WallRunState wallRunState;
    public AirborneState airborneState;


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
    void Awake(){ 
        defaultState = airborneState; 
        
        airborneState.OnReset();
        groundedState.OnReset();
        wallRunState.OnReset();
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
        
    }

    protected override void OnFixedUpdate()
    {
        movementInput = move.action.ReadValue<Vector2>();
        if(!wasMovingLastFrame && isConsideredMoving)
        {
            OnStartWalking();
        }


        wasMovingLastFrame = isConsideredMoving;
        hadMovementInputLastFrame = hasMovementInput; 
    }

    public void OnStartWalking()
    {
        currentState.OnStartWalking();
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
        Debug.Log(Vector3.SignedAngle(headTf.forward, transform.forward, transform.right));
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

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall")){
            wallRunState.storedCollision = collision;
            SetState(wallRunState);
        }
    }

    void OnCollisionExit(Collision collision){
        currentState.OnCollisionExit(collision);
    }
    
    void OnCollisionStay(Collision collision){
        if (collision.gameObject.layer ==LayerMask.NameToLayer("Ground") && currentState != groundedState){
            SetState(groundedState);
        }
    }
    
}   
    