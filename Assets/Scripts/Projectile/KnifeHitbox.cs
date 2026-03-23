using System.Collections.Generic;
using UnityEngine;

public class KnifeHitbox : MonoBehaviour
{
    [SerializeField] Projectile knife;
    [SerializeField] List<Projectile.ProjectileState> statesWithHitboxes;
    [SerializeField] BoxCollider hitbox;
    [SerializeField] LayerMask enemyMask;
    [SerializeField] bool parryOnlyHitbox = false;
    [SerializeField] int damage = 1;

    List<EnemyBase> struckEnemies = new();
    private void Start()
    {
        if (knife == null) knife = GetComponentInParent<Projectile>();
        if (hitbox == null) hitbox = GetComponent <BoxCollider>();
        hitbox.isTrigger = true;

        knife.stateChanged.AddListener(OnKnifeStateChanged);
    }

    void OnKnifeStateChanged(Projectile.ProjectileState state)
    {
       
        bool inValidState = statesWithHitboxes.Contains(state);
        bool parryStateValid;
        if (!parryOnlyHitbox) parryStateValid = true;
        else parryStateValid = knife.projectileAbilities.ProjectileInParryState();
        hitbox.enabled = inValidState && parryStateValid;
        struckEnemies.Clear();
    }

    private void FixedUpdate()
    {
        if (!hitbox.enabled) return;

        var overlap = Physics.OverlapBox(hitbox.bounds.center, hitbox.bounds.extents, hitbox.transform.rotation, enemyMask, QueryTriggerInteraction.Collide);
        foreach (var obj in overlap)
        {
            if (obj.transform.parent.TryGetComponent(out EnemyBase enemy))
            {
                if (struckEnemies.Contains(enemy)) continue; //only hit each enemy once
                enemy.Damage(damage);
                struckEnemies.Add(enemy);
                knife.OnEnemyStruck();
            }
        }
    }
}
