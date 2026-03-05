using UnityEngine;

public class ArmCameraProportionalDampen : MonoBehaviour
{
    public float Kp = 0.1f, minVel;
    public Vector3 vel, lastPos;
    public Vector3 intensity;
    public Vector3 velRangeToMaxIntensity;
    public Transform mainCamTf;
    public PlayerMovementStateMachine playerMovement;
    

    // Update is called once per frame
    void Update()
    {
        vel = mainCamTf.worldToLocalMatrix.MultiplyVector(playerMovement.rigidbody.linearVelocity);
        vel = new Vector3(
            Mathf.Clamp(vel.x/velRangeToMaxIntensity.x, -1, 1),
            playerMovement.CheckGrounded() ? 0 : Mathf.Clamp(vel.y/velRangeToMaxIntensity.y, -1, 1),
            Mathf.Clamp(vel.z/velRangeToMaxIntensity.z, -1, 1)
        );
        
        // if(vel.magnitude > 1) vel.Normalize();
        // if(vel.magnitude < minVel) vel = Vector3.zero;

        Vector3 target = new Vector3(
            vel.x * intensity.x,
            vel.y * intensity.y,
            vel.z * intensity.z
        );

        transform.localPosition = Vector3.Lerp(transform.localPosition, target, Kp);
        // lastPos = transform.localPosition;
    }
}
