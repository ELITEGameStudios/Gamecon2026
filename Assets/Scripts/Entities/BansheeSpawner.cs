using System;
using UnityEngine;

public class BansheeSpawner : EnemyBase
{

    [SerializeField] BansheeProjectile bansheePrefab;
    [SerializeField] Projectile knife;
    static Player player;


    private void Start()
    {
        if (player == null) player = Player.instance;
        if (player == null)
        {
            Debug.LogError("No player found in scene for " + name);
            return;
        }

        if (knife == null)
        {
           knife = FindFirstObjectByType<Projectile>();
        }
        BansheeProjectile projectile = Instantiate(bansheePrefab);
        projectile.InitProjectile(transform, knife);
        projectile.Activate(player.transform, transform.position);
    }
}