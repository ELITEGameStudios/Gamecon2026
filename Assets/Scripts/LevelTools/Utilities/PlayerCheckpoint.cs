using UnityEngine;
using UnityEngine.Events;

public class PlayerCheckpoint : MonoBehaviour
{
    [SerializeField] Color inactiveColor;
    [SerializeField] Color activeColor;

    [SerializeField] BoxCollider checkpointCollider;
    [SerializeField] MeshRenderer checkpointModel;

    LayerMask runnerMask;
    bool reachedCheckpoint = false;

    [HideInInspector] public UnityEvent<PlayerCheckpoint> checkpointReached = new();

    Vector3 colliderHalfExtents;

    private void Start()
    {
        checkpointModel.material.color = activeColor;
        if (checkpointCollider == null) checkpointCollider = GetComponentInChildren<BoxCollider>();
        runnerMask = LayerMask.GetMask("Player");
    }
    private void FixedUpdate()
    {
        if (reachedCheckpoint) return;

        var overlap = Physics.OverlapBox(checkpointCollider.bounds.center, checkpointCollider.bounds.extents, checkpointCollider.transform.rotation,runnerMask);
        if (overlap.Length > 0) OnCheckpointReached();
    }

    void OnCheckpointReached()
    {
        reachedCheckpoint = true;

        checkpointModel.material.color = inactiveColor;
        checkpointReached.Invoke(this);
    }
}
