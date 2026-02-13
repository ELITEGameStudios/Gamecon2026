using UnityEngine;

public class ProjectileExpirationModifier : ProjectileLifetimeModifier
{
    [SerializeField] float maxLifetime;

    public override void UpdateModifier()
    {
        base.UpdateModifier();
        if (GetLifetime() > maxLifetime && projectile.Active)
        {
            projectile.DestroyProjectile();
        }
    }
}