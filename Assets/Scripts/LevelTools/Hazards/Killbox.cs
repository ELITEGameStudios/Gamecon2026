using UnityEngine;

public class Killbox : MonoBehaviour
{
    /// <summary>
    /// If this is true, destroys entities that exit the trigger area. Prevents player from being able to escape play area.
    /// </summary>
    [SerializeField] bool isPlayAreaLimiter = false;
    [SerializeField] bool affectKnife = true;
    Collider killboxCollider;

    LayerMask knifeMask;

    Collider[] knifeHolder;

    private void Start()
    {
        killboxCollider = GetComponent<Collider>();
        knifeMask = LayerMask.GetMask("Projectile");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isPlayAreaLimiter) return;
        ClearEntity(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlayAreaLimiter) return;
        ClearEntity(other);
    }

    void ClearEntity(Collider other)
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
        else if (other.TryGetComponent(out Projectile knife))
        {
            // if the killbox kills things inside of the box, and the knife is within the box, give it back to the player
            //if the killbox kills things outside of the box, and the knife is outside the box, give it back to the player

            if (!affectKnife) return;
            bool insideContainer = killboxCollider.bounds.Contains(knife.transform.position);
           
            if (insideContainer && !isPlayAreaLimiter || !insideContainer && isPlayAreaLimiter) knife.Pickup();
        }
    }

}