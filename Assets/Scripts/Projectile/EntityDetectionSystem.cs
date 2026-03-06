using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class EntityDetectionSystem : MonoBehaviour
{
    [SerializeField] Projectile knife;
    [SerializeField] CinematicManager cinematics;
    [SerializeField] Transform knifeHolder;
    [SerializeField] float rotationSpeed = 10.0f;
    IEntityManager entityManager;

    bool init = false;

    [SerializeField] List<EnemyType> blacklistedEnemies = new();

    public void InitDetectionSystem(IEntityManager manager)
    {
        entityManager = manager;
        init = true;
        Debug.Log("Init FEDS");
    }

    private void Update()
    {

        if (!init || cinematics.inCinematic)
        {
            return;
        }

        if (knife.currentState != Projectile.ProjectileState.Idle)
        {
            return;
        }
        var enemy = entityManager.GetClosestEnemyToPosition(knife.transform.position, blacklistedEnemies);
        if (enemy != null)
        {
            var enemyPosition = enemy.collider.bounds.center;
            

            var enemyProjected = Vector3.ProjectOnPlane(enemyPosition, knifeHolder.transform.up);
            var knifeProjected = Vector3.ProjectOnPlane(knife.transform.position, knifeHolder.transform.up);

            var knifeProjectedLookingAtEnemyProjected = Quaternion.LookRotation(enemyProjected - knifeProjected, knifeHolder.transform.up);
            var knifeProjectedLookingAtEnemyProjectedEulerAngles = knifeProjectedLookingAtEnemyProjected.eulerAngles;
            knifeProjectedLookingAtEnemyProjectedEulerAngles.x += 90;
           // knifeProjectedLookingAtEnemyProjectedEulerAngles.y = 0;
            knifeProjectedLookingAtEnemyProjectedEulerAngles.z = 0;
            knifeProjectedLookingAtEnemyProjected.eulerAngles = knifeProjectedLookingAtEnemyProjectedEulerAngles;
            knife.transform.rotation = Quaternion.RotateTowards(knife.transform.rotation, knifeProjectedLookingAtEnemyProjected, rotationSpeed);
        }
    }
}