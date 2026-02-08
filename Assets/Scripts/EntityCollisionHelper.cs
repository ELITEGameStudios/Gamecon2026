using UnityEngine;

public class EntityCollisionHelper : MonoBehaviour
{
    [SerializeField] private EntityBase targetEntity;
    void OnCollisionEnter(Collision collision)
    {
        targetEntity.CollisionEnterEvent(collision);
    }
}
