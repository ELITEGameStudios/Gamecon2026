using UnityEngine;

[System.Serializable]
public class AirborneState : PlayerMovementState
{
    public float airStrafeForce = 0.5f;
    public float topDownTargetMagnitude;
    public float maxDownwardVelocity;
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
        topDownTargetMagnitude = topDownVelocityVector.magnitude;
        initialRelativeVelocity = transform.worldToLocalMatrix.MultiplyPoint(transform.position + rigidbody.linearVelocity);
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

        Vector3 initialToWorld = transform.localToWorldMatrix.MultiplyPoint(initialRelativeVelocity ) - transform.position;

        if (airStrafeForce > 0){
            rigidbody.AddForce(
            (
                (transform.right * movement.movementInput.x) 
                + (transform.forward * movement.movementInput.y)
            ) * Time.fixedDeltaTime * airStrafeForce, ForceMode.Force);
        }
        
        rigidbody.linearVelocity = new Vector3(
            initialToWorld.x,
            Mathf.Clamp(rigidbody.linearVelocity.y, maxDownwardVelocity, Mathf.Infinity),
            initialToWorld.z
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