using UnityEngine;
/// <summary>
/// Shield entity. Completely invincible to normal attacks, but a parried projectile will destroy it.
/// </summary>
public class ShieldEntity : EntityBase
{

    [SerializeField] Collider shieldCollider;
    [SerializeField] LayerMask knifeMask;
    public override void Damage(int damage = 1)
    {
        Debug.Log("Shield hit but not parried");
    }

    private void FixedUpdate()
    {
        var overlap = Physics.OverlapBox(shieldCollider.bounds.center, shieldCollider.bounds.extents, shieldCollider.transform.rotation, knifeMask);
        foreach (var col in overlap)
        {
            if (col.TryGetComponent(out Projectile knife))
            {
                if (knife.projectileAbilities.ProjectileInParryState())
                {
                    OnDeath();
                }
            }
        }

    }
}