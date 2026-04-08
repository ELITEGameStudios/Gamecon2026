using UnityEngine;

public class Killbox : MonoBehaviour
{
    /// <summary>
    /// If this is true, destroys entities that exit the trigger area. Prevents player from being able to escape play area.
    /// </summary>
    [SerializeField] bool isPlayAreaLimiter = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EntityBase entity))
        {
            entity.Die();
        }
        else if (other.TryGetComponent(out EnemyProjectile proj))
        {
            foreach (var mod in proj.projectileModifiers)
            {
                if (mod.GetType() == typeof(ProjectileIndestructibleModifier))
                {
                    return;
                }
            }
            proj.DestroyProjectile();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlayAreaLimiter) return;


        if (other.TryGetComponent(out EntityBase entity))
        {
            entity.Die();
        }
        else if (other.TryGetComponent(out EnemyProjectile proj))
        {
            foreach (var mod in proj.projectileModifiers)
            {
                if (mod.GetType() == typeof(ProjectileIndestructibleModifier))
                {
                    return;
                }
            }
            proj.DestroyProjectile();
        }
    }

}
