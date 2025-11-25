using UnityEngine;

[System.Serializable]
public class AirborneState : PlayerMovementState
{
    public float jumpPower;
    public bool canAirstrafe;
    public int extraJumps, jumpsLeft;

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
        jumpsLeft = extraJumps;
    }

    public override void FixedUpdate()
    {
        movement.CalculateLookRotation();

        if (canAirstrafe){
            movement.NormalWalking();
        }
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")){
            movement.SetState(movement.groundedState);
            movement.playerLand.start();
        }
    }

    public override void End(bool interrupted = false)
    {
        base.End(interrupted);
    }
}