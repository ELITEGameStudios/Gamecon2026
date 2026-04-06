
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Modifier that handles damage/collision logic.
/// </summary>
/// 
public class ProjectileContactModifier : ProjectileModifier
{

    public UnityEvent<EnemyProjectile, Player> contactEvent;

    [Header("Attributes")]
    [SerializeField] protected int damage = 1;
    [SerializeField] protected bool hasLimitedHits = true;
    [SerializeField, ShowIf(nameof(HasLimitedHits))] protected int numberOfHits = 1;
    [SerializeField] LayerMask contactableLayers;

    Vector3 previousProjectilePosition;
    int hitsRemaining = 1;

    protected HashSet<Collider> ignoredColliders = new();


    bool overrideCollisionLogic = false;// Other mods can set this to true to override the default collision logic and just use the raycast check for contact

    bool HasLimitedHits() => hasLimitedHits;
    public override void InitModifier(EnemyProjectile projectile)
    {
        base.InitModifier(projectile);
        projectile.projectileFired.AddListener(OnProjectileFired);
    }

    public void OverrideCollisionLogic()
    {
        overrideCollisionLogic = true;
    }
    public void OnProjectileFired(EnemyProjectile projectile)
    {
        ResetContactModifiers();
    }

    public void ResetContactModifiers()
    {
        ignoredColliders.Clear();
        ignoredColliders.Add(projectile.enemy.GetComponent<Collider>());
        hitsRemaining = numberOfHits;
    }
    public void CheckForContact()
    {
        var terrainCheck = PerformRaycastCheck(previousProjectilePosition, projectile.rb.position, contactableLayers);
        if (terrainCheck.collider != null)
        {
            var hit = terrainCheck.collider;
            if (hit.TryGetComponent(out Player player))
            {
                OnPlayerCollision(player, hit);
            }
            OnContact(projectile);
        }

    }

    public void OnPlayerCollision(Player player, Collider hit)
    {
        player.Damage(damage);
        ignoredColliders.Add(hit);
        contactEvent.Invoke(projectile, player);
    }
    public virtual void OnContact(EnemyProjectile projectile)
    {
        if (!hasLimitedHits) return;
        hitsRemaining -= 1;
        if (hitsRemaining == 0 && !overrideCollisionLogic)
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
        previousProjectilePosition = projectile.rb.position;
    }
}