using UnityEngine;

[System.Serializable]
public class WallRunState : PlayerMovementState
{
    public float jumpPower;
    public float wallRunSpeed;
    public bool jumpWithOffset;
    [SerializeField] float lookAngleDifference;
    public Vector3 wallRunDirection;
    public Collision storedCollision;
    public Vector3 wallRunContactNormal;
    public WallRunState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        name = "Wall Running";
    }

    public override void Start()
    {
        movement.OnStopWalking();
        Vector3 closestPoint = storedCollision.collider.ClosestPoint(transform.position);
        Vector3 raycastDir = (closestPoint - transform.position).normalized;
        
        if (Physics.Raycast(transform.position, raycastDir, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Wall")))
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

        movement.CalculateLookRotation();
        lookAngleDifference = Vector3.Angle(transform.forward, wallRunDirection);
        
        if(lookAngleDifference > 50)
        {
            jumpWithOffset = true;
            Jump();
        }

        if (Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask("Wall")).Length == 0){
            jumpWithOffset = false;
            Jump();
            movement.SetState(movement.groundedState);
        }
        
        movement.CalculateLookRotation();
        rigidbody.linearVelocity =
            wallRunDirection * Time.fixedDeltaTime * wallRunSpeed ;
            // + (transform.up * rb.linearVelocity.y);
    }

    public override void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Vector3 closestPoint = collision.collider.ClosestPoint(transform.position);
            Vector3 raycastDir = (closestPoint - transform.position).normalized;
            
            if (Physics.Raycast(transform.position, raycastDir, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Wall")))
            {
                wallRunDirection = Vector3.Cross(Vector3.up, hit.normal);
                wallRunContactNormal = hit.normal;
                if(Vector3.Angle(wallRunDirection, transform.forward) > 90)
                {
                    wallRunDirection *= -1;
                }

                Debug.DrawRay(hit.point, hit.normal);
                Debug.DrawRay(hit.point, Vector3.Cross(Vector3.up, hit.normal));
            }
        }


        // ContactPoint[] contacts = new ContactPoint[]{};
        // collision.GetContacts(contacts);

        // foreach (ContactPoint contact in contacts){
        //     if(contact.otherCollider)
        // }
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
        base.End(interrupted);
    }
}