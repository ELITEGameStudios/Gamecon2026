using System;
using UnityEngine;

public class BansheeSpawner : EnemyBase
{

    [SerializeField] EnemyProjectile bansheePrefab;

    static Player player;


    private void Start()
    {
        if (player == null) player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("No player found in scene for " + name);
            return;
        }

        EnemyProjectile projectile = Instantiate(bansheePrefab);
        projectile.InitProjectile(transform);
        projectile.Activate(player.transform, transform.position);
    }
}