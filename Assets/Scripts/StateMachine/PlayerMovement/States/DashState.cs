using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class DashState : PlayerMovementState
{
    [SerializeField] float empoweredDashDrainRate = 120.0f;
    [SerializeField] float FOVIncrease = 10;
    [SerializeField] float FOVTweenDuration = 0.1f;
    [SerializeField] Camera FPSCamera;

    float tweenTracker = 0.0f;

    InputActionReference dashButton;
    WindManager windManager;
    public Vector3 dashVelocity;
    public float dashTime;
    public float additiveForceThreshold = 75;
    public float currentDashTimer;
    public float dashPower = 5;
    public float minimumAdditiveVelocity = 7;
    public AnimationCurve dashPowerOverSpeed;
    public bool additive, canAirJump;



    bool empowered = false;
    float initialSpeedWhenEmpowered;

    float baseFOV = 0;

    public DashState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        name = "Dash State";
    }

    public void Initialize(InputActionReference button, WindManager wind)
    {
        dashButton = button;
        windManager = wind;
        baseFOV = FPSCamera.fieldOfView;
    }

    public override void OnReset()
    {
        base.OnReset();
        currentDashTimer = dashTime;
    }

    public override void Start()
    {
        empowered = false;
        movement.hasDash = false;
        currentDashTimer = dashTime;
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

        if (!empowered)
        {
            dashVelocity = new Vector3(
                movementVector.x,
                0,
                movementVector.y
            ) * (dashPower * dashPowerOverSpeed.Evaluate(currentVelocity.magnitude) + (additive ? currentVelocity.magnitude : 0));
        }
        else
        {
            dashVelocity =
               movement.headTf.transform.forward
            * (dashPower * dashPowerOverSpeed.Evaluate(currentVelocity.magnitude) + initialSpeedWhenEmpowered);
        }

        // Debug.Log(dashPowerOverSpeed.Evaluate(currentVelocity.magnitude));
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

    public override void FixedUpdate()
    {
        movement.CalculateLookRotation();

        rigidbody.linearVelocity = dashVelocity;
        currentDashTimer -= Time.deltaTime;

        if (currentDashTimer > 0) { currentDashTimer -= Time.fixedDeltaTime; }
        else
        {
            if (windManager.CurrentWind > 0 && dashButton.action.IsPressed())
            {
                EmpowerDash();
            }
            else
            {
                End();
            }
        }
        EmpowerLogic();
    }

    public override void InactiveUpdate()
    {
        if (tweenTracker < FOVTweenDuration)
        {
            tweenTracker += Time.deltaTime;
            FPSCamera.fieldOfView = Mathf.Lerp(FOVIncrease + baseFOV, baseFOV, tweenTracker / FOVTweenDuration);
        }
    }
    void EmpowerLogic()
    {
        if (!empowered) return;
        windManager.CurrentWind -= (empoweredDashDrainRate * Time.fixedDeltaTime);
        if (windManager.CurrentWind <= 0.001f || !dashButton.action.IsPressed())
        {
            End();
        }
        SetDashVelocity();
    }
    void EmpowerDash()
    {
        if (empowered) return;
        empowered = true;
        windManager.pauseWindGeneration = true;
        initialSpeedWhenEmpowered = rigidbody.linearVelocity.magnitude;
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
        windManager.pauseWindGeneration = false;
        tweenTracker = 0;
    }
}