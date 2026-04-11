using UnityEngine;

/// <summary>
/// Modifier to indicate that a projectile cannot be destroyed by killboxes or similar hazards.
/// </summary>
public class ProjectileIndestructibleModifier : ProjectileModifier
{

    public bool warpBackToSpawnOnContact = true;
    ProjectileContactModifier contactModifier;

    Vector3 spawnPos;

    public override void InitModifier(EnemyProjectile projectile)
    {
        base.InitModifier(projectile);
        contactModifier = projectile.GetProjectileModifier<ProjectileContactModifier>();
        if (contactModifier != null)
        {
            contactModifier.contactEvent.AddListener(OnProjectileContact);
            contactModifier.OverrideCollisionLogic();
        }
        spawnPos = projectile.enemy.transform.position;
    }

    void OnProjectileContact(EnemyProjectile projectile, Player player)
    {
        if (!warpBackToSpawnOnContact) return;
       projectile.rb.MovePosition(spawnPos);
       if (contactModifier != null) contactModifier.ResetIgnoredColliders();
    }

    private void OnDestroy()
    {
        contactModifier.contactEvent.RemoveListener(OnProjectileContact);
    }

}
