using System.Collections.Generic;
using UnityEngine;

public class EntityDetectionSystem : MonoBehaviour
{
    [SerializeField] Projectile knife;
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
        Debug.Log("init FEDS");
    }

    private void Update()
    {

        if (!init || cinematics.inCinematic) return;

        if (knife.currentState != Projectile.ProjectileState.Idle) return;
        var enemy = entityManager.GetClosestEnemyToPosition(knife.transform.position, blacklistedEnemies);
        if (enemy != null)
        {
            var prev = knife.transform.rotation;
            var target = Quaternion.LookRotation(enemy.collider.bounds.center - knife.transform.position);
            target = Quaternion.Euler(target.x + 90, target.y, target.z);

            //Knife will point straight up when looking at target, rotating by 90 makes it point
            knife.transform.rotation = target;

            Debug.Log("Changing knife rotation from " + prev + " to " + knife.transform.rotation + " to point at enemy " + enemy.transform.name);
            
        }
        else
        {
            Debug.Log("No enemies present");
        }


    }
}