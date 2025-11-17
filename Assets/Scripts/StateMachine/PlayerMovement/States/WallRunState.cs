using UnityEngine;

[System.Serializable]
public class WallRunState : PlayerMovementState
{
    public float jumpPower;
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

        wallRunContactNormal = storedCollision.GetContact(0).normal;
        wallRunContactNormal -= Vector3.up * wallRunContactNormal.y;
    }

    public override void FixedUpdate()
    {
        movement.CalculateLookRotation();

        if (Physics.OverlapSphere(transform.position, 1, LayerMask.GetMask("Wall")).Length == 0){
            movement.SetState(movement.groundedState);
        }
        
        movement.CalculateLookRotation();
        rigidbody.linearVelocity =
            wallRunDirection * Time.fixedDeltaTime * movement.liveMaxSpeed;
            // + (transform.up * rb.linearVelocity.y);
    }

    public override void Jump()
    {
        RaycastHit left, right;

        Physics.Raycast(transform.position, transform.right * -1, out left, LayerMask.GetMask("Wall"));
        Physics.Raycast(transform.position, transform.right, out right, LayerMask.GetMask("Wall"));
        float xJumpDirection = 1 * (left.collider != null?  1 : right.collider != null? -1 : 0);

        rigidbody.AddForce(
            ( transform.up + (Vector3.right * xJumpDirection)  ).normalized
            * jumpPower, ForceMode.Impulse);
        
        movement.SetState(movement.airborneState);

    }

    public override void End(bool interrupted = false)
    {
        base.End(interrupted);
    }
}