using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class DashState : PlayerMovementState
{
    [SerializeField] float empoweredDashDrainRate = 120.0f;
    [SerializeField] float FOVIncrease = 10;
    [SerializeField] float FOVTweenDuration = 0.1f;
    [SerializeField] Camera FPSCamera;
    [SerializeField] float empoweredDashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1.5f;

    float tweenTracker = 0.0f;


    InputActionReference dashButton;
    public float dashVelocity;
    public float dashTime;
    public float additiveForceThreshold = 75;
    public float elapsedDashTime;
    public float dashPower = 5;
    public float minimumAdditiveVelocity = 7;
    public AnimationCurve dashPowerOverSpeed;
    public bool additive, canAirJump;

    

    float baseFOV = 0;

    public float CooldownTracker { get; private set; }
    public DashState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        name = "Dash State";
    }

    public void Initialize(InputActionReference button)
    {
        dashButton = button;
        baseFOV = FPSCamera.fieldOfView;
    }

    public override void OnReset()
    {
        base.OnReset();
        elapsedDashTime = 0;
    }

    public override void Start()
    {
        if (CooldownTracker > 0)
        { 
            End(true); 
            return; 
        }
        movement.hasDash = false;
        elapsedDashTime = 0;
        //    Debug.Log("Started dash");
        SetDashVelocity();

        movement.playerDash.start();
        HUDManager.Instance.dashElement.Activate();
        tweenTracker = 0;
    }


   
    private void SetDashVelocity()
    {

        Vector2 input = movement.movementInput;
        if (input == Vector2.zero) { input = Vector2.up; }

        Vector2 currentVelocity = new Vector2(
            rigidbody.linearVelocity.x,
            rigidbody.linearVelocity.z
        );

        Vector2 movementVector = new Vector2(
            (transform.forward.x * input.y) + (transform.right.x * input.x),
            (transform.forward.z * input.y) + (transform.right.z * input.x)
        ).normalized;
        // Debug.Log(input.y);

        float angle = Vector2.Angle(currentVelocity, movementVector);
        additive = angle < additiveForceThreshold && currentVelocity.magnitude > minimumAdditiveVelocity;

        dashVelocity = (dashPower * dashPowerOverSpeed.Evaluate(currentVelocity.magnitude) + (additive ? currentVelocity.magnitude : 0));
        
        PlayerVFXManager.instance.DashEffect(input);
    }
    public override void Update()
    {
        if (tweenTracker < FOVTweenDuration)
        {
            tweenTracker += Time.deltaTime;
            FPSCamera.fieldOfView = Mathf.Lerp(baseFOV, baseFOV + FOVIncrease, tweenTracker / FOVTweenDuration);
        }
    }

    public void EndCooldown()
    {
        CooldownTracker = 0;   
    }

    public override void FixedUpdate()
    {
        movement.CalculateLookRotation();

        rigidbody.linearVelocity = dashVelocity * movement.headTf.transform.forward;

        if (elapsedDashTime < dashTime)
        {
            elapsedDashTime += Time.fixedDeltaTime;
        }
        if (elapsedDashTime >= dashTime || !dashButton.action.IsPressed())
        {
            CooldownTracker = dashCooldown;
            End();
        }
    }
    public override void InactiveUpdate()
    {
        if (tweenTracker < FOVTweenDuration)
        {
            tweenTracker += Time.deltaTime;
            FPSCamera.fieldOfView = Mathf.Lerp(FOVIncrease + baseFOV, baseFOV, tweenTracker / FOVTweenDuration);
        }
        CooldownTracker -= Time.deltaTime;
    }


    public override void Jump()
    {
        if(canAirJump || movement.CheckGrounded())
        {
            base.Jump();
            movement.SetState(movement.airborneState);
        }
    }

    public override void End(bool interrupted = false)
    {
        base.End(interrupted);
        tweenTracker = 0;
    }
}