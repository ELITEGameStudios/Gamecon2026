
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
        previousProjectilePosition = projectile.rb.position;
        projectile.projectileCollider.enabled = true;
    }

    public void ResetContactModifiers()
    {
        ignoredColliders.Clear();
        ignoredColliders.Add(projectile.projectileCollider);
        hitsRemaining = numberOfHits;
        previousProjectilePosition = projectile.rb.position;
    }
    public void CheckForContact()
    {
        var terrainCheck = PerformRaycastCheck(previousProjectilePosition, projectile.rb.position, contactableLayers);
        if (terrainCheck.collider != null)
        {
            var hit = terrainCheck.collider;
            if (hit.gameObject.CompareTag("Player"))
            {
                OnPlayerCollision(hit);
            }
            OnContact(terrainCheck);
        }
    }
    public void OnPlayerCollision(Collider hit)
    {
        Player.instance.Damage(damage);
        ignoredColliders.Add(hit);
        contactEvent.Invoke(projectile, Player.instance);
    }
    public virtual void OnContact(RaycastHit hit)
    {
        if (!hasLimitedHits || hitsRemaining <= 0) return;
        hitsRemaining -= 1;
        if (hitsRemaining <= 0 && !overrideCollisionLogic)
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
            else
            {
                Debug.Log("Ignoring collider " + hit.collider);
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