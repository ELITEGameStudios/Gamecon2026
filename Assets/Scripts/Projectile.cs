using System;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public enum ProjectileState {Idle, Flying, Embedded, PickedUp}
    public ProjectileState currentState { get; private set; } = ProjectileState.Idle;
    
    [Header("Flying Settings")]
    public float speed = 30f;
    public float lifetime = 5f;
    
    [Header("Embedded Settings")]
    public float pickUpRadius = 2f;
    public float embedDepth = 0.5f;
    public GameObject hitEffect;
    
    [Header("Idle Settings")]
    public Transform initProjectilePosition;
    
    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;

    private Rigidbody rb;
    private Collider col;
    
    private Transform embedParent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        ReturnToIdle();
    }
    
    void Update()
    {
        if (playerTransform != null)
        {
            TryPickUp();
        }
    }

    public void SetState(ProjectileState newState)
    {
        currentState = newState;
        
        switch (currentState)
        {
            case ProjectileState.Idle:
                rb.isKinematic = true;
                col.enabled = true;
                col.isTrigger = false;
                break;
            case ProjectileState.Flying:
                rb.isKinematic = false;
                col.isTrigger = false;
                break;
            case ProjectileState.Embedded:
                rb.isKinematic = true;
                col.isTrigger = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                break;
            case ProjectileState.PickedUp:
                rb.isKinematic = true;
                col.enabled = false;
                gameObject.SetActive(false);
                break;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (currentState != ProjectileState.Flying)
            return;
        
        if (hitEffect)
            Instantiate(hitEffect, transform.position, Quaternion.identity);

        embedParent = collision.collider.transform;
        transform.SetParent(embedParent);

        transform.position += transform.forward * embedDepth; // set the knife deeper
        
        SetState(ProjectileState.Embedded);
        
        CancelInvoke(nameof(ReturnToIdle));
    }
    
    public void CastProjectile(Vector3 direction)
    {
        transform.SetParent(null);
        SetState(ProjectileState.Flying);
        
        rb.linearVelocity = direction * speed;
        
        CancelInvoke(nameof(ReturnToIdle));
        Invoke(nameof(ReturnToIdle), lifetime);
    }

    private void ReturnToIdle()
    {
        if (currentState != ProjectileState.Flying)
            return;
        
        SetState(ProjectileState.Idle);
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        transform.SetParent(initProjectilePosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        gameObject.SetActive(true);
    }
    
    public void TryPickUp()
    {
        if (currentState != ProjectileState.Embedded)
            return;
        
        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (distance <= pickUpRadius)
        {
            SetState(ProjectileState.PickedUp);
            
            // Instantly return to hand + idle state
            transform.SetParent(initProjectilePosition);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        
            gameObject.SetActive(true);
            SetState(ProjectileState.Idle);
        }
    }
}
