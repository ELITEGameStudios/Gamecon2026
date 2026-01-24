using UnityEngine;

[System.Serializable]
public class WallRunState : PlayerMovementState
{
    public float wallRunMinSpeed;
    public float currentWallRunSpeed;
    public float wallRunMaxDist;
    public float raycastForwardCheckDist = 0.1f;
    
    public bool jumpWithOffset, isRight;
    [SerializeField] float lookAngleDifference;
    public Vector3 wallRunDirection;
    [SerializeField] Vector3 lastWallRunDirection;
    public float maxTransitionAngle; // Cuts off a wall run if the angle between two surfaces are too great
    public RaycastHit storedCollision;
    public Vector3 wallRunContactNormal;
    public Vector3 lastContactPosition;
    public Vector3 raycastCheckVector => (transform.forward * raycastForwardCheckDist) + transform.right * (isRight ? 1 : -1);
    public ApplyAngleProportional camAngle;
    

    // Determines if the player cannot wall run for a set time after finishing a wall run. should be a short interval
    public float wallRunSleepInterval = 0.2f;
    float currentWallRunSleepTimer;
    public bool canWallRun => currentWallRunSleepTimer <= 0 && movement.currentVelocity >= wallRunMinSpeed;


    public WallRunState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        name = "Wall Running";
    }

    public override void Start()
    {
        movement.hasDash = true;
        movement.OnStopWalking();
        Vector3 closestPoint = storedCollision.collider.ClosestPoint(transform.position);
        Vector3 raycastDir = (closestPoint - transform.position).normalized;
        
        camAngle.SetAngle(15 * (isRight ? 1 : -1));
        if (Physics.Raycast(transform.position, raycastDir, out RaycastHit hit, Mathf.Infinity))
        {
            // Determine the direction in world space the player is supposed to run
            wallRunDirection = Vector3.Cross(Vector3.up, hit.normal);
            if(Vector3.Angle(wallRunDirection, transform.forward) > 90)
            {
                wallRunDirection *= -1;
            }


            // Determine the speed in which the player will wall run
            float currentSpeed = movement.currentVelocity;
            // if(currentSpeed < wallRunMinSpeed){currentWallRunSpeed = wallRunMinSpeed; return;} // Sets to the min wall run speed if your speed is slower. (May be obselete since wall run might reqire you tp be this speed)
            
            // Speed will be between the current speed and the minimum wall run speed, determined by the angle of your entry velocity and the wall's run direction
            currentWallRunSpeed = Mathf.Lerp(
                currentSpeed, wallRunMinSpeed, 
                Mathf.Clamp(Vector3.Angle(rigidbody.linearVelocity, wallRunDirection) / 90, 0, 1)
            )/ Time.fixedDeltaTime;
        }
    }

    public override void FixedUpdate()
    {
        lastWallRunDirection = wallRunDirection;



        movement.CalculateLookRotation();
        lookAngleDifference = Vector3.Angle(transform.forward, wallRunDirection);

        if(Physics.Raycast(transform.position, raycastCheckVector.normalized, out RaycastHit hitInfo, movement.maxWallCheckDist))
        {
            // Check for valid wall angle 
            float upDiff = Vector3.Angle(Vector3.up, hitInfo.normal);
            float downDiff = Vector3.Angle(Vector3.down, hitInfo.normal);
            float angleRoll = upDiff < downDiff ? upDiff : downDiff;

            if(angleRoll < movement.minWallTangentSlope){ 
                movement.SetState(movement.airborneState);
            }
            
            // Calculate movement direction
            Vector3 hitPoint = hitInfo.point;

            wallRunDirection = Vector3.Cross(Vector3.up, hitInfo.normal);
            wallRunContactNormal = hitInfo.normal;
            if(Vector3.Angle(wallRunDirection, transform.forward) > 90)
            {
                wallRunDirection *= -1;
            }

            if(Vector3.Angle(wallRunDirection, lastWallRunDirection) > maxTransitionAngle){
                movement.SetState(movement.airborneState);
                return;
            }
    
            // Debug info
            // Debug.DrawRay(hitInfo.point, hitInfo.normal);
            // Debug.DrawRay(hitInfo.point, Vector3.Cross(Vector3.up, hitInfo.normal));

            // Apply velocities

            // if(Vector3.Distance(transform.position, hitPoint) > wallRunMaxDist) { transform.position = hitPoint + hitInfo.normal * movement.bodyRadius; } This was causing a bug where the player moves abnormally fast when facing away from the wall at a certain angle. Meant to be a way to ensure the player is confined to be against the wall
            rigidbody.linearVelocity =
            wallRunDirection * Time.fixedDeltaTime * currentWallRunSpeed;
            Debug.DrawRay(hitInfo.point, hitInfo.normal);

        }
        else
        {
            movement.SetState(movement.airborneState);
        }
        
        movement.CalculateLookRotation();
    }

    public override void InactiveUpdate()
    {
        if (!canWallRun) { currentWallRunSleepTimer -= Time.deltaTime; }
    }

    public override void Jump()
    {   

        float xJumpDirection = 
            jumpWithOffset 
                ? (1 * ( Vector3.SignedAngle(wallRunDirection, wallRunContactNormal, Vector3.up) < 0 ? 1 : -1 ))
                : 0;

        rigidbody.AddForce(
            ( transform.up + (transform.right * xJumpDirection)  ).normalized
            * jumpPower, ForceMode.Impulse);

        movement.SetState(movement.airborneState);
    }

    public override void End(bool interrupted = false)
    {
        camAngle.SetAngle(0);
        currentWallRunSleepTimer = wallRunSleepInterval;
        base.End(interrupted);
    }
}