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
        //can't be damaged by normal attacks
    }

    public void Damage(bool parryDamage)
    {
        if (parryDamage) OnDeath();
    }

}