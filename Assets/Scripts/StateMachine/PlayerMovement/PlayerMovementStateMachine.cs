using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
// using Pathfinding;

public class PlayerMovementStateMachine : StateMachine
{
    public static PlayerMovementStateMachine instance {get; private set;}


    [Header("Base Properties")]
    public float baseSpeed = 5f;
    public float clampedRotationY = 85;
    public float movingConsiderationDeadzone = 0.1f;
    public float minWallTangentSlope = 80;
    public float wallRunLinearVelocityMaxAngleDif = 60;
    public float maxWallCheckDist = 5, blinkWallCheckDistance = 2;
    public float groundedCheckDist = 0.15f;
    public float speedMultiplier = 1f;
    public float bodyRadius = 0.5f;
    public Vector3 blinkBoxSize;
    public LayerMask blinkBoxLayerMask;
    public PhysicsMaterial frictionMat, slipMat;
    public float liveMaxSpeed {get { return baseSpeed * speedMultiplier; }}
    public float currentVelocity {get { return rigidbody.linearVelocity.magnitude; }}
    public float previousCurrentVelocity;
    public float current2DVelocity {
        get { 
            return new Vector2(
                rigidbody.linearVelocity.x, 
                rigidbody.linearVelocity.z
            ).magnitude; 
        }
    }
    
    public PlayerMovementState currentState => base.currentState as PlayerMovementState;
    
    [Header("States")]
    public GroundedState groundedState;
    public WallRunState wallRunState;
    public AirborneState airborneState;
    public DashState dashState;
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
    public bool hasDash;

    public bool isConsideredMoving => hasMovementInput && rigidbody.linearVelocity.magnitude > movingConsiderationDeadzone;
    public bool canDash => currentState != dashState && hasDash && currentState != wallRunState;
    public bool wasMovingLastFrame;

    public InputActionReference move, jump, look, dash;
    

    [Header("FMOD events")]
    public string FMODJumpEvent = "";
    public string FMODLandEvent = "";
    public string FMODDashEvent = "";
    public FMOD.Studio.EventInstance playerJump, playerLand, playerDash;

    [Header("External References")]
    public Collider mainCol;
    public Projectile featherKnife;
    public GroundedHelper groundedHelper;
    public Animator armAnimator;

    void Awake(){ 

        if(instance == null) {instance = this;}
        else if(instance != this){Destroy(this);}
        
        defaultState = airborneState; 

        airborneState.OnReset();
        groundedState.OnReset();
        wallRunState.OnReset();
        dashState.OnReset();

        playerJump = FMODUnity.RuntimeManager.CreateInstance(FMODJumpEvent);
        playerLand = FMODUnity.RuntimeManager.CreateInstance(FMODLandEvent);
        playerDash = FMODUnity.RuntimeManager.CreateInstance(FMODDashEvent);
    }

    protected override void OnStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnUnityEnable()
    {
        jump.action.started += Jump;
        dash.action.performed += OnDashInput;
    }

    protected override void OnUnityDisable()
    {
        jump.action.started -= Jump;
        dash.action.performed -= OnDashInput;
    }

    protected override void OnUpdate()
    {
        HUDManager.Instance.dashElement.SetReady(canDash);
        armAnimator.SetBool("isMoving", isConsideredMoving);
        armAnimator.SetBool("IsGrounded", CheckGrounded());
        // HUDManager.Instance.dashElement.SetFillFactor();

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(playerJump, transform);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(playerLand, transform);
    }

    protected override void OnFixedUpdate()
    {
        movementInput = move.action.ReadValue<Vector2>();

        if(!wasMovingLastFrame && isConsideredMoving) { OnStartWalking(); }
        if(wasMovingLastFrame && !isConsideredMoving) { OnStopWalking(); }
        
        mainCol.material = hasMovementInput ? slipMat : frictionMat;
        if(currentState != groundedState){
            if (CheckGrounded() && currentState != dashState)
            {
                SetState(groundedState);
                playerLand.start();
            } 
        }

        wasMovingLastFrame = isConsideredMoving;
        hadMovementInputLastFrame = hasMovementInput; 
    }

