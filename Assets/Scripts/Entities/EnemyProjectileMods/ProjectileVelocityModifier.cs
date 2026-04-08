using UnityEngine;

/// <summary>
/// Modifier that handles projectile movement.
/// </summary>
public class ProjectileVelocityModifier : ProjectileModifier
{
    [SerializeField] Vector3 rotationOffset = Vector3.zero;
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
        projectile.projectileVelocity.Speed = projectileSpeed;
        projectile.meshObjects.transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(rotationOffset);
    }
    void StopMovement()
    {
        projectile.projectileVelocity.Speed = 0.0f;
    }

    public override void UpdateModifier()
    {
        base.UpdateModifier();
        Debug.Log("Projectile speed: " + projectile.projectileVelocity.Speed);
    }
}