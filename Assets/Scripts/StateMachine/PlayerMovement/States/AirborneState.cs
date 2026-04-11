using UnityEngine;

[System.Serializable]
public class AirborneState : PlayerMovementState
{
    public float airStrafeForwardForce = 0.5f;
    public float airStrafeForce = 0.5f;
    public float maxDownwardVelocity;
    public float strafeThresholdVel;
    public int extraJumps, jumpsLeft;
    public Vector3 initialRelativeVelocity, topDownVelocityVector;

    public AirborneState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        name = "Airborne State";
    }

    public override void OnReset()
    {
        base.OnReset();
    }

    public override void Start()
    {
        movement.OnStopWalking();
        jumpsLeft = extraJumps;
        topDownVelocityVector = new Vector2(
            rigidbody.linearVelocity.x,
            rigidbody.linearVelocity.z
        );

        movement.wallRunState.StartWallrunCooldown();
        // initialRelativeVelocity = transform.worldToLocalMatrix.MultiplyPoint(transform.position + rigidbody.linearVelocity);
        // Debug.Log
    }

    public override void FixedUpdate()
    {
        movement.CalculateLookRotation();
        // Vector3 newVelocityFactor = transform.localToWorldMatrix.MultiplyPoint(initialRelativeVelocity);
        
        // rigidbody.linearVelocity = new Vector3(
        //     newVelocityFactor.x,
        //     rigidbody.linearVelocity.y,
        //     newVelocityFactor.z
        // );

        // Vector3 initialToWorld = transform.localToWorldMatrix.MultiplyPoint(initialRelativeVelocity ) - transform.position;
        Vector2 velocity2D = new Vector2(
            rigidbody.linearVelocity.x, 
            rigidbody.linearVelocity.z
        );

        Vector2 right2D = new Vector2(
            transform.right.x, 
            transform.right.z
        );

        Vector2 forward2D = new Vector2(
            transform.forward.x, 
            transform.forward.z
        );
        
        float previousVel = velocity2D.magnitude;


        if (airStrafeForce > 0){

            Vector2 movementInput = inputManager.GetMovementDirection();
            velocity2D += 
                ((right2D * movementInput.x * airStrafeForce) + (forward2D * movementInput.y * airStrafeForwardForce)) * Time.fixedDeltaTime;
            
            if(previousVel > strafeThresholdVel && velocity2D.magnitude >= previousVel) { 
                velocity2D = velocity2D.normalized * previousVel;
            }
        }

        rigidbody.linearVelocity = new Vector3(
            velocity2D.x,
            Mathf.Clamp(rigidbody.linearVelocity.y, maxDownwardVelocity, Mathf.Infinity),
            velocity2D.y
        );  
        
    }

    public override void Jump()
    {
        if(jumpsLeft > 0){
            rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
            jumpsLeft--;
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        // if (!movement.CheckGrounded())
        // {
        //     rigidbody.linearVelocity += collision.impulse;
        //     initialRelativeVelocity = transform.worldToLocalMatrix.MultiplyPoint(transform.position + rigidbody.linearVelocity);
        // }
    }

    public override void End(bool interrupted = false)
    {
        base.End(interrupted);
    }
}