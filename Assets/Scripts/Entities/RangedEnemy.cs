using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyBase
{

    [SerializeField] float projectilePoolSize = 10;
    [SerializeField] List<ProjectileFireInformation> projectileInfo;
    [SerializeField] EntityDetector entityDetector;
    [SerializeField] float cooldown = 20.0f;

    float cooldownTracker = 0.0f;

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
                projectile.InitProjectile(this);
                projectile.DestroyProjectile();
                projectilePools[projectileInfo[i]].Enqueue(projectile);
            }

        }
    }

    private void FixedUpdate()
    {
        if (entityDetector.DetectedEntities.Count > 0 && cooldownTracker <= 0.0f)
        {
            foreach (var entity in entityDetector.DetectedEntities)
            {
                if (entity is Player player)
                {
                    StartCoroutine(FireProjectilesInBurst(player));
                    cooldownTracker = cooldown;
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
        yield return new WaitForSeconds(projectileInfo[0].delayBeforeShot);
        foreach (var info in projectileInfo)
        {
            EnemyProjectile projectile = projectilePools[info].Dequeue();
            if (info.useTransformForOffset) 
            {
                info.offset = info.offsetTransform.localPosition;
            }
            projectile.Activate(player.transform, transform.position + info.offset);
            projectilePools[info].Enqueue(projectile);
            yield return new WaitForSeconds(info.delayAfterShot);
        }
    }
}