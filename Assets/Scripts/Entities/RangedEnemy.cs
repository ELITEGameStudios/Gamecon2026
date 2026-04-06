using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RangedEnemy : EnemyBase
{

    [SerializeField] protected float projectilePoolSize = 10;
    [SerializeField] protected List<ProjectileFireInformation> projectileInfo;
    [SerializeField] protected EntityDetector entityDetector;
    [SerializeField] protected Collider hurtbox;

    [Header("Firing Attributes")]
    [SerializeField] protected float cooldown = 20.0f;
    [SerializeField] protected float delayBeforeFiring = 0.0f;

   
    protected float cooldownTracker = 0.0f;

    protected bool firing = false;

    protected Dictionary<ProjectileFireInformation, Queue<EnemyProjectile>> projectilePools = new();

    protected static LayerMask playerMask;

    Collider[] playerCollider = new Collider[1];

    protected UnityEvent startedFiring = new UnityEvent();

    protected override void Init()
    {
        base.Init();
        if (projectileInfo == null)
        {
            Debug.LogWarning("Could not find velocity manager on projectile " + name);
            Destroy(gameObject);
        }
        InitProjectilePool();
        playerMask = LayerMask.GetMask("Player");
    }


    protected void InitProjectilePool()
    {
        for (int i = 0; i < projectileInfo.Count; i++)
        {
            projectilePools[projectileInfo[i]] = new ();
            for (int x = 0; x < projectilePoolSize; x++)
            {
                var prefab = projectileInfo[i].projectilePrefab;
                var projectile = Instantiate(prefab);
                projectile.InitProjectile(transform);
                projectile.DestroyProjectile();
                projectilePools[projectileInfo[i]].Enqueue(projectile);
            }

        }
    }

    private void FixedUpdate()
    {
        if (entityDetector.DetectedEntities.Count > 0)
        {
            foreach (var entity in entityDetector.DetectedEntities)
            {
                if (entity is Player player )
                {
                   if (!firing && cooldownTracker <= 0.0f) Shoot(player.transform);
                
                    Vector3 lookTarget = new Vector3(
                        player.transform.position.x,
                        transform.position.y,
                        player.transform.position.z
                    );
                   transform.LookAt(lookTarget);
                }
            }
        }

        var overlappingColliders = Physics.OverlapBoxNonAlloc(hurtbox.bounds.center, hurtbox.bounds.extents, playerCollider, hurtbox.transform.rotation, playerMask);
        if (overlappingColliders > 0)
        {
            Player.instance.Damage();
        }

    }

    protected virtual void Shoot(Transform target)
    {
        StartCoroutine(FireProjectilesInBurst(target));
    }  

    void Update()
    {
        if (cooldownTracker > 0.0f)
        {
            cooldownTracker -= Time.deltaTime;
            if (cooldownTracker < 0.0f) cooldownTracker = 0.0f;
        }
    }

    protected virtual IEnumerator FireProjectilesInBurst(Transform player)
    {
        if (projectileInfo == null) yield break;
        firing = true;
        startedFiring.Invoke();
        yield return new WaitForSeconds(delayBeforeFiring);
        foreach (var info in projectileInfo)
        {
            EnemyProjectile projectile = projectilePools[info].Dequeue();
            // if (info.useTransformForOffset) 
            // {
            //     info.offset = info.offsetTransform.localPosition;
            // }
            projectile.Activate(player.transform, info.useTransformForOffset ? info.offsetTransform.position : transform.position);
            projectilePools[info].Enqueue(projectile);
           if (info != projectileInfo[^1]) yield return new WaitForSeconds(info.delayAfterShot);
        }
        cooldownTracker = cooldown;
        firing = false;
    }

    protected override void OnDeath()
    {
        StartCoroutine(ClearEnemyFromMemory());
    }

    protected IEnumerator ClearEnemyFromMemory()
    {
        foreach (var pool in projectilePools)
        {
            yield return null;
            foreach (var projectile in pool.Value)
            {
                Destroy(projectile.gameObject);
            }
        }
        base.OnDeath();
    }
}