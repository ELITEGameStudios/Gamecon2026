using UnityEngine;

public class ProjectileModifier : MonoBehaviour
{
    public int priority = 0;
    protected EnemyProjectile projectile;
    public virtual void InitModifier(EnemyProjectile projectile)
    {
        this.projectile = projectile;
    }
    public virtual void UpdateModifier()
    {

    }
}