using UnityEngine;

[System.Serializable]
public class DashState : PlayerMovementState
{
    public Vector3 dashVelocity;
    public float dashTime;
    public float additiveForceThreshold = 75;
    public float currentDashTimer;
    public float dashPower = 5;
    public float minimumAdditiveVelocity = 7;
    public AnimationCurve dashPowerOverSpeed;
    public bool additive, canAirJump;

    public DashState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        name = "Dash State";
    }

    public override void OnReset()
    {
        base.OnReset();
        currentDashTimer = dashTime;
    }

    public override void Start()
    {
        movement.hasDash = false;
        currentDashTimer = dashTime;
        Debug.Log("Started dash");
        SetDashVelocity();

        movement.playerDash.start();
        HUDManager.Instance.dashElement.Activate();
    }

    private void SetDashVelocity()
    {

        Vector2 input = movement.movementInput;
        if(input == Vector2.zero){input = Vector2.up;}

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

        dashVelocity = new Vector3(
            movementVector.x,
            0,
            movementVector.y
        ) * ( dashPower * dashPowerOverSpeed.Evaluate(currentVelocity.magnitude) + (additive ? currentVelocity.magnitude : 0));

        // Debug.Log(dashPowerOverSpeed.Evaluate(currentVelocity.magnitude));
        PlayerVFXManager.instance.DashEffect(input);
    }
    public override void FixedUpdate()
    {
        movement.CalculateLookRotation();

        rigidbody.linearVelocity = dashVelocity;        
        currentDashTimer -= Time.deltaTime;
        
        if (currentDashTimer > 0){ currentDashTimer -= Time.fixedDeltaTime; }
        else{ End(); }

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
    }
}