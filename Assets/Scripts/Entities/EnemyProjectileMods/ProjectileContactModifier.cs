
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Modifier that handles damage/collision logic.
/// </summary>
/// 
public class ProjectileContactModifier : ProjectileModifier
{

    [Header("Attributes")]
    [SerializeField] protected int damage = 1;
    [SerializeField] protected int numberOfHits = 1;
    [SerializeField] LayerMask contactableLayers;

    Vector3 previousProjectilePosition;
    int hitsRemaining = 1;

    protected HashSet<Collider> ignoredColliders = new();

    public override void InitModifier(EnemyProjectile projectile)
    {
        base.InitModifier(projectile);
        projectile.projectileFired.AddListener(OnProjectileFired);
    }

    public void OnProjectileFired(EnemyProjectile projectile)
    {
        ignoredColliders.Clear();
        ignoredColliders.Add(projectile.enemy.collider);
    }
    public void CheckForContact()
    {
        var terrainCheck = PerformRaycastCheck(previousProjectilePosition, projectile.rb.position, contactableLayers);
        if (terrainCheck.collider != null)
        {
            var hit = terrainCheck.collider;
            if (hit.TryGetComponent(out Player player))
            {
                player.Damage(damage);
                ignoredColliders.Add(hit);
            }
            OnContact(projectile);
        }

    }
    public virtual void OnContact(EnemyProjectile projectile)
    {
        hitsRemaining -= 1;
        if (hitsRemaining == 0)
        {
            projectile.DestroyProjectile();
        }
    }
    public RaycastHit PerformRaycastCheck(Vector3 previous, Vector3 current, LayerMask collisionMask)
    {
        Vector3 travelVector = current - previous;
        float checkerDistance = travelVector.magnitude;
        if (checkerDistance < 0.001f) return new RaycastHit();

        Ray ray = new(previous, travelVector.normalized);

        var hits = Physics.RaycastAll(ray, checkerDistance, collisionMask, QueryTriggerInteraction.Collide);
        foreach (var hit in hits)
        {
            if (!ignoredColliders.Contains(hit.collider))
            {
                return hit;
            }
        }
        return new RaycastHit();
    }
    public override void UpdateModifier()
    {
        if (projectile.Active)
        {
            CheckForContact();
        }
    }
}