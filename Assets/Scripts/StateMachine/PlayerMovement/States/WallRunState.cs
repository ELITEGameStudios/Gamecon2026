using UnityEngine;

[System.Serializable]
public class WallRunState : PlayerMovementState
{
    public float jumpPower;
    public float wallRunSpeed;
    public float wallRunMaxDist;
    
    public bool jumpWithOffset, isRight;
    [SerializeField] float lookAngleDifference;
    public Vector3 wallRunDirection;
    public RaycastHit storedCollision;
    public Vector3 wallRunContactNormal;
    public ApplyAngleProportional camAngle;
    public WallRunState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        name = "Wall Running";
    }

    public override void Start()
    {
        movement.OnStopWalking();
        Vector3 closestPoint = storedCollision.collider.ClosestPoint(transform.position);
        Vector3 raycastDir = (closestPoint - transform.position).normalized;
        
        camAngle.SetAngle(15 * (isRight ? 1 : -1));
        if (Physics.Raycast(transform.position, raycastDir, out RaycastHit hit, Mathf.Infinity))
        {
            wallRunDirection = Vector3.Cross(Vector3.up, hit.normal);
            if(Vector3.Angle(wallRunDirection, transform.forward) > 90)
            {
                wallRunDirection *= -1;
            }

            Debug.DrawRay(hit.point, hit.normal);
            Debug.DrawRay(hit.point, Vector3.Cross(Vector3.up, hit.normal));
        }

        // wallRunDirection = storedCollision.transform.forward;
        // wallRunDirection =
        //     Vector3.Angle(transform.forward, wallRunDirection) <
        //     Vector3.Angle(transform.forward, -wallRunDirection) ?
        //     wallRunDirection :
        //     wallRunDirection * -1;

        // wallRunContactNormal = (transform.position - storedCollision.GetContact(0).point).normalized;
        // // wallRunContactNormal -= Vector3.up * wallRunContactNormal.y;
        // Debug.Log(wallRunContactNormal);
    }

    public override void FixedUpdate()
    {
        Debug.Log(wallRunContactNormal);
        Debug.Log("IS WALL RUNNING");

        movement.CalculateLookRotation();
        lookAngleDifference = Vector3.Angle(transform.forward, wallRunDirection);

        if(Physics.Raycast(transform.position, transform.right * (isRight ? 1 : -1), out RaycastHit hitInfo, movement.maxWallCheckDist))
        {
            // Calculate movement direction
            Vector3 hitPoint = hitInfo.point;
            
            wallRunDirection = Vector3.Cross(Vector3.up, hitInfo.normal);
            wallRunContactNormal = hitInfo.normal;
            if(Vector3.Angle(wallRunDirection, transform.forward) > 90)
            {
                wallRunDirection *= -1;
            }

            Debug.DrawRay(hitInfo.point, hitInfo.normal);
            Debug.DrawRay(hitInfo.point, Vector3.Cross(Vector3.up, hitInfo.normal));


            // Apply velocities
            if(Vector3.Distance(transform.position, hitPoint) > wallRunMaxDist) { transform.position = hitPoint + hitInfo.normal * movement.bodyRadius; }

            rigidbody.linearVelocity =
            wallRunDirection * Time.fixedDeltaTime * wallRunSpeed ;
                Debug.DrawRay(hitInfo.point, hitInfo.normal);
            // }
                
        }
        else
        {
            Jump();
        }
        
        // if(lookAngleDifference > 50)
        // {
        //     jumpWithOffset = true;
        //     Jump();
        // }

        
        movement.CalculateLookRotation();
        // Update position
        // rigidbody.linearVelocity =
        //     wallRunDirection * Time.fixedDeltaTime * wallRunSpeed ;
            // + (transform.up * rb.linearVelocity.y);
    }

    public override void OnCollisionStay(Collision collision)
    {
        
    }

    public override void Jump()
    {
        movement.SetState(movement.airborneState);
        return;
        // RaycastHit left, right;

        // Physics.Raycast(transform.position, transform.right * -1, out left, LayerMask.GetMask("Wall"));
        // Physics.Raycast(transform.position, transform.right, out right, LayerMask.GetMask("Wall"));
        
        float xJumpDirection = 
            jumpWithOffset 
                ? (1 * ( Vector3.SignedAngle(wallRunDirection, wallRunContactNormal, Vector3.up) < 0 ? 1 : -1 ))
                : 0;
        // Debug.Log(Vector3.SignedAngle(wallRunDirection, wallRunContactNormal, Vector3.up));

        rigidbody.AddForce(
            ( transform.up + (transform.right * xJumpDirection)  ).normalized
            * jumpPower, ForceMode.Impulse);
        

    }

    public override void End(bool interrupted = false)
    {
        camAngle.SetAngle(0);
        base.End(interrupted);
    }
}