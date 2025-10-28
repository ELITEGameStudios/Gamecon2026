using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float baseSpeed = 5f;
    [SerializeField] private float speedMultiplier = 1f;
    public float liveMaxSpeed {get { return baseSpeed * speedMultiplier; }}
    public float jumpPower = 5f;
    public Vector2 rotSensitivity;
    public Rigidbody rb;
    public Vector2 targetVelocity;
    public MoveState moveState;
    public Vector2 movementInput;
    public Vector2 lookInput;
    public Vector3 wallRunDirection;
    public Vector3 wallRunContactNormal;
    public Transform headTf;
    public bool jumpInput, grounded;

    public enum MoveState
    {
        Normal,
        Wall,
        AirborneBuffer
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        lookInput = Input.mousePositionDelta;
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        movementInput = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        if(jumpInput){ Jump(); }
    }

    void FixedUpdate()
    {

        switch (moveState)
            {
                case MoveState.Normal:
                    CalculateLookRotation();
                    CalculateVelocityNormalState();
                break;
                    
                case MoveState.Wall:
                    if (Physics.OverlapSphere(transform.position, 1, LayerMask.GetMask("Wall")).Length == 0)
                    {
                        ExitWallRun();
                    }
                    
                    CalculateLookRotation();
                    CalculateVelocityWallState();

                    break;

            }
    }
    
    void SetupWallRun(Collision collision)
    {
        wallRunDirection = collision.transform.forward;
        wallRunDirection =
            Vector3.Angle(transform.forward, wallRunDirection) <
            Vector3.Angle(transform.forward, -wallRunDirection) ?
            wallRunDirection :
            wallRunDirection * -1;

        wallRunContactNormal = collision.GetContact(0).normal;
        wallRunContactNormal -= Vector3.up * wallRunContactNormal.y;

        moveState = MoveState.Wall;
    }
    void ExitWallRun()
    {
        moveState = MoveState.Normal;
    }

    void TriggerAirborneBuffer(){
        moveState = MoveState.AirborneBuffer;
    }

    void Jump(){
        if (moveState == MoveState.Normal){
            if(!grounded){ return; }
            rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
        }
        if (moveState == MoveState.Wall){
            ExitWallRun();
            rb.AddForce(transform.up + wallRunContactNormal.normalized * jumpPower, ForceMode.Impulse);
        }
    }

    void CalculateLookRotation()
    {
        transform.Rotate(new Vector3(0, lookInput.x * rotSensitivity.x, 0));
        headTf.Rotate(new Vector3(-lookInput.y * rotSensitivity.y, 0, 0));
    }

    void CalculateVelocityNormalState()
    {
        // if(!grounded) { return; }
        rb.linearVelocity =
            (
                (transform.right * movementInput.x) +
                (transform.forward * movementInput.y)
            ) * Time.fixedDeltaTime * liveMaxSpeed
            + (transform.up * rb.linearVelocity.y);
    }
    void CalculateVelocityWallState()
    {
        rb.linearVelocity =
            wallRunDirection * Time.fixedDeltaTime * liveMaxSpeed;
            // + (transform.up * rb.linearVelocity.y);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            grounded = true;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            SetupWallRun(collision);
        }
    }
    
    void OnCollisionStay(Collision collision){
        if (collision.gameObject.layer ==LayerMask.NameToLayer("Ground")){
            grounded = true;
        }
    }
    
    void OnCollisionExit(Collision collision){
        if (collision.gameObject.layer ==LayerMask.NameToLayer("Ground")){
            grounded = false;
        }
    }
}
