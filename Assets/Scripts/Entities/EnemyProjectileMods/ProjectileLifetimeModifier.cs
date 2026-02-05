using UnityEngine;

/// <summary>
/// Modifier that allows for functionality based on projectile lifetime.
/// </summary>
public abstract class ProjectileLifetimeModifier : ProjectileModifier
{

    float lifetime;

    public override void InitModifier(EnemyProjectile projectile)
    {
        base.InitModifier(projectile);
        projectile.projectileFired.AddListener(OnProjectileFired);
    }

    public void OnProjectileFired(EnemyProjectile projectile)
    {
        lifetime = 0.0f;
    }

    public override void UpdateModifier()
    {
        if (projectile.Active)
        {
            lifetime += Time.deltaTime;
        }
    }

    public float GetLifetime()
    {
        return lifetime;
    }
}
