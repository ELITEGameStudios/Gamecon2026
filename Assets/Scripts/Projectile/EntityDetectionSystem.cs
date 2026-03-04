using System.Collections.Generic;
using UnityEngine;

public class EntityDetectionSystem : MonoBehaviour
{
    [SerializeField] Projectile knife;
    [SerializeField] Transform player;
    [SerializeField] CinematicManager cinematics;
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
    }

    private void Update()
    {

        if (!init || cinematics.inCinematic) return;

        if (knife.currentState != Projectile.ProjectileState.Idle) return;
        var enemy = entityManager.GetClosestEnemyToPosition(knife.transform.position, blacklistedEnemies);
        if (enemy != null)
        {
            var vector = (enemy.collider.bounds.center - knife.transform.position);
            vector.y = 0;
            vector.z = 0; // makes knife assume that we're always level with the target;
            //knife.transform.LookAt(player);
           var target = Quaternion.LookRotation(vector);
            //Knife will point straight up when looking at target, rotating by 90 makes it point
            knife.transform.rotation = target;

            Debug.Log("Looking at target " + enemy.name);
        }

    }
}