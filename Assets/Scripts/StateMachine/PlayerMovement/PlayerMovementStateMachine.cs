using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
// using Pathfinding;

public class PlayerMovementStateMachine : StateMachine
{
    public float baseSpeed = 5f;
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
        lookInput = look.action.ReadValue<Vector2>();
        transform.Rotate(new Vector3(0, lookInput.x * rotSensitivity.x, 0));
        headTf.Rotate(new Vector3(-lookInput.y * rotSensitivity.y, 0, 0));
    }

    public void NormalWalking()
    {
        if(movementInput.magnitude > 1){movementInput.Normalize();}
        Debug.Log("Moving");

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
    