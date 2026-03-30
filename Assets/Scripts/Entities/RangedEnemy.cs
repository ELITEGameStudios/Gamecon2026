using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyBase
{

    [SerializeField] float projectilePoolSize = 10;
    [SerializeField] List<ProjectileFireInformation> projectileInfo;
    [SerializeField] EntityDetector entityDetector;

    [Header("Firing Attributes")]
    [SerializeField] float cooldown = 20.0f;
    [SerializeField] float delayBeforeFiring = 0.0f;

   
    float cooldownTracker = 0.0f;

    bool firing = false;

    Dictionary<ProjectileFireInformation, Queue<EnemyProjectile>> projectilePools = new();  
    private void Start()
    {
        if (projectileInfo ==  null) 
        {
            Debug.LogWarning("Could not find velocity manager on projectile " + name);
            Destroy(gameObject);
        }
        InitProjectilePool();
    }

    void InitProjectilePool()
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
                   if (!firing && cooldownTracker <= 0.0f) StartCoroutine(FireProjectilesInBurst(player));
                   transform.LookAt(player.transform);
                }
            }
        }
    }

    void Update()
    {
        if (cooldownTracker > 0.0f)
        {
            cooldownTracker -= Time.deltaTime;
            if (cooldownTracker < 0.0f) cooldownTracker = 0.0f;
        }
    }

    IEnumerator FireProjectilesInBurst(Player player)
    {
        if (projectileInfo == null) yield break;
        firing = true;
        yield return new WaitForSeconds(delayBeforeFiring);
        foreach (var info in projectileInfo)
        {
            EnemyProjectile projectile = projectilePools[info].Dequeue();
            if (info.useTransformForOffset) 
            {
                info.offset = info.offsetTransform.localPosition;
            }
            projectile.Activate(player.transform, transform.position + info.offset);
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

    IEnumerator ClearEnemyFromMemory()
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