    protected override void PostStateFixedUpdate()
    {
        previousCurrentVelocity = currentVelocity;
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

    public void OnDashInput(InputAction.CallbackContext ctx){
        if (canDash)
        {
            Dash();       
        }
    }

    public void Dash(){
        SetState(dashState);
        featherKnife.projectileAbilities.dashPerformed.Invoke();
    }

    public void Blink(){
        
        SetState(airborneState);
        Vector3 targetPos = featherKnife.GetBlinkPosition();
        KnifeRetrievalInfo info = new ()
        {
            pickupType = KnifeRetrievalType.Blink,
            blinkDistance = Vector3.Distance(transform.position, targetPos),
        };
        featherKnife.knifeRetrieved.Invoke(info);
        Quaternion rot = featherKnife.transform.rotation;
        // for (float i = 0; i < 0.5f; i += 0.1f)
        // {
        //     targetPos = featherKnife.transform.position - featherKnife.throwDirection * i;
        //     if(Physics.OverlapBox(targetPos, blinkBoxSize, Quaternion.identity, blinkBoxLayerMask).Length > 0){
        //         continue;
        //     }
        //     break;
        // }

        transform.position = targetPos;
        
        CheckWallViaRay(ignoreWallRunTimer: true, fromBlink: true);
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
        return groundedHelper.isGrounded;

        Debug.DrawLine(feetTf.position, feetTf.position + Vector3.down * groundedCheckDist);
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

    void CheckWallViaRay(Collision collision = null, bool ignoreWallRunTimer = false, bool fromBlink = false)
    {
        if(!wallRunState.canWallRun && !ignoreWallRunTimer ){return;}

        RaycastHit hit;

        // Checks if there is a potential wall to run on and the general direction it is relative to the player
        if(Physics.Raycast(transform.position, transform.right * -1, out hit, fromBlink ? blinkWallCheckDistance : maxWallCheckDist)){ wallRunState.isRight = false;  }
        else if(Physics.Raycast(transform.position, transform.right, out hit, fromBlink ? blinkWallCheckDistance : maxWallCheckDist)){ wallRunState.isRight = true;  }
        else{return;}
        
        // Checks if the wall running collider is the same as the grounding collider for the player
        Collider groundedCol = GetGroundedCollider();
        if(groundedCol == hit.collider){return;}
        
        // Getting angle roll difference
        float upDiff = Vector3.Angle(Vector3.up, hit.normal);
        float downDiff = Vector3.Angle(Vector3.down, hit.normal);
        float angleRoll = upDiff < downDiff ? upDiff : downDiff;

        // Wall run direction
        Vector3 testWallRunDir = Vector3.Cross(Vector3.up, hit.normal);
        if(Vector3.Angle(testWallRunDir, transform.forward) > 90) { testWallRunDir *= -1; }
        //if(Vector3.Angle(testWallRunDir, rigidbody.linearVelocity) > wallRunLinearVelocityMaxAngleDif){return;}

        // Testing look difference between player and wall direction
        float currentLookAngleDifference = Vector3.Angle(transform.forward, testWallRunDir);
        if(currentLookAngleDifference < wallRunState.lookAngleDifferenceRange.x || currentLookAngleDifference > wallRunState.lookAngleDifferenceRange.y)
        {
            return;
        }

        Vector3 movementRelativeInput = (transform.forward * movementInput.y) + (transform.right * movementInput.x);

        if(currentState != dashState && !fromBlink){
            if(
                Vector3.Angle(movementRelativeInput, testWallRunDir) > wallRunState.maxMovementInputAngleDifference && currentState != dashState
                || !hasMovementInput
            )
            {return;}
        }

        // if(collision == null || hit.collider == collision.collider){ 
        if(angleRoll >= minWallTangentSlope && (collision == null || hit.collider == collision.collider)){ 

            wallRunState.storedCollision = hit; 
            wallRunState.initRaycastDir = wallRunState.isRight? transform.right : transform.right * -1;
            SetState(wallRunState);

            Debug.DrawRay(hit.point, hit.normal);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if(currentState != null){
            if(currentState != wallRunState && currentState != groundedState){
                CheckWallViaRay(collision);
            }

            currentState.OnCollisionEnter(collision);
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
    