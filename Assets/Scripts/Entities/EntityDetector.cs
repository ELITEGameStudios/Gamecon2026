using UnityEngine;
using System.Collections.Generic;

public class EntityDetector : MonoBehaviour
{

    [SerializeField] SphereCollider Collider;

    [SerializeField] LayerMask detectionMask;

    [SerializeField] List<EntityBase> entities = new();

    public  IReadOnlyList<EntityBase> DetectedEntities  => entities;

    private void Awake()
    {
        if (Collider == null) Collider = GetComponent<SphereCollider>();
    }

    public void UpdateDetectionLayers(LayerMask layerMask)
    {
        detectionMask = layerMask;
    }

    private void FixedUpdate()
    {
        entities.Clear();
        if (!Collider.enabled) return;
        var overlap = Physics.OverlapSphere(Collider.bounds.center, Collider.radius, detectionMask, QueryTriggerInteraction.Collide);
        foreach (var obj in overlap)
        {
            if (obj.TryGetComponent(out EntityBase entity))
            {
                entities.Add(entity);
            }
        }
    }


}
