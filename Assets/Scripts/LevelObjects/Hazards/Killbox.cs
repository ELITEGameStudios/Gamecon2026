using UnityEngine;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detected collider " + other.name);
        if (other.TryGetComponent(out EntityBase entity))
        {
            entity.Damage(entity.health + 1);
        }
        else
        {
            Debug.Log("Couldn't find entity " + entity.name);
        }
    }
}
