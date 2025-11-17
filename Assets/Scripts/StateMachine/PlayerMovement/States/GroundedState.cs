using UnityEngine;

[System.Serializable]
public class GroundedState : PlayerMovementState
{
    public float jumpPower;
    public GroundedState(PlayerMovementStateMachine stateMachine) : base(stateMachine)
    {
        name = "Grounded State";
    }

    public override void Start() {}

    public override void FixedUpdate()
    {
        movement.CalculateLookRotation();
        movement.NormalWalking();
    }

    public override void Jump()
    {
        rigidbody.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }

    public override void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")){
            movement.SetState(movement.airborneState);
        }
    }

    public override void End(bool interrupted = false)
    {
        base.End(interrupted);
    }
}