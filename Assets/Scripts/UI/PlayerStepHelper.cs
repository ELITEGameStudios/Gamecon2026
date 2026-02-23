using UnityEngine;

public class PlayerStepHelper : MonoBehaviour
{
    [SerializeField] PlayerMovementStateMachine movement;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float stepHeight, range;
    [SerializeField] float naturalSlideDist;
    [SerializeField] Vector3 testV;
    void OnCollisionEnter(Collision collision)
    {
        return;
        // Debug.DrawRay(movement.feetTf.position + (transform.up * (stepHeight + naturalSlideDist)), testV.normalized, Color.green, 3);
        for(int i = 0; i < collision.contacts.Length; i++)
        {
            Debug.Log("STARTED");
            ContactPoint contact = collision.contacts[i];
            // if(Vector3.Angle(contact.normal, transform.up) > 45) { continue; } // The collided point is more so a wall instead of a floor
            if(contact.point.y <= movement.feetTf.position.y + naturalSlideDist) { continue; } // The collided point is under the feet

            Vector3 pointDirection = new Vector3(
                contact.point.x - transform.position.x ,
                0,
                contact.point.z - transform.position.z 
            );

            Debug.DrawRay(movement.feetTf.position + (transform.up * (stepHeight + naturalSlideDist)), pointDirection.normalized, Color.red, 3);
            // bool canStepOver = false;
            
            if(Physics.Raycast(new Ray(movement.feetTf.position + (transform.up * (stepHeight + naturalSlideDist)), pointDirection), out RaycastHit Hit, range * 2, layerMask)) { 
                Debug.Log("EVADED");
                continue; 
            }
            
            float collisionHeight = movement.feetTf.position.y + stepHeight;
            for(float j = movement.feetTf.position.y + stepHeight; j > movement.feetTf.position.y; j -= stepHeight / 10)
            {
                if(Physics.Raycast(new Ray(movement.feetTf.position + (transform.up * stepHeight), pointDirection), out RaycastHit test, 0.2f, layerMask)) { 
                    collisionHeight = j + 0.1f;
                    Debug.Log("BROKE");
                    break; 
                }
            }

            Vector2 linearVel2D = new Vector2(
                movement.rigidbody.linearVelocity.x,
                movement.rigidbody.linearVelocity.z
            );
            Debug.Log("STEP INTWST");

            // if(Vector2.Angle(linearVel2D, pointDirection) > 180){continue;}

            transform.position = new Vector3(
                transform.position.x + pointDirection.x * range,
                collisionHeight - movement.feetTf.localPosition.y,
                transform.position.z + pointDirection.y * range
            );
        }
        // if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")){
        //     if(Physics.Raycast(movement.feetTf.position, Vector3.down, 0.2f, LayerMask.GetMask("Ground"))){
        //         return;

        //     }
        //     movement.SetState(movement.airborneState);
        // }
    }
}
