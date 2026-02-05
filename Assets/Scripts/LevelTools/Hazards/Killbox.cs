using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EntityBase entity))
        {
            entity.Damage(entity.health + 1);
        }
        else if (other.TryGetComponent(out EnemyProjectile proj))
        {
            proj.DestroyProjectile();
        }
    }
}
