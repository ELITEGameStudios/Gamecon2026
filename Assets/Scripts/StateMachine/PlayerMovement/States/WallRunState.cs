using UnityEngine;

[System.Serializable]
public class WallRunState : PlayerMovementState
{
    public float jumpPower;
    public float wallRunSpeed;
    public Vector3 wallRunDirection;
    public Collision storedCollision;
    public Vector3 wallRunContactNormal;
    public WallRunState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        name = "Wall Running";
    }

    public override void Start()
    {
        wallRunDirection = storedCollision.transform.forward;
        wallRunDirection =
            Vector3.Angle(transform.forward, wallRunDirection) <
            Vector3.Angle(transform.forward, -wallRunDirection) ?
            wallRunDirection :
            wallRunDirection * -1;

        wallRunContactNormal = (transform.position - storedCollision.GetContact(0).point).normalized;
        // wallRunContactNormal -= Vector3.up * wallRunContactNormal.y;
        Debug.Log(wallRunContactNormal);
    }

    public override void FixedUpdate()
    {
        Debug.Log(wallRunContactNormal);
        movement.CalculateLookRotation();
        if(Vector3.Angle(transform.forward, wallRunDirection) > 50)
        {
            Jump();
        }

        if (Physics.OverlapSphere(transform.position, 1, LayerMask.GetMask("Wall")).Length == 0){
            movement.SetState(movement.groundedState);
        }
        
        movement.CalculateLookRotation();
        rigidbody.linearVelocity =
            wallRunDirection * Time.fixedDeltaTime * wallRunSpeed ;
            // + (transform.up * rb.linearVelocity.y);
    }

    public override void Jump()
    {
        // RaycastHit left, right;

        // Physics.Raycast(transform.position, transform.right * -1, out left, LayerMask.GetMask("Wall"));
        // Physics.Raycast(transform.position, transform.right, out right, LayerMask.GetMask("Wall"));

        float xJumpDirection = 1 * ( Vector3.SignedAngle(wallRunDirection, wallRunContactNormal, Vector3.up) < 0 ? 1 : -1 );
        // Debug.Log(Vector3.SignedAngle(wallRunDirection, wallRunContactNormal, Vector3.up));

        rigidbody.AddForce(
            ( transform.up + (transform.right * xJumpDirection)  ).normalized
            * jumpPower, ForceMode.Impulse);
        
        movement.SetState(movement.airborneState);

    }

    public override void End(bool interrupted = false)
    {
        base.End(interrupted);
    }
}