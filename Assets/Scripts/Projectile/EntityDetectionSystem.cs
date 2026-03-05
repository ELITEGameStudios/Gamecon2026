using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class EntityDetectionSystem : MonoBehaviour
{
    [SerializeField] Projectile knife;
    [SerializeField] CinematicManager cinematics;
    [SerializeField] Transform knifeHolder;
    /// <summary>
    /// Limitation of the degrees that the knife will rotate on the z axis to prevent the knife pointing directly up or down. For example, a deadzone of 60
    /// will give you 300 degrees of rotation, preventing degrees from 330 to 30;
    /// </summary>
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
            var handPos = knifeHolder.position;

            var handLookingAtEnemy = Quaternion.FromToRotation(knife.transform.forward, (handPos - enemyPosition));

            var knifeEuler = knife.transform.rotation.eulerAngles;
            //knifeEuler.y = handLookingAtEnemy.eulerAngles.y;
            knifeEuler.y = 0;
            knifeEuler.z = 0;
            knifeEuler.x = 90 + knifeHolder.eulerAngles.x;
            knife.transform.eulerAngles = knifeEuler;
        }
    }
}