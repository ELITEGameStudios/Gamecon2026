using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class EntityDetectionSystem : MonoBehaviour
{
    [SerializeField] Projectile knife;
    [SerializeField] CinematicManager cinematics;
    [SerializeField] Transform knifeHolder, knifeTf;
    [SerializeField] float rotationSpeed = 10.0f;
    [SerializeField] float Kp = 0.01f;
    IEntityManager entityManager;

    bool init = false;

    [SerializeField] List<EnemyType> blacklistedEnemies = new();

    public void InitDetectionSystem(IEntityManager manager)
    {
        entityManager = manager;
        init = true;
    }

    private void Update()
    {

        if (!init)
        {
            Debug.Log("not initialized, no FEDS");
            return;
        }

        if (cinematics.inCinematic)
        {
            Debug.Log("In cinematic, no FEDS");
            return;
        }

        if (knife.currentState != Projectile.ProjectileState.Idle)
        {
            Debug.Log("Wrong knife state, no FEDS");
            return;
        }
        Debug.Log("YO! FEDS IS WORKING!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        var enemy = entityManager.GetClosestEnemyToPosition(knife.transform.position, blacklistedEnemies);
        if (enemy != null)
        {
            var enemyPosition = enemy.collider.bounds.center;            

            var enemyProjected = Vector3.ProjectOnPlane(enemyPosition, knifeHolder.transform.up);
            var knifeProjected = Vector3.ProjectOnPlane(knife.transform.position, knifeHolder.transform.up);

            var knifeProjectedLookingAtEnemyProjected = Quaternion.LookRotation(enemyProjected - knifeProjected, knifeHolder.transform.up);
            var knifeProjectedLookingAtEnemyProjectedEulerAngles = knifeProjectedLookingAtEnemyProjected.eulerAngles;
            knifeProjectedLookingAtEnemyProjectedEulerAngles.x += 90;
            knifeProjectedLookingAtEnemyProjectedEulerAngles.z = 0;
            knifeProjectedLookingAtEnemyProjected.eulerAngles = knifeProjectedLookingAtEnemyProjectedEulerAngles;
            // knife.transform.rotation = Quaternion.RotateTowards(knife.transform.rotation, knifeProjectedLookingAtEnemyProjected, rotationSpeed);

            knifeTf.transform.rotation = Quaternion.Slerp(knifeTf.transform.rotation, knifeProjectedLookingAtEnemyProjected, Kp);
        }
        else
        {
            Debug.Log("Could not find enemy");
        }
    }

    void OnDisable()
    {
        // knifeTf.transform.rotation = initRotation;
        knifeTf.transform.localEulerAngles = Vector3.zero;
    }
}