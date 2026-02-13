using UnityEngine;

/// <summary>
/// Modifier that handles projectile movement.
/// </summary>
public class ProjectileVelocityModifier : ProjectileModifier
{

    public float projectileSpeed;
    public override void InitModifier(EnemyProjectile projectile)
    {
        base.InitModifier(projectile);
        projectile.projectileActivated.AddListener(OnProjectileFired);
        projectile.projectileDestroyed.AddListener((projectile) => StopMovement());
    }

     void OnProjectileFired(EnemyProjectile.ProjectileTarget target)
    {
        var dir = (target.targetTransform.position - projectile.rb.position).normalized;

        StartMovement(dir);
    }

    void StartMovement(Vector3 dir)
    {
        projectile.projectileSpeed = dir * projectileSpeed;
        projectile.projectileCollider.enabled = true;
    }
    void StopMovement()
    {
       projectile.projectileSpeed = Vector3.zero;
    }

}