using System.Collections.Generic;
using UnityEngine;

public class EntityDetectionSystem : MonoBehaviour
{
    [SerializeField] Transform knife;
    [SerializeField] CinematicManager cinematics;
    /// <summary>
    /// Limitation of the degrees that the knife will rotate on the z axis to prevent the knife pointing directly up or down. For example, a deadzone of 60
    /// will give you 300 degrees of rotation, preventing degrees from 330 to 30;
    /// </summary>
    [Range(0, 360), SerializeField] float deadzone; 
    [Range(0.001f, 100), SerializeField] float turnSpeed;
   IEntityManager entityManager;
    Vector3 lookTarget;

    bool init = false;

    [SerializeField] List<EnemyType> blacklistedEnemies = new();

    Vector2 topDeadzone = Vector2.zero;
    Vector2 bottomDeadzone = Vector2.zero;
    public void InitDetectionSystem(IEntityManager manager)
    {
        entityManager = manager;
        init = true;
        topDeadzone = new Vector2(0 - (deadzone/2), 0 + (deadzone/2));
        bottomDeadzone = new Vector2(180 - (deadzone/2), 180 + (deadzone/2));
        Debug.Log("init FEDS");
    }

    private void Update()
    {

        if (!init || cinematics.inCinematic) return;

     
        var enemy = entityManager.GetClosestEnemyToPosition(knife.transform.position, blacklistedEnemies);
        if (enemy != null)
        {
            lookTarget = enemy.transform.position;
            //var target = Quaternion.LookRotation(lookTarget, knife.transform.up);
            //var euler = target.eulerAngles;
            //euler.z = Mathf.Clamp(euler.z, topDeadzone.x, topDeadzone.y);
            //euler.z = Mathf.Clamp(euler.z, bottomDeadzone.x, bottomDeadzone.y);
            //target.eulerAngles = euler;
            var prev = knife.transform.rotation;
            //knife.transform.rotation = Quaternion.RotateTowards(knife.transform.rotation, target, turnSpeed);
            knife.transform.LookAt(lookTarget);
           // knife.transform.rotation = Quaternion.LookRotation(enemy.transform.position - knife.transform.position, knife.transform.up);

            Debug.Log("Changing knife rotation from " + prev + " to " + knife.transform.rotation + " to point at enemy " + enemy.transform.name);
            
        }
        else
        {
            Debug.Log("No enemies present");
        }


    }
}