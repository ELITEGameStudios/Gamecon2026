using System.Collections.Generic;
using UnityEngine;

public class KnifeHitbox : MonoBehaviour
{
    const float SAFE_MARGIN = 1.01f;

    [SerializeField] Projectile knife;
    [SerializeField] List<Projectile.ProjectileState> statesWithHitboxes;
    [SerializeField] BoxCollider hitbox;
    [SerializeField] bool parryOnlyHitbox = false;
    [SerializeField] int damage = 1;

    List<EnemyBase> struckEnemies = new();
    List<ShieldEntity> struckShields = new();
    LayerMask enemyMask;

    int tick;
    private void Start()
    {
        if (knife == null) knife = GetComponentInParent<Projectile>();
        if (hitbox == null) hitbox = GetComponent <BoxCollider>();
        hitbox.isTrigger = true;

        knife.stateChanged.AddListener(OnKnifeStateChanged);

        hitbox.enabled = false;
        enemyMask = LayerMask.GetMask("EnemyHurtbox");
    }

    void OnKnifeStateChanged(Projectile.ProjectileState state)
    {
       
        bool inValidState = statesWithHitboxes.Contains(state);
        bool parryStateValid;
        if (!parryOnlyHitbox) parryStateValid = true;
        else parryStateValid = knife.projectileAbilities.ParryActive;
        hitbox.enabled = inValidState && parryStateValid;
        struckEnemies.Clear();
        struckShields.Clear();
    }

    private void FixedUpdate()
    {
        if (!hitbox.enabled) return;

        tick++;
        var overlap = Physics.OverlapBox(hitbox.bounds.center, hitbox.bounds.extents, hitbox.transform.rotation, enemyMask, QueryTriggerInteraction.Collide);
        foreach (var obj in overlap)
        {
            if (obj.transform.parent.TryGetComponent(out ShieldEntity shield))
            {
                if (struckShields.Contains(shield)) continue;
                struckShields.Add(shield);
                Debug.Log("Struck shield " + shield.name + " on tick " + tick);
            }
            else if (obj.transform.parent.TryGetComponent(out EnemyBase enemy))
            {
                if (struckEnemies.Contains(enemy)) continue; //only hit each enemy once
                else if (enemy.EnemyShield != null)
                {
                    if (struckShields.Contains(enemy.EnemyShield))
                    {
                        continue;
                    }
                }
                Debug.Log("struck enemy " + enemy.name + " on tick " + tick);
                enemy.Damage(damage);
                struckEnemies.Add(enemy);
                knife.OnEnemyStruck(CalculateNormal(obj));
            }
        }
    }

    Vector3 CalculateNormal(Collider collider)
    {
        var closestPoint = collider.ClosestPoint(hitbox.bounds.center);
        var pointToHitboxCenter = closestPoint - hitbox.bounds.center;
        var ray = new Ray(hitbox.bounds.center, pointToHitboxCenter);
        var raycast = Physics.Raycast(ray, out RaycastHit info, pointToHitboxCenter.magnitude + SAFE_MARGIN, enemyMask, QueryTriggerInteraction.Collide);

        if (info.collider != null)
        {
            return info.normal;
        }
        return Vector3.zero;
    }
}